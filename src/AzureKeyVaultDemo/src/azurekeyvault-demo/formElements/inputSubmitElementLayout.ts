

import ko = require("knockout");
import koLayout = require("si-portal-framework/koExtensions/koLayout");
import setDefaultProperties = require("si-portal-framework/utils/setDefaultProperties");
import isDefined = require("si-portal-framework/utils/isDefined");
import "template!./templates/inputSubmitElementTemplate.html";
import "knockout.validation";


class inputSubmitElementLayout implements koLayout {

    submitText: KnockoutObservable<string>;
    valid: KnockoutObservable<boolean>;
    

    action: () => void;

    constructor(options?) {
        setDefaultProperties(this, options, { submitText: "create", valid: true });
        this.action = options.action ? options.action : () => { };
       
    }

  
    showExternalLinkAdornment = ko.observable(true);
   

    templateOptions() {

        return {
            name: "inputSubmitElementTemplate",
            data: this,
        };
    }

    
}

export =inputSubmitElementLayout;