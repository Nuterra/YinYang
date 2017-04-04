Nuterra.loadTemplate('tmpl-profile-box', '/app/profile-box.html', function(template) {
    Mustache.parse(template);

    var steamId = $.cookie('YinYang.SteamID');
    Nuterra.getAccount(steamId, function(account){
        var accountData = {
            profile: account,
        };
        var rendered = Mustache.render(template, accountData);
        $('#profile-box').html(rendered);
    });
});