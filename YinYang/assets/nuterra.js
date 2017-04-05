window.Nuterra = (function () {
    var myNuterra = {
        templates: {},
        loadTemplate: function (name, url, onload) {
            var template = this.templates[name];
            if (template) {
                onload(template.html());
            } else {
                this.newTemplate(name, url, onload);
            }
        },
        newTemplate: function(name, url, onload) {
            var template = $("<script type='x-tmpl-mustache' id='" + name + "'>");
            $("head script[type='x-tmpl-mustache']").last().after(template);
            var templates = this.templates;
            $.ajax({
                type: 'GET',
                url: url,
                success: function (data) {
                    template.text(data);
                    templates[name] = template;
                    if (onload) {
                        onload(template.html());
                    }
                }
            });
        },

        getAccount: function (steamId, callback) {
            if (!steamId) {
                console.warn('steamId is undefined!');
                callback(null);
                return;
            }
            $.ajax({
                type: 'GET',
                url: '/api/accounts/' + steamId,
                success: function (data) {
                    var account = JSON.parse(data);
                    callback(account);
                }
            });
        },
        getCurrentAccount: function(callback) {
            var steamId = $.cookie('YinYang.SteamID');
            Nuterra.getAccount(steamId, callback);
        },
        getAccounts: function (skip, take, callback) {
            $.ajax({
                type: 'GET',
                url: '/api/accounts?skip=' + skip + '&take='+ take,
                success: function (data) {
                    var account = JSON.parse(data);
                    callback(account);
                }
            });
        },
        getTechsForAccount: function (steamId, callback) {
            $.ajax({
                type: 'GET',
                url: '/api/techs/' + steamId,
                success: function (data) {
                    var techs = JSON.parse(data);
                    callback(techs);
                }
            });
        },
        getTechInfo: function (techId, callback) {
            $.ajax({
                type: 'GET',
                url: '/api/techs/' + techId,
                success: function (data) {
                    var tech = JSON.parse(data);
                    callback(tech);
                }
            });
        },
        pages: {},
        addPage: function (name, handler) {
            this.pages[name] = {
                name: name,
                callback: handler,
                navItems: [],
            };
        },
        showPage: function (name, id, prevent_push) {
            var page = this.pages[name];
            if (!page) {
                console.warn("Unknown page: '" + name + "' tell a developer!");
                return;
            }

            var handler = page.callback;
            var hash = "#" + name + "=" + id + ":";
            handler(id);

            $('#navbar li.active').removeClass('active');
            $(page.navItems).addClass("active");

            if (!prevent_push) {
                history.pushState({ name: name, id: id }, name + ": #" + id, hash);
            }
        },
        getPage: function(link){
            var matches = link.match(/^app\/([^\/]*?)(\/(\w+)(\/.*)?)?$/);
            if (matches != null) {
                var pageName = matches[1];
                var pageId = matches[3] || null;
                return this.pages[pageName];
            } else {
                return null;
            }
        },
        setPageNavItem: function (name, element) {
            var page = this.getPage(name);
            if (page) {
                page.navItems.push(element);
            } else {
                console.warn('Setting nav item for unknown page ', name, element);
            }
        },
        setTitle: function (text) {
            var hash = window.location.pathname.match(/^\/app\/([^\/]*?)((\/(\w+))?(\/.*)?)?$/);
            hash += "/" + text.replace(/\s/g, '+');
            window.location.pathname = hash;
        },
        loadPageFromUrlHash: function () {
            var hash = window.location.pathname;
            var matches = hash.match(/^\/app\/([^\/]*?)((\/(\w+))?(\/.*)?)?$/);
            if (matches != null) {
                var pageName = matches[1];
                var pageId = matches[3] || null;
                this.showPage(pageName, pageId, true);
            } else {
                this.showPage('home', null, true);
            }
        },
    };
    return myNuterra;
}());

window.onpopstate = function (event) {
    if (event.state == null) {
        Nuterra.loadPageFromUrlHash();
    } else {
        Nuterra.showPage(event.state.name, event.state.id, true);
    }
};


$(function () {
    $("#navbar li > a[href]").each(function (index, elem) {
        var linkElem = $(elem);
        var pageLink = linkElem.attr("href");
        var listElem = linkElem.parent()[0];
        Nuterra.setPageNavItem(pageLink, listElem);
    });

});
$(function () { Nuterra.loadPageFromUrlHash(); });

$.fn.editable.defaults.mode = 'inline';

function test(a) {
    var matches = a.match(/^\/app\/([^\/]*?)((\/(\w+))?(\/.*)?)?$/);
    if (matches != null) {
        var pageName = matches[1];
        var pageId = matches[3] || null;
        console.log(a + " -> " + pageName + "=" + pageId);
    } else {
        console.log(a + " -> no match");
    }
}
