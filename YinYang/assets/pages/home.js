Nuterra.addPage('home', function () {
    Nuterra.loadTemplate("home", "/assets/pages/home.html", function (template) {
        $("#main-content").html(template);
    });
});