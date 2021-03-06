﻿import * as ko from "knockout"
import { AppLib } from "./AppLib";

class MASTERCOUNTRY {
    ID: KnockoutObservable<number>;
    CountryName: KnockoutObservable<string>;   
    constructor() {
        this.ID = ko.observable<number>();
        this.CountryName = ko.observable<string>();      
    }

    private static _toList: MASTERCOUNTRY[];
    static toList(): any {
        if (MASTERCOUNTRY._toList == undefined) {
            $.ajax({
                url: AppLib.SLURL + 'MasterCountry/tolist',
                type: 'get',
                async: false,
                cache: false,
                timeout: 30000,
                error: (err) => {
                    console.log(err);
                },
                success: (data) => {
                    MASTERCOUNTRY._toList = data;
                },
            });
        }

        return MASTERCOUNTRY._toList;
    }

}
export { MASTERCOUNTRY };