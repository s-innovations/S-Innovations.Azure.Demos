
import "si-portal-framework/koExtensions/verboseConsoleBindingErrorReporter";

import siPortalLoader = require("si-portal-framework/siPortal/siPortalLoader");
import AzurePortalLayout = require("./AzurePortalLayout");
import OAuth2Client = require("../oAuth2/OAuth2Client");


var oauth = new OAuth2Client({ url: "https://login.microsoftonline.com/car2cloudb2c.onmicrosoft.com/oauth2/v2.0/authorize", storagePrefix: "__akv_oauth" });
if (oauth.isAuthorizeCallBack("__akv_oauth", "__akv_oauth2")) {

} else {
    oauth.load(window.sessionStorage);
    if (oauth.isAuthenticated()) {

        
    }

}

class loader extends siPortalLoader {
    constructor() {
        super({
            rootLayout: new AzurePortalLayout({
                oauth: oauth,
            }),
        });

                                               
        ////administrate https://portal.azure.com/car2cloudb2c.onmicrosoft.com/?Microsoft_AAD_B2CAdmin=true#blade/Microsoft_AAD_B2CAdmin/TenantManagementBlade/id/car2cloudb2c.onmicrosoft.com
       

    }
}

export = loader;