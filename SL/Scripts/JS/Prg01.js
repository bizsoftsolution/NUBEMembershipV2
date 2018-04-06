define(["require", "exports", "knockout", "jquery", "./Membership/Membership", "jqueryui", "js/masterstate", "knockout-jqAutocomplete"], function (require, exports, ko, $, Membership_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var HelloViewModel = /** @class */ (function () {
        function HelloViewModel(language, framework) {
            this.language = ko.observable(language);
            this.framework = ko.observable(framework);
            this.m1 = new Membership_1.Membership();
            this.stateList = MASTERSTATE.toList();
        }
        HelloViewModel.prototype.btnOk_click = function () {
            alert(this.m1.MEMBER_NAME);
            console.log(this.m1);
            console.log(this);
            console.log(ko.toJS(this));
            console.log(ko.toJSON(this.m1));
        };
        return HelloViewModel;
    }());
    var mm = ko.applyBindings(new HelloViewModel("TypeScript", "Knockout"));
    $("#txtBox01").datepicker();
});
//# sourceMappingURL=Prg01.js.map