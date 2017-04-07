(function () {
    var accountTemplate = Nuterra.addTemplate('account', '/assets/pages/account.mustache');

    Nuterra.addPage('account', function (id) {
        var showAccount = function (account) {
            accountTemplate.render({ profile: account }, function (rendered) {
                $('#main-content').html(rendered);
                $('#main-content .btn').button();
                $('#main-content .editable').editable();
            });
        }
        if (id == null) {
            Nuterra.getCurrentAccount(showAccount);
        } else {
            Nuterra.getAccount(id, showAccount);
        }
    });
})();