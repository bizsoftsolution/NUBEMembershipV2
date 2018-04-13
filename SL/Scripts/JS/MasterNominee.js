define(["require", "exports", "knockout"], function (require, exports, ko) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var MASTERNOMINEE = /** @class */ (function () {
        function MASTERNOMINEE() {
            this.MEMBER_CODE = ko.observable();
            this.ICNO_OLD = ko.observable();
            this.ICNO_NEW = ko.observable();
            this.NAME = ko.observable();
            this.SEX = ko.observable();
            this.AGE = ko.observable();
            this.RELATION_CODE = ko.observable();
            this.ADDRESS1 = ko.observable();
            this.ADDRESS2 = ko.observable();
            this.ADDRESS3 = ko.observable();
            this.CITY_CODE = ko.observable();
            this.STATE_CODE = ko.observable();
            this.COUNTRY = ko.observable();
            this.ZIPCODE = ko.observable();
            this.PHONE = ko.observable();
            this.MOBILE = ko.observable();
            this.USER_CODE = ko.observable();
            this.ENTRY_DATE = ko.observable();
            this.ENTRY_TIME = ko.observable();
        }
        return MASTERNOMINEE;
    }());
    exports.MASTERNOMINEE = MASTERNOMINEE;
});
//# sourceMappingURL=MasterNominee.js.map