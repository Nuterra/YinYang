(function () {
    var techListTemplate = Nuterra.addTemplate('tech-list', '/assets/pages/techs.mustache');
    var techTemplate = Nuterra.addTemplate('tech', '/assets/pages/tech.mustache');

    techListTemplate.addDependency(techTemplate);

    Nuterra.addPage('techs', function (id) {
        var ctx = Nuterra.createContext();
        if (id == null) {
            Nuterra.getTechsForAccount('all', function (techs) {
                techListTemplate.render({ techs: techs },
                    function (rendered) {
                        if (Nuterra.checkContext(ctx)) {
                            $('#main-content').html(rendered);
                            $('#main-content > .tech').chunk(3).wrapAll('<div class="row"></div>');
                        }
                        Nuterra.releaseContext(ctx);
                    });
            });
        } else {
            Nuterra.getTechInfo(id, function (techs) {
                techTemplate.render(techs[0],
                    function (rendered) {
                        if (Nuterra.checkContext(ctx)) {
                            $('#main-content').html(rendered);
                        }
                        Nuterra.releaseContext(ctx);
                    });
            });
        }
    });
})();