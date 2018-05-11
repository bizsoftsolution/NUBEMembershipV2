import * as ko from "knockout"
import { AppLib } from "./AppLib";

class MASTERRELATION {
    RELATION_CODE: KnockoutObservable<number>;
    RELATION_NAME: KnockoutObservable<string>;    
    DELETED: KnockoutObservable<boolean>;
    constructor() {
        this.RELATION_CODE = ko.observable<number>();
        this.RELATION_NAME = ko.observable<string>();        
        this.DELETED = ko.observable<boolean>();
    }

    private static _toList: MASTERRELATION[];
    static toList(): any {
        if (MASTERRELATION._toList == undefined) {
            $.ajax({
                url: AppLib.SLURL+ 'MasterRelation/tolist',
                type: 'get',
                async: false,
                cache: false,
                timeout: 30000,
                error: (err) => {
                    console.log(err);
                },
                success: (data) => {
                    MASTERRELATION._toList = data;
                },
            });
        }

        return MASTERRELATION._toList;
    }

}
export { MASTERRELATION };