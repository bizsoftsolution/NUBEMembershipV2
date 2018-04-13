import * as ko from "knockout";
import * as $ from "jquery";
import "bootstrap";
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
    public static joinupVM: Joinup;
    data: MASTERMEMBER;
    dataGuardian: MASTERGUARDIAN;
    nominee: KnockoutObservable<string>;
    Guardian: KnockoutObservable<string>;
    bankList: MASTERBANK[];
    branchList: KnockoutComputed<MASTERBRANCH[]>;
    raceList: MASTERRACE[];
    cityList: MASTERCITY[];
    stateList: MASTERSTATE[];
    countryList: MASTERCOUNTRY[];
    nomineeList: KnockoutObservableArray<MASTERNOMINEE>;
    relationList: MASTERRELATION[];

    constructor() {
        this.data = new MASTERMEMBER();
        this.dataGuardian = new MASTERGUARDIAN();
        this.nomineeList = ko.observableArray<MASTERNOMINEE>();
        this.AddNominee();

        this.nominee = ko.observable<string>("No");
        this.Guardian = ko.observable<string>("No");

        this.bankList = MASTERBANK.toList();
        this.branchList = ko.computed(() => { return MASTERBRANCH.toList(this.data.BANK_CODE()); });
        this.raceList = MASTERRACE.toList();
        this.cityList = MASTERCITY.toList();
        this.stateList = MASTERSTATE.toList();
        this.countryList = MASTERCOUNTRY.toList();
        this.relationList = MASTERRELATION.toList();              
    }
    MemberAttachment(Member_Code: string, AttachmentName: string, file: HTMLInputElement): void {
        if (file != undefined) {

            var mFiles = new FormData();
            mFiles.append('MemberCode', Member_Code);
            mFiles.append('AttachmentName', AttachmentName);
            mFiles.append('AttachmentData', file.files[0]);
            $.ajax({
                url: 'http://localhost/MembershipTest/MasterMember/AttachmentUpload',
                type: "POST",
                data: mFiles,
                processData: false,
                contentType: false,
                success: (d) => {
                    console.log(d);
                }
            });
           
        }
    }
    btnSave(): void {

        var d = ko.toJS(this.data);
        console.log(d);
        $.post('http://localhost/MembershipTest/MasterMember/Insert', d, (resMember) => {
            console.log(resMember);
            if (resMember.isSaved) {

                this.MemberAttachment(resMember.MEMBER_CODE, "fPhoto", $('#fPhoto')[0] as HTMLInputElement);
                this.MemberAttachment(resMember.MEMBER_CODE, "fDSign", $('#fDSign')[0] as HTMLInputElement);
                this.MemberAttachment(resMember.MEMBER_CODE, "fIC", $('#fIC')[0] as HTMLInputElement);
                this.MemberAttachment(resMember.MEMBER_CODE, "fELetter", $('#fELetter')[0] as HTMLInputElement);
               
                if (this.nominee() == "Yes") {
                    ko.utils.arrayForEach(this.nomineeList(), (n) => {
                        n.MEMBER_CODE(resMember.MEMBER_CODE);
                        var dN = ko.toJS(n);
                        $.post('http://localhost/MembershipTest/Nominee/Insert', dN, (resNominee) => {

                        });
                    });
                }

                if (this.Guardian() == "Yes") {
                    this.dataGuardian.MEMBER_CODE(resMember.MEMBER_CODE);
                    var dG = ko.toJS(this.dataGuardian);
                    $.post('http://localhost/MembershipTest/Guardian/Insert', dG, (resGuardian) => {

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
Joinup.joinupVM = new Joinup();
ko.applyBindings(Joinup.joinupVM);
var dateOption = { dateFormat: 'dd/mm/yy' };
$('#dob').datepicker(dateOption);
$('#doe').datepicker(dateOption);

$('#dob').change((e) => {
    var dt = $(e.target).datepicker('getDate');
    Joinup.joinupVM.data.DATEOFBIRTH(dt);
});
$('#doe').change((e) => {
    var dt = $(e.target).datepicker('getDate');
    Joinup.joinupVM.data.DATEOFEMPLOYMENT(dt);
});


export { Joinup };