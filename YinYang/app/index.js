window.onload = function () {
    var steamID = false;

    steamID = $.cookie('YinYang.SteamID');

    if (steamID) {
        renderProfileInfo(steamID);
    } else {
        renderLoginForm();
    }
}

function renderProfileInfo(steamID) {
    //Grab the inline template
    var template = document.getElementById('profile-info').innerHTML;

    //Parse it (optional, only necessary if template is to be used again)
    Mustache.parse(template);

    var username = '';

    //Render the data into the template
    var rendered = Mustache.render(template, { steamID: steamID });

    //Overwrite the contents of #target with the rendered HTML
    document.body.innerHTML = rendered;

    var form = $(document.body).find('form').first();
    $.ajax({
        url: '/api/account/' + steamID,
        type: 'GET',
        context: form,
    }).done(function (data) {
        console.log(data);
        var account = JSON.parse(data);
        var is_admin = (account.Flags & 0x04) != 0;
        is_admin = true;
        if (is_admin) {
            $(document.body).addClass("is-admin");
        } else {
            $(document.body).removeClass("is-admin");
        }


        $.ajax({
            url: '/api/account/all',
            type: 'GET',
            context: form,
        }).done(function (data) {
            renderUserList(JSON.parse(data), is_admin);
        });

        console.log(account);
        $(this).children().first().val(account.Username);


        $(this).submit(function (event) {
            $.ajax({
                url: '/api/account/' + steamID,
                type: 'PUT',
                contentType: 'application/json',
                data: JSON.stringify({ username: form.children().first().val() }),
                dataType: 'json',
                context: this,
            }).done(function () {
                console.log('DONE');
            });
            event.preventDefault();
        });
    });

}

function renderLoginForm() {
    //Grab the inline template
    var template = document.getElementById('login-form').innerHTML;

    //Parse it (optional, only necessary if template is to be used again)
    Mustache.parse(template);

    //Render the data into the template
    var rendered = Mustache.render(template, { name: 'Luke', power: 'force' });

    //Overwrite the contents of #target with the rendered HTML
    document.body.innerHTML = rendered;
}

function renderUserList(steamIDs, admin_mode) {
    //Grab the inline template
    var template = document.getElementById('user-list').innerHTML;

    //Parse it (optional, only necessary if template is to be used again)
    Mustache.parse(template);

    //Render the data into the template
    var rendered = $(Mustache.render(template));

    //Add list to the bottom of the page
    $(document.body).find("form").append(rendered);
    $.each(steamIDs, function (index, steamID) {
        console.log('My array has at position ' + index + ', this steamID: ' + steamID);
        var userTemplate = document.getElementById('user-item').innerHTML;
        Mustache.parse(userTemplate);
        var renderedUser = $(Mustache.render(userTemplate, { steamID: steamID, username: 'Retrieving...', admin_mode: admin_mode }));
        $(rendered).append(renderedUser);
        $.ajax({
            url: '/api/account/' + steamID,
            type: 'GET',
        }).done(function (data) {
            var obj = JSON.parse(data);
            $(renderedUser).find(".username").each(function (index, element) {
                var elem = $(element);
                if (elem.is("input")) {
                    elem.val(obj.Username);
                } else {
                    elem.html(obj.Username);
                }
            });
        });
        var changeForm = renderedUser.find("form");
        changeForm.submit(function (event) {
            event.preventDefault();
            var id = $(this).data('steam-id')
            $.ajax({
                url: '/api/account/' + id,
                type: 'PUT',
                contentType: 'application/json',
                data: JSON.stringify({ username: $(this).children().first().val() }),
                dataType: 'json',
                context: this,
            }).done(function () {
                console.log('DONE');
            });
        });
    });
}