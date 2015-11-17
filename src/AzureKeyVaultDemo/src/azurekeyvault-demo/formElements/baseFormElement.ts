
import ko = require("knockout");
import koLayout = require("si-portal-framework/koExtensions/koLayout");
import setDefaultProperties = require("si-portal-framework/utils/setDefaultProperties");
import isDefined = require("si-portal-framework/utils/isDefined");
import "template!./templates/formElementErrorTemplate.html";
import "knockout.validation";

ko.validation.init({
    decorateElement: true,
    errorElementClass: 'err',
    messageTemplate:"formElementErrorTemplate" 
});

//ko.validation.insertValidationMessage = function (element) {
//    var div = document.createElement('div');
//    div.className = "si-control si-dockedballoon si-dockedballoon-validation si-bg-default si-bg-invalid";

  
//    element.parentNode.insertBefore(div, element.nextSibling);
//    return div;
//};  

class baseFormElement<T> implements koLayout {

    private templateName: string;
    value: KnockoutObservable<T>;
    required: KnockoutObservable<boolean>;
    valueUpdateTrigger: KnockoutObservable<string>;

   


    constructor(options: { value: T | KnockoutObservable<T>, templateName?: string }, name?: string) {
        setDefaultProperties(this, options, { value: undefined, required: false, valueUpdateTrigger:"input", });
        this.templateName = options.templateName || name;
       
    }

    isModified = ko.computed({
        read: () => this.value.isModified && this.value.isModified(),
        deferEvaluation: true
    });
    isValid = ko.computed({ read: () => !this.isModified() || (!isDefined(this.value.isValid) || this.value.isValid()), deferEvaluation: true }).extend({ rateLimit: 0 });
    isInvalid = ko.computed({ read: () => !this.isValid(), deferEvaluation: true }).extend({ rateLimit: 0 });
   
   


    showInvalid = ko.computed({
        deferEvaluation: true,
        read: () => {
            if (this.showErrors() && this.value.isValid && !this.value.isValid())
                return true;
            return false;
        }
    });
    
    private _showErrorsFlag = ko.observable(false);
    showErrors = ko.computed({
        read: () => {
            if (this._showErrorsFlag() && this.value.isValid && !this.value.isValid()) {
                return true;
            }
            return false;
        },
        write: (flag)=>{
            this._showErrorsFlag(flag);
        },
        deferEvaluation :true,
    });

    extend(ko) {
        this.value.extend(ko);
        return this;
    }

    templateOptions() {

        return {
            name: this.templateName,
            data: this,
        };
    }
   
}
export = baseFormElement;