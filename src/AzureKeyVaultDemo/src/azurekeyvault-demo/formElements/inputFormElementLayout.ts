

import ko = require("knockout");
import koLayout = require("si-portal-framework/koExtensions/koLayout");
import setDefaultProperties = require("si-portal-framework/utils/setDefaultProperties");
import "template!./templates/inputFormElementTemplate.html";

class inputFormElementLayout implements koLayout {

    label: KnockoutObservable<string>;
    name: KnockoutObservable<string>;
    value: KnockoutObservable<string>;
    disabled: KnockoutObservable<boolean>;
    placeholder: KnockoutObservable<string>;
    readonly: KnockoutObservable<boolean>;

    constructor(options?) {
        setDefaultProperties(this, options, { label: undefined, name: undefined, value: undefined, disabled: false, placeholder: undefined,readonly:false });
    }

    balloonVisible = ko.observable(true);
    validationState = ko.observable(1);
    dirty = ko.observable(false);
    focused = ko.observable(false);

    getValueUpdateTrigger() {
        return "keyup";
    }

    templateOptions() {

        return {
            name: "inputFormElementTemplate",
            data: this,
        };
    }

}

export =inputFormElementLayout;