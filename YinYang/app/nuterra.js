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
            this.pages[name] = handler;
        },
        showPage: function (name, id, nopush) {
            var handler = this.pages[name];
            if (!handler) {
                alert("Unknown handler: '" + name + "' tell a developer!");
            }
            var hash = "#" + name + "=" + id + ":";
            handler(id);
            if (!nopush) {
                history.pushState({ name: name, id: id }, name + ": #" + id, hash);
            }
        },
        setPageTitle: function (text) {
            var hash = window.location.hash.match(/^#([^=]*?)=(\w+)/g);
            hash += ":" + text.replace(/\s/g, '+');
            window.location.hash = hash;
        },
        loadPageFromUrlHash: function () {
            var hash = window.location.hash;
            var matches = hash.match(/^#([^=]*?)(=(\w+)(:.*)?)?$/);
            console.log(matches);
            if (matches != null) {
                var pageName = matches[1];
                var pageId = matches[3] || null;
                this.showPage(pageName, pageId, true);
            } else {
                this.showPage('frontpage', null, true);
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