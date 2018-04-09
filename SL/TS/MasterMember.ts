import * as ko from "knockout";

class MASTERMEMBER {
    MEMBER_CODE: KnockoutObservable<number>;
    MEMBER_NAME: KnockoutObservable<string>;
    MEMBER_ID: KnockoutObservable<number>;
    MEMBER_TITLE: KnockoutObservable<string>;
    MEMBER_INITIAL: KnockoutObservable<string>;
    BF_NO: KnockoutObservable<number>;
    ICNO_OLD: KnockoutObservable<string>;
    ICNO_NEW: KnockoutObservable<string>;
    BANK_CODE: KnockoutObservable<number>;
    BANKBRANCH_CODE: KnockoutObservable<number>;
    ADDRESS1: KnockoutObservable<string>;
    ADDRESS2: KnockoutObservable<string>;
    ADDRESS3: KnockoutObservable<string>;
    CITY_CODE: KnockoutObservable<number>;
    STATE_CODE: KnockoutObservable<number>;
    CountryName: KnockoutObservable<string>;
    ZIPCODE: KnockoutObservable<string>;
    PHONE: KnockoutObservable<string>;
    MOBILE: KnockoutObservable<string>;
    EMAIL: KnockoutObservable<string>;
    DATEOFJOINING: KnockoutObservable<Date>;
    DATEOFBIRTH: KnockoutObservable<Date>;
    AGE_IN_YEARS: KnockoutObservable<number>;
    DATEOFEMPLOYMENT: KnockoutObservable<Date>;
    MEMBERTYPE_CODE: KnockoutObservable<number>;
    SEX: KnockoutObservable<string>;
    RACE_CODE: KnockoutObservable<number>;
    OCCUPATION: KnockoutObservable<string>;
    REJOINED: KnockoutObservable<boolean>;

    constructor() {

        this.MEMBER_CODE = ko.observable<number>();
        this.MEMBER_NAME = ko.observable<string>();
        this.MEMBER_ID = ko.observable<number>();
        this.MEMBER_TITLE = ko.observable<string>();
        this.MEMBER_INITIAL = ko.observable<string>();
        this.BF_NO = ko.observable<number>();
        this.ICNO_OLD = ko.observable<string>();
        this.ICNO_NEW = ko.observable<string>();
        this.BANK_CODE = ko.observable<number>();
        this.BANKBRANCH_CODE = ko.observable<number>();
        this.ADDRESS1 = ko.observable<string>();
        this.ADDRESS2 = ko.observable<string>();
        this.ADDRESS3 = ko.observable<string>();
        this.CITY_CODE = ko.observable<number>();
        this.STATE_CODE = ko.observable<number>();
        this.CountryName = ko.observable<string>();
        this.ZIPCODE = ko.observable<string>();
        this.PHONE = ko.observable<string>();
        this.MOBILE = ko.observable<string>();
        this.EMAIL = ko.observable<string>();
        this.DATEOFJOINING = ko.observable<Date>();
        this.DATEOFBIRTH = ko.observable<Date>();
        this.AGE_IN_YEARS = ko.observable<number>();
        this.DATEOFEMPLOYMENT = ko.observable<Date>();
        this.MEMBERTYPE_CODE = ko.observable<number>();
        this.SEX = ko.observable<string>();
        this.RACE_CODE = ko.observable<number>();
        this.REJOINED = ko.observable<boolean>();
        this.OCCUPATION = ko.observable<string>();;


    }

}

export { MASTERMEMBER };