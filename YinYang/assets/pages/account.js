var accountTemplate;
Nuterra.loadTemplate('tmpl-account', '/assets/pages/account.html', function (template) {
    accountTemplate = template;
    Mustache.parse(accountTemplate);
});

var techsTemplate;
Nuterra.loadTemplate('tmpl-tech-list', '/assets/pages/techs.html', function (template) {
    techsTemplate = template;
    Mustache.parse(techsTemplate);
});

Nuterra.addPage('account', function (id) {
    if (id == null) {
        //My account
        Nuterra.getCurrentAccount(function (account) {
            var rendered = Mustache.render(accountTemplate, { profile: account });
            $('#main-content').html(rendered);
            $('#main-content .btn').button();
            $('#main-content .editable').editable();
        });
    } else {
        Nuterra.getAccount(id, function (account) {
            var rendered = Mustache.render(accountTemplate, { profile: account });
            $('#main-content').html(rendered);
        });
        { return; }
        Nuterra.getTechsForAccount(id, function (techs) {
            var rendered = Mustache.render(techsTemplate, { techs: techs });
            $('#tech-list').html(rendered);
        });
    }
});