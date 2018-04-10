import * as ko from "knockout";
import * as $ from "jquery";
import "jqueryui";
import {MASTERSTATE} from "./MASTERSTATE";
import "knockout-jqAutocomplete";
import { Membership } from "./Membership/Membership";

class HelloViewModel {
    language: KnockoutObservable<string>;
    framework: KnockoutObservable<string>;
    m1: Membership;    
    stateList: MASTERSTATE[];
    
    constructor(language: string, framework: string) {
        this.language = ko.observable(language);
        this.framework = ko.observable(framework);               
        this.m1 = new Membership();
        this.stateList = MASTERSTATE.toList();        
    }
    btnOk_click(): void {
        alert(this.m1.MEMBER_NAME);
        console.log(this.m1);  
        console.log(this);
        console.log(ko.toJS(this));
        console.log(ko.toJSON(this.m1));
    }

}
var mm= ko.applyBindings(new HelloViewModel("TypeScript", "Knockout"));
$("#txtBox01").datepicker();

ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var options = allBindingsAccessor().datepickerOptions || {},
            $el = $(element);

        //initialize datepicker with some optional options
        $el.datepicker(options);

        //handle the field changing
        ko.utils.registerEventHandler(element, "change", function () {
            var observable = valueAccessor();
            observable($el.datepicker("getDate"));
        });

        //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $el.datepicker("destroy");
        });

    },
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        $(element).datepicker("setValue", value);
    }
};
