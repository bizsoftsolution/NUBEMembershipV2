define(["require", "exports", "knockout"], function (require, exports, ko) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var MASTERCOUNTRY = /** @class */ (function () {
        function MASTERCOUNTRY() {
            this.ID = ko.observable();
            this.CountryName = ko.observable();
        }
        MASTERCOUNTRY.toList = function () {
            if (MASTERCOUNTRY._toList == undefined) {
                $.ajax({
                    url: 'http://localhost/MembershipTest/MasterCountry/tolist',
                    type: 'get',
                    async: false,
                    cache: false,
                    timeout: 30000,
                    error: function (err) {
                        console.log(err);
                    },
                    success: function (data) {
                        MASTERCOUNTRY._toList = data;
                    },
                });
            }
            return MASTERCOUNTRY._toList;
        };
        return MASTERCOUNTRY;
    }());
    exports.MASTERCOUNTRY = MASTERCOUNTRY;
});
//# sourceMappingURL=MasterCountry.js.map