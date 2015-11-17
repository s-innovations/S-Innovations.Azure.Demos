
import ko = require("knockout");
import inputFormElementLayout = require("../formElements/inputFormElementLayout");
import inputSubmitElementLayout = require("../formElements/inputSubmitElementLayout");
import AzurePortalSideBarViewModel = require("../sidebar/AzurePortalSideBarViewModel");
import OAuth2Client = require("../../oAuth2/OAuth2Client");


import koLayout = require("si-portal-framework/koExtensions/koLayout");
class label implements koLayout {
    el: HTMLElement;
    constructor(label) {
        this.el = document.createElement("div");
        this.el.innerHTML = "<h1>" + label+ "</h1>";
    }
    templateOptions() {

        return {
            nodes: [this.el],
            data: this,
        };
    }
}



class AzurePortalFlyoutViewModel {

    constructor(private opt: { sideBarVm: AzurePortalSideBarViewModel, oauth: OAuth2Client }) {

    }

    getElements() {
        return [this.keyVaultName, this.linkButton, new label("Deploy Keyvault"), this.authority,
            this.deployAzureButton0, this.deployAzureButton1, this.deployAzureButton2, this.deployAzureButton3];
    }

    private keyVaultName = new inputFormElementLayout({ label: "KeyVault Name", name: "vaultName", placeholder: "https://{vaultname}.vault.azure.net" })
    private linkButton = new inputSubmitElementLayout({ submitText: "Link Keyvault" });
    private authority = new inputFormElementLayout({ valueUpdateTrigger: "keyup", label: "Azure AD Tenant", name: "tenantId", placeholder: "yourdomain.onmicrosoft.com", required: true }).extend({ required: true });

    private deployAzureButton0 = new inputSubmitElementLayout({  valid: this.authority.isValid, submitText: "LiveID - RM Template", action: this.deployAzureKeyvault.bind(this, "id_token", "&domain_hint=live.com", "login") });
    private deployAzureButton1 = new inputSubmitElementLayout({ valid: this.authority.isValid, submitText: "LiveID - RM REST", action: this.deployAzureKeyvault.bind(this, "id_token token", "&resource=https://management.core.windows.net/&domain_hint=live.com","login") });   
    private deployAzureButton2 = new inputSubmitElementLayout({ valid:true,submitText: "Work Account - RM Template", action: this.deployAzureKeyvault.bind(this, "id_token","","login") });
    private deployAzureButton3 = new inputSubmitElementLayout({ valid:true,submitText: "Work Account - RM REST", action: this.deployAzureKeyvault.bind(this, "id_token token", "&resource=https://management.core.windows.net/","login") });
    

    deployAzureKeyvault(responseType, query: string, prompt :string) {

        if (typeof(query) === "string" && query.indexOf("live.com") !== -1) {
            if (!this.authority.value.isValid()) {
                this.authority.value.isModified(true);
                return
            }
        }
    
        var oauth = new OAuth2Client({ url: "https://login.microsoftonline.com/" + (this.authority.value() || "common") + "/oauth2/authorize", storagePrefix: "__akv_oauth2" });
                                                                                //""
       var obj = oauth.createImplicitFlowRequest("84eeeca9-408f-477d-9af8-9de3de6923c0", "http://localhost:11809/", "openid profile", {
           responseType: responseType,
           prompt: prompt,
        });


       window.sessionStorage.setItem("__akv_oauth2afterAuthenticationAction", "deployAzure");
       window.location.href = obj.url + query;

    }        
}
export =AzurePortalFlyoutViewModel;