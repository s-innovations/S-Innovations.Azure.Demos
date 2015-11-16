
import OAuth2Result = require("./OAuth2Result");
import ImplicitRequestOptions = require("./ImplicitRequestOptions");
import HashObservable = require("./HashObservable");
import setDefaultProperties = require("si-portal-framework/utils/setDefaultProperties");

class Oauth2Client {

    private url: string;
    private storagePrefix: string;
    private storage: Storage;
    constructor(options: { url: string; storagePrefix?: string; storage?: Storage }) {
        this.url = options.url;
        this.storagePrefix = options.storagePrefix || "__oauth_";
        this.storage = options.storage || window.sessionStorage;
     
    }

    oauthResult: OAuth2Result;

    createImplicitFlowRequest(clientid, callback, scope, options: ImplicitRequestOptions) {


        var state = ((Date.now() + Math.random()) * Math.random())
            .toString().replace(".", "");
        var nonce = ((Date.now() + Math.random()) * Math.random())
            .toString().replace(".", "");

        var url =
            this.url + "?" +
            "client_id=" + encodeURIComponent(clientid) + "&" +
            "redirect_uri=" + encodeURIComponent(callback + (options.isSilence ? "/iframe.html" : "")) + "&" +
            "response_type=" + encodeURIComponent(options.responseType) + "&" +
            "scope=" + encodeURIComponent(scope) + "&" +
            "state=" + encodeURIComponent(state) + "&" +
            "nonce=" + encodeURIComponent(nonce);

        if (typeof (options.prompt) !== "undefined") {
            url += "&prompt=" + encodeURIComponent(options.prompt)
        }
        if (typeof (options.login_hint) !== "undefined") {
            url += "&login_hint=" + encodeURIComponent(options.login_hint)
        }
        if (typeof (options.acr_values) !== "undefined") {
            url += "&acr_values=" + encodeURIComponent(options.acr_values)
        }


        var data = {
            url: url,
            state: state,
            nonce: nonce
        };
        this.storage.setItem(this.storagePrefix, JSON.stringify(data));
        return data;
    }


    parseResult(queryStringOrParams) {
        if (typeof (queryStringOrParams) === "string") {
            var params = {},
                regex = /([^&=]+)=([^&]*)/g,
                m;

            while (m = regex.exec(queryStringOrParams)) {
                params[decodeURIComponent(m[1])] = decodeURIComponent(m[2]);
            }

            for (var prop in params) {
                return new OAuth2Result(this, params);
            }
        } else {
            return new OAuth2Result(this, queryStringOrParams);
        }
    }

    load(storage: Storage) {
        var stored = JSON.parse(this.storage.getItem(this.storagePrefix));
        this.oauthResult = new OAuth2Result(this, stored);
    }
    isAuthenticated() {
        return this.oauthResult && typeof (this.oauthResult.id_token) !== "undefined";
    }

    private handleAuthorizationCallback(hash, storages: string[]) {

        var result = this.parseResult(hash);

        for (var i = 0; i < storages.length; i++) {

            var stored = JSON.parse(this.storage.getItem(storages[i]));
      
            console.log([stored, result]);

            if (result && stored && stored.state === result.state) {
                this.storage.setItem(storages[i], JSON.stringify(result));
                this.oauthResult = result;
                this.storagePrefix = storages[i];
                //   this.load();
                return true;
            }           
        }
        return false;
    }

    private cleanUpHash() {
        location.hash = "";
    }

    isAuthorizeCallBack(...storages: string[]) {
        
        //  http://localhost:11809/?error=unsupported_response_type&error_description=AADSTS70005%3a+The+WS-Federation+sign-in+response+message+contains+an+unsupported+OAuth+parameter+value+in+the+encoded+wctx%3a+%27response_type%27%0d%0aTrace+ID%3a+e41a2aaa-9c90-4c48-8ff1-6dbc8e3cdc43%0d%0aCorrelation+ID%3a+3b51b80f-50f1-4dc7-b80b-f81227c3ad58%0d%0aTimestamp%3a+2015-11-15+14%3a59%3a14Z&state=7044168540027505


        if (HashObservable.params() && (HashObservable.params()["id_token"] || HashObservable.params()["error"])) {
            if (!this.handleAuthorizationCallback(HashObservable.params(), storages)) {
                if (confirm("Authorization Failed. Reset?")) {

                //    this.user.clearCache();
                //    window.location.href = window.location.protocol + "//" + window.location.host;

                }
            } else {
                var action = this.storage.getItem(this.storagePrefix+"afterAuthenticationAction");
                if (action) {
                    this.storage.removeItem(this.storagePrefix+"afterAuthenticationAction");
                    window.location.hash = action;
                } else {
                    this.cleanUpHash();
                }
            }
            return true;
        }
        return false;

    }
}
export = Oauth2Client;