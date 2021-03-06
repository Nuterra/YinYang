﻿(function () {
    var techCreateTemplate = Nuterra.addTemplate('tech-create', '/assets/pages/tech-create.mustache');

    function postRender() {
        $('#main-content .btn').button();
        $('#main-content .file').fileinput();
        $('#main-content form').validator({
            custom: {
                'file-single': function ($el) {
                    if ($el[0].files.length > 1) {
                        return "Only one file allowed";
                    }
                },
                'file-type': function ($el) {
                    var matchValue = $el.data('file-type');
                    var myFile = $el[0].files[0];
                    if (myFile && myFile.type !== matchValue) {
                        return "File must be of type " + matchValue;
                    }
                },
            }
        }).on('submit', function (e) {
            if (!e.isDefaultPrevented()) {
                e.preventDefault();
                var formData = new FormData();
                formData.append('title', $('#tech-title').val());
                formData.append('file', $('#tech-file')[0].files[0]);
                formData.append('title2', $('#tech-title').val());

                $.ajax({
                    url: '/api/techs',
                    type: 'POST',
                    data: formData,
                    processData: false,  // tell jQuery not to process the data
                    contentType: false,  // tell jQuery not to set contentType
                }).done(function (data, textStatus) {
                    var techId = data;
                    Nuterra.showPage('techs', techId);
                }).fail(function (jqXhr, textStatus) {
                    alert("error: " + textStatus);
                })
            }
        });
    }

    Nuterra.addPage('add-tech', function (id) {
        Nuterra.getCurrentAccount(function (account) {
            techCreateTemplate.render({ logged_in: (account != null) }, function (rendered) {
                $('#main-content').html(rendered);
                postRender();
            });
        });
    });
})();