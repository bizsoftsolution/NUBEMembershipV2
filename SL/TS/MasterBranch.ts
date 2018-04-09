import * as ko from "knockout"

class MASTERBRANCH {
    BANKBRANCH_CODE: KnockoutObservable<number>;
    BANK_CODE: KnockoutObservable<number>;
    BANKBRANCH_NAME: KnockoutObservable<string>;
    BANKBRANCH_USERCODE: KnockoutObservable<string>;
    BANKBRANCH_ADDRESS1: KnockoutObservable<string>;
    BANKBRANCH_ADDRESS2: KnockoutObservable<string>;
    BANKBRANCH_ADDRESS3: KnockoutObservable<string>;
    BANKBRANCH_CITY_CODE: KnockoutObservable<number>;
    BANKBRANCH_STATE_CODE: KnockoutObservable<number>;
    BANKBRANCH_COUNTRY: KnockoutObservable<string>;
    BANKBRANCH_ZIPCODE: KnockoutObservable<string>;
    BANKBRANCH_PHONE1: KnockoutObservable<string>;
    NUBE_BRANCH_CODE: KnockoutObservable<number>;
    DELETED: KnockoutObservable<boolean>;
    HEAD_QUARTERS: KnockoutObservable<number>;
    constructor() {
        this.BANKBRANCH_CODE = ko.observable<number>();
        this.BANK_CODE = ko.observable<number>();
        this.BANKBRANCH_NAME = ko.observable<string>();
        this.BANKBRANCH_USERCODE = ko.observable<string>();
        this.BANKBRANCH_ADDRESS1 = ko.observable<string>();
        this.BANKBRANCH_ADDRESS2 = ko.observable<string>();
        this.BANKBRANCH_ADDRESS3 = ko.observable<string>();
        this.BANKBRANCH_CITY_CODE = ko.observable<number>();
        this.BANKBRANCH_STATE_CODE = ko.observable<number>();
        this.BANKBRANCH_COUNTRY = ko.observable<string>();
        this.BANKBRANCH_ZIPCODE = ko.observable<string>();
        this.BANKBRANCH_PHONE1 = ko.observable<string>();
        this.NUBE_BRANCH_CODE = ko.observable<number>();
        this.DELETED = ko.observable<boolean>();
        this.HEAD_QUARTERS = ko.observable<number>();
    }

    private static _toList: MASTERBRANCH[];
    static toList(): any {
        if (MASTERBRANCH._toList == undefined) {
            $.ajax({
                url: 'http://localhost/MembershipTest/MasterBranch/tolist',
                type: 'get',
                async: false,
                cache: false,
                timeout: 30000,
                error: (err) => {
                    console.log(err);
                },
                success: (data) => {
                    MASTERBRANCH._toList = data;
                },
            });
        }

        return MASTERBRANCH._toList;
    }

}

export { MASTERBRANCH };