define(["require", "exports", "knockout"], function (require, exports, ko) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var MASTERBANK = /** @class */ (function () {
        function MASTERBANK() {
            this.BANK_CODE = ko.observable();
            this.BANK_NAME = ko.observable();
            this.BANK_USERCODE = ko.observable();
            this.DELETED = ko.observable();
            this.HEADER_BANK_CODE = ko.observable();
        }
        MASTERBANK.toList = function () {
            if (MASTERBANK._toList == undefined) {
                $.ajax({
                    url: 'http://localhost/MembershipTest/MasterBank/tolist',
                    type: 'get',
                    async: false,
                    cache: false,
                    timeout: 30000,
                    error: function (err) {
                        console.log(err);
                    },
                    success: function (data) {
                        MASTERBANK._toList = data;
                    },
                });
            }
            return MASTERBANK._toList;
        };
        return MASTERBANK;
    }());
    exports.MASTERBANK = MASTERBANK;
});
//# sourceMappingURL=MasterBank.js.map