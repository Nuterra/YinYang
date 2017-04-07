$(function () {
    $("#navbar li > a[href]").each(function (index, elem) {
        var linkElem = $(elem);
        var pageLink = linkElem.attr("href");
        var listElem = linkElem.parent()[0];
        Nuterra.setPageNavItem(pageLink, listElem);
    });
});

$(function () { Nuterra.showPageFromUrl(window.location.pathname, true); });

$.fn.editable.defaults.mode = 'inline';

$(function () {
    $("#navbar").on("click", "a", function (event) {
        var elem = event.target;
        if (elem.host == window.location.host) {
            event.preventDefault();
            var link = elem.pathname;
            Nuterra.showPageFromUrl(link, false);
        }
    });
});
