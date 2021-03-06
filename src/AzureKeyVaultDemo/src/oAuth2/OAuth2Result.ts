
import OAuthClient = require("./OAuth2Client");

class OAuth2Result {


    constructor(public client: OAuthClient, data) {
        if (data) {

            this.id_token = data.id_token;
            this.expires_in = data.expires_in;
            this.scope = data.scope;
            this.state = data.state;
            this.token_type = data.token_type;
            this.access_token = data.access_token;
            this.expires_at = new Date().getTime() + parseFloat((<string>data.expires_in)) * 1000;
        }
    }

    id_token: string;
    access_token: string;
    expires_in: number;
    scope: string;
    state: string;
    token_type: string;
    expires_at: number;
}

export = OAuth2Result;