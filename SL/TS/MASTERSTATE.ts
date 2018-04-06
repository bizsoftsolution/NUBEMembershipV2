
class MASTERSTATE {
    STATE_CODE: number;
    STATE_NAME: string;
    DELETED: boolean;
    Country: number;
    constructor() {
        this.STATE_CODE = 0;
        this.STATE_NAME = '';
        this.DELETED = false;
        this.Country = null;
    }
    static _toList: MASTERSTATE[];

    static toList(): any {
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
                error: (err) => {
                    console.log(err);
                },
                success: (data) => {
                    MASTERSTATE._toList = data;
                },
            });
        }
        
        return MASTERSTATE._toList;
    }
}
