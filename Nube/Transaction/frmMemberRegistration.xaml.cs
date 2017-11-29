using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Reporting.WinForms;
using System.IO;
using Nube.MasterSetup;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.Data.SqlClient;
using System.Drawing;

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmMemberRegistration.xaml
    /// </summary>
    public partial class frmMemberRegistration : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        decimal dMember_Code = 0;
        Boolean bValidation = false;
        private int m_currentPageIndex;
        private IList<Stream> m_streams;
        public static string connStr = AppLib.connStr;
        Boolean bIsUpdate = false;
        decimal BF = 0;
        decimal UC = 0;
        decimal Ins = 0;
        decimal Subs = 0;
        decimal dTotlMonthsPaid = 0;
        decimal dTotlMonthsPaidUC = 0;
        List<MASTERNOMINEENAMES> lstmstnom = new List<MASTERNOMINEENAMES>();

        public frmMemberRegistration(decimal dMembercode = 0)
        {
            InitializeComponent();

            dMember_Code = dMembercode;
            FormLoad();
            bIsUpdate = false;

            //LoadTempViewMaster();

            if (dMember_Code != 0)
            {
                FormFill();
                bIsUpdate = true;
            }

        }

        #region "BUTTON EVENTS"

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                fNew();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bValidation = false;
                BeforeUpdate();
                if (bValidation == false)
                {
                    decimal dCityCode = 0;
                    decimal dStateCode = 0;
                    decimal dGCitycode = 0;
                    decimal dGStatecode = 0;
                    decimal dGRelationcode = 0;
                    double dSalary = 0;
                    decimal dGAge = 0;
                    decimal dENTRANCEFEE = 0;
                    decimal dAge = 0;

                    decimal dACCBENEFIT = 0;
                    decimal dMONTHLYSUBSCRIPTION = 0;
                    decimal dTOTALMONTHSPAID = 0;
                    decimal dACCSUBSCRIPTION = 0;
                    decimal dMONTHLYBF = 0;
                    decimal dACCBF = 0;
                    decimal dCURRENT_YTDBF = 0;
                    decimal dCURRENT_YTDSUBSCRIPTION = 0;
                    int iLevyAmount = 0;
                    int iTDFAmount = 0;

                    if (!string.IsNullOrEmpty(txtEntranceFee.Text))
                    {
                        dENTRANCEFEE = Convert.ToDecimal(txtEntranceFee.Text);
                    }
                    if (!string.IsNullOrEmpty(txtAge.Text))
                    {
                        dAge = Convert.ToDecimal(txtAge.Text);
                    }
                    if (!string.IsNullOrEmpty(txtLevyAmount.Text))
                    {
                        iLevyAmount = Convert.ToInt32(txtLevyAmount.Text);
                    }
                    if (!string.IsNullOrEmpty(txtTDFAmount.Text))
                    {
                        iTDFAmount = Convert.ToInt32(txtTDFAmount.Text);
                    }

                    if (!string.IsNullOrEmpty(txtAccBenefit.Text))
                    {
                        dACCBENEFIT = Convert.ToDecimal(txtAccBenefit.Text);
                    }
                    if (!string.IsNullOrEmpty(txtMonthlySub.Text))
                    {
                        dMONTHLYSUBSCRIPTION = Convert.ToDecimal(txtMonthlySub.Text);
                    }
                    if (!string.IsNullOrEmpty(txtTotalMonthPaidSubs.Text))
                    {
                        dTOTALMONTHSPAID = Convert.ToDecimal(txtTotalMonthPaidSubs.Text);
                    }
                    if (!string.IsNullOrEmpty(txtAccSub.Text))
                    {
                        dACCSUBSCRIPTION = Convert.ToDecimal(txtAccSub.Text);
                    }
                    if (!string.IsNullOrEmpty(txtMonthlyBF.Text))
                    {
                        dMONTHLYBF = Convert.ToDecimal(txtMonthlyBF.Text);
                    }
                    if (!string.IsNullOrEmpty(txtAccBF.Text))
                    {
                        dACCBF = Convert.ToDecimal(txtAccBF.Text);
                    }
                    if (!string.IsNullOrEmpty(txtCurrentYTDBF.Text))
                    {
                        dCURRENT_YTDBF = Convert.ToDecimal(txtCurrentYTDBF.Text);
                    }
                    if (!string.IsNullOrEmpty(txtCurrentYTDSub.Text))
                    {
                        dCURRENT_YTDSUBSCRIPTION = Convert.ToDecimal(txtCurrentYTDSub.Text);
                    }

                    if (!string.IsNullOrEmpty(cmbResCity.Text))
                    {
                        dCityCode = Convert.ToDecimal(cmbResCity.SelectedValue);
                    }
                    if (!string.IsNullOrEmpty(cmbResState.Text))
                    {
                        dStateCode = Convert.ToDecimal(cmbResState.SelectedValue);
                    }
                    if (!string.IsNullOrEmpty(txtSalary.Text))
                    {
                        dSalary = Convert.ToDouble(txtSalary.Text);
                    }
                    if (!string.IsNullOrEmpty(cmbGurRelation.Text))
                    {
                        dGRelationcode = Convert.ToDecimal(cmbGurRelation.SelectedValue);
                    }
                    if (!string.IsNullOrEmpty(cmbGurCity.Text))
                    {
                        dGCitycode = Convert.ToDecimal(cmbGurCity.SelectedValue);
                    }
                    if (!string.IsNullOrEmpty(cmbGurState.Text))
                    {
                        dGStatecode = Convert.ToDecimal(cmbGurState.SelectedValue);
                    }
                    if (!string.IsNullOrEmpty(txtGurAge.Text))
                    {
                        dGAge = Convert.ToDecimal(txtGurAge.Text);
                    }

                    decimal dtxtMember_ID = Convert.ToDecimal(txtMemberNo.Text);
                    if (bIsUpdate == true)
                    {
                        MASTERMEMBER mm = (from mas in db.MASTERMEMBERs where mas.MEMBER_CODE == dMember_Code select mas).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(mm);
                        if (mm != null)
                        {
                            mm.MEMBERTYPE_CODE = Convert.ToDecimal(cmbMemberType.SelectedValue);
                            mm.MEMBER_ID = dtxtMember_ID;
                            mm.MEMBER_TITLE = cmbMemberInit.Text;
                            mm.MEMBER_NAME = txtMemberName.Text;
                            mm.DATEOFBIRTH = dtpDOB.SelectedDate;
                            mm.AGE_IN_YEARS = Convert.ToDecimal(txtAge.Text);
                            mm.SEX = cmbGender.Text;
                            mm.REJOINED = Convert.ToDecimal(ckbRejoined.IsChecked);
                            mm.RACE_CODE = Convert.ToDecimal(cmbRace.SelectedValue);
                            mm.ICNO_NEW = string.IsNullOrEmpty(txtNewIC.Text) ? "" : txtNewIC.Text;
                            mm.ICNO_OLD = string.IsNullOrEmpty(txtOldIC.Text) ? "" : txtOldIC.Text;
                            mm.DATEOFJOINING = dtpDOJ.SelectedDate;

                            mm.BANK_CODE = Convert.ToDecimal(cmbBankCode.SelectedValue);
                            mm.BRANCH_CODE = Convert.ToDecimal(cmbBranchCode.SelectedValue);
                            mm.DATEOFEMPLOYMENT = dtpDOEmp.SelectedDate;
                            mm.Salary = dSalary;
                            mm.LEVY = cmbLevy.Text;
                            mm.TDF = cmbTDF.Text;
                            mm.LEVY_AMOUNT = iLevyAmount;
                            mm.TDF_AMOUNT = iTDFAmount;
                            mm.LevyPaymentDate = DateTime.Now;
                            mm.Tdf_PaymentDate = DateTime.Now;

                            mm.ENTRANCEFEE = Convert.ToDecimal(txtEntranceFee.Text);
                            mm.ACCBENEFIT = Convert.ToDecimal(txtAccBenefit.Text);
                            mm.MONTHLYSUBSCRIPTION = Convert.ToDecimal(txtMonthlySub.Text);
                            mm.TOTALMONTHSPAID = Convert.ToDecimal(txtTotalMonthPaidSubs.Text);
                            mm.ACCSUBSCRIPTION = Convert.ToDecimal(txtAccSub.Text);
                            mm.MONTHLYBF = Convert.ToDecimal(txtMonthlyBF.Text);
                            mm.ACCBF = Convert.ToDecimal(txtAccBF.Text);
                            mm.CURRENT_YTDBF = Convert.ToDecimal(txtCurrentYTDBF.Text);
                            mm.CURRENT_YTDSUBSCRIPTION = Convert.ToDecimal(txtCurrentYTDSub.Text);
                            mm.ADDRESS1 = txtResAddress1.Text;
                            mm.ADDRESS2 = txtResAddress2.Text;
                            mm.ADDRESS3 = txtResAddress3.Text;
                            mm.PHONE = txtResPhoneNo.Text;
                            mm.MOBILE = txtResMobileNo.Text;
                            mm.EMAIL = txtResEmail.Text + cmbResMail.Text.ToString();
                            mm.CITY_CODE = dCityCode;
                            mm.ZIPCODE = txtResPostalCode.Text;
                            mm.STATE_CODE = dStateCode;
                            mm.COUNTRY = cmbResCountry.Text;
                            mm.UpdatedBy = AppLib.iUserCode;
                            mm.UpdatedOn = DateTime.Now;

                            db.SaveChanges();

                            var NewData = new JSonHelper().ConvertObjectToJSon(mm);
                            AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "MASTERMEMBER");

                            var ms = (from x in db.MemberStatusLogs where x.MEMBER_CODE == dMember_Code select x).FirstOrDefault();
                            if (ms != null)
                            {
                                ms.MEMBER_NAME = txtMemberName.Text;
                                ms.MEMBER_ID = Convert.ToInt32(dtxtMember_ID);
                                ms.MEMBERTYPE_CODE = Convert.ToInt32(cmbMemberType.SelectedValue);
                                if (Convert.ToInt32(cmbMemberType.SelectedValue) == 1)
                                {
                                    ms.MEMBERTYPE_NAME = "C";
                                }
                                else
                                {
                                    ms.MEMBERTYPE_NAME = "N";
                                }
                                ms.SEX = cmbGender.Text;

                                if (cmbGender.Text == "Male")
                                {
                                    ms.SEX_MF = "M";
                                }
                                else
                                {
                                    ms.SEX_MF = "F";
                                }

                                if (cmbRace.Text == "MALAY")
                                {
                                    ms.RACE = "M";
                                }
                                else if (cmbRace.Text == "INDIAN")
                                {
                                    ms.RACE = "I";
                                }
                                else if (cmbRace.Text == "CHINESE")
                                {
                                    ms.RACE = "C";
                                }
                                else if (cmbRace.Text == "OTHERS")
                                {
                                    ms.RACE = "O";
                                }
                                ms.ICNO_NEW = string.IsNullOrEmpty(txtNewIC.Text) ? "" : txtNewIC.Text;
                                ms.ICNO_OLD = string.IsNullOrEmpty(txtOldIC.Text) ? "" : txtOldIC.Text;
                                ms.Levy = cmbLevy.Text;
                                ms.TDF = cmbTDF.Text;
                                ms.CITY_CODE = Convert.ToInt32(dCityCode);
                                ms.STATE_CODE = Convert.ToInt32(dStateCode);
                                ms.MOBILE_NO = txtResMobileNo.Text;
                                ms.DATEOFBIRTH = dtpDOB.SelectedDate;
                                ms.BANK_CODE = Convert.ToInt32(cmbBankCode.SelectedValue);
                                ms.BANKUSER_CODE = cmbBankCode.Text;
                                ms.BRANCH_CODE = Convert.ToInt32(cmbBranchCode.SelectedValue);
                                ms.BRANCH_NAME = cmbBranchCode.Text;
                                ms.DATEOFJOINING = dtpDOJ.SelectedDate;
                                ms.REJOINED = Convert.ToBoolean(ckbRejoined.IsChecked);
                                db.SaveChanges();
                            }

                            MASTERGUARDIAN mg = (from mas in db.MASTERGUARDIANs where mas.MEMBER_CODE == dMember_Code select mas).FirstOrDefault();
                            var OldData1 = new JSonHelper().ConvertObjectToJSon(mg);
                            if (mg != null)
                            {
                                mg.MEMBER_CODE = dMember_Code;
                                mg.NAME = txGurName.Text;
                                mg.RELATION_CODE = dGRelationcode;
                                mg.ADDRESS1 = txtGurAddress.Text;
                                mg.ADDRESS2 = txtGurAddress2.Text;
                                mg.ADDRESS3 = txtGurAddress3.Text;
                                mg.AGE = dGAge;
                                mg.SEX = cmbGurSex.Text;
                                mg.ICNO_NEW = txtGurICNO.Text;
                                mg.ICNO_OLD = txtGurOldICNO.Text;
                                mg.CITY_CODE = dGCitycode;
                                mg.STATE_CODE = dGStatecode;
                                mg.COUNTRY = cmbGurCountry.Text;
                                mg.ZIPCODE = txtGurZipCode.Text;
                                mg.PHONE = txtGurPhNo.Text;
                                mg.MOBILE = txtGurMobileNo.Text;
                                db.SaveChanges();

                                var NewData1 = new JSonHelper().ConvertObjectToJSon(mg);
                                AppLib.EventHistory(this.Tag.ToString(), 1, OldData1, NewData1, "MASTERGUARDIAN");
                            }
                            if (dgNomination.Items.Count > 0)
                            {
                                var mn = (from mas in db.MASTERNOMINEEs where mas.MEMBER_CODE == dMember_Code select mas).ToList();
                                var OldData2 = new JSonHelper().ConvertObjectToJSon(mn);
                                if (mn != null)
                                {
                                    db.MASTERNOMINEEs.RemoveRange(db.MASTERNOMINEEs.Where(x => x.MEMBER_CODE == dMember_Code));
                                    db.SaveChanges();

                                    DataTable dt = new DataTable();
                                    dt = ((DataView)dgNomination.ItemsSource).ToTable();
                                    if (dt.Rows.Count > 0)
                                    {
                                        List<MASTERNOMINEE> lstNominee = new List<MASTERNOMINEE>();
                                        for (int i = 0; i < dt.Rows.Count; i++)
                                        {
                                            MASTERNOMINEE mstnom = new MASTERNOMINEE
                                            {
                                                NAME = dt.Rows[i]["NAME"].ToString(),
                                                AGE = Convert.ToDecimal(dt.Rows[i]["AGE"]),
                                                SEX = dt.Rows[i]["SEX"].ToString(),
                                                RELATION_CODE = Convert.ToDecimal(dt.Rows[i]["RELATION_CODE"]),
                                                ICNO_NEW = dt.Rows[i]["ICNO_NEW"].ToString(),
                                                ICNO_OLD = dt.Rows[i]["ICNO_OLD"].ToString(),
                                                ADDRESS1 = dt.Rows[i]["ADDRESS1"].ToString(),
                                                ADDRESS2 = dt.Rows[i]["ADDRESS2"].ToString(),
                                                ADDRESS3 = dt.Rows[i]["ADDRESS3"].ToString(),
                                                CITY_CODE = Convert.ToDecimal((dt.Rows[i]["CITY_CODE"]) == null ? 0 : dt.Rows[i]["CITY_CODE"]),
                                                STATE_CODE = Convert.ToDecimal((dt.Rows[i]["STATE_CODE"]) == null ? 0 : dt.Rows[i]["STATE_CODE"]),
                                                MOBILE = dt.Rows[i]["MOBILE"].ToString(),
                                                PHONE = dt.Rows[i]["PHONE"].ToString(),
                                                MEMBER_CODE = dMember_Code,
                                            };
                                            lstNominee.Add(mstnom);
                                        }

                                        if (lstNominee.Count > 0)
                                        {
                                            db.MASTERNOMINEEs.AddRange(lstNominee);
                                            db.SaveChanges();

                                            var NewData2 = new JSonHelper().ConvertObjectToJSon(lstNominee);
                                            AppLib.EventHistory(this.Tag.ToString(), 1, OldData2, NewData2, "MASTERNOMINEE");
                                        }
                                    }
                                }
                            }
                            MessageBox.Show("Updated Sucessfully");
                            fNew();
                        }
                        else
                        {
                            MessageBox.Show("No Record to Update !", "Update Error");
                        }
                    }
                    else
                    {
                        //int sFid = Convert.ToInt32(db.MASTERMEMBERs.Max(x => x.MEMBER_CODE));
                        MASTERMEMBER mstmm = new MASTERMEMBER
                        {
                            MEMBERTYPE_CODE = Convert.ToDecimal((cmbMemberType.SelectedValue == null) ? 0 : cmbMemberType.SelectedValue),
                            MEMBER_ID = Convert.ToDecimal(txtMemberNo.Text),
                            MEMBER_TITLE = cmbMemberInit.Text,
                            MEMBER_NAME = txtMemberName.Text,
                            DATEOFBIRTH = dtpDOB.SelectedDate,
                            AGE_IN_YEARS = dAge,
                            SEX = cmbGender.Text,
                            REJOINED = Convert.ToDecimal(ckbRejoined.IsChecked),
                            RACE_CODE = Convert.ToDecimal(cmbRace.SelectedValue),
                            ICNO_NEW = string.IsNullOrEmpty(txtNewIC.Text) ? "" : txtNewIC.Text,
                            ICNO_OLD = string.IsNullOrEmpty(txtOldIC.Text) ? "" : txtOldIC.Text,
                            DATEOFJOINING = dtpDOJ.SelectedDate,

                            BANK_CODE = Convert.ToDecimal(cmbBankCode.SelectedValue),
                            BRANCH_CODE = Convert.ToDecimal(cmbBranchCode.SelectedValue),
                            DATEOFEMPLOYMENT = dtpDOEmp.SelectedDate,
                            Salary = dSalary,
                            LEVY = cmbLevy.Text,
                            TDF = cmbTDF.Text,
                            LEVY_AMOUNT = iLevyAmount,
                            TDF_AMOUNT = iTDFAmount,
                            LevyPaymentDate = DateTime.Now,
                            Tdf_PaymentDate = DateTime.Now,

                            ENTRANCEFEE = dENTRANCEFEE,
                            ACCBENEFIT = dACCBENEFIT,
                            MONTHLYSUBSCRIPTION = dMONTHLYSUBSCRIPTION,
                            TOTALMONTHSPAID = dTOTALMONTHSPAID,
                            ACCSUBSCRIPTION = dACCSUBSCRIPTION,
                            MONTHLYBF = dMONTHLYBF,
                            ACCBF = dACCBF,
                            CURRENT_YTDBF = dCURRENT_YTDBF,
                            CURRENT_YTDSUBSCRIPTION = dCURRENT_YTDSUBSCRIPTION,

                            ADDRESS1 = txtResAddress1.Text,
                            ADDRESS2 = txtResAddress2.Text,
                            ADDRESS3 = txtResAddress3.Text,
                            PHONE = txtResPhoneNo.Text,
                            MOBILE = txtResMobileNo.Text,
                            EMAIL = txtResEmail.Text + cmbResMail.Text.ToString(),
                            CITY_CODE = dCityCode,
                            ZIPCODE = txtResPostalCode.Text,
                            STATE_CODE = dStateCode,
                            COUNTRY = cmbResCountry.Text,
                            STATUS_CODE = 1,
                            CreatedBy = AppLib.iUserCode,
                            CreatedOn = DateTime.Now
                        };
                        db.MASTERMEMBERs.Add(mstmm);
                        db.SaveChanges();

                        var NewData = new JSonHelper().ConvertObjectToJSon(mstmm);
                        AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "MASTERMEMBER");

                        MemberStatusLog ms = new MemberStatusLog();
                        ms.MEMBER_NAME = txtMemberName.Text;
                        ms.MEMBER_ID = Convert.ToInt32(dtxtMember_ID);
                        ms.MEMBERTYPE_CODE = Convert.ToInt32(cmbMemberType.SelectedValue);
                        if (Convert.ToInt32(cmbMemberType.SelectedValue) == 1)
                        {
                            ms.MEMBERTYPE_NAME = "C";
                        }
                        else
                        {
                            ms.MEMBERTYPE_NAME = "N";
                        }
                        ms.SEX = cmbGender.Text;

                        if (cmbGender.Text == "Male")
                        {
                            ms.SEX_MF = "M";
                        }
                        else
                        {
                            ms.SEX_MF = "F";
                        }

                        if (cmbRace.Text == "MALAY")
                        {
                            ms.RACE = "M";
                        }
                        else if (cmbRace.Text == "INDIAN")
                        {
                            ms.RACE = "I";
                        }
                        else if (cmbRace.Text == "CHINESE")
                        {
                            ms.RACE = "C";
                        }
                        else if (cmbRace.Text == "OTHERS")
                        {
                            ms.RACE = "O";
                        }

                        ms.ICNO_NEW = string.IsNullOrEmpty(txtNewIC.Text) ? "" : txtNewIC.Text;
                        ms.ICNO_OLD = string.IsNullOrEmpty(txtOldIC.Text) ? "" : txtOldIC.Text;
                        ms.Levy = cmbLevy.Text;
                        ms.TDF = cmbTDF.Text;
                        ms.CITY_CODE = Convert.ToInt32(dCityCode);
                        ms.STATE_CODE = Convert.ToInt32(dStateCode);
                        ms.MOBILE_NO = txtResMobileNo.Text;
                        ms.DATEOFBIRTH = dtpDOB.SelectedDate;
                        ms.BANK_CODE = Convert.ToInt32(cmbBankCode.SelectedValue);
                        ms.BANKUSER_CODE = cmbBankCode.Text;
                        ms.BRANCH_CODE = Convert.ToInt32(cmbBranchCode.SelectedValue);
                        ms.BRANCH_NAME = cmbBranchCode.Text;
                        ms.DATEOFJOINING = dtpDOJ.SelectedDate;
                        ms.REJOINED = Convert.ToBoolean(ckbRejoined.IsChecked);
                        ms.TOTALMONTHSPAID = 1;
                        ms.TOTALMOTHSDUE = 0;
                        ms.LASTPAYMENT_DATE = DateTime.Today;
                        ms.MEMBERSTATUS = "ACTIVE";
                        ms.MEMBERSTATUSCODE = 1;
                        db.MemberStatusLogs.Add(ms);
                        db.SaveChanges();

                        dMember_Code = Convert.ToDecimal(db.MASTERMEMBERs.Max(x => x.MEMBER_CODE));
                        if (!string.IsNullOrEmpty(txGurName.Text))
                        {
                            MASTERGUARDIAN mstgrd = new MASTERGUARDIAN
                            {
                                MEMBER_CODE = dMember_Code,
                                NAME = txGurName.Text,
                                RELATION_CODE = dGRelationcode,
                                ADDRESS1 = txtGurAddress.Text,
                                ADDRESS2 = txtGurAddress2.Text,
                                ADDRESS3 = txtGurAddress3.Text,
                                AGE = dGAge,
                                SEX = cmbGurSex.Text,
                                ICNO_NEW = txtGurICNO.Text,
                                ICNO_OLD = txtGurOldICNO.Text,
                                CITY_CODE = dGCitycode,
                                STATE_CODE = dGStatecode,
                                COUNTRY = cmbGurCountry.Text,
                                ZIPCODE = txtGurZipCode.Text,
                                PHONE = txtGurPhNo.Text,
                                MOBILE = txtGurMobileNo.Text,
                            };
                            db.MASTERGUARDIANs.Add(mstgrd);
                            db.SaveChanges();

                            var NewData1 = new JSonHelper().ConvertObjectToJSon(mstgrd);
                            AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData1, "MASTERGUARDIAN");
                        }

                        DataTable dt = new DataTable();
                        if (dgNomination.Items.Count > 0)
                        {
                            dt = ((DataView)dgNomination.ItemsSource).ToTable();
                            if (dt.Rows.Count > 0)
                            {
                                List<MASTERNOMINEE> lstNominee = new List<MASTERNOMINEE>();
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    MASTERNOMINEE mstnom = new MASTERNOMINEE
                                    {
                                        NAME = dt.Rows[i]["NAME"].ToString(),
                                        AGE = Convert.ToDecimal(dt.Rows[i]["AGE"]),
                                        SEX = dt.Rows[i]["SEX"].ToString(),
                                        RELATION_CODE = Convert.ToDecimal(dt.Rows[i]["RELATION_CODE"]),
                                        ICNO_NEW = dt.Rows[i]["ICNO_NEW"].ToString(),
                                        ICNO_OLD = dt.Rows[i]["ICNO_OLD"].ToString(),
                                        ADDRESS1 = dt.Rows[i]["ADDRESS1"].ToString(),
                                        ADDRESS2 = dt.Rows[i]["ADDRESS2"].ToString(),
                                        ADDRESS3 = dt.Rows[i]["ADDRESS3"].ToString(),
                                        CITY_CODE = Convert.ToDecimal((dt.Rows[i]["CITY_CODE"]) == null ? 0 : dt.Rows[i]["CITY_CODE"]),
                                        STATE_CODE = Convert.ToDecimal((dt.Rows[i]["STATE_CODE"]) == null ? 0 : dt.Rows[i]["STATE_CODE"]),
                                        MOBILE = dt.Rows[i]["MOBILE"].ToString(),
                                        PHONE = dt.Rows[i]["PHONE"].ToString(),
                                        MEMBER_CODE = dMember_Code,
                                    };
                                    lstNominee.Add(mstnom);
                                }

                                if (lstNominee.Count > 0)
                                {
                                    db.MASTERNOMINEEs.AddRange(lstNominee);
                                    db.SaveChanges();

                                    var NewData2 = new JSonHelper().ConvertObjectToJSon(lstNominee);
                                    AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData2, "MASTERNOMINEE");
                                }
                            }
                        }
                        MessageBox.Show("Saved Sucessfully");
                        fNew();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dMember_Code != 0)
                {
                    if (MessageBox.Show("Do you want to delete this Member?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        MASTERMEMBER mr = (from mas in db.MASTERMEMBERs where mas.MEMBER_CODE == dMember_Code select mas).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(mr);

                        if (mr != null)
                        {
                            db.MASTERMEMBERs.Remove(mr);
                            db.SaveChanges();

                            AppLib.EventHistory(this.Tag.ToString(), 2, OldData, "", "MASTERMEMBER");

                            var mn = (from mas in db.MASTERNOMINEEs where mas.MEMBER_CODE == dMember_Code select mas).ToList();
                            var OldData1 = new JSonHelper().ConvertObjectToJSon(mn);
                            if (mn != null)
                            {
                                db.MASTERNOMINEEs.RemoveRange(db.MASTERNOMINEEs.Where(x => x.MEMBER_CODE == dMember_Code));
                                db.SaveChanges();

                                AppLib.EventHistory(this.Tag.ToString(), 2, OldData1, "", "MASTERNOMINEE");
                            }

                            MASTERGUARDIAN mg = (from mas in db.MASTERGUARDIANs where mas.MEMBER_CODE == dMember_Code select mas).FirstOrDefault();
                            var OldData2 = new JSonHelper().ConvertObjectToJSon(mg);
                            if (mg != null)
                            {
                                db.MASTERGUARDIANs.Remove(mg);
                                db.SaveChanges();

                                AppLib.EventHistory(this.Tag.ToString(), 2, OldData2, "", "MASTERGUARDIAN");
                            }
                            MessageBox.Show("Deleted Sucessfully");
                            fNew();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please Select Any Member!");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMemberNo.Text))
                {
                    MessageBox.Show("Member No is Empty!");
                    txtMemberNo.Focus();
                    return;
                }
                else if (dMember_Code == 0)
                {
                    MessageBox.Show("Please Select any Member!");
                    txtMemberNo.Focus();
                    return;
                }
                else
                {
                    frmMemberHistory frm = new frmMemberHistory("MEMBER REGISTRATION");
                    frm.FormLoad(Convert.ToDecimal(txtMemberNo.Text), dtpDOJ.SelectedDate.Value, cmbBankName.Text, cmbBranchName.Text, txtMonthlyBF.Text, txtMonthlySub.Text, cmbMemberType.Text.ToString());
                    //this.Close();
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            frmMemberQuery frm = new frmMemberQuery("Registration");
            this.Close();
            frm.ShowDialog();
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            frmHomeMembership frm = new frmHomeMembership();
            this.Close();
            frm.ShowDialog();
        }

        private void btnNomAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMemberNo.Text))
                {
                    MessageBox.Show("Membership No is Empty!");
                    txtMemberNo.Focus();
                }
                else if (string.IsNullOrEmpty(txtNomName.Text))
                {
                    MessageBox.Show("Nominee Name is Empty!");
                    txtNomName.Focus();
                }
                else if (string.IsNullOrEmpty(cmbNomRelation.Text))
                {
                    MessageBox.Show("Nominee Relation is Empty!");
                    cmbNomRelation.Focus();
                }
                else if (string.IsNullOrEmpty(cmbNomSex.Text))
                {
                    MessageBox.Show("Nominee SEX is Empty!");
                    cmbNomSex.Focus();
                }
                else if (string.IsNullOrEmpty(txtNomAge.Text))
                {
                    MessageBox.Show("Nominee Age is Empty!");
                    txtNomAge.Focus();
                }
                else
                {
                    decimal dCitycode = 0;
                    decimal dStatecode = 0;
                    decimal dRelation = 0;
                    decimal dCountry = 0;
                    string sCity = "";
                    string sState = "";
                    string sRelation = "";
                    string sCountry = "";
                    if (Convert.ToDecimal(cmbNomState.SelectedValue) != 0)
                    {
                        dStatecode = Convert.ToDecimal(cmbNomState.SelectedValue);
                    }
                    if (Convert.ToDecimal(cmbNomRelation.SelectedValue) != 0)
                    {
                        dRelation = Convert.ToDecimal(cmbNomRelation.SelectedValue);
                    }
                    if (Convert.ToDecimal(cmbNomCity.SelectedValue) != 0)
                    {
                        dCitycode = Convert.ToDecimal(cmbNomCity.SelectedValue);
                    }
                    if (Convert.ToDecimal(cmbNomCountry.SelectedValue) != 0)
                    {
                        dCountry = Convert.ToDecimal(cmbNomCountry.SelectedValue);
                    }

                    var QR = db.MASTERCITies.Where(X => X.CITY_CODE == dCitycode).FirstOrDefault();
                    var ST = db.MASTERSTATEs.Where(X => X.STATE_CODE == dStatecode).FirstOrDefault();
                    var rt = db.MASTERRELATIONs.Where(X => X.RELATION_CODE == dRelation).FirstOrDefault();
                    var Co = db.CountrySetups.Where(X => X.ID == dCountry).FirstOrDefault();

                    if (QR != null)
                    {
                        sCity = QR.CITY_NAME;
                    }
                    if (ST != null)
                    {
                        sState = ST.STATE_NAME;
                    }
                    if (rt != null)
                    {
                        sRelation = rt.RELATION_NAME;
                    }
                    if (Co != null)
                    {
                        sCountry = Co.CountryName;
                    }

                    MASTERNOMINEENAMES mstnom = new MASTERNOMINEENAMES
                    {
                        NAME = txtNomName.Text,
                        AGE = Convert.ToDecimal(txtNomAge.Text),
                        SEX = cmbNomSex.Text.ToString(),
                        RELATION_CODE = dRelation,
                        ICNO_NEW = txtNomICNO.Text,
                        ICNO_OLD = txtNomOldICNO.Text,
                        ADDRESS1 = txtNomAddress.Text,
                        ADDRESS2 = txtNomAddress2.Text,
                        ADDRESS3 = txtNomAddress3.Text,
                        CITY_CODE = dCitycode,
                        STATE_CODE = dStatecode,
                        MOBILE = txtNomMobileNo.Text,
                        PHONE = txtNomPhNo.Text,
                        MEMBER_CODE = dMember_Code,
                        CITY_NAME = sCity,
                        STATE_NAME = sState,
                        RELATION_NAME = sRelation,
                    };
                    lstmstnom.Add(mstnom);
                    DataTable dt = new DataTable();
                    dt = AppLib.LINQResultToDataTable(lstmstnom);
                    dgNomination.ItemsSource = dt.DefaultView;

                    txtNomName.Text = "";
                    txtNomAge.Text = "";
                    cmbNomSex.Text = "";
                    cmbNomRelation.Text = "";
                    cmbNomCity.Text = "";
                    cmbNomState.Text = "";
                    txtNomICNO.Text = "";
                    txtNomOldICNO.Text = "";
                    txtNomAddress.Text = "";
                    txtNomAddress2.Text = "";
                    txtNomAddress3.Text = "";
                    txtNomMobileNo.Text = "";
                    txtNomPhNo.Text = "";
                    cmbNomCountry.Text = "";
                    txtNomPostalCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnNomClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtNomName.Text = "";
                txtNomAge.Text = "";
                cmbNomSex.Text = "";
                cmbNomRelation.Text = "";
                cmbNomCity.Text = "";
                cmbNomState.Text = "";
                txtNomICNO.Text = "";
                txtNomOldICNO.Text = "";
                txtNomAddress.Text = "";
                txtNomAddress2.Text = "";
                txtNomAddress3.Text = "";
                txtNomMobileNo.Text = "";
                txtNomPhNo.Text = "";
                cmbNomCountry.Text = "";
                txtNomPostalCode.Text = "";
                dgNomination.ItemsSource = null;
                lstmstnom.Clear();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSetNomState_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmStateSetup frm = new frmStateSetup("REGISTRATION");
                frm.ShowDialog();

                cmbNomState.ItemsSource = db.MASTERSTATEs.ToList();
                cmbNomState.SelectedValuePath = "STATE_CODE";
                cmbNomState.DisplayMemberPath = "STATE_NAME";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSetResState_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmStateSetup frm = new frmStateSetup("REGISTRATION");
                frm.ShowDialog();

                cmbNomState.ItemsSource = db.MASTERSTATEs.ToList();
                cmbNomState.SelectedValuePath = "STATE_CODE";
                cmbNomState.DisplayMemberPath = "STATE_NAME";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSetResCity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmCitySetup frm = new frmCitySetup("REGISTRATION");
                frm.ShowDialog();

                cmbResCity.ItemsSource = db.MASTERCITies.ToList();
                cmbResCity.SelectedValuePath = "CITY_CODE";
                cmbResCity.DisplayMemberPath = "CITY_NAME";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSetNomRelation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmRelationSetup frm = new frmRelationSetup("REGISTRATION");
                frm.ShowDialog();

                cmbNomRelation.ItemsSource = db.MASTERRELATIONs.ToList();
                cmbNomRelation.SelectedValuePath = "RELATION_CODE";
                cmbNomRelation.DisplayMemberPath = "RELATION_NAME";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSetNomCity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmCitySetup frm = new frmCitySetup("REGISTRATION");
                frm.ShowDialog();

                cmbNomCity.ItemsSource = db.MASTERCITies.ToList();
                cmbNomCity.SelectedValuePath = "CITY_CODE";
                cmbNomCity.DisplayMemberPath = "CITY_NAME";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSetGurRelation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmRelationSetup frm = new frmRelationSetup("REGISTRATION");
                frm.ShowDialog();

                cmbGurRelation.ItemsSource = db.MASTERRELATIONs.ToList();
                cmbGurRelation.SelectedValuePath = "RELATION_CODE";
                cmbGurRelation.DisplayMemberPath = "RELATION_NAME";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSetGurCountry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmCountrySetup frm = new frmCountrySetup("REGISTRATION");
                frm.ShowDialog();

                cmbGurCountry.ItemsSource = db.CountrySetups.ToList();
                cmbGurCountry.SelectedValuePath = "Id";
                cmbGurCountry.DisplayMemberPath = "CountryName";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSetGurState_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmStateSetup frm = new frmStateSetup("REGISTRATION");
                frm.ShowDialog();

                cmbGurState.ItemsSource = db.MASTERSTATEs.ToList();
                cmbGurState.SelectedValuePath = "STATE_CODE";
                cmbGurState.DisplayMemberPath = "STATE_NAME";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSetGurCity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmCitySetup frm = new frmCitySetup("REGISTRATION");
                frm.ShowDialog();

                cmbGurCity.ItemsSource = db.MASTERCITies.ToList();
                cmbGurCity.SelectedValuePath = "CITY_CODE";
                cmbGurCity.DisplayMemberPath = "CITY_NAME";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSetNomCountry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmCountrySetup frm = new frmCountrySetup("REGISTRATION");
                frm.ShowDialog();

                cmbNomCountry.ItemsSource = db.CountrySetups.ToList();
                cmbNomCountry.SelectedValuePath = "Id";
                cmbNomCountry.DisplayMemberPath = "CountryName";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSetResCountry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmCountrySetup frm = new frmCountrySetup("REGISTRATION");
                frm.ShowDialog();

                cmbResCountry.ItemsSource = db.CountrySetups.ToList();
                cmbResCountry.SelectedValuePath = "Id";
                cmbResCountry.DisplayMemberPath = "CountryName";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSetBank_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmBankSetup frm = new frmBankSetup("REGISTRATION");
                frm.ShowDialog();

                cmbBankCode.ItemsSource = db.MASTERBANKs.ToList();
                cmbBankCode.SelectedValuePath = "BANK_CODE";
                cmbBankCode.DisplayMemberPath = "BANK_USERCODE";

                cmbBankName.ItemsSource = db.MASTERBANKs.ToList();
                cmbBankName.SelectedValuePath = "BANK_CODE";
                cmbBankName.DisplayMemberPath = "BANK_NAME";

                decimal dBankCode = Convert.ToDecimal(cmbBankCode.SelectedValue);
                if (dBankCode != 0)
                {
                    var mBnk = db.MASTERBANKs.Where(x => x.BANK_CODE == dBankCode).FirstOrDefault();
                    cmbBankName.SelectedValue = mBnk.BANK_CODE;

                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANK_CODE == dBankCode).ToList();
                    cmbBranchCode.ItemsSource = mbr;
                    cmbBranchCode.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchCode.DisplayMemberPath = "BANKBRANCH_USERCODE";

                    cmbBranchName.ItemsSource = mbr;
                    cmbBranchName.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchName.DisplayMemberPath = "BANKBRANCH_NAME";
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSetBranch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmBranchSetup frm = new frmBranchSetup("REGISTRATION");
                frm.ShowDialog();

                cmbBankCode.ItemsSource = db.MASTERBANKs.ToList();
                cmbBankCode.SelectedValuePath = "BANK_CODE";
                cmbBankCode.DisplayMemberPath = "BANK_USERCODE";

                cmbBankName.ItemsSource = db.MASTERBANKs.ToList();
                cmbBankName.SelectedValuePath = "BANK_CODE";
                cmbBankName.DisplayMemberPath = "BANK_NAME";

                decimal dBankCode = Convert.ToDecimal(cmbBankCode.SelectedValue);
                if (dBankCode != 0)
                {
                    var mBnk = db.MASTERBANKs.Where(x => x.BANK_CODE == dBankCode).FirstOrDefault();
                    cmbBankName.SelectedValue = mBnk.BANK_CODE;

                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANK_CODE == dBankCode).ToList();
                    cmbBranchCode.ItemsSource = mbr;
                    cmbBranchCode.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchCode.DisplayMemberPath = "BANKBRANCH_USERCODE";

                    cmbBranchName.ItemsSource = mbr;
                    cmbBranchName.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchName.DisplayMemberPath = "BANKBRANCH_NAME";
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion

        #region "USER DEFINED FUNCTIONS"

        public partial class MASTERNOMINEENAMES
        {
            public Nullable<decimal> MEMBER_CODE { get; set; }
            public string ICNO_OLD { get; set; }
            public string ICNO_NEW { get; set; }
            public string NAME { get; set; }
            public string SEX { get; set; }
            public Nullable<decimal> AGE { get; set; }
            public Nullable<decimal> RELATION_CODE { get; set; }
            public string ADDRESS1 { get; set; }
            public string ADDRESS2 { get; set; }
            public string ADDRESS3 { get; set; }
            public Nullable<decimal> CITY_CODE { get; set; }
            public Nullable<decimal> STATE_CODE { get; set; }
            public string COUNTRY { get; set; }
            public string ZIPCODE { get; set; }
            public string PHONE { get; set; }
            public string MOBILE { get; set; }
            public Nullable<decimal> USER_CODE { get; set; }
            public Nullable<System.DateTime> ENTRY_DATE { get; set; }
            public string ENTRY_TIME { get; set; }
            public int ID { get; set; }
            public string CITY_NAME { get; set; }
            public string STATE_NAME { get; set; }
            public string RELATION_NAME { get; set; }
            public string COUNTRY_CODE { get; set; }
        }

        void fNew()
        {
            ckbRejoined.IsEnabled = true;
            cmbRace.Text = "";
            cmbBankCode.Text = "";
            cmbBankName.Text = "";
            cmbBranchCode.Text = "";
            cmbBranchName.Text = "";
            txtCity.Text = "";
            txtState.Text = "";
            cmbMemberType.Text = "";

            cmbResCity.Text = "";
            cmbResState.Text = "";

            cmbGurCity.Text = "";
            cmbGurState.Text = "";
            cmbGurRelation.Text = "";
            txtMemberNo.Text = "";
            cmbMemberInit.Text = "";
            txtMemberName.Text = "";
            dtpDOB.Text = "";
            txtAge.Text = "";
            cmbGender.Text = "";
            ckbRejoined.IsChecked = false;
            txtNewIC.Text = "";
            txtOldIC.Text = "";
            dtpDOJ.Text = "";
            dtpDOEmp.Text = "";
            txtSalary.Text = "";
            cmbLevy.Text = "";

            txtNomName.Text = "";
            txtNomAge.Text = "";
            cmbNomSex.Text = "";
            cmbNomRelation.Text = "";
            cmbNomCity.Text = "";
            cmbNomState.Text = "";
            txtNomICNO.Text = "";
            txtNomOldICNO.Text = "";
            txtNomAddress.Text = "";
            txtNomAddress2.Text = "";
            txtNomAddress3.Text = "";
            txtNomMobileNo.Text = "";
            txtNomPhNo.Text = "";
            cmbNomCountry.Text = "";
            txtNomPostalCode.Text = "";
            dgNomination.ItemsSource = null;

            txtResAddress1.Text = "";
            txtResAddress2.Text = "";
            txtResAddress3.Text = "";
            txtResPhoneNo.Text = "";
            txtResMobileNo.Text = "";
            txtEmail.Text = "";
            txtResPostalCode.Text = "";
            cmbResCountry.Text = "";

            txtAddress.Text = "";
            txtAddress2.Text = "";
            txtAddress3.Text = "";
            txtPostalCode.Text = "";
            txtCountry.Text = "";
            txtPhoneNo.Text = "";
            txtMobileNo.Text = "";
            txtEmail.Text = "";

            txGurName.Text = "";
            txtGurAddress.Text = "";
            txtGurAddress2.Text = "";
            txtGurAddress3.Text = "";
            txtGurAge.Text = "";
            cmbGurSex.Text = "";
            txtGurICNO.Text = "";
            txtGurOldICNO.Text = "";
            cmbGurCountry.Text = "";
            txtGurZipCode.Text = "";
            txtGurPhNo.Text = "";
            txtGurMobileNo.Text = "";
            dMember_Code = 0;
            lblStatus.Content = "";
            txtResEmail.Text = "";
            cmbResMail.Text = "";
            txtTDFAmount.Text = "";
            txtLevyAmount.Text = "";
            cmbTDF.Text = "";

            txtLevyAmount.Text = "";
            txtTDFAmount.Text = "";
            bIsUpdate = false;
            LoadFundDetails();

            FormLoad();
        }

        void LoadTempViewMaster()
        {
            try
            {
                if (AppLib.lstMstMember.Count == 0)
                {
                    var lstMM = (from x in db.ViewMasterMembers select x).ToList();
                    AppLib.lstMstMember = lstMM;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        void LoadFundDetails()
        {
            var ns = db.MASTERNAMESETUPs.FirstOrDefault();

            if (ns != null)
            {
                txtEntranceFee.Text = ns.EnterenceFees.ToString();
                txtBuildingFund.Text = ns.BuildingFund.ToString();
                txtBadgeAmt.Text = ns.BadgeAmount.ToString();
                txtMonthlyBF.Text = ns.BF.ToString();
                txtMonthlyIns.Text = ns.Insurance.ToString();
                txtAccBF.Text = ns.BF.ToString();
                txtAccIns.Text = ns.Insurance.ToString();

                txtCurrentYTDBF.Text = ns.BF.ToString();
                txtCurrentYTDIns.Text = ns.Insurance.ToString();
            }
            else
            {
                txtEntranceFee.Text = "0";
                txtBuildingFund.Text = "0";
                txtBadgeAmt.Text = "0";

                txtMonthlyBF.Text = "0";
                txtMonthlyIns.Text = "0";
                txtAccBF.Text = "0";
                txtAccIns.Text = "0";

                txtCurrentYTDBF.Text = "0";
                txtCurrentYTDIns.Text = "0";
            }
            txtAccBenefit.Text = "0";
            txtServicePeriod.Text = "0";
            txtMonthlySub.Text = "0";
            txtAccSub.Text = "0";
            txtCurrentYTDSub.Text = "0";
            txtTotalMonthPaidSubs.Text = "1";
            txtTotalMonthPaidBF.Text = "1";
            txtTotalMonthPaidIns.Text = "1";
            txtTotalMonthsDueSubs.Text = "0";
            txtTotalMonthsDueBF.Text = "0";
            txtTotalMonthsDueIns.Text = "0";

            dtpLastPay.SelectedDate = DateTime.Today;
        }

        void FormLoad()
        {
            try
            {
                var city = db.MASTERCITies.ToList();
                var state = db.MASTERSTATEs.ToList();
                var country = db.CountrySetups.ToList();
                var relation = db.MASTERRELATIONs.ToList();
                var title = db.NameTitleSetups.ToList();

                cmbBankCode.ItemsSource = db.MASTERBANKs.ToList();
                cmbBankCode.SelectedValuePath = "BANK_CODE";
                cmbBankCode.DisplayMemberPath = "BANK_USERCODE";

                cmbBankName.ItemsSource = db.MASTERBANKs.ToList();
                cmbBankName.SelectedValuePath = "BANK_CODE";
                cmbBankName.DisplayMemberPath = "BANK_NAME";

                cmbBranchCode.ItemsSource = db.MASTERBANKBRANCHes.ToList();
                cmbBranchCode.SelectedValuePath = "BANKBRANCH_CODE";
                cmbBranchCode.DisplayMemberPath = "BANKBRANCH_USERCODE";

                cmbBranchName.ItemsSource = db.MASTERBANKBRANCHes.ToList();
                cmbBranchName.SelectedValuePath = "BANKBRANCH_CODE";
                cmbBranchName.DisplayMemberPath = "BANKBRANCH_NAME";

                cmbMemberType.ItemsSource = db.MASTERMEMBERTYPEs.ToList();
                cmbMemberType.SelectedValuePath = "MEMBERTYPE_CODE";
                cmbMemberType.DisplayMemberPath = "MEMBERTYPE_NAME";

                cmbRace.ItemsSource = db.MASTERRACEs.ToList();
                cmbRace.SelectedValuePath = "RACE_CODE";
                cmbRace.DisplayMemberPath = "RACE_NAME";

                cmbMemberInit.ItemsSource = title;
                cmbMemberInit.SelectedValuePath = "ID";
                cmbMemberInit.DisplayMemberPath = "TitleName";

                cmbResCity.ItemsSource = city;
                cmbResCity.SelectedValuePath = "CITY_CODE";
                cmbResCity.DisplayMemberPath = "CITY_NAME";

                cmbResState.ItemsSource = state;
                cmbResState.SelectedValuePath = "STATE_CODE";
                cmbResState.DisplayMemberPath = "STATE_NAME";

                cmbResCountry.ItemsSource = country;
                cmbResCountry.SelectedValuePath = "ID";
                cmbResCountry.DisplayMemberPath = "CountryName";

                cmbNomRelation.ItemsSource = relation;
                cmbNomRelation.SelectedValuePath = "RELATION_CODE";
                cmbNomRelation.DisplayMemberPath = "RELATION_NAME";

                cmbNomCity.ItemsSource = city;
                cmbNomCity.SelectedValuePath = "CITY_CODE";
                cmbNomCity.DisplayMemberPath = "CITY_NAME";

                cmbNomState.ItemsSource = state;
                cmbNomState.SelectedValuePath = "STATE_CODE";
                cmbNomState.DisplayMemberPath = "STATE_NAME";

                cmbNomCountry.ItemsSource = country;
                cmbNomCountry.SelectedValuePath = "ID";
                cmbNomCountry.DisplayMemberPath = "CountryName";

                cmbGurCity.ItemsSource = city;
                cmbGurCity.SelectedValuePath = "CITY_CODE";
                cmbGurCity.DisplayMemberPath = "CITY_NAME";

                cmbGurState.ItemsSource = state;
                cmbGurState.SelectedValuePath = "STATE_CODE";
                cmbGurState.DisplayMemberPath = "STATE_NAME";

                cmbGurCountry.ItemsSource = country;
                cmbGurCountry.SelectedValuePath = "ID";
                cmbGurCountry.DisplayMemberPath = "CountryName";

                cmbGurRelation.ItemsSource = relation;
                cmbGurRelation.SelectedValuePath = "RELATION_CODE";
                cmbGurRelation.DisplayMemberPath = "RELATION_NAME";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void FormFill()
        {
            try
            {
                ckbRejoined.IsEnabled = false;

                var qry = (from x in db.MASTERMEMBERs where x.MEMBER_CODE == dMember_Code orderby x.DATEOFJOINING descending select x).FirstOrDefault();

                var status = (from x in db.MemberStatusLogs where x.MEMBER_CODE == dMember_Code select x).FirstOrDefault();

                var brnch = (from bc in db.MASTERBANKBRANCHes
                             join ct in db.MASTERCITies on bc.BANKBRANCH_CITY_CODE equals ct.CITY_CODE
                             join st in db.MASTERSTATEs on bc.BANKBRANCH_STATE_CODE equals st.STATE_CODE
                             where bc.BANKBRANCH_CODE == qry.BRANCH_CODE
                             select new
                             {
                                 bc.BANKBRANCH_ADDRESS1,
                                 bc.BANKBRANCH_ADDRESS2,
                                 bc.BANKBRANCH_ADDRESS3,
                                 ct.CITY_NAME,
                                 st.STATE_NAME,
                                 bc.BANKBRANCH_ZIPCODE,
                                 bc.BANKBRANCH_COUNTRY,
                                 bc.BANKBRANCH_PHONE1,
                                 bc.BANKBRANCH_PHONE2,
                                 bc.BANKBRANCH_EMAIL
                             }
                              ).FirstOrDefault();

                var nominee = (from x in db.VIEWNOMINEEs where x.MEMBER_CODE == dMember_Code orderby x.ID descending select x).ToList();
                if (nominee != null)
                {
                    DataTable dt = new DataTable();
                    dt = AppLib.LINQResultToDataTable(nominee);
                    if (dt.Rows.Count > 0)
                    {
                        dgNomination.ItemsSource = dt.DefaultView;
                    }
                    foreach (DataRow dr in dt.Rows)
                    {
                        MASTERNOMINEENAMES mstnom = new MASTERNOMINEENAMES
                        {
                            NAME = dr["NAME"].ToString(),
                            AGE = Convert.ToDecimal(dr["AGE"]),
                            SEX = dr["SEX"].ToString(),
                            RELATION_CODE = Convert.ToDecimal(dr["RELATION_CODE"]),
                            ICNO_NEW = dr["ICNO_NEW"].ToString(),
                            ICNO_OLD = dr["ICNO_OLD"].ToString(),
                            ADDRESS1 = dr["ADDRESS1"].ToString(),
                            ADDRESS2 = dr["ADDRESS2"].ToString(),
                            ADDRESS3 = dr["ADDRESS3"].ToString(),
                            CITY_CODE = Convert.ToDecimal(dr["CITY_CODE"]),
                            STATE_CODE = Convert.ToDecimal(dr["STATE_CODE"]),
                            MOBILE = dr["MOBILE"].ToString(),
                            PHONE = dr["PHONE"].ToString(),
                            MEMBER_CODE = dMember_Code,
                            CITY_NAME = dr["CITY_NAME"].ToString(),
                            STATE_NAME = dr["STATE_NAME"].ToString(),
                            RELATION_NAME = dr["RELATION_NAME"].ToString(),
                        };
                        lstmstnom.Add(mstnom);
                    }
                }

                var gurdian = (from gr in db.MASTERGUARDIANs
                               where gr.MEMBER_CODE == dMember_Code
                               select new
                               {
                                   Name = gr.NAME,
                                   gr.RELATION_CODE,
                                   gr.ADDRESS1,
                                   gr.ADDRESS2,
                                   gr.ADDRESS3,
                                   gr.AGE,
                                   gr.ICNO_NEW,
                                   gr.ICNO_OLD,
                                   gr.CITY_CODE,
                                   gr.STATE_CODE,
                                   gr.COUNTRY,
                                   gr.ZIPCODE,
                                   gr.PHONE,
                                   gr.MOBILE,
                                   gr.SEX
                               }
                               ).FirstOrDefault();

                DataTable dtFee = new DataTable();
                using (SqlConnection con = new SqlConnection(AppLib.connStr))
                {
                    SqlCommand cmd;
                    string str = "SELECT ISNULL(AMOUNTBF,0)AMOUNTBF,ISNULL(UNIONCONTRIBUTION,0)UNIONCONTRIBUTION,ISNULL(AMOUNTINS,0)AMOUNTINS, \r" +
                                 " ISNULL(AMTSUBS,0)AMTSUBS,ISNULL(TOTALMONTHSPAID, 0)TOTALMONTHSPAID,ISNULL(TOTALMONTHSPAIDINS, 0)TOTALMONTHSPAIDINS \r" +
                                 " FROM FEESDETAILS(NOLOCK)" +
                                 " WHERE UPDATEDSTATUS = 'NOT UPDATED' AND MEMBERCODE=" + dMember_Code;

                    cmd = new SqlCommand(str, con);
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dtFee);
                }

                BF = 0; UC = 0; Ins = 0; Subs = 0; dTotlMonthsPaid = 0; dTotlMonthsPaidUC = 0;
                foreach (DataRow dr in dtFee.Rows)
                {
                    BF = BF + Convert.ToDecimal(dr["AmountBf"]);
                    UC = UC + Convert.ToDecimal(dr["UnionContribution"]);
                    Ins = Ins + Convert.ToDecimal(dr["AmountIns"]);
                    Subs = Subs + Convert.ToDecimal(dr["AmtSubs"]);
                    dTotlMonthsPaid = dTotlMonthsPaid + Convert.ToDecimal(dr["TotalMonthsPaid"]);
                    dTotlMonthsPaidUC = dTotlMonthsPaidUC + Convert.ToDecimal(dr["TotalMonthsPaidIns"]);
                }
                //var ArPost = (from ap in db.ArrearPostDetails where ap.MemberCode == dMember_Code && ap.UpdatedStatus == "Not Updated" select ap).ToList();
                //DataTable dtArrearPost = AppLib.LINQResultToDataTable(ArPost);
                //foreach (DataRow dr in dtArrearPost.Rows)
                //{
                //    BF = BF + Convert.ToDecimal(dr["AmountBf"]);
                //    Ins = Ins + Convert.ToDecimal(dr["AmountIns"]);
                //    Subs = Subs + Convert.ToDecimal(dr["AmtSubs"]);
                //    dTotlMonthsPaid = dTotlMonthsPaid + 1;
                //}
                //var ArPre = (from ap in db.ArrearPreDetails where ap.MemberCode == dMember_Code && ap.UpdatedStatus == "Not Updated" select ap).ToList();
                //DataTable dtArrearPre = AppLib.LINQResultToDataTable(ArPre);
                //foreach (DataRow dr in dtArrearPre.Rows)
                //{
                //    BF = BF + Convert.ToDecimal(dr["AmountBf"]);
                //    Ins = Ins + Convert.ToDecimal(dr["AmountIns"]);
                //    Subs = Subs + Convert.ToDecimal(dr["AmtSubs"]);
                //    dTotlMonthsPaid = dTotlMonthsPaid + 1;
                //}

                if (qry != null)
                {
                    cmbMemberType.SelectedValue = qry.MEMBERTYPE_CODE;
                    txtMemberNo.Text = qry.MEMBER_ID.ToString();
                    cmbMemberInit.Text = qry.MEMBER_TITLE;
                    txtMemberName.Text = qry.MEMBER_NAME;
                    txtAge.Text = qry.AGE_IN_YEARS.ToString();
                    txtResAddress1.Text = qry.ADDRESS1;
                    txtResAddress2.Text = qry.ADDRESS2;
                    txtResAddress3.Text = qry.ADDRESS3;
                    txtResPhoneNo.Text = qry.PHONE;
                    txtResMobileNo.Text = qry.MOBILE;
                    dtpDOB.SelectedDate = Convert.ToDateTime(qry.DATEOFBIRTH);
                    cmbGender.Text = qry.SEX;
                    ckbRejoined.IsChecked = Convert.ToBoolean(qry.REJOINED);
                    cmbRace.SelectedValue = qry.RACE_CODE;
                    txtNewIC.Text = qry.ICNO_NEW;
                    txtOldIC.Text = qry.ICNO_OLD;
                    dtpDOJ.SelectedDate = Convert.ToDateTime(qry.DATEOFJOINING);

                    cmbBankCode.SelectedValue = qry.BANK_CODE;
                    cmbBankName.SelectedValue = qry.BANK_CODE;
                    cmbBranchCode.SelectedValue = qry.BRANCH_CODE;
                    cmbBranchName.SelectedValue = qry.BRANCH_CODE;
                    dtpDOEmp.SelectedDate = Convert.ToDateTime(qry.DATEOFEMPLOYMENT);
                    txtSalary.Text = qry.Salary.ToString();
                    if (qry.TDF != null)
                    {
                        cmbTDF.Text = qry.TDF.ToString();
                    }

                    if (qry.LEVY != null)
                    {
                        cmbLevy.Text = qry.LEVY.ToString();
                    }

                    if (qry.LEVY_AMOUNT != null)
                    {
                        txtLevyAmount.Text = qry.LEVY_AMOUNT.ToString();
                    }

                    if (qry.TDF_AMOUNT != null)
                    {
                        txtTDFAmount.Text = qry.TDF_AMOUNT.ToString();
                    }

                    txtEntranceFee.Text = qry.ENTRANCEFEE.ToString();
                    txtBuildingFund.Text = qry.HQFEE.ToString();
                    txtAccBenefit.Text = qry.ACCBENEFIT.ToString();

                    txtMonthlySub.Text = qry.MONTHLYSUBSCRIPTION.ToString();
                    txtMonthlyBF.Text = qry.MONTHLYBF.ToString();
                    txtMonthlyUC.Text = "7";
                    txtMonthlyIns.Text = "10";

                    txtAccSub.Text = (qry.ACCSUBSCRIPTION + Subs).ToString();
                    txtAccBF.Text = (qry.ACCBF + BF).ToString();
                    txtAccUC.Text = (dTotlMonthsPaidUC * 7).ToString();
                    txtAccIns.Text = Ins.ToString();

                    txtCurrentYTDSub.Text = Subs.ToString();
                    txtCurrentYTDBF.Text = BF.ToString();
                    txtCurrentYTDUC.Text = (dTotlMonthsPaidUC * 7).ToString();
                    txtCurrentYTDIns.Text = Ins.ToString();

                    txtTotalMonthPaidSubs.Text = (qry.TOTALMONTHSPAID + dTotlMonthsPaid).ToString();
                    txtTotalMonthPaidBF.Text = (qry.TOTALMONTHSPAID + dTotlMonthsPaid).ToString();
                    txtTotalMonthPaidUC.Text = (dTotlMonthsPaidUC).ToString();
                    txtTotalMonthPaidIns.Text = Ins.ToString();

                    txtTotalMonthsDueSubs.Text = status.TOTALMOTHSDUE.ToString();
                    txtTotalMonthsDueBF.Text = status.TOTALMOTHSDUE.ToString();
                    txtTotalMonthsDueUC.Text = "0";
                    txtTotalMonthsDueIns.Text = "0";

                    dtpLastPay.SelectedDate = Convert.ToDateTime(status.LASTPAYMENT_DATE);
                    dtpLastPay.IsEnabled = false;

                    txtBadgeAmt.Text = qry.BatchAmt.ToString();
                    TimeSpan ts = Convert.ToDateTime(status.LASTPAYMENT_DATE) - Convert.ToDateTime(status.DATEOFJOINING);

                    int iSerYear = Convert.ToInt32(ts.Days) / 365;
                    txtServicePeriod.Text = iSerYear.ToString();

                    if (qry.EMAIL != null)
                    {
                        string str = qry.EMAIL.ToString();
                        if (str.Contains("@") == true)
                        {
                            string[] Email = qry.EMAIL.Split('@');
                            txtResEmail.Text = Email[0];
                            cmbResMail.Text = "@" + Email[1];
                        }
                        else
                        {
                            txtResEmail.Text = qry.EMAIL.ToString();
                        }
                    }

                    if (qry.CITY_CODE != null)
                    {
                        cmbResCity.SelectedValue = qry.CITY_CODE;
                    }
                    if (qry.ZIPCODE != null)
                    {
                        txtResPostalCode.Text = qry.ZIPCODE;
                    }
                    if (qry.STATE_CODE != null)
                    {
                        cmbResState.SelectedValue = qry.STATE_CODE;
                    }
                    if (qry.COUNTRY != null)
                    {
                        cmbResCountry.Text = qry.COUNTRY;
                    }

                    if (status.RESIGNED == true || status.MEMBERSTATUSCODE == 6)
                    {
                        lblStatus.Content = string.Format("Member Resigned, {0:dd/MMM/yyyy}", status.VOUCHER_DATE);
                    }
                    else
                    {
                        if (status.MEMBERSTATUSCODE == 1)
                        {
                            lblStatus.Content = "Active Member; " + status.TOTALMOTHSDUE + " Arrears Pending";
                        }
                        else if (status.MEMBERSTATUSCODE == 2)
                        {
                            lblStatus.Content = "Defaulter; " + status.TOTALMOTHSDUE + " Arrears Pending";
                        }
                        else if (status.MEMBERSTATUSCODE == 3)
                        {
                            lblStatus.Content = "Struck Off; " + status.TOTALMOTHSDUE + " Arrears Pending";
                        }
                        else if (status.MEMBERSTATUSCODE == 6)
                        {
                            lblStatus.Content = string.Format("Member Resigned, {0:dd/MMM/yyyy}", status.VOUCHER_DATE);
                        }
                    }
                }

                if (brnch != null)
                {
                    txtAddress.Text = brnch.BANKBRANCH_ADDRESS1;
                    txtAddress2.Text = brnch.BANKBRANCH_ADDRESS2;
                    txtAddress3.Text = brnch.BANKBRANCH_ADDRESS3;
                    txtCity.Text = brnch.CITY_NAME;
                    txtState.Text = brnch.STATE_NAME;
                    txtPostalCode.Text = brnch.BANKBRANCH_ZIPCODE;
                    txtCountry.Text = brnch.BANKBRANCH_COUNTRY;
                    txtPhoneNo.Text = brnch.BANKBRANCH_PHONE1;
                    txtMobileNo.Text = brnch.BANKBRANCH_PHONE2;
                    txtEmail.Text = brnch.BANKBRANCH_EMAIL;
                }

                if (gurdian != null)
                {
                    txGurName.Text = gurdian.Name;
                    cmbGurRelation.SelectedValue = gurdian.RELATION_CODE;
                    txtGurAddress.Text = gurdian.ADDRESS1;
                    txtGurAddress2.Text = gurdian.ADDRESS2;
                    txtGurAddress3.Text = gurdian.ADDRESS3;
                    txtGurAge.Text = gurdian.AGE.ToString();
                    cmbGurSex.Text = gurdian.SEX;
                    txtGurICNO.Text = gurdian.ICNO_NEW;
                    txtGurOldICNO.Text = gurdian.ICNO_OLD;
                    cmbGurCity.SelectedValue = gurdian.CITY_CODE;
                    cmbGurState.SelectedValue = gurdian.STATE_CODE;
                    cmbGurCountry.Text = gurdian.COUNTRY;
                    txtGurZipCode.Text = gurdian.ZIPCODE;
                    txtGurPhNo.Text = gurdian.PHONE;
                    txtGurMobileNo.Text = gurdian.MOBILE;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        void BeforeUpdate()
        {
            if (string.IsNullOrEmpty(cmbMemberType.Text))
            {
                MessageBox.Show("Member Type is Empty!");
                cmbMemberType.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(txtMemberNo.Text))
            {
                MessageBox.Show("Membership No is Empty!");
                txtMemberNo.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(txtMemberName.Text))
            {
                MessageBox.Show("Member Name is Empty!");
                txtMemberName.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(cmbBankCode.Text))
            {
                MessageBox.Show("Bank Code is Empty!");
                cmbBankCode.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(cmbBankName.Text))
            {
                MessageBox.Show("Bank Name is Empty!");
                cmbBankName.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(cmbBranchCode.Text))
            {
                MessageBox.Show("Branch Code is Empty!");
                cmbBankName.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(cmbBranchName.Text))
            {
                MessageBox.Show("Branch Name is Empty!");
                cmbBranchName.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(txtNewIC.Text) && string.IsNullOrEmpty(txtOldIC.Text))
            {
                MessageBox.Show("ICNo is Empty!");
                txtNewIC.Focus();
                bValidation = true;
                return;
            }

            else if (string.IsNullOrEmpty(cmbRace.Text))
            {
                MessageBox.Show("Race is Empty!");
                cmbRace.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(cmbGender.Text))
            {
                MessageBox.Show("Gender is Empty!");
                cmbGender.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(dtpDOB.Text))
            {
                MessageBox.Show("DOB is Empty!");
                dtpDOB.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(dtpDOJ.Text))
            {
                MessageBox.Show("DOJ is Empty!");
                dtpDOJ.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(dtpDOEmp.Text))
            {
                MessageBox.Show("Date Of Emp is Empty!");
                dtpDOEmp.Focus();
                bValidation = true;
                return;
            }
            //else if (string.IsNullOrEmpty(cmbResCity.Text))
            //{
            //    MessageBox.Show("City Name is Empty!");
            //    cmbResCity.Focus();
            //    bValidation = true;
            //    return;
            //}
            //else if (string.IsNullOrEmpty(cmbResState.Text))
            //{
            //    MessageBox.Show("State Name is Empty!");
            //    cmbResState.Focus();
            //    bValidation = true;
            //    return;
            //}
            //else if (string.IsNullOrEmpty(cmbResCountry.Text))
            //{
            //    MessageBox.Show("Country Name is Empty!");
            //    cmbResCountry.Focus();
            //    bValidation = true;
            //    return;
            //}
            //else if (string.IsNullOrEmpty(txtSalary.Text))
            //{
            //    MessageBox.Show("Salary is Empty!");
            //    txtSalary.Focus();
            //    bValidation = true;
            //    return;
            //}
            else if (!string.IsNullOrEmpty(txGurName.Text))
            {
                if (string.IsNullOrEmpty(cmbGurRelation.Text))
                {
                    MessageBox.Show("Guardian Relation is Empty!");
                    cmbGurRelation.Focus();
                    bValidation = true;
                    return;
                }
                else if (string.IsNullOrEmpty(cmbGurSex.Text))
                {
                    MessageBox.Show("Guardian Sex is Empty!");
                    cmbGurSex.Focus();
                    bValidation = true;
                    return;
                }
                else if (string.IsNullOrEmpty(txtGurAge.Text))
                {
                    MessageBox.Show("Guardian Age is Empty!");
                    txtGurAge.Focus();
                    bValidation = true;
                    return;
                }
            }

            if (bIsUpdate == false)
            {
                decimal dMem = Convert.ToDecimal(txtMemberNo.Text);
                var dMember = db.MASTERMEMBERs.Where(x => x.MEMBER_ID == dMem).Take(1).FirstOrDefault();
                if (dMember != null)
                {
                    MessageBox.Show("Membership No Already Exist!");
                    txtMemberNo.Focus();
                    bValidation = true;
                    return;
                }
            }

        }

        #endregion

        #region "OTHER EVENTS"

        private void txtSalary_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dMember_Code == 0)
            {
                if (!string.IsNullOrEmpty(txtSalary.Text))
                {
                    int iSalary = Convert.ToInt32(txtSalary.Text);
                    if (iSalary > 0)
                    {
                        var ns = db.MASTERNAMESETUPs.FirstOrDefault();
                        if (ns != null)
                        {
                            int iAmount = ((iSalary * Convert.ToInt32(ns.Subscription)) / 100);
                            int iSubscription = iAmount - Convert.ToInt32(ns.BF + ns.Insurance);

                            txtMonthlySub.Text = iSubscription.ToString();
                            txtAccSub.Text = iSubscription.ToString();
                            txtCurrentYTDSub.Text = iSubscription.ToString();
                        }
                    }
                }
            }
        }

        private void dtpDOB_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DateTime now = DateTime.Today;
                TimeSpan ts = DateTime.Now - Convert.ToDateTime(dtpDOB.SelectedDate);
                int age = Convert.ToInt32(ts.Days) / 365;
                txtAge.Text = age.ToString();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void ckbRejoined_Click(object sender, RoutedEventArgs e)
        {
            if (dMember_Code == 0)
            {
                var ns = db.MASTERNAMESETUPs.FirstOrDefault();
                if (ckbRejoined.IsChecked == true)
                {
                    if (!string.IsNullOrEmpty(txtMemberNo.Text))
                    {
                        txtEntranceFee.Text = (ns.EnterenceFees + ns.RejoinAmount).ToString();
                    }
                }
                else if (ckbRejoined.IsChecked == false)
                {
                    if (!string.IsNullOrEmpty(txtMemberNo.Text))
                    {
                        txtEntranceFee.Text = ns.EnterenceFees.ToString();
                    }
                }
            }
        }

        #endregion

        #region "COMBO BOX EVENTS"

        private void cmbBankCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                decimal dBankCode = Convert.ToDecimal(cmbBankCode.SelectedValue);
                if (dBankCode != 0)
                {
                    var mBnk = db.MASTERBANKs.Where(x => x.BANK_CODE == dBankCode).FirstOrDefault();
                    cmbBankName.SelectedValue = mBnk.BANK_CODE;

                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANK_CODE == dBankCode).ToList();
                    cmbBranchCode.ItemsSource = mbr;
                    cmbBranchCode.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchCode.DisplayMemberPath = "BANKBRANCH_USERCODE";

                    cmbBranchName.ItemsSource = mbr;
                    cmbBranchName.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchName.DisplayMemberPath = "BANKBRANCH_NAME";

                    txtAddress.Text = "";
                    txtAddress2.Text = "";
                    txtAddress3.Text = "";
                    txtCity.Text = "";
                    txtState.Text = "";
                    txtPostalCode.Text = "";
                    txtCountry.Text = "";
                    txtPhoneNo.Text = "";
                    txtMobileNo.Text = "";
                    txtEmail.Text = "";
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void cmbBankName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                decimal dBankCode = Convert.ToDecimal(cmbBankName.SelectedValue);
                if (dBankCode != 0)
                {
                    var mBnk = db.MASTERBANKs.Where(x => x.BANK_CODE == dBankCode).FirstOrDefault();
                    cmbBankCode.SelectedValue = mBnk.BANK_CODE;

                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANK_CODE == dBankCode).ToList();
                    cmbBranchCode.ItemsSource = mbr;
                    cmbBranchCode.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchCode.DisplayMemberPath = "BANKBRANCH_USERCODE";

                    cmbBranchName.ItemsSource = mbr;
                    cmbBranchName.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchName.DisplayMemberPath = "BANKBRANCH_NAME";

                    txtAddress.Text = "";
                    txtAddress2.Text = "";
                    txtAddress3.Text = "";
                    txtCity.Text = "";
                    txtState.Text = "";
                    txtPostalCode.Text = "";
                    txtCountry.Text = "";
                    txtPhoneNo.Text = "";
                    txtMobileNo.Text = "";
                    txtEmail.Text = "";
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void cmbBranchCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                decimal dBrnkCode = Convert.ToDecimal(cmbBranchCode.SelectedValue);
                if (dBrnkCode != 0)
                {
                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANKBRANCH_CODE == dBrnkCode).FirstOrDefault();
                    cmbBranchName.SelectedValue = mbr.BANKBRANCH_CODE;

                    var brnch = (from bc in db.MASTERBANKBRANCHes
                                 join ct in db.MASTERCITies on bc.BANKBRANCH_CITY_CODE equals ct.CITY_CODE
                                 join st in db.MASTERSTATEs on bc.BANKBRANCH_STATE_CODE equals st.STATE_CODE
                                 where bc.BANKBRANCH_CODE == mbr.BANKBRANCH_CODE
                                 select new
                                 {
                                     bc.BANKBRANCH_ADDRESS1,
                                     bc.BANKBRANCH_ADDRESS2,
                                     bc.BANKBRANCH_ADDRESS3,
                                     ct.CITY_NAME,
                                     st.STATE_NAME,
                                     bc.BANKBRANCH_ZIPCODE,
                                     bc.BANKBRANCH_COUNTRY,
                                     bc.BANKBRANCH_PHONE1,
                                     bc.BANKBRANCH_PHONE2,
                                     bc.BANKBRANCH_EMAIL
                                 }
                          ).FirstOrDefault();
                    if (brnch != null)
                    {
                        txtAddress.Text = brnch.BANKBRANCH_ADDRESS1;
                        txtAddress2.Text = brnch.BANKBRANCH_ADDRESS2;
                        txtAddress3.Text = brnch.BANKBRANCH_ADDRESS3;
                        txtCity.Text = brnch.CITY_NAME.ToString();
                        txtState.Text = brnch.STATE_NAME.ToString();
                        txtPostalCode.Text = brnch.BANKBRANCH_ZIPCODE;
                        txtCountry.Text = brnch.BANKBRANCH_COUNTRY;
                        txtPhoneNo.Text = brnch.BANKBRANCH_PHONE1;
                        txtMobileNo.Text = brnch.BANKBRANCH_PHONE2;
                        txtEmail.Text = brnch.BANKBRANCH_EMAIL;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void cmbBranchName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                decimal dBrnkCode = Convert.ToDecimal(cmbBranchName.SelectedValue);
                if (dBrnkCode != 0)
                {
                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANKBRANCH_CODE == dBrnkCode).FirstOrDefault();
                    cmbBranchCode.SelectedValue = mbr.BANKBRANCH_CODE;

                    var brnch = (from bc in db.MASTERBANKBRANCHes
                                 join ct in db.MASTERCITies on bc.BANKBRANCH_CITY_CODE equals ct.CITY_CODE
                                 join st in db.MASTERSTATEs on bc.BANKBRANCH_STATE_CODE equals st.STATE_CODE
                                 where bc.BANKBRANCH_CODE == mbr.BANKBRANCH_CODE
                                 select new
                                 {
                                     bc.BANKBRANCH_ADDRESS1,
                                     bc.BANKBRANCH_ADDRESS2,
                                     bc.BANKBRANCH_ADDRESS3,
                                     ct.CITY_NAME,
                                     st.STATE_NAME,
                                     bc.BANKBRANCH_ZIPCODE,
                                     bc.BANKBRANCH_COUNTRY,
                                     bc.BANKBRANCH_PHONE1,
                                     bc.BANKBRANCH_PHONE2,
                                     bc.BANKBRANCH_EMAIL
                                 }
                           ).FirstOrDefault();
                    if (brnch != null)
                    {
                        txtAddress.Text = brnch.BANKBRANCH_ADDRESS1;
                        txtAddress2.Text = brnch.BANKBRANCH_ADDRESS2;
                        txtAddress3.Text = brnch.BANKBRANCH_ADDRESS3;
                        txtCity.Text = brnch.CITY_NAME.ToString();
                        txtState.Text = brnch.STATE_NAME.ToString();
                        txtPostalCode.Text = brnch.BANKBRANCH_ZIPCODE;
                        txtCountry.Text = brnch.BANKBRANCH_COUNTRY;
                        txtPhoneNo.Text = brnch.BANKBRANCH_PHONE1;
                        txtMobileNo.Text = brnch.BANKBRANCH_PHONE2;
                        txtEmail.Text = brnch.BANKBRANCH_EMAIL;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void cmbMemberType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                decimal dMemberType = Convert.ToDecimal(cmbMemberType.SelectedValue);
                if (dMemberType != 0)
                {
                    if (dMember_Code == 0)
                    {
                        if (string.IsNullOrEmpty(txtMemberNo.Text))
                        {
                            var dMember = Convert.ToDecimal(db.MASTERMEMBERs.Max(x => x.MEMBER_ID)) + 1;
                            txtMemberNo.Text = dMember.ToString();
                        }
                        var ns = db.MASTERNAMESETUPs.FirstOrDefault();
                        if (ns != null)
                        {
                            txtMonthlyBF.Text = ns.BF.ToString();
                            txtMonthlyIns.Text = ns.Insurance.ToString();
                            txtBuildingFund.Text = ns.BuildingFund.ToString();
                            txtAccBenefit.Text = "1200";
                            txtBadgeAmt.Text = ns.BadgeAmount.ToString(); ;

                            if (dMemberType == 1)
                            {
                                txtMonthlySub.Text = "8";
                            }
                            else if (dMemberType == 2)
                            {
                                txtMonthlySub.Text = "4";
                            }

                            if (ckbRejoined.IsChecked == true)
                            {
                                txtEntranceFee.Text = (ns.EnterenceFees + ns.RejoinAmount).ToString();
                            }
                            else
                            {
                                txtEntranceFee.Text = ns.EnterenceFees.ToString();
                            }
                        }
                        else
                        {
                            txtMonthlyBF.Text = "3";
                            txtMonthlyIns.Text = "4";
                            txtBuildingFund.Text = "2";
                            txtAccBenefit.Text = "1200";
                            txtBadgeAmt.Text = "5";

                            if (dMemberType == 1)
                            {
                                txtMonthlySub.Text = "8";
                            }
                            else if (dMemberType == 2)
                            {
                                txtMonthlySub.Text = "4";
                            }

                            if (ckbRejoined.IsChecked == true)
                            {
                                txtEntranceFee.Text = "20";
                            }
                            else
                            {
                                txtEntranceFee.Text = "5";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion

        #region "MEMBER CARD PRINT"

        int PrintNoOfCharPerLine = 57;
        string sMemberName = "";
        string sBankName = "";
        string sBankAddress1 = "";
        string sBankAddress2 = "";
        string sBankAddress3 = "";
        string sBankUserCode = "";
        string sICNO = "";
        string sDOJ = "";
        string sMemberID = "";

        void ClearVariables()
        {
            sMemberName = "";
            sBankName = "";
            sBankAddress1 = "";
            sBankAddress2 = "";
            sBankAddress3 = "";
            sBankUserCode = "";
            sICNO = "";
            sDOJ = "";
            sMemberID = "";
        }

        enum PrintTextAlignType
        {
            Left,
            Center,
            Right,
            Top
        }

        String PrintLine(string Text, PrintTextAlignType AlignType)
        {

            String RValue = "";
            if (AlignType == PrintTextAlignType.Left)
            {
                RValue = Text;
            }
            else if (AlignType == PrintTextAlignType.Center)
            {
                RValue = new string(' ', Math.Abs(PrintNoOfCharPerLine - Text.Length) / 2) + Text;
            }
            else
            {
                RValue = new string(' ', Math.Abs(PrintNoOfCharPerLine - Text.Length)) + Text;
            }
            return RValue + System.Environment.NewLine;
        }

        private void MemberCard_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            System.Drawing.Font textfont1 = new System.Drawing.Font("Courier New", 9, System.Drawing.FontStyle.Regular);
            System.Drawing.Font textfont2 = new System.Drawing.Font("Courier New", 8, System.Drawing.FontStyle.Regular);

            int currentChar = 0;
            int w = 0, h = 0, left = 0, top = 0;
            System.Drawing.Rectangle b = new System.Drawing.Rectangle(left, top, w, h);
            StringFormat format = new StringFormat(StringFormatFlags.LineLimit);

            int line = 0, chars = 0;

            if (sMemberName.Length > 33)
            {
                e.Graphics.MeasureString(sMemberName, textfont2, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sMemberName.Substring(currentChar, chars), textfont2, System.Drawing.Brushes.Black, 126, 23);
            }
            else
            {
                e.Graphics.MeasureString(sMemberName, textfont1, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sMemberName.Substring(currentChar, chars), textfont1, System.Drawing.Brushes.Black, 135, 23);
            }

            if (sBankName.Length > 32)
            {
                e.Graphics.MeasureString(sBankName, textfont2, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sBankName.Substring(currentChar, chars), textfont2, System.Drawing.Brushes.Black, 126, 38);
            }
            else
            {
                e.Graphics.MeasureString(sBankName, textfont1, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sBankName.Substring(currentChar, chars), textfont1, System.Drawing.Brushes.Black, 135, 38);
            }

            if (sBankAddress1.Length > 26)
            {
                e.Graphics.MeasureString(sBankAddress1, textfont2, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sBankAddress1.Substring(currentChar, chars), textfont2, System.Drawing.Brushes.Black, 183, 52);
            }
            else
            {
                e.Graphics.MeasureString(sBankAddress1, textfont1, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sBankAddress1.Substring(currentChar, chars), textfont1, System.Drawing.Brushes.Black, 183, 52);
            }

            if (sBankAddress2.Length > 26)
            {
                e.Graphics.MeasureString(sBankAddress2, textfont2, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sBankAddress2.Substring(currentChar, chars), textfont2, System.Drawing.Brushes.Black, 183, 68);
            }
            else
            {
                e.Graphics.MeasureString(sBankAddress2, textfont1, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sBankAddress2.Substring(currentChar, chars), textfont1, System.Drawing.Brushes.Black, 183, 68);
            }

            if (sBankAddress3.Length > 26)
            {
                e.Graphics.MeasureString(sBankAddress3, textfont2, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sBankAddress3.Substring(currentChar, chars), textfont2, System.Drawing.Brushes.Black, 183, 83);
            }
            else
            {
                e.Graphics.MeasureString(sBankAddress3, textfont1, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sBankAddress3.Substring(currentChar, chars), textfont1, System.Drawing.Brushes.Black, 183, 83);
            }

            if (sBankUserCode.Length > 32)
            {
                e.Graphics.MeasureString(sBankUserCode, textfont2, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sBankUserCode.Substring(currentChar, chars), textfont2, System.Drawing.Brushes.Black, 253, 102);
            }
            else
            {
                e.Graphics.MeasureString(sBankUserCode, textfont1, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sBankUserCode.Substring(currentChar, chars), textfont1, System.Drawing.Brushes.Black, 253, 102);
            }

            if (sICNO.Length > 32)
            {
                e.Graphics.MeasureString(sICNO, textfont2, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sICNO.Substring(currentChar, chars), textfont2, System.Drawing.Brushes.Black, 253, 120);
            }
            else
            {
                e.Graphics.MeasureString(sICNO, textfont1, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sICNO.Substring(currentChar, chars), textfont1, System.Drawing.Brushes.Black, 253, 120);
            }

            if (sDOJ.Length > 32)
            {
                e.Graphics.MeasureString(sDOJ, textfont2, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sDOJ.Substring(currentChar, chars), textfont2, System.Drawing.Brushes.Black, 253, 140);
            }
            else
            {
                e.Graphics.MeasureString(sDOJ, textfont1, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sDOJ.Substring(currentChar, chars), textfont1, System.Drawing.Brushes.Black, 253, 140);
            }

            if (sMemberID.Length > 32)
            {
                e.Graphics.MeasureString(sMemberID, textfont2, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sMemberID.Substring(currentChar, chars), textfont2, System.Drawing.Brushes.Black, 253, 158);
            }
            else
            {
                e.Graphics.MeasureString(sMemberID, textfont1, new System.Drawing.SizeF(0, 0), format, out chars, out line);
                e.Graphics.DrawString(sMemberID.Substring(currentChar, chars), textfont1, System.Drawing.Brushes.Black, 253, 158);
            }
        }

        private void btnMembercard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearVariables();
                var ns = (from x in db.MASTERNAMESETUPs select x).FirstOrDefault();
                string sPrinterName = "";
                if (ns != null)
                {
                    sPrinterName = ns.PrinterName.ToString();
                    if (!string.IsNullOrEmpty(sPrinterName))
                    {
                        var mm = (from x in db.ViewMasterMembers where x.MEMBER_CODE == dMember_Code select x).FirstOrDefault();
                        if (mm != null)
                        {
                            System.Drawing.Printing.PrintDocument prnMemberCard = new System.Drawing.Printing.PrintDocument();
                            prnMemberCard.PrintPage += MemberCard_PrintPage;

                            prnMemberCard.DefaultPageSettings.PaperSize = new PaperSize("NORMAL", 400, 180);
                            Margins margins = new Margins(0, 0, 0, 0);
                            prnMemberCard.DefaultPageSettings.Margins = margins;

                            prnMemberCard.DefaultPageSettings.PrinterSettings.PrinterName = @sPrinterName;

                            prnMemberCard.PrintController = new System.Drawing.Printing.StandardPrintController();

                            sMemberName = PrintLine(mm.MEMBER_NAME, PrintTextAlignType.Left);
                            sBankName = PrintLine(mm.BANK_NAME, PrintTextAlignType.Left);
                            sBankAddress1 = PrintLine((mm.BRANCHADR1 == null ? "" : mm.BRANCHADR1), PrintTextAlignType.Left);
                            sBankAddress2 = PrintLine((mm.BRANCHADR2 == null ? "" : mm.BRANCHADR2), PrintTextAlignType.Left);
                            sBankAddress3 = PrintLine((mm.BRANCHADR3 == null ? "" : mm.BRANCHADR3), PrintTextAlignType.Left);
                            sBankUserCode = PrintLine(mm.BANK_USERCODE.ToString() + "/" + mm.BRANCHUSERCODE.ToString(), PrintTextAlignType.Left);
                            sICNO = PrintLine((mm.ICNO_NEW == null ? mm.ICNO_OLD == null ? "" : mm.ICNO_OLD : mm.ICNO_NEW), PrintTextAlignType.Left);
                            sDOJ = PrintLine(string.Format("{0:dd/MMM/yyyy}", mm.DATEOFJOINING), PrintTextAlignType.Left);
                            sMemberID = PrintLine(mm.MEMBER_ID.ToString(), PrintTextAlignType.Left);

                            //TextToPrint += PrintLine("", PrintTextAlignType.Left);
                            //TextToPrint += PrintLine("", PrintTextAlignType.Left);
                            //TextToPrint += PrintLine("                   " + mm.MEMBER_NAME, PrintTextAlignType.Left);
                            //TextToPrint += PrintLine("                   " + mm.BANK_NAME, PrintTextAlignType.Left);
                            //TextToPrint += PrintLine("                          " + (mm.ADDRESS1 == null ? "" : mm.ADDRESS1), PrintTextAlignType.Left);
                            //TextToPrint += PrintLine("                          " + (mm.ADDRESS2 == null ? "" : mm.ADDRESS2), PrintTextAlignType.Left);
                            //TextToPrint += PrintLine("                          " + (mm.ADDRESS3 == null ? "" : mm.ADDRESS3), PrintTextAlignType.Left);
                            //TextToPrint += PrintLine("", PrintTextAlignType.Left);
                            ////TextToPrint += PrintLine(string.Format("%30s", mm.BANK_USERCODE.ToString() + "/" + mm.BRANCHUSERCODE.ToString()), PrintTextAlignType.Left);
                            //TextToPrint += PrintLine("                                   " + mm.BANK_USERCODE.ToString() + "/" + mm.BRANCHUSERCODE.ToString(), PrintTextAlignType.Left);
                            //TextToPrint += PrintLine("                                   " + (mm.ICNO_NEW == null ? mm.ICNO_OLD == null ? "" : mm.ICNO_OLD : mm.ICNO_NEW), PrintTextAlignType.Left);
                            //TextToPrint += PrintLine("                                   " + string.Format("{0:dd/MMM/yyyy}", mm.DATEOFJOINING), PrintTextAlignType.Left);
                            //TextToPrint += PrintLine("                                   " + mm.MEMBER_ID.ToString(), PrintTextAlignType.Left);
                            prnMemberCard.Dispose();
                            prnMemberCard.Print();
                            MessageBox.Show("Member Card Printed Sucessfully!");
                        }
                        else
                        {
                            MessageBox.Show("No Record Found!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please Enter Your Printer Name in Name Setup!", "Printer Configuration Error");
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter Your Printer Name in Name Setup!", "Printer Configuration Error");
                }

            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion

        #region "PRINT FUNCTION"

        private void LoadReport()
        {
            ReportViewer.Reset();
            //DataTable dt = GetTempleDetails();
            // ReportDataSource masterData = new ReportDataSource("MasterMember", dt);

            //  ReportViewer.LocalReport.DataSources.Add(masterData);
            ReportViewer.LocalReport.ReportEmbeddedResource = "Nube.MemberCard.rdlc";


            decimal mc = Convert.ToDecimal(txtMemberNo.Text);
            string bank = cmbBankName.Text;
            MASTERMEMBER member = db.MASTERMEMBERs.Where(x => x.MEMBER_CODE == mc).FirstOrDefault();
            ReportParameter[] rp = new ReportParameter[8];
            rp[0] = new ReportParameter("MemberName", member.MEMBER_NAME);
            rp[1] = new ReportParameter("BankName", db.MASTERBANKs.Where(x => x.BANK_NAME == bank).Select(x => x.BANK_NAME).FirstOrDefault().ToString());
            rp[2] = new ReportParameter("Address1", txtAddress.Text);
            rp[3] = new ReportParameter("Address2", txtAddress2.Text);
            rp[4] = new ReportParameter("Address3", txtAddress3.Text);
            rp[4] = new ReportParameter("BankID", cmbBankCode.Text);
            rp[5] = new ReportParameter("ICNEW", txtNewIC.Text);
            rp[7] = new ReportParameter("DOJ", string.Format("{0:dd-MM-yyyy}", dtpDOJ.SelectedDate));
            rp[6] = new ReportParameter("MemberNo", txtMemberNo.Text);

            ReportViewer.LocalReport.SetParameters(rp);
            ReportViewer.RefreshReport();
            Export(ReportViewer.LocalReport);
            Print();

        }

        private Stream CreateStream(string name,
        string fileNameExtension, Encoding encoding,

        string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        private void Export(LocalReport report)
        {
            string deviceInfo =
              @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>8.5in</PageWidth>
                <PageHeight>11in</PageHeight>
                <MarginTop>0in</MarginTop>
                <MarginLeft>0in</MarginLeft>
                <MarginRight>0in</MarginRight>
                <MarginBottom>0in</MarginBottom>
            </DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream,
               out warnings);
            foreach (Stream stream in m_streams)
                stream.Position = 0;
        }

        private void Print()
        {

            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                m_currentPageIndex = 0;
                printDoc.Print();
            }
        }

        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
           Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            System.Drawing.Rectangle adjustedRect = new System.Drawing.Rectangle(
            ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
            ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
            ev.PageBounds.Width,
            ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(System.Drawing.Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

        #endregion

        #region "Mobile Format"

        private void txtPhoneNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPhoneNo.Text.Length == 11)
            {
                txtPhoneNo.Text = NoFormate.sPhoneNo(txtPhoneNo.Text);
            }
        }

        private void txtMobileNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtMobileNo.Text.Length == 14)
            {
                txtMobileNo.Text = NoFormate.sMobileNo(txtMobileNo.Text);
            }
        }

        private void txtNomMobileNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNomMobileNo.Text.Length == 14)
            {
                txtNomMobileNo.Text = NoFormate.sMobileNo(txtNomMobileNo.Text);
            }
        }

        private void txtResPhoneNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtResPhoneNo.Text.Length == 11)
            {
                txtResPhoneNo.Text = NoFormate.sPhoneNo(txtResPhoneNo.Text);
            }
        }

        private void txtResMobileNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtResMobileNo.Text.Length == 14)
            {
                txtResMobileNo.Text = NoFormate.sMobileNo(txtResMobileNo.Text);
            }
        }

        private void txtNomMobileNo_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (txtNomMobileNo.Text.Length == 14)
            {
                txtNomMobileNo.Text = NoFormate.sMobileNo(txtNomMobileNo.Text);
            }
        }

        private void txtNomPhNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNomPhNo.Text.Length == 11)
            {
                txtNomPhNo.Text = NoFormate.sPhoneNo(txtNomPhNo.Text);
            }
        }

        private void txtGurMobileNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtGurMobileNo.Text.Length == 14)
            {
                txtGurMobileNo.Text = NoFormate.sMobileNo(txtGurMobileNo.Text);
            }
        }

        private void txtGurPhNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtGurPhNo.Text.Length == 11)
            {
                txtGurPhNo.Text = NoFormate.sPhoneNo(txtGurPhNo.Text);
            }
        }

        #endregion

        #region "TEXT BOX EVENT"

        private void txtMonthlyInsurance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtCurrentYTDIns_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtResPhoneNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtResMobileNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtEntranceFee_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtBuildingFund_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtAccBenefit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtBadgeAmt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtMonthlySub_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtAccSub_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtTotalMonthsPaid_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtTotalMonthsDueSubs_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtMonthlyBF_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtAccBF_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtTotalMonthsDue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtTotalMonthsDueBF_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtMonthlyIns_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtAccIns_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtTotalMonthsDueIns_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtCurrentYTDBF_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtCurrentYTDSub_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtServicePeriod_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtNomAge_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtNomMobileNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtNomPhNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtGurAge_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtGurMobileNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtGurPhNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtInsDue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtBFDue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtSubsDue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtMemberNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtSalary_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtTDFAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtLevyAmount_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtLevyAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtResPostalCode_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtGurZipCode_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtNomPostalCode_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtMemberNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(txtMemberNo.Text))
            {
                decimal dMemberId = Convert.ToDecimal(txtMemberNo.Text);
                var mm = (from x in db.MASTERMEMBERs where x.MEMBER_ID == dMemberId select x).FirstOrDefault();
                if (mm != null)
                {
                    dMember_Code = mm.MEMBER_CODE;
                    bIsUpdate = true;
                    FormFill();
                }
                else
                {
                    MessageBox.Show("No Record Found!");
                }
            }
        }


        #endregion        

    }
}
