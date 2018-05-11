define(["require", "exports", "knockout", "./AppLib"], function (require, exports, ko, AppLib_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var MASTERBRANCH = /** @class */ (function () {
        function MASTERBRANCH() {
            this.BANKBRANCH_CODE = ko.observable();
            this.BANK_CODE = ko.observable();
            this.BANKBRANCH_NAME = ko.observable();
            this.BANKBRANCH_USERCODE = ko.observable();
            this.BANKBRANCH_ADDRESS1 = ko.observable();
            this.BANKBRANCH_ADDRESS2 = ko.observable();
            this.BANKBRANCH_ADDRESS3 = ko.observable();
            this.BANKBRANCH_CITY_CODE = ko.observable();
            this.BANKBRANCH_STATE_CODE = ko.observable();
            this.BANKBRANCH_COUNTRY = ko.observable();
            this.BANKBRANCH_ZIPCODE = ko.observable();
            this.BANKBRANCH_PHONE1 = ko.observable();
            this.NUBE_BRANCH_CODE = ko.observable();
            this.DELETED = ko.observable();
            this.HEAD_QUARTERS = ko.observable();
        }
        MASTERBRANCH.toList = function (BankCode) {
            $.ajax({
                url: AppLib_1.AppLib.SLURL + 'MasterBranch/tolist?Bank_Code=' + BankCode,
                type: 'get',
                async: false,
                cache: false,
                timeout: 30000,
                error: function (err) {
                    console.log(err);
                },
                success: function (data) {
                    MASTERBRANCH._toList = data;
                },
            });
            return MASTERBRANCH._toList;
        };
        return MASTERBRANCH;
    }());
    exports.MASTERBRANCH = MASTERBRANCH;
});
//# sourceMappingURL=MasterBranch.js.map