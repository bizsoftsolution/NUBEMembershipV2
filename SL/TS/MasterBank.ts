import * as ko from "knockout"
import { AppLib } from "./AppLib";

class MASTERBANK {
    BANK_CODE: KnockoutObservable<number>;
    BANK_NAME: KnockoutObservable<string>;
    BANK_USERCODE: KnockoutObservable<string>;
    DELETED: KnockoutObservable<boolean>;
    HEADER_BANK_CODE: KnockoutObservable<number>;
    constructor() {
        this.BANK_CODE = ko.observable<number>();
        this.BANK_NAME = ko.observable<string>();
        this.BANK_USERCODE = ko.observable<string>();
        this.DELETED = ko.observable<boolean>();
        this.HEADER_BANK_CODE = ko.observable<number>();
    }

    private static _toList: MASTERBANK[];
    static toList(): any {
        if (MASTERBANK._toList == undefined) {
            $.ajax({
                url: AppLib.SLURL + 'MasterBank/tolist',
                type: 'get',
                async: false,
                cache: false,
                timeout: 30000,
                error: (err) => {
                    console.log(err);
                },
                success: (data) => {
                    MASTERBANK._toList = data;
                },
            });
        }

        return MASTERBANK._toList;
    }

}

export { MASTERBANK };