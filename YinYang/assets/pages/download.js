var downloadTemplate;
Nuterra.loadTemplate('tmpl-download', '/assets/pages/download.html', function (template) {
    downloadTemplate = template;
    Mustache.parse(downloadTemplate);
});

Nuterra.addPage('download', function (id) {
    var rendered = Mustache.render(downloadTemplate, {});
    $('#main-content').html(rendered);
    $('#main-content .btn').button();
});