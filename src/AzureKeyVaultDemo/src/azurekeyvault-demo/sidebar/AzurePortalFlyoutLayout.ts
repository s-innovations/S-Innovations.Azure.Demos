
import ko = require("knockout");
import koLayout = require("si-portal-framework/koExtensions/koLayout");
import siStackLayout = require('si-portal-framework/siPortal/stackLayout/siStackLayout');
import siStackLayoutOrientation = require('si-portal-framework/siPortal/stackLayout/siStackLayoutOrientation');

import deployKeyvaultLayout = require("../deployKeyvault/deployKeyvaultLayout");
import OAuth2Client = require("../../oAuth2/OAuth2Client");
import AzurePortalSideBarViewModel = require("./AzurePortalSideBarViewModel");
import DeployFlyoutViewModel = require("../deployKeyvault/DeployFlyoutViewModel");
import AzurePortalFlyoutViewModel = require("./AzurePortalFlyoutViewModel");

import HashObservable = require("../../oAuth2/HashObservable");


var oauth = new OAuth2Client({ url: "https://login.microsoftonline.com/common/oauth2/authorize", storagePrefix: "__akv_oauth2" });

class AzurePortalFlyoutLayout extends siStackLayout {


    constructor(opt: { sideBarVm: AzurePortalSideBarViewModel }) {
        super({ orientation: siStackLayoutOrientation.vertical });


        ko.computed(() => {
            this.elements.removeAll();
            if ("#deployazure" === HashObservable.hash()) {
                opt.sideBarVm.isFlyoutOpen(true);
                var subscribeOne = opt.sideBarVm.isFlyoutOpen.subscribe(v=> {
                    subscribeOne.dispose();
                    window.location.hash = "#";
                });
                this.elements.push.apply(this.elements, new DeployFlyoutViewModel({ sideBarVm: opt.sideBarVm, oauth: oauth }).getElements());
            } else {

                this.elements.push.apply(this.elements, new AzurePortalFlyoutViewModel({ sideBarVm: opt.sideBarVm, oauth: oauth }).getElements());
            }
        });
    }
}

export =AzurePortalFlyoutLayout;