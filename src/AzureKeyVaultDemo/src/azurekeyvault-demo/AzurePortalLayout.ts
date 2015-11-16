

import WebContainerLayout = require("si-portal-framework/siPortal/rootLayouts/webContainerLayout");
import SIStackLayout = require("si-portal-framework/siPortal/stackLayout/siStackLayout");
import SIStackLayoutOrientation = require("si-portal-framework/siPortal/stackLayout/siStackLayoutOrientation");
import AzurePortalSideBarLayout = require("./sidebar/AzurePortalSideBarLayout");
import FixedLayout = require("../fixedLayout/FixedLayout");
import siStackLayoutItem = require("si-portal-framework/siPortal/stackLayout/siStackLayoutItem");
import startupModalLayout = require("./startupModal/startupModalLayout");
import OAuth2Client = require("../oAuth2/OAuth2Client");
import User = require("./User");

var fixedLayout = new FixedLayout({
    elements: []
});

class AzurePortalLayout extends WebContainerLayout {
    private user: User;
    constructor(opt: { oauth: OAuth2Client }) {
        super({
            layout: new SIStackLayout({
                classes: ["portal-main"],
                orientation: SIStackLayoutOrientation.horizontal,
                elements: [new AzurePortalSideBarLayout(
                    {
                        collapsed: true,
                        favorites: {
                            favorites: [
                                { opensExternal: true, label: "Test 1", uri: "#/Test1" },
                                { opensExternal: true, label: "Test 2", uri: "#/Test2" },
                                { opensExternal: true, label: "Test 3", uri: "#/Test3" },
                                { opensExternal: true, label: "Test 4", uri: "#/Test4" },
                                { opensExternal: true, label: "Test 5", uri: "#/Test5" },
                            ]
                        }
                    }
                ),
                    new siStackLayoutItem({
                        item: fixedLayout
                })
                ]
            })
        });
        if (!opt.oauth.isAuthenticated()) {
            fixedLayout.elements.push(new startupModalLayout());
        }
        this.user = new User(opt);
        console.log(this.user.getEmails());
    }

    

}

export =  AzurePortalLayout;