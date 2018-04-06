define(["require", "exports", "knockout"], function (require, exports, ko) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var MASTERMEMBER = (function () {
        function MASTERMEMBER() {
            this.MEMBER_CODE = ko.observable();
            this.MEMBER_NAME = ko.observable();
            this.MEMBER_ID = ko.observable();
            this.MEMBER_TITLE = ko.observable();
            this.MEMBER_INITIAL = ko.observable();
            this.BF_NO = ko.observable();
            this.ICNO_OLD = ko.observable();
            this.ICNO_NEW = ko.observable();
            this.BANK_CODE = ko.observable();
            this.BRANCH_CODE = ko.observable();
            this.ADDRESS1 = ko.observable();
            this.ADDRESS2 = ko.observable();
            this.ADDRESS3 = ko.observable();
            this.CITY_CODE = ko.observable();
            this.STATE_CODE = ko.observable();
            this.COUNTRY = ko.observable();
            this.ZIPCODE = ko.observable();
            this.PHONE = ko.observable();
            this.MOBILE = ko.observable();
            this.EMAIL = ko.observable();
            this.DATEOFJOINING = ko.observable();
            this.DATEOFBIRTH = ko.observable();
            this.AGE_IN_YEARS = ko.observable();
            this.DATEOFEMPLOYMENT = ko.observable();
            this.MEMBERTYPE_CODE = ko.observable();
            this.SEX = ko.observable();
            this.RACE_CODE = ko.observable();
            ;
        }
        return MASTERMEMBER;
    }());
    exports.MASTERMEMBER = MASTERMEMBER;
});
//# sourceMappingURL=MasterMember.js.map