Nuterra.addPage('home', function () {
    Nuterra.loadTemplate("home", "/app/pages/home.html", function (template) {
        $("#main-content").html(template);
    });
});