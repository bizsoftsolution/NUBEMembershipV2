define(["require", "exports", "knockout", "jquery", "./MasterMember", "./MasterBank", "./MasterBranch", "./MasterRace", "./MASTERCITY", "./MASTERSTATE", "./MasterCountry", "./MasterNominee", "./MasterGuardian", "./MasterRelation", "./AppLib", "jquery-validation", "bootstrap", "jqueryui", "knockout-jqAutocomplete"], function (require, exports, ko, $, MasterMember_1, MasterBank_1, MasterBranch_1, MasterRace_1, MASTERCITY_1, MASTERSTATE_1, MasterCountry_1, MasterNominee_1, MasterGuardian_1, MasterRelation_1, AppLib_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Joinup = /** @class */ (function () {
        function Joinup() {
            var _this = this;
            this.data = new MasterMember_1.MASTERMEMBER();
            this.dataGuardian = new MasterGuardian_1.MASTERGUARDIAN();
            this.nomineeList = ko.observableArray();
            this.AddNominee();
            this.data.MEMBERTYPE_CODE = ko.observable("2");
            this.data.REJOINED = ko.observable("0");
            this.nominee = ko.observable("No");
            this.Guardian = ko.observable("No");
            this.bankList = MasterBank_1.MASTERBANK.toList();
            this.branchList = ko.computed(function () { return MasterBranch_1.MASTERBRANCH.toList(_this.data.BANK_CODE()); });
            this.raceList = MasterRace_1.MASTERRACE.toList();
            this.cityList = MASTERCITY_1.MASTERCITY.toList();
            this.stateList = MASTERSTATE_1.MASTERSTATE.toList();
            this.countryList = MasterCountry_1.MASTERCOUNTRY.toList();
            this.relationList = MasterRelation_1.MASTERRELATION.toList();
        }
        Joinup.prototype.MemberAttachment = function (Member_Code, AttachmentName, file) {
            if (file != undefined) {
                var mFiles = new FormData();
                mFiles.append('MemberCode', Member_Code);
                mFiles.append('AttachmentName', AttachmentName);
                mFiles.append('AttachmentData', file.files[0]);
                $.ajax({
                    url: AppLib_1.AppLib.SLURL + 'MasterMember/AttachmentUpload',
                    type: "POST",
                    data: mFiles,
                    processData: false,
                    contentType: false,
                    success: function (d) {
                        console.log(d);
                    }
                });
            }
        };
        Joinup.prototype.btnSave = function () {
            var _this = this;
            if (!$('#frmMember').valid()) {
                alert("Please enter  the all required data");
            }
            else {
                var d = ko.toJS(this.data);
                console.log(d);
                $.post(AppLib_1.AppLib.SLURL + 'MasterMember/Insert', d, function (resMember) {
                    console.log(resMember);
                    if (resMember.isSaved) {
                        _this.MemberAttachment(resMember.MEMBER_CODE, "fPhoto", $('#fPhoto')[0]);
                        _this.MemberAttachment(resMember.MEMBER_CODE, "fDSign", $('#fDSign')[0]);
                        _this.MemberAttachment(resMember.MEMBER_CODE, "fIC", $('#fIC')[0]);
                        _this.MemberAttachment(resMember.MEMBER_CODE, "fELetter", $('#fELetter')[0]);
                        _this.MemberAttachment(resMember.MEMBER_CODE, "fEPPayment", $('#fEPPayment')[0]);
                        if (_this.nominee() == "Yes") {
                            ko.utils.arrayForEach(_this.nomineeList(), function (n) {
                                n.MEMBER_CODE(resMember.MEMBER_CODE);
                                var dN = ko.toJS(n);
                                $.post(AppLib_1.AppLib.SLURL + 'Nominee/Insert', dN, function (resNominee) {
                                });
                            });
                        }
                        if (_this.Guardian() == "Yes") {
                            _this.dataGuardian.MEMBER_CODE(resMember.MEMBER_CODE);
                            var dG = ko.toJS(_this.dataGuardian);
                            $.post(AppLib_1.AppLib.SLURL + 'Guardian/Insert', dG, function (resGuardian) {
                            });
                        }
                        window.location.replace("http://membership.nube.org.my/Joinup/sucessfully");
                    }
                });
            }
        };
        Joinup.prototype.AddNominee = function () {
            this.nomineeList.push(new MasterNominee_1.MASTERNOMINEE());
        };
        Joinup.prototype.RemoveNominee = function (data) {
            if (confirm("Are you remove this nominee?")) {
                this.nomineeList.remove(function (x) {
                    return x == data;
                });
            }
        };
        return Joinup;
    }());
    exports.Joinup = Joinup;
    Joinup.joinupVM = new Joinup();
    ko.applyBindings(Joinup.joinupVM);
    var dateOption = {
        dateFormat: 'dd/mm/yy', changeMonth: true,
        changeYear: true, yearRange: "-100:+0"
    };
    $('#dob').datepicker(dateOption);
    $('#doe').datepicker(dateOption);
    $('#dob').change(function (e) {
        var dt = $(e.target).datepicker('getDate');
        Joinup.joinupVM.data.DATEOFBIRTH(dt.getFullYear() + "-" + dt.getMonth() + "-" + dt.getDay());
    });
    $('#doe').change(function (e) {
        var dt = $(e.target).datepicker('getDate');
        Joinup.joinupVM.data.DATEOFEMPLOYMENT(dt.getFullYear() + "-" + dt.getMonth() + "-" + dt.getDay());
    });
    $('#frmMember').validate({
        rules: {
            dob: {
                required: true,
            },
            MEMBERTYPE_CODE: {
                required: true
            },
            REJOINED: {
                required: true
            },
            BANK_NAME: {
                required: true
            },
            BANKBRANCH_NAME: {
                required: true
            },
            SALARY: {
                required: true
            },
            RACE_NAME: {
                required: true
            },
        }
    });
});
//# sourceMappingURL=Joinup.js.map