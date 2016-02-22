function getExternalProvidersList() {
    $.getJSON("/account/getexternalLogins?returnUrl=%2F&generateState=true",
        function (data) { getExternalProvidersListCallback(data); }
    );
}

function getExternalProvidersListCallback(data) {
    var extProviders = $("#extProviders");

    //External Providers auth
    $.each(data, function (key, value) {
        var img = jQuery('<img/>', {
            src: "../Content/Images/" + value.Provider + ".png"
        });
        var providerId = "extPr-" + value.Provider;
        var div = jQuery('<div/>', {
            id: providerId,
            title: value.Provider + ' authentication',
            onclick: "javascript:extAuth('" + providerId + "');",
        });

        img.appendTo(div);
        div.appendTo(extProviders);
    });
}

/* Authenticate a user via an external provider */
function extAuth(id) {
    var clickedExternalLogin = $('#' + id);

    var source = $("#busyIndicatorTemplate").html();
    var template = Handlebars.compile(source);
    var text = template();
    clickedExternalLogin.html(text);

    window.location = "/Account/ExternalLinkLogin?provider=Google";
}


/* Authenticate a user via PASSWORD */
function byPassAuth() {
    resetToken();
    $("#info").hide();

    //Method requests a new token for a registered user

    //TODO: User's registration should be a separate API call -> POST api/account/register, data: email + password
    //TODO: Refresh token should be a separate API call -> POST api/token, data:grant_type=refresh token

    var username = $('#UserName').val();
    var pass = $('#Password').val();

   
}
