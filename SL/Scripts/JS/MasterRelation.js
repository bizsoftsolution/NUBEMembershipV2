define(["require", "exports", "knockout"], function (require, exports, ko) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var MASTERRELATION = /** @class */ (function () {
        function MASTERRELATION() {
            this.RELATION_CODE = ko.observable();
            this.RELATION_NAME = ko.observable();
            this.DELETED = ko.observable();
        }
        MASTERRELATION.toList = function () {
            if (MASTERRELATION._toList == undefined) {
                $.ajax({
                    url: 'http://localhost/MembershipTest/MasterRelation/tolist',
                    type: 'get',
                    async: false,
                    cache: false,
                    timeout: 30000,
                    error: function (err) {
                        console.log(err);
                    },
                    success: function (data) {
                        MASTERRELATION._toList = data;
                    },
                });
            }
            return MASTERRELATION._toList;
        };
        return MASTERRELATION;
    }());
});
//# sourceMappingURL=MasterRelation.js.map