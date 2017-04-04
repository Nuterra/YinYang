window.Nuterra = (function () {
    var myNuterra = {
        loadTemplate: function (name, url, onload) {
            var template = $("<script type='x-tmpl-mustache' id='" + name + "'>");
            $("head script[type='x-tmpl-mustache']").last().after(template);
            $.ajax({
                type: 'GET',
                url: url,
                success: function (data) {
                    template.text(data);
                    if (onload) {
                        onload.call(template[0]);
                    }
                }
            });
        },

        getAccount: function (steamId, callback) {
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
            var matches = link.match(/^#([^=]*?)(=(\w+)(:.*)?)?$/);
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
            var hash = window.location.hash.match(/^#([^=]*?)=(\w+)/g);
            hash += ":" + text.replace(/\s/g, '+');
            window.location.hash = hash;
        },
        loadPageFromUrlHash: function () {
            var hash = window.location.hash;
            var matches = hash.match(/^#([^=]*?)(=(\w+)(:.*)?)?$/);
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

///#mod=1:my-first-mod
//<url>#<pagename>=<id>:<userdata>

/*
window.onpopstate = function (event) {
    alert("location: " + document.location + ", state: " + JSON.stringify(event.state));
};

history.pushState({ page: 1 }, "title 1", "?page=1");
history.pushState({ page: 2 }, "title 2", "?page=2");
history.replaceState({ page: 3 }, "title 3", "?page=3");
history.back(); // alerts "location: http://example.com/example.html?page=1, state: {"page":1}"
history.back(); // alerts "location: http://example.com/example.html, state: null
history.go(2);  // alerts "location: http://example.com/example.html?page=3, state: {"page":3}
*/