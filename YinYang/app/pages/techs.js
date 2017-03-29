var techsTemplate;
Nuterra.loadTemplate('tmpl-tech-list', '/app/pages/techs.html', function () {
    techsTemplate = $(this).html();
    Mustache.parse(techsTemplate);
});

Nuterra.addPage('tech', function (id) {
    if (id == null) {
    } else {
        Nuterra.getTechInfo(id, function (tech) {
            // <div id="#latest-techs" />
            var rendered = Mustache.render(techsTemplate, { techs: tech });
            $('#tech-list').html(rendered);
        });
    }
});