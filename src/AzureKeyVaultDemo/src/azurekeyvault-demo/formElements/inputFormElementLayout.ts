

import ko = require("knockout");
import koLayout = require("si-portal-framework/koExtensions/koLayout");
import baseFormElement = require("./baseFormElement");
import setDefaultProperties = require("si-portal-framework/utils/setDefaultProperties");
                                                      
import "template!./templates/inputFormElementTemplate.html";


class inputFormElementLayout extends baseFormElement<string> {

    label: KnockoutObservable<string>;
    name: KnockoutObservable<string>;
    disabled: KnockoutObservable<boolean>;
    placeholder: KnockoutObservable<string>;
    readonly: KnockoutObservable<boolean>;
   

    constructor(options?) {
        super(options, "inputFormElementTemplate");
        setDefaultProperties(this, options, { label: undefined, name: undefined, value: undefined, disabled: false, placeholder: undefined,readonly:false });
    }

    balloonVisible = ko.observable(true);
    validationState = ko.observable(0);
    dirty = ko.observable(false);
    focused = ko.observable(false);    

}

export =inputFormElementLayout;