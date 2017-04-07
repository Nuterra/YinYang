(function () { 
    var template = Nuterra.addTemplate('home', '/assets/pages/home.mustache');

    Nuterra.addPage('home', function (id) {
        template.render({}, function (rendered) {
            $("#main-content").html(rendered);
            $('#main-content .btn').button();
        });
    });
})();