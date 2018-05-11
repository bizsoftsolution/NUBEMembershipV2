define(["require", "exports", "knockout", "./AppLib"], function (require, exports, ko, AppLib_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var MASTERRACE = /** @class */ (function () {
        function MASTERRACE() {
            this.RACE_CODE = ko.observable();
            this.RACE_NAME = ko.observable();
            this.RACE_SHORTCODE = ko.observable();
            this.DELETED = ko.observable();
        }
        MASTERRACE.toList = function () {
            if (MASTERRACE._toList == undefined) {
                $.ajax({
                    url: AppLib_1.AppLib.SLURL + 'MasterRace/tolist',
                    type: 'get',
                    async: false,
                    cache: false,
                    timeout: 30000,
                    error: function (err) {
                        console.log(err);
                    },
                    success: function (data) {
                        MASTERRACE._toList = data;
                    },
                });
            }
            return MASTERRACE._toList;
        };
        return MASTERRACE;
    }());
    exports.MASTERRACE = MASTERRACE;
});
//# sourceMappingURL=MasterRace.js.map