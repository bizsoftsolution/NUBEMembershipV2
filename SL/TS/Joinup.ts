import * as ko from "knockout";
import * as $ from "jquery";
import "jqueryui";
import "knockout-jqAutocomplete";
import { MASTERMEMBER } from "./MasterMember";
import { MASTERBANK } from "./MasterBank";
import { MASTERBRANCH } from "./MasterBranch";
import { MASTERRACE } from "./MasterRace";
import { MASTERCITY } from "./MASTERCITY";
import { MASTERSTATE } from "./MASTERSTATE";
import { MASTERCOUNTRY } from "./MasterCountry";
import { MASTERNOMINEE } from "./MasterNominee";
import { MASTERGUARDIAN } from "./MasterGuardian";
import { MASTERRELATION } from "./MasterRelation";

class Joinup {
    data: MASTERMEMBER;
    dataGuardian: MASTERGUARDIAN;
    nominee: KnockoutObservable<string>;
    Guardian: KnockoutObservable<string>;
    bankList: MASTERBANK[];
    branchList: MASTERBRANCH[];
    raceList: MASTERRACE[];
    cityList: MASTERCITY[];
    stateList: MASTERSTATE[];
    countryList: MASTERCOUNTRY[];
    nomineeList: KnockoutObservableArray<MASTERNOMINEE>;
    relationList: MASTERRELATION[];

    constructor() {
        this.data = new MASTERMEMBER();
        this.nominee = ko.observable<string>("No");
        this.Guardian = ko.observable<string>("No");
        
        this.bankList = MASTERBANK.toList();
        this.branchList = MASTERBRANCH.toList();
        this.raceList = MASTERRACE.toList();
        this.cityList = MASTERCITY.toList();
        this.stateList = MASTERSTATE.toList();
        this.countryList = MASTERCOUNTRY.toList();        
        this.relationList = MASTERRELATION.toList();

        this.nomineeList = ko.observableArray<MASTERNOMINEE>();
        this.AddNominee();
    }

    btnSave(): void {
        var d = ko.toJS(this.data);
        console.log(d);
        $.post('./MasterMember/Insert', d,  (resMember)=> {
            console.log(resMember);
            if (resMember.isSaved) {
                if (this.nominee() == "Yes") {
                    ko.utils.arrayForEach(this.nomineeList(), (n) => {
                        n.MEMBER_CODE(resMember.MEMBER_CODE());
                        var dN = ko.toJS(n);
                        $.post('./MasterNominee/Insert', dN, (resNominee) => {
                            
                        });
                    });
                }

                if (this.Guardian() == "Yes") {
                    this.Guardian.MEMBER_CODE(resMember.MEMBER_CODE());
                    var dG = ko.toJS(this.dataGuardian);
                    $.post('./MasterGuardian/Insert', dG, (resGuardian) => {

                    });
                }
            }
        });

    }

    AddNominee(): void {
        this.nomineeList.push(new MASTERNOMINEE());
    }

    RemoveNominee(data: MASTERNOMINEE): void {
        if (confirm("Are you remove this nominee?")) {
            this.nomineeList.remove(x => {
               return x == data;
            });
        }        
    }

}
ko.applyBindings(new Joinup());

