var MASTERSTATE = /** @class */ (function () {
    function MASTERSTATE() {
        this.STATE_CODE = 0;
        this.STATE_NAME = '';
        this.DELETED = false;
        this.Country = null;
    }
    MASTERSTATE.toList = function () {
        //let i: number = 0;
        //while (MASTERSTATE._toList == undefined && i++ < 5) {
        //    console.log("Try to get the state list #: ", i);
        //    $.get("http://localhost/MembershipTest/Masterstate/tolist", { async: false }, (data) => {
        //        MASTERSTATE._toList = data;
        //    });
        //}
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