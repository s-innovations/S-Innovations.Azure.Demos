

import ko = require("knockout");
import koLayout = require("si-portal-framework/koExtensions/koLayout");
import setDefaultProperties = require("si-portal-framework/utils/setDefaultProperties");
import "template!./templates/inputSelectElementTemplate.html";

class inputSelectElementLayout implements koLayout {

    label: KnockoutObservable<string>;
    name: KnockoutObservable<string>;
    value: KnockoutObservable<string>;
    disabled: KnockoutObservable<boolean>;
    placeholder: KnockoutObservable<string>;
    readonly: KnockoutObservable<boolean>;

    currentItemText: KnockoutObservable<string>
    items: KnockoutObservableArray<any>;

    constructor(options?) {
        setDefaultProperties(this, options, { items: [], label: undefined, name: undefined, value: "", disabled: false, placeholder: undefined, readonly: false });
        this.currentItemText = ko.computed(() => {
            var value = this.value();
            var items = this.items();
            if (items.length) {
                for (var i = 0; i < items.length; i++) {
                    if (items[i].value === value) {
                        return items[i].text;
                    }
                }
                this.value(items[0].value);
                return items[0].text;
            }
            return "loading ...";

        }).extend({ rateLimit: 0 });
    }

    balloonVisible = ko.observable(true);
    validationState = ko.observable(1);
    dirty = ko.observable(false);
    focused = ko.observable(false);

    syncSelectedIndex() {
        console.log("aaa");
        console.log(this.value());
    }
   

    templateOptions() {

        return {
            name: "inputSelectElementTemplate",
            data: this,
        };
    }

}

export =inputSelectElementLayout;