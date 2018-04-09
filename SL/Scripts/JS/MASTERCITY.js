define(["require", "exports", "knockout"], function (require, exports, ko) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var MASTERCITY = /** @class */ (function () {
        function MASTERCITY() {
            this.CITY_CODE = ko.observable();
            this.CITY_NAME = ko.observable();
            this.STATE_CODE = ko.observable();
            this.DELETED = ko.observable();
        }
        MASTERCITY.toList = function () {
            if (MASTERCITY._toList == undefined) {
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
                        MASTERCITY._toList = data;
                    },
                });
            }
            return MASTERCITY._toList;
        };
        return MASTERCITY;
    }());
});
//# sourceMappingURL=MASTERCITY.js.map