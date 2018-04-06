
class MASTERSTATE {
    STATE_CODE: KnockoutObservable<number>;
    STATE_NAME: KnockoutObservable<string>;
    DELETED: KnockoutObservable<boolean>;
    Country: KnockoutObservable<number>;
    constructor() {
        this.STATE_CODE = ko.observable();
        this.STATE_NAME = ko.observable();
        this.DELETED = ko.observable();
        this.Country = ko.observable();
    }


    static _toList: MASTERSTATE[];
    static toList(): any {        
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
