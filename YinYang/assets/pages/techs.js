var techsTemplate;
Nuterra.loadTemplate('tmpl-tech-list', '/assets/pages/techs.html', function (template) {
    techsTemplate = template;
    Mustache.parse(techsTemplate);
});

Nuterra.addPage('tech', function (id) {
    if (id == null) {
        Nuterra.getTechsForAccount('all', function(techs){
            var rendered = Mustache.render(techsTemplate, { techs: techs });
            $('#main-content').html(rendered);
        });
    } else {
        Nuterra.getTechInfo(id, function (tech) {
            var rendered = Mustache.render(techsTemplate, { techs: tech });
            $('#main-content').html(rendered);
        });
    }
});