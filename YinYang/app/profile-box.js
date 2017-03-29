$(function () {
    importTemplate('tmpl-profile-box', 'profile-box.html', function() {
        var template = $(this).html();

        //Parse it (optional, only necessary if template is to be used again)
        Mustache.parse(template);
        
        //Render the data into the template
        var rendered = Mustache.render(template, { profile: {steamID: "mysteamid", username: "im bob"} });

        $('#profile-box').html(rendered);
    });
});