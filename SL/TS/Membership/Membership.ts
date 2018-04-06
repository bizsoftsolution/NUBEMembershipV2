import * as ko from "knockout";

class Membership {
    MEMBER_CODE: KnockoutObservable<number>;
    MEMBER_NAME: KnockoutObservable<string>;
    MEMBER_ID: KnockoutObservable<number>;
    MEMBER_TITLE: KnockoutObservable<string>;
    MEMBER_INITIAL: KnockoutObservable<string>;
    BF_NO: KnockoutObservable<number>;
    ICNO_OLD: KnockoutObservable<string>;
    ICNO_NEW: KnockoutObservable<string>;
    BANK_CODE: KnockoutObservable<number>;
    BRANCH_CODE: KnockoutObservable<number>;
    ADDRESS1: KnockoutObservable<string>;
    ADDRESS2: KnockoutObservable<string>;
    ADDRESS3: KnockoutObservable<string>;
    CITY_CODE: KnockoutObservable<number>;
    STATE_CODE: KnockoutObservable<number>;
    COUNTRY: KnockoutObservable<string>;
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

    constructor() {
        this.MEMBER_CODE = ko.observable();
        this.MEMBER_NAME = ko.observable();
        this.MEMBER_ID = ko.observable();
        this.MEMBER_TITLE = ko.observable();
        this.MEMBER_INITIAL = ko.observable();
        this.BF_NO = ko.observable();
        this.ICNO_OLD = ko.observable();
        this.ICNO_NEW = ko.observable();
        this.BANK_CODE = ko.observable();
        this.BRANCH_CODE = ko.observable();
        this.ADDRESS1 = ko.observable();
        this.ADDRESS2 = ko.observable();
        this.ADDRESS3 = ko.observable();
        this.CITY_CODE = ko.observable();
        this.STATE_CODE = ko.observable();
        this.COUNTRY = ko.observable();
        this.ZIPCODE = ko.observable();
        this.PHONE = ko.observable();
        this.MOBILE = ko.observable();
        this.EMAIL = ko.observable();
        this.DATEOFJOINING = ko.observable();
        this.DATEOFBIRTH = ko.observable();
        this.AGE_IN_YEARS = ko.observable();
        this.DATEOFEMPLOYMENT = ko.observable();
        this.MEMBERTYPE_CODE = ko.observable();
        this.SEX = ko.observable();
        this.RACE_CODE = ko.observable();
    }
    
}

export { Membership };