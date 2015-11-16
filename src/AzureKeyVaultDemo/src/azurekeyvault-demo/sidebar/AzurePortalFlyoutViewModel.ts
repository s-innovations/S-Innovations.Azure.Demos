
import inputFormElementLayout = require("../formElements/inputFormElementLayout");
import inputSubmitElementLayout = require("../formElements/inputSubmitElementLayout");
import AzurePortalSideBarViewModel = require("../sidebar/AzurePortalSideBarViewModel");
import OAuth2Client = require("../../oAuth2/OAuth2Client");

class AzurePortalFlyoutViewModel {

    constructor(private opt: { sideBarVm: AzurePortalSideBarViewModel, oauth: OAuth2Client }) {

    }

    getElements() {
        return [this.keyVaultName, this.linkButton, this.deployAzureButton1, this.deployAzureButton2];
    }

    private keyVaultName = new inputFormElementLayout({ label: "KeyVault Name", name: "vaultName", placeholder: "https://{vaultname}.vault.azure.net" })
    private linkButton = new inputSubmitElementLayout({ submitText: "Link Keyvault"});        
    private deployAzureButton1 = new inputSubmitElementLayout({ submitText: "Deploy Keyvault (id_token)", action: this.deployAzureKeyvault1 });
    private deployAzureButton2 = new inputSubmitElementLayout({ submitText: "Deploy Keyvault (id_token token)", action: this.deployAzureKeyvault2 });
    

    deployAzureKeyvault1() {
        var obj = this.opt.oauth.createImplicitFlowRequest("fb4fbba5-6fac-404e-b7b6-d7bf4b56a7ea", "http://localhost:11809/#/oauth2", "openid", {
            responseType: "id_token"
        });


        window.sessionStorage.setItem("__akv_oauth2afterAuthenticationAction", "deployAzure");
        window.location.href = obj.url;

    }
    deployAzureKeyvault2() {
        //   window.open("https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-quickstart-templates%2Fmaster%2F101-create-key-vault%2Fazuredeploy.json");
        var obj = this.opt.oauth.createImplicitFlowRequest("fb4fbba5-6fac-404e-b7b6-d7bf4b56a7ea", "http://localhost:11809/#/oauth2", "openid profile", {
            responseType: "id_token token"
        });

        window.sessionStorage.setItem("__akv_oauth2afterAuthenticationAction", "deployAzure");
        window.location.href = obj.url + "&resource=https://management.core.windows.net/";

    }
}
export =AzurePortalFlyoutViewModel;