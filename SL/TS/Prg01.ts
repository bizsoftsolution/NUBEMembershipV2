import * as ko from "knockout";
import * as $ from "jquery";
import "jqueryui";
import "js/Membership/Membership";

class HelloViewModel {
    language: KnockoutObservable<string>;
    framework: KnockoutObservable<string>;
    m1: Membership;
    
    constructor(language: string, framework: string) {
        this.language = ko.observable(language);
        this.framework = ko.observable(framework);               
        this.m1 = new Membership();
    }
    btnOk_click(): void {
        alert(this.m1.Member_Name);
        $.get('', (data) => {
            console.log(data);
        });
        console.log(this.m1);
    }
}
ko.applyBindings(new HelloViewModel("TypeScript", "Knockout"));
$("#txtBox01").datepicker();

