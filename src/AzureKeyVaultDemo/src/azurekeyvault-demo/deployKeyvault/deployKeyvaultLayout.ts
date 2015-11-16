

import ko = require("knockout");
import koLayout = require("si-portal-framework/koExtensions/koLayout");
import setDefaultProperties = require("si-portal-framework/utils/setDefaultProperties");
import "template!./templates/deployTemplate.html";

class deployKeyvaultLayout implements koLayout {

  

    constructor(options?) {
        setDefaultProperties(this, options, {  });
    }

  
   

    templateOptions() {

        return {
            name: "deployTemplate",
            data: this,
        };
    }

}

export =deployKeyvaultLayout;