define(["require", "exports", "knockout", "jquery", "./MASTERSTATE", "./Membership/Membership", "jqueryui", "knockout-jqAutocomplete"], function (require, exports, ko, $, MASTERSTATE_1, Membership_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var HelloViewModel = /** @class */ (function () {
        function HelloViewModel(language, framework) {
            this.language = ko.observable(language);
            this.framework = ko.observable(framework);
            this.m1 = new Membership_1.Membership();
            this.stateList = MASTERSTATE_1.MASTERSTATE.toList();
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
    ko.bindingHandlers.datepicker = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var options = allBindingsAccessor().datepickerOptions || {}, $el = $(element);
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
});
//# sourceMappingURL=Prg01.js.map