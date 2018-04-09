import * as ko from "knockout";

class MASTERCITY {
    CITY_CODE: KnockoutObservable<number>;
    CITY_NAME: KnockoutObservable<string>;
    STATE_CODE: KnockoutObservable<number>;
    DELETED: KnockoutObservable<boolean>;
    constructor() {
        this.CITY_CODE = ko.observable<number>();
        this.CITY_NAME = ko.observable<string>();
        this.STATE_CODE = ko.observable<number>();
        this.DELETED = ko.observable<boolean>();
    }

    private static _toList: MASTERCITY[];
    static toList(): any {
        if (MASTERCITY._toList == undefined) {
            $.ajax({
                url: 'http://localhost/MembershipTest/MasterCity/tolist',
                type: 'get',
                async: false,
                cache: false,
                timeout: 30000,
                error: (err) => {
                    console.log(err);
                },
                success: (data) => {
                    MASTERCITY._toList = data;
                },
            });
        }

        return MASTERCITY._toList;
    }
}

export { MASTERCITY };