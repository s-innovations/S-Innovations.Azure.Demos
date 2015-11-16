



import ko = require("knockout");
import koLayout = require("si-portal-framework/koExtensions/koLayout");
import OAuth2Client = require("../../oAuth2/OAuth2Client");
import "template!./templates/startupModalTemplate.html";
import "si-portal-framework/koExtensions/singleClickBindingHandler";





class startupModalLayout implements koLayout {


    constructor() {


    }

    templateOptions() {

        return {
            name: "startupModalTemplate",
            data: this,
            as: "$startupModalLayout"
        };
    }

    login(event) {
        var oauth = new OAuth2Client({ url: "https://login.microsoftonline.com/car2cloudb2c.onmicrosoft.com/oauth2/v2.0/authorize", storagePrefix: "__akv_oauth" });
        var obj = oauth.createImplicitFlowRequest("7230cf4a-fb8d-4a01-b6e4-cdbb76a3995b", "http://localhost:11809/", "openid", {
            responseType: "code id_token"
        });
        window.location.href = obj.url + "&p=B2C_1_car2cloud-b2c-signin";
    }
    signup(event) {
        var oauth = new OAuth2Client({ url: "https://login.microsoftonline.com/car2cloudb2c.onmicrosoft.com/oauth2/v2.0/authorize", storagePrefix: "__akv_oauth" });
        var obj = oauth.createImplicitFlowRequest("7230cf4a-fb8d-4a01-b6e4-cdbb76a3995b", "http://localhost:11809/", "openid", {
            responseType: "id_token"
        });
        window.localStorage.setItem("__akv_signedup", "true");
        window.location.href = obj.url + "&p=b2c_1_car2cloud-b2c-signup";
    }

    isFirstVisit = ko.observable(window.localStorage.getItem("__akv_signedup") !== "true");
}

export =startupModalLayout;