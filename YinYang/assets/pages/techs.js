(function () {
    var techListTemplate = Nuterra.addTemplate('tech-list', '/assets/pages/techs.mustache');
    var techTemplate = Nuterra.addTemplate('tech', '/assets/pages/tech.mustache');

    techListTemplate.addDependency(techTemplate);

    Nuterra.addPage('tech', function (id) {
        var showTechs = function (techs) {
            techListTemplate.render({ techs: techs }, function (rendered) {
                $('#main-content').html(rendered);
            });
        };
        if (id == null) {
            Nuterra.getTechsForAccount('all', showTechs);
        } else {
            Nuterra.getTechInfo(id, showTechs);
        }
    });
})();