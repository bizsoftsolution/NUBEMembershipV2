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

class Joinup{
    data: MASTERMEMBER;
    bankList: MASTERBANK[];
    branchList: MASTERBRANCH[];
    raceList: MASTERRACE[];
    cityList: MASTERCITY[];
    stateList: MASTERSTATE[];
    countryList: MASTERCOUNTRY[];

    constructor() {
        this.data = new MASTERMEMBER();
        this.bankList = MASTERBANK.toList();
        this.branchList = MASTERBRANCH.toList();
        this.raceList = MASTERRACE.toList();
        this.cityList = MASTERCITY.toList();
        this.stateList = MASTERSTATE.toList();
        this.countryList = MASTERCOUNTRY.toList();
    }

    btnSave(): void {
        var d = ko.toJS(this.data);
        console.log(d);
        $.post('./MasterMember/Insert', d, function (r) {
            console.log(r);
        });

    }

}
ko.applyBindings(new Joinup());

