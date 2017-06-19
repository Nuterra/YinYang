(function () {
    var template = Nuterra.addTemplate('download', '/assets/pages/download.mustache');

    Nuterra.addPage('download', function (id) {
        var ctx = Nuterra.createContext();
        template.render({}, function (rendered) {
            if (Nuterra.checkContext(ctx)) {
                $('#main-content').html(rendered);
                $('#main-content .btn').button();
            }
            Nuterra.releaseContext(ctx);
        });
    });
})();