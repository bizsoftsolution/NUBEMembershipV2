define(["require", "exports", "knockout", "jquery", "./MasterMember", "./MasterBank", "./MasterBranch", "./MasterRace", "./MASTERCITY", "./MASTERSTATE", "./MasterCountry", "jqueryui", "knockout-jqAutocomplete"], function (require, exports, ko, $, MasterMember_1, MasterBank_1, MasterBranch_1, MasterRace_1, MASTERCITY_1, MASTERSTATE_1, MasterCountry_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Joinup = /** @class */ (function () {
        function Joinup() {
            this.data = new MasterMember_1.MASTERMEMBER();
            this.bankList = MasterBank_1.MASTERBANK.toList();
            this.branchList = MasterBranch_1.MASTERBRANCH.toList();
            this.raceList = MasterRace_1.MASTERRACE.toList();
            this.cityList = MASTERCITY_1.MASTERCITY.toList();
            this.stateList = MASTERSTATE_1.MASTERSTATE.toList();
            this.countryList = MasterCountry_1.MASTERCOUNTRY.toList();
        }
        Joinup.prototype.btnSave = function () {
            var d = ko.toJS(this.data);
            console.log(d);
            $.post('./MasterMember/Insert', d, function (r) {
                console.log(r);
            });
        };
        return Joinup;
    }());
    ko.applyBindings(new Joinup());
});
//# sourceMappingURL=Joinup.js.map