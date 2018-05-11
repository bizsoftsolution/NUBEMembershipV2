import * as ko from "knockout";
import { AppLib } from "./AppLib";

class MASTERSTATE {
    STATE_CODE: KnockoutObservable<number>;
    STATE_NAME: KnockoutObservable<string>;
    DELETED: KnockoutObservable<boolean>;
    Country: KnockoutObservable<number>;
    constructor() {
        this.STATE_CODE = ko.observable<number>();
        this.STATE_NAME = ko.observable<string>();
        this.DELETED = ko.observable<boolean>();
        this.Country = ko.observable<number>();
    }


    private static _toList: MASTERSTATE[];
    static toList(): any {
        if (MASTERSTATE._toList == undefined) {
            $.ajax({
                url: AppLib.SLURL + 'Masterstate/tolist',
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
export { MASTERSTATE }