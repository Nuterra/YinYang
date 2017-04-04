Nuterra.addPage('home', function () {
    Nuterra.loadTemplate("home", "/app/pages/home.html", function () {
        var template = $(this).html();
        $("#main-content").html(template);
    });
});