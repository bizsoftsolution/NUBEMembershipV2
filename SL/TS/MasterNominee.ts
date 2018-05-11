import * as ko from "knockout";
import { AppLib } from "./AppLib";

class MASTERNOMINEE {
    MEMBER_CODE: KnockoutObservable<number>;
    ICNO_OLD: KnockoutObservable<string>;
    ICNO_NEW: KnockoutObservable<string>;
    NAME: KnockoutObservable<string>;
    SEX: KnockoutObservable<string>;
    AGE: KnockoutObservable<number>;
    RELATION_CODE: KnockoutObservable<number>;
    ADDRESS1: KnockoutObservable<string>;
    ADDRESS2: KnockoutObservable<string>;
    ADDRESS3: KnockoutObservable<string>;
    CITY_CODE: KnockoutObservable<number>;
    STATE_CODE: KnockoutObservable<number>;
    COUNTRY: KnockoutObservable<string>;
    ZIPCODE: KnockoutObservable<string>;
    PHONE: KnockoutObservable<string>;
    MOBILE: KnockoutObservable<string>;
    USER_CODE: KnockoutObservable<number>;
    ENTRY_DATE: KnockoutObservable<Date>;
    ENTRY_TIME: KnockoutObservable<Date>;              

    constructor() {
        this.MEMBER_CODE = ko.observable<number>();
        this.ICNO_OLD = ko.observable<string>();
        this.ICNO_NEW = ko.observable<string>();
        this.NAME = ko.observable<string>();
        this.SEX = ko.observable<string>();
        this.AGE = ko.observable<number>();
        this.RELATION_CODE = ko.observable<number>();
        this.ADDRESS1 = ko.observable<string>();
        this.ADDRESS2 = ko.observable<string>();
        this.ADDRESS3 = ko.observable<string>();
        this.CITY_CODE = ko.observable<number>();
        this.STATE_CODE = ko.observable<number>();
        this.COUNTRY = ko.observable<string>();
        this.ZIPCODE = ko.observable<string>();
        this.PHONE = ko.observable<string>();
        this.MOBILE = ko.observable<string>();
        this.USER_CODE = ko.observable<number>();
        this.ENTRY_DATE = ko.observable<Date>();
        this.ENTRY_TIME = ko.observable<Date>();        
    }


    //private static _toList: MASTERNOMINEE[];
    //static toList(): any {
    //    if (MASTERNOMINEE._toList == undefined) {
    //        $.ajax({
    //            url: AppLib.SLURL + 'MasterCity/tolist',
    //            type: 'get',
    //            async: false,
    //            cache: false,
    //            timeout: 30000,
    //            error: (err) => {
    //                console.log(err);
    //            },
    //            success: (data) => {
    //                MASTERNOMINEE._toList = data;
    //            },
    //        });
    //    }

    //    return MASTERNOMINEE._toList;
    //}
}

export { MASTERNOMINEE };