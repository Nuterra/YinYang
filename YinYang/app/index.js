window.onload = function () {
    var loggedIn = false;

    loggedIn = $.cookie('YinYang.SteamID');

    if (loggedIn) {
        renderProfileInfo();
    } else {
        renderLoginForm();
    }
}

function renderProfileInfo() {
    //Grab the inline template
    var template = document.getElementById('profile-info').innerHTML;

    //Parse it (optional, only necessary if template is to be used again)
    Mustache.parse(template);

    //Render the data into the template
    var rendered = Mustache.render(template, { name: "Luke", power: "force" });

    //Overwrite the contents of #target with the rendered HTML
    document.body.innerHTML = rendered;
}

function renderLoginForm() {
    //Grab the inline template
    var template = document.getElementById('login-form').innerHTML;

    //Parse it (optional, only necessary if template is to be used again)
    Mustache.parse(template);

    //Render the data into the template
    var rendered = Mustache.render(template, { name: "Luke", power: "force" });

    //Overwrite the contents of #target with the rendered HTML
    document.body.innerHTML = rendered;
}