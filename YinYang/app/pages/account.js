var accountTemplate;
Nuterra.loadTemplate('tmpl-account', '/app/pages/account.html', function () {
    accountTemplate = $(this).html();
    Mustache.parse(accountTemplate);
});

var techsTemplate;
Nuterra.loadTemplate('tmpl-tech-list', '/app/pages/techs.html', function () {
    techsTemplate = $(this).html();
    Mustache.parse(techsTemplate);
});

Nuterra.addPage('account', function (id) {
    if (id == null) {
    } else {
        Nuterra.getAccount(id, function (account) {
            var rendered = Mustache.render(accountTemplate, account);
            $('#profile-box').html(rendered);
        });
        Nuterra.getTechsForAccount(id, function (techs) {
            var rendered = Mustache.render(techsTemplate, { techs: techs });
            $('#tech-list').html(rendered);
        });
    }
});