
import OAuth2Client = require("../oAuth2/OAuth2Client");

class User {
    claims: { [key: string]: string };

    constructor(opt: {
        oauth: OAuth2Client
    }) {
        if (opt.oauth.oauthResult.id_token) {
            this.claims = JSON.parse(atob(opt.oauth.oauthResult.id_token.split(".")[1]));
        } else {
            this.claims = {}
        }
    }

    getEmails() {
        return this.claims["emails"];
    }
}

export = User;