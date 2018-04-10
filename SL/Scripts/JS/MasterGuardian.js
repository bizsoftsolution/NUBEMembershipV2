define(["require", "exports", "knockout"], function (require, exports, ko) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var MASTERGUARDIAN = /** @class */ (function () {
        function MASTERGUARDIAN() {
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
            this.MOBILE = ko.observable();
            this.PHONE = ko.observable();
            this.USER_CODE = ko.observable();
            this.ENTRY_DATE = ko.observable();
            this.ENTRY_TIME = ko.observable();
        }
        MASTERGUARDIAN.toList = function () {
            if (MASTERGUARDIAN._toList == undefined) {
                $.ajax({
                    url: 'http://localhost/MembershipTest/MasterCity/tolist',
                    type: 'get',
                    async: false,
                    cache: false,
                    timeout: 30000,
                    error: function (err) {
                        console.log(err);
                    },
                    success: function (data) {
                        MASTERGUARDIAN._toList = data;
                    },
                });
            }
            return MASTERGUARDIAN._toList;
        };
        return MASTERGUARDIAN;
    }());
    exports.MASTERGUARDIAN = MASTERGUARDIAN;
});
//# sourceMappingURL=MasterGuardian.js.map