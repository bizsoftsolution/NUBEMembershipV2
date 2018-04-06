var MASTERSTATE = /** @class */ (function () {
    function MASTERSTATE() {
        this.STATE_CODE = ko.observable();
        this.STATE_NAME = ko.observable();
        this.DELETED = ko.observable();
        this.Country = ko.observable();
    }
    MASTERSTATE.toList = function () {
        if (MASTERSTATE._toList == undefined) {
            $.ajax({
                url: 'http://localhost/MembershipTest/Masterstate/tolist',
                type: 'get',
                async: false,
                cache: false,
                timeout: 30000,
                error: function (err) {
                    console.log(err);
                },
                success: function (data) {
                    MASTERSTATE._toList = data;
                },
            });
        }
        return MASTERSTATE._toList;
    };
    return MASTERSTATE;
}());
//# sourceMappingURL=MASTERSTATE.js.map