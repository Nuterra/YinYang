(function () {
    var template = Nuterra.addTemplate('download', '/assets/pages/download.mustache');

    Nuterra.addPage('download', function (id) {
        template.render({}, function (rendered) {
            $('#main-content').html(rendered);
            $('#main-content .btn').button();
        });
    });
})();