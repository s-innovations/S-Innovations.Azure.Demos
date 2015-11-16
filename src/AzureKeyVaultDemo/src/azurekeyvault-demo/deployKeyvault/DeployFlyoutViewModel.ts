

import inputFormElementLayout = require("../formElements/inputFormElementLayout");
import inputSubmitElementLayout = require("../formElements/inputSubmitElementLayout");
import inputSelectElementLayout = require("../formElements/inputSelectElementLayout");
import sendRequest = require("../utils/sendRequest");
import AzurePortalSideBarViewModel = require("../sidebar/AzurePortalSideBarViewModel");
import OAuth2Client = require("../../oAuth2/OAuth2Client");

class DeployFlyoutViewModel {

    constructor(private opt: { sideBarVm: AzurePortalSideBarViewModel, oauth: OAuth2Client }) {

    }

    private subscriptionid = new inputSelectElementLayout({ label: "Azure Subscription", name: "subid", placeholder: "loading..." });
    private location = new inputSelectElementLayout({ label: "Resource Group Location", name: "rg_loc", placeholder: "loading...", items: ["Central US", "East US", "East US 2", "North Central US", "South Central US", "West US", "North Europe", "West Europe", "East Asia", "Southeast Asia", "Japan East", "Japan West", "Brazil South", "Australia East", "Australia Southeast"].map(l=> { return { text: l, value: l, disabled: false } }) });
    private keyVaultName = new inputFormElementLayout({ label: "KeyVault Name", name: "vaultName", placeholder: "https://{vaultname}.vault.azure.net" })
    private rg = new inputFormElementLayout({ label: "Resource Group", name: "resourceGroup", placeholder: "keyvault-demo-rg" });
    private objid = new inputFormElementLayout({ label: "Object Id", name: "objid", placeholder: "keyvault-demo-rg", readonly: true })
    private tenantid = new inputFormElementLayout({ label: "TenantId", name: "tenantid", placeholder: "keyvault-demo-rg", readonly: true })
    private deployAzureButton = new inputSubmitElementLayout({ submitText: "Deploy Keyvault", action: this.deployAzureKeyvault.bind(this) });

    deployAzureKeyvault() {
        if (!this.opt.oauth.oauthResult.access_token) {
            window.open("https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-quickstart-templates%2Fmaster%2F101-create-key-vault%2Fazuredeploy.json");

        } else {
            if (!this.subscriptionid.value() || !this.location.value() || !this.rg.value() || !this.keyVaultName.value()) {
                alert("missing fields");
                return;
            }
            console.log(this.subscriptionid.value());
            console.log(this.location.value());
            console.log(this.rg.value());

            if (confirm("Sure you want to create resource group '" + this.rg.value() + "' on '" + this.subscriptionid.value() + "'")) {
                sendRequest("PUT", "https://management.azure.com/subscriptions/" + this.subscriptionid.value() + "/resourcegroups/" + this.rg.value() + "?api-version=2015-01-01",
                    this.opt.oauth.oauthResult.access_token,
                    (data) => {
                        if (confirm("Resource Group created with success, continue to deploy keyvault")) {
                            sendRequest("PUT", "https://management.azure.com/subscriptions/" + this.subscriptionid.value() + "/resourcegroups/" + this.rg.value() + "/providers/microsoft.resources/deployments/" + this.keyVaultName.value() + "-deployment?api-version=2015-01-01",
                                this.opt.oauth.oauthResult.access_token,
                                deployment=> {
                                    alert("Deployment succes");

                                }, err=> {
                                    alert("Sorry, something went wronge");

                                }, JSON.stringify({
                                    properties: {
                                        mode: "Incremental",
                                        templateLink: {
                                            uri: "https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/101-create-key-vault/azuredeploy.json",
                                            contentVersion: "1.0.0.0",
                                        },
                                        parameters: {
                                            "keyVaultName": {
                                                "value": this.keyVaultName.value()
                                            },
                                            "location": {
                                                "value": this.location.value()
                                            },
                                            "tenantId": {
                                                "value": this.tenantid.value()
                                            },
                                            "objectId": {
                                                "value": this.objid.value()
                                            },
                                            "keysPermissions": {
                                                "value": ["all"]
                                            },
                                            "secretsPermissions": {
                                                "value": ["all"]
                                            },
                                            "skuName": {
                                                "value": "Standard"
                                            },
                                            "enableVaultForDeployment": {
                                                "value": true
                                            },
                                            "enableVaultForDiskEncryption": {
                                                "value": true
                                            }
                                        }
                                    }
                                }));

                        }

                    }, err=> {

                        alert("Sorry, something went wronge");

                    }, JSON.stringify({
                        location: this.location.value(),
                        tags: {
                        }
                    }));
            }

        }
    }


    getElements() {
        this.opt.oauth.load(window.sessionStorage);
        console.log(this.opt.oauth.oauthResult);
        if (this.opt.oauth.oauthResult.access_token) {
            console.log(JSON.parse(atob(this.opt.oauth.oauthResult.access_token.split(".")[1])));

            sendRequest("GET", "https://management.azure.com/subscriptions?api-version=2015-01-01", this.opt.oauth.oauthResult.access_token, (data) => {
                console.log(data);
                data.value.forEach(s=> {
                    this.subscriptionid.items.push({ text: s.displayName, disabled: s.state !== "Enabled", value: s.subscriptionId });
                });
               
                // this.subscriptionid.value(data.value[0].displayName);
            }, () => { });


        }



        console.log(JSON.parse(atob(this.opt.oauth.oauthResult.id_token.split(".")[1])));

        var acc = JSON.parse(atob(this.opt.oauth.oauthResult.id_token.split(".")[1]));
        this.objid.value(acc["oid"]);
        this.tenantid.value(acc["tid"]);
        if (this.opt.oauth.oauthResult.access_token)
            return [this.keyVaultName, this.subscriptionid, this.rg, this.location, this.tenantid, this.objid, this.deployAzureButton];
        return [this.tenantid, this.objid, this.deployAzureButton];
    }
}

export = DeployFlyoutViewModel;