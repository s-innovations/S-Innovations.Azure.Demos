

import ko = require("knockout");
import koLayout = require("si-portal-framework/koExtensions/koLayout");
import setDefaultProperties = require("si-portal-framework/utils/setDefaultProperties");
import "template!./templates/inputSubmitElementTemplate.html";

class inputSubmitElementLayout implements koLayout {

    submitText: KnockoutObservable<string>;

    action: () => void;

    constructor(options?) {
        setDefaultProperties(this, options, { submitText: "create" });
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