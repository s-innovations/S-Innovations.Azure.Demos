


import ko = require("knockout");
import koLayout = require("si-portal-framework/koExtensions/koLayout");
import AzurePortalSideBarFavoritesLayout = require("./AzurePortalSideBarFavoritesLayout");
import SideBarFavoritesViewModel = require("./sideBarFavorites/SideBarFavoritesViewModel");
import AzurePortalSideBarViewModel = require("./AzurePortalSideBarViewModel"); 
import AzurePortalSideBarLayoutOptions = require("./AzurePortalSideBarLayoutOptions");
import AzurePortalFlyoutLayout = require("./AzurePortalFlyoutLayout");

import "template!./templates/sidebarTemplate.html";
import "si-portal-framework/koExtensions/singleClickBindingHandler";

class AzurePortalSideBarLayout implements koLayout {
                                                                

    vm: AzurePortalSideBarViewModel;
    flyoutLayout: AzurePortalFlyoutLayout;
    favoritesLayout: AzurePortalSideBarFavoritesLayout; 
  
    constructor(options: AzurePortalSideBarLayoutOptions) {

        var sideBarFavoritesViewModel = new SideBarFavoritesViewModel(options.favorites);
        this.vm = new AzurePortalSideBarViewModel({
            collapsed: options.collapsed,
            favorites: sideBarFavoritesViewModel
        });
        this.favoritesLayout = new AzurePortalSideBarFavoritesLayout({ sideBarFavoritesViewModel: sideBarFavoritesViewModel });
        this.flyoutLayout = new AzurePortalFlyoutLayout({ sideBarVm:this.vm});      
    }


    templateOptions() {

        return {
            name: "sidebarTemplate",
            data: this,
            as: "$azurePortalSideBarLayout" 
        };
    }



}

export = AzurePortalSideBarLayout;