window.Nuterra = (function () {
    function Template(url, partialCallback) {
        this.callbacks = [];
        this.dependsOn = [];
        this.pendingDependencies = [];
        this.downloaded = false;
        this.text = null;
        this.partialCallback = partialCallback;
        this.url = url;
        this.download();
    }
    Template.prototype.use = function (callback) {
        if (this.downloaded && this.pendingDependencies.length == 0) {
            callback(this.text);
        } else {
            this.callbacks.push(callback);
        }
    }
    Template.prototype.render = function (data, callback) {
        var self = this;
        this.use(function (text) {
            var rendered = Mustache.render(text, data, self.partialCallback);
            callback(rendered);
        });
    }
    Template.prototype.download = function () {
        if (this.downloaded) {
            return;
        }
        var self = this;
        $.ajax({
            type: 'GET',
            url: this.url,
            success: function (data) {
                self.text = data;
                self.downloaded = true;
                if (self.pendingDependencies.length == 0) {
                    self.onDownloaded();
                }
            }
        });
    }
    Template.prototype.onDownloaded = function () {
        for (var i = 0; i < this.callbacks.length; i++) {
            var callback = this.callbacks[i];
            callback(this.text);
        }
        this.callbacks.length = 0;
    }
    Template.prototype.addDependency = function (otherTemplate) {
        for (var i = 0; i < this.pendingDependencies.length; i++) {
            if (this.dependsOn[i] == otherTemplate) {
                return;
            }
        }
        this.dependsOn.push(otherTemplate);
        if (!otherTemplate.downloaded) {
            this.pendingDependencies.push(otherTemplate);
            var self = this;
            otherTemplate.use(function () {
                self.dependencyReady(otherTemplate);
            });
        }
    }
    Template.prototype.dependencyReady = function (otherTemplate) {
        for (var i = 0; i < this.pendingDependencies.length; i++) {
            var dependency = this.pendingDependencies[i];
            if (dependency == otherTemplate) {
                this.pendingDependencies.splice(i, 1);
                break;
            }
        }
        if (this.downloaded) {
            this.onDownloaded(this.text);
        }
    }

    var myNuterra = {
        templates: {},
        getTemplateText: function (name) {
            var template = this.templates[name];
            if (!template) {
                console.warn("Missing template: '" + name + "'");
                return null;
            }
            if (!template.downloaded) {
                console.warn("Template not yet downloaded: '" + name + "'");
                return null;
            }
            return template.text;
        },
        addTemplate: function (name, url) {
            if (!this.templates.hasOwnProperty(name)) {
                this.templates[name] = new Template(url, this.getTemplateText.bind(this));
            }
            var template = this.templates[name];
            console.assert(template.url == url);
            return template;
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
            }).done(function (data) {
                var account = JSON.parse(data);
                callback(account);
            }).fail(function (xhr, statusText) {
                callback(null);
            });
        },
        getCurrentAccount: function(callback) {
            Nuterra.getAccount("me", callback);
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
            }).done(function (data) {
                var tech = JSON.parse(data);
                callback(tech);
            }).fail(function (xhr, statusText) {
                callback(null);
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
            var newUrl = "/app/" + name + "/";
            if (id) {
                newUrl += id + "/";
            }
            handler(id);

            $('#navbar li.active').removeClass('active');
            $(page.navItems).addClass("active");

            if (!prevent_push) {
                history.pushState({ name: name, id: id }, name + ": #" + id, newUrl);
            }
        },
        getPage: function(link){
            var info = this.analyzeUrl(link);
            if (info) {
                return this.pages[info.page];
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
            window.document.title = text;
        },
        showPageFromUrl: function (url, prevent_push) {
            var info = this.analyzeUrl(url);
            if (info) {
                this.showPage(info.page, info.data, prevent_push);
            } else {
                this.showPage('home', null, prevent_push);
            }
        },
        analyzeUrl: function (url) {
            var matches = url.match(/^\/app\/([^\/]*?)((\/(\w+))?(\/.*)?)?$/);
            if (matches) {
                return { page: matches[1], data: matches[4] }
            } else {
                return null;
            }
        },
    };
    return myNuterra;
}());

window.onpopstate = function (event) {
    if (event.state == null) {
        Nuterra.showPageFromUrl(window.location.pathname, true);
    } else {
        Nuterra.showPage(event.state.name, event.state.id, true);
    }
};
