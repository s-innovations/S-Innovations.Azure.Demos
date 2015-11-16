


import ko = require("knockout");
import koLayout = require("si-portal-framework/koExtensions/koLayout");
import setDefaultProperties = require("si-portal-framework/utils/setDefaultProperties");

class FixedLayout implements koLayout {

    elements: KnockoutObservableArray<koLayout>;

    private _div: HTMLDivElement;
    constructor(opt: { elements: Array<koLayout> }) {
        setDefaultProperties(this, opt, { elements: [] });
        this._div = document.createElement("div");
        this._div.style.backgroundColor = "#2e80ab";
        this._div.classList.add("fixed-layout");
        this._div.dataset["bind"] = "template : { foreach : elements }";
        this._div.innerHTML = "<!-- koLayout: $data -->";
    }

    templateOptions() {

        return {
            nodes: [this._div],
            data: this,
            as: "$azurePortalSideBarLayout"
        };
    }
}

export =FixedLayout;