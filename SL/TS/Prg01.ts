import * as ko from "knockout";
import * as $ from "jquery";
import "jqueryui";
import "js/masterstate";
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
