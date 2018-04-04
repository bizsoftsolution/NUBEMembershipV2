define(["require", "exports", "knockout", "jquery", "jqueryui", "js/Membership/Membership"], function (require, exports, ko, $) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var HelloViewModel = /** @class */ (function () {
        function HelloViewModel(language, framework) {
            this.language = ko.observable(language);
            this.framework = ko.observable(framework);
            this.m1 = new Membership();
        }
        HelloViewModel.prototype.btnOk_click = function () {
            alert(this.m1.Member_Name);
            $.get('', function (data) {
                console.log(data);
            });
            console.log(this.m1);
        };
        return HelloViewModel;
    }());
    ko.applyBindings(new HelloViewModel("TypeScript", "Knockout"));
    $("#txtBox01").datepicker();
});
//# sourceMappingURL=Prg01.js.map