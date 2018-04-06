import * as ko from "knockout"

class MASTERRACE {
    RACE_CODE: KnockoutObservable<number>;
    RACE_NAME: KnockoutObservable<string>;
    RACE_SHORTCODE: KnockoutObservable<string>;
    DELETED: KnockoutObservable<boolean>;    
    constructor() {
        this.RACE_CODE = ko.observable<number>();
        this.RACE_NAME = ko.observable<string>();
        this.RACE_SHORTCODE = ko.observable<string>();
        this.DELETED = ko.observable<boolean>();        
    }

    private static _toList: MASTERRACE[];
    static toList(): any {
        if (MASTERRACE._toList == undefined) {
            $.ajax({
                url: 'http://localhost/MembershipTest/MasterRace/tolist',
                type: 'get',
                async: false,
                cache: false,
                timeout: 30000,
                error: (err) => {
                    console.log(err);
                },
                success: (data) => {
                    MASTERRACE._toList = data;
                },
            });
        }

        return MASTERRACE._toList;
    }

}