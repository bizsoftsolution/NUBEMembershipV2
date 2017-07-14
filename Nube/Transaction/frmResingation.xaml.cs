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
using System.Globalization;
using System.Data.SqlClient;
using Nube.MasterSetup;

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmResingation.xaml
    /// </summary>
    public partial class frmResingation : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        Boolean bValidation = false;
        decimal dMember_Code = 0;
        int iStatusCode = 0;

        public frmResingation(decimal dMembercode = 0)
        {
            InitializeComponent();
            dMember_Code = dMembercode;

            FormLoad();
            if (dMember_Code != 0)
            {
                FormFill();
            }
        }

        #region BUTTON EVENTS

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            frmHomeMembership frm = new frmHomeMembership();
            this.Close();
            frm.ShowDialog();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            frmMemberQuery frm = new frmMemberQuery("Resingation");
            this.Close();
            frm.ShowDialog();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bValidation = false;
                BeforeUpdate();
                if (bValidation == false)
                {
                    if (MessageBox.Show("Total Amount is " + txtRegTotalAmount.Text + ". \r Sure to Resign ?", "Resign Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        MASTERMEMBER mm = (from mas in db.MASTERMEMBERs where mas.MEMBER_CODE == dMember_Code select mas).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(mm);
                        if (mm != null)
                        {
                            mm.RESIGNED = 1;
                            db.SaveChanges();

                            RESIGNATION rg = (from mas in db.RESIGNATIONs where mas.MEMBER_CODE == dMember_Code select mas).FirstOrDefault();
                            if (rg != null)
                            {
                                db.RESIGNATIONs.Remove(rg);
                                db.SaveChanges();
                            }

                            RESIGNATION rsg = new RESIGNATION();
                            rsg.MEMBER_CODE = dMember_Code;
                            rsg.RESIGNATION_DATE = dtpRegResignDate.SelectedDate;
                            //rsg.RESIGNSTATUS_CODE = iStatusCode;
                            rsg.ICNO_OLD = mm.ICNO_OLD;
                            rsg.ICNO = mm.ICNO_NEW;
                            if (!string.IsNullOrEmpty(cmbRegClaimer.Text))
                            {
                                rsg.RELATION_CODE = Convert.ToDecimal(cmbRegClaimer.SelectedValue);
                            }

                            rsg.REASON_CODE = Convert.ToDecimal(cmbRegReason.SelectedValue);
                            rsg.CLAIMER_NAME = txtRegClaimerName.Text.ToString();
                            rsg.MONTHS_CONTRIBUTED = Convert.ToDecimal(txtRegContributedMonths.Text);
                            rsg.ACCBF = Convert.ToDecimal(txtRegBFContribution.Text);
                            rsg.ACCBENEFIT = Convert.ToDecimal(txtRegSubTotal.Text);
                            rsg.CHEQUENO = txtRegPaymode.Text.ToString();
                            rsg.CHEQUEDATE = dtpRegChequeDate.SelectedDate;
                            rsg.AMOUNT = Convert.ToDecimal(txtRegTotalAmount.Text);
                            rsg.TOTALARREARS = Convert.ToDecimal(txtTotalMonthsDueSubs.Text);
                            rsg.USER_CODE = Convert.ToDecimal(AppLib.iUserCode);
                            rsg.ENTRY_DATE = DateTime.Now.Date;
                            //rsg.ENTRY_TIME = DateTime.Now.TimeOfDay.ToString();
                            rsg.CURRENTDATE = DateTime.Now;
                            rsg.VOUCHER_DATE = dtpRegVData.SelectedDate;
                            rsg.SERVICE_YEAR = Convert.ToDecimal(txtRegServiceYear.Text);
                            db.RESIGNATIONs.Add(rsg);
                            db.SaveChanges();


                            var NewData = new JSonHelper().ConvertObjectToJSon(mm);
                            AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "MASTERMEMBER");

                            int iMonth = Convert.ToInt32(((Convert.ToDateTime(dtpRegResignDate.SelectedDate).Year - Convert.ToDateTime(dtpDOJ.SelectedDate).Year) * 12) + Convert.ToDateTime(dtpRegResignDate.SelectedDate).Month - Convert.ToDateTime(dtpDOJ.SelectedDate).Month);

                            MessageBox.Show("Member Resign Sucessfully Completed!");
                            if (MessageBox.Show("Do you want to Print this Receipt?", "RECEIPT", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                frmResignReport frm = new frmResignReport(dMember_Code, cmbRegReason.Text.ToString());
                                frm.ShowDialog();
                            }
                            if (MessageBox.Show("Do you want to Resgin Another Member?", "Resign Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                fNew();
                            }
                            else
                            {
                                frmHomeMembership frm = new frmHomeMembership();
                                this.Close();
                                frm.ShowDialog();
                            }

                        }
                        else
                        {
                            MessageBox.Show("Membership Number Does Not found!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSetRegReason_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmReasonSetup frm = new frmReasonSetup("REGISTRATION");
                frm.ShowDialog();

                cmbRegReason.ItemsSource = db.MASTERRESIGNSTATUS.ToList();
                cmbRegReason.SelectedValuePath = "RESIGNSTATUS_CODE";
                cmbRegReason.DisplayMemberPath = "RESIGNSTATUS_NAME";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion

        #region OTHER EVENTS

        private void cmbRegClaimer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dMember_Code != 0)
                {
                    decimal dClaimer = Convert.ToDecimal(cmbRegClaimer.SelectedValue);
                    if (dClaimer == 13)
                    {
                        txtRegClaimerName.Text = txtMemberName.Text;
                    }
                    else if (dClaimer != 0)
                    {
                        Nube.MASTERNOMINEE mm = (from mas in db.MASTERNOMINEEs where mas.MEMBER_CODE == dMember_Code && mas.RELATION_CODE == dClaimer select mas).Take(1).FirstOrDefault();
                        if (mm != null)
                        {
                            txtRegClaimerName.Text = mm.NAME.ToString();
                        }
                        else
                        {
                            txtRegClaimerName.Text = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void cmbRegReason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dMember_Code != 0)
                {
                    int iBenefitAmt = 0;
                    decimal dReason = Convert.ToDecimal(cmbRegReason.SelectedValue);
                    int iTotalmonth = Convert.ToInt32(dtpRegResignDate.SelectedDate.Value.Subtract(dtpDOJ.SelectedDate.Value).Days / (365.25 / 12));
                    //if (dReason != 0 && Convert.ToDecimal(txtTotalMonthsDueBF.Text) <= 0)
                    if (dReason != 0)
                    {
                        var rr = (from d in db.MASTERRESIGNSTATUS where d.RESIGNSTATUS_CODE == dReason select d).FirstOrDefault();
                        if (rr != null)
                        {
                            if (rr.IsBenefitValid == true)
                            {
                                //int iTotalmonth = Convert.ToInt32(((Convert.ToDateTime(dtpRegResignDate.SelectedDate).Year - Convert.ToDateTime(dtpDOJ.SelectedDate).Year) * 12) + Convert.ToDateTime(dtpRegResignDate.SelectedDate).Month - Convert.ToDateTime(dtpDOJ.SelectedDate).Month);

                                int iBenefitYrs = Convert.ToInt32(txtRegBenefitYear.Text);
                                if ((Convert.ToInt32(txtTotalMonthPaidSubs.Text) + 3) >= iTotalmonth && iBenefitYrs >= rr.MinimumYear)
                                {
                                    int AmtPerYear1 = Convert.ToInt32(rr.AmtPerYear1);
                                    int AmtPerYear2 = Convert.ToInt32(rr.AmtPerYear2);

                                    iBenefitAmt = Convert.ToInt32((AmtPerYear1) + ((iBenefitYrs - 5) * AmtPerYear2));
                                }
                            }
                        }
                        if (iBenefitAmt < rr.MinimumRefund)
                        {
                            iBenefitAmt = Convert.ToInt32(rr.MinimumRefund);
                        }
                        if (iBenefitAmt > rr.MaximumRefund)
                        {
                            iBenefitAmt = Convert.ToInt32(rr.MaximumRefund);
                        }
                    }

                    if (!string.IsNullOrEmpty(txtRegBenefits.Text))
                    {
                        iBenefitAmt = Convert.ToInt32(iBenefitAmt + Convert.ToDecimal(txtRegBenefits.Text));
                    }

                    //txtRegBenefits.Text = iBenefitAmt.ToString();
                    //txtRegContributedMonths.Text = iTotalmonth.ToString();
                    txtRegSubTotal.Text = iBenefitAmt.ToString();
                    //txtRegBenefits.Text = iBenefitAmt.ToString();
                    fTotal();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
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

        private void dtpRegResignDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(dtpRegResignDate.Text))
            {
                TimeSpan ts = Convert.ToDateTime(dtpRegResignDate.SelectedDate) - Convert.ToDateTime(dtpDOJ.SelectedDate);
                int iSerYear = Convert.ToInt32(ts.Days) / 365;
                txtRegServiceYear.Text = iSerYear.ToString();
                int totalMonths = Convert.ToInt32(dtpRegResignDate.SelectedDate.Value.Subtract(dtpDOJ.SelectedDate.Value).Days / (365.25 / 12));
                txtRegAmount.Text = (totalMonths * 3).ToString();
                if (iSerYear > 0)
                {
                    txtTotalMonthsDueSubs.Text = (totalMonths - Convert.ToInt32(txtTotalMonthPaidSubs.Text)).ToString();
                    txtTotalMonthsDueBF.Text = (totalMonths - Convert.ToInt32(txtTotalMonthsDueBF.Text)).ToString();
                }
            }
        }

        private void txtRegContributedMonths_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtRegContributedMonths.Text))
            {
                txtRegBenefitYear.Text = (Convert.ToDecimal(txtRegContributedMonths.Text) / 12).ToString();
            }
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
                    FormFill();
                }
                else
                {
                    MessageBox.Show("No Record Found!");
                }
            }
        }

        #endregion

        #region TEXT BOX EVENTS

        private void txtMemberNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtRegBenefitYear_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtRegServiceYear_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtRegAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtRegBenefits_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtRegSubTotal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtRegTotalAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtRegContributedMonths_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtRegBFContribution_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        #endregion

        #region "USER DEFINED FUNCTIONS"

        void BeforeUpdate()
        {
            if (string.IsNullOrEmpty(txtMemberNo.Text))
            {
                MessageBox.Show("Membership No is Empty!");
                txtMemberNo.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(txtRegPaidTill.Text))
            {
                MessageBox.Show("Enter Correct Value!");
                txtRegPaidTill.Focus();
                bValidation = true;
                return;
            }
            else if (string.IsNullOrEmpty(cmbRegReason.Text))
            {
                MessageBox.Show("Reason is Empty!");
                cmbRegReason.Focus();
                bValidation = true;
                return;
            }
            //else
            //{
            //    Nube.MASTERMEMBER mm = (from mas in db.MASTERMEMBERs where mas.MEMBER_CODE == dMember_Code && mas.RESIGNED == 1 select mas).FirstOrDefault();
            //    if (mm != null)
            //    {
            //        bValidation = true;
            //        MessageBox.Show("MEMBER IS ALREADY RESIGN!");
            //        return;
            //    }

            //}
        }

        void fNew()
        {
            ckbRejoined.IsEnabled = false;
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
            txtTDFAmount.Text = "";
            txtLevyAmount.Text = "";
            cmbTDF.Text = "";

            txtLevyAmount.Text = "";
            txtTDFAmount.Text = "";


            txtEntranceFee.Text = "0";
            txtBuildingFund.Text = "0";
            txtBadgeAmt.Text = "0";
            txtMonthlyBF.Text = "0";
            txtMonthlyIns.Text = "0";
            txtAccBF.Text = "0";
            txtAccIns.Text = "0";
            txtCurrentYTDBF.Text = "0";
            txtCurrentYTDIns.Text = "0";
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

            dtpLastPay.Text = "";
            dtpRegResignDate.Text = "";
            cmbRegReason.Text = "";
            txtRegPaidTill.Text = "";
            txtRegServiceYear.Text = "";
            txtRegAmount.Text = "";
            txtRegPaymode.Text = "";
            txtRegTotalAmount.Text = "";
            txtRegBFContribution.Text = "";
            txtRegClaimerName.Text = "";

            txtRegContributedMonths.Text = "";
            cmbRegClaimer.Text = "";
            txtRegBenefitYear.Text = "";
            txtRegBenefits.Text = "";
            dtpRegChequeDate.Text = "";
            dtpRegVData.Text = "";
            cmbPaymode.Text = "";
            txtRegSubTotal.Text = "";
            iStatusCode = 0;
            FormLoad();
        }

        void FormLoad()
        {
            try
            {
                var city = db.MASTERCITies.ToList();
                var state = db.MASTERSTATEs.ToList();
                var relation = db.MASTERRELATIONs.ToList();

                cmbRegClaimer.ItemsSource = relation;
                cmbRegClaimer.SelectedValuePath = "RELATION_CODE";
                cmbRegClaimer.DisplayMemberPath = "RELATION_NAME";

                cmbRegReason.ItemsSource = db.MASTERRESIGNSTATUS.ToList();
                cmbRegReason.SelectedValuePath = "RESIGNSTATUS_CODE";
                cmbRegReason.DisplayMemberPath = "RESIGNSTATUS_NAME";

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

                cmbResCity.ItemsSource = city;
                cmbResCity.SelectedValuePath = "CITY_CODE";
                cmbResCity.DisplayMemberPath = "CITY_NAME";

                cmbResState.ItemsSource = state;
                cmbResState.SelectedValuePath = "STATE_CODE";
                cmbResState.DisplayMemberPath = "STATE_NAME";

                cmbGurCity.ItemsSource = city;
                cmbGurCity.SelectedValuePath = "CITY_CODE";
                cmbGurCity.DisplayMemberPath = "CITY_NAME";

                cmbGurState.ItemsSource = state;
                cmbGurState.SelectedValuePath = "STATE_CODE";
                cmbGurState.DisplayMemberPath = "STATE_NAME";

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
                var qry = (from mm in db.MASTERMEMBERs
                           where mm.MEMBER_CODE == dMember_Code
                           select new
                           {
                               mm.MEMBERTYPE_CODE,
                               mm.MEMBER_CODE,
                               mm.MEMBER_TITLE,
                               mm.MEMBER_NAME,
                               mm.DATEOFBIRTH,
                               mm.AGE_IN_YEARS,
                               mm.SEX,
                               mm.REJOINED,
                               mm.RACE_CODE,
                               mm.ICNO_OLD,
                               mm.ICNO_NEW,
                               mm.DATEOFJOINING,
                               mm.BANK_CODE,
                               mm.BRANCH_CODE,
                               mm.DATEOFEMPLOYMENT,
                               mm.Salary,
                               LEVY = (mm.LEVY == "NULL" ? "" : mm.LEVY),
                               TDF = (mm.TDF == "NULL" ? "" : mm.TDF),
                               mm.LEVY_AMOUNT,
                               mm.TDF_AMOUNT,
                               mm.LevyPaymentDate,
                               mm.Tdf_PaymentDate,
                               mm.ENTRANCEFEE,
                               mm.ACCBENEFIT,
                               mm.MONTHLYSUBSCRIPTION,
                               mm.TOTALMONTHSPAID,
                               mm.ACCSUBSCRIPTION,
                               mm.MONTHLYBF,
                               mm.ACCBF,
                               mm.CURRENT_YTDBF,
                               mm.CURRENT_YTDSUBSCRIPTION,
                               mm.LASTPAYMENT_DATE,
                               mm.ADDRESS1,
                               mm.ADDRESS2,
                               mm.ADDRESS3,
                               resCitycode = mm.CITY_CODE,
                               resStatecode = mm.STATE_CODE,
                               mm.COUNTRY,
                               mm.ZIPCODE,
                               mm.PHONE,
                               mm.MOBILE,
                               mm.EMAIL,
                               mm.MEMBER_ID,
                               mm.STATUS_CODE,
                               mm.BatchAmt,
                               mm.RESIGNED
                           }
                         ).FirstOrDefault();
                DataTable dtResign = new DataTable();
                using (SqlConnection con = new SqlConnection(AppLib.connStr))
                {
                    SqlCommand cmd;
                    cmd = new SqlCommand(string.Format("SELECT * FROM VIEWRESIGN(NOLOCK) WHERE MEMBER_CODE={0}", dMember_Code), con);
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dtResign.Rows.Clear();
                    adp.SelectCommand.CommandTimeout = 0;
                    adp.Fill(dtResign);
                }

                var status = (from x in db.ViewMasterMembers where x.MEMBER_CODE == dMember_Code select x).FirstOrDefault();

                if (status != null)
                {
                    iStatusCode = Convert.ToInt32(status.MEMBERSTATUSCODE);
                }


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

                var nominee = (from nm in db.MASTERNOMINEEs
                               join ct in db.MASTERCITies on nm.CITY_CODE equals ct.CITY_CODE
                               join st in db.MASTERSTATEs on nm.STATE_CODE equals st.STATE_CODE
                               join rl in db.MASTERRELATIONs on nm.RELATION_CODE equals rl.RELATION_CODE
                               where nm.MEMBER_CODE == dMember_Code
                               orderby nm.Id descending
                               select new
                               {
                                   nm.NAME,
                                   nm.AGE,
                                   nm.SEX,
                                   rl.RELATION_NAME,
                                   nm.ICNO_NEW,
                                   nm.ICNO_OLD,
                                   nm.ADDRESS1,
                                   nm.ADDRESS2,
                                   nm.ADDRESS3,
                                   ct.CITY_NAME,
                                   st.STATE_NAME,
                                   nm.MOBILE,
                                   nm.PHONE,
                                   nm.MEMBER_CODE,
                                   nm.CITY_CODE,
                                   nm.STATE_CODE,
                                   nm.RELATION_CODE
                               }).ToList();

                if (nominee != null)
                {
                    DataTable dt = new DataTable();
                    dt = AppLib.LINQResultToDataTable(nominee);
                    if (dt.Rows.Count > 0)
                    {
                        dgNomination.ItemsSource = dt.DefaultView;
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

                var fees = (from fs in db.FeesDetails where fs.MemberCode == dMember_Code && fs.UpdatedStatus == "Not Updated" select fs).ToList();
                DataTable dtFee = AppLib.LINQResultToDataTable(fees);

                decimal BF = 0, Ins = 0, Subs = 0, dTotlMonthsPaid = 0;

                foreach (DataRow dr in dtFee.Rows)
                {
                    BF = BF + Convert.ToDecimal(dr["AmountBf"]);
                    Ins = Ins + Convert.ToDecimal(dr["AmountIns"]);
                    Subs = Subs + Convert.ToDecimal(dr["AmtSubs"]);
                    dTotlMonthsPaid = dTotlMonthsPaid + Convert.ToDecimal(dr["TotalMonthsPaid"]);
                }
                //var ArPost = (from ap in db.ArrearPostDetails where ap.MemberCode == dMember_Code && ap.UpdatedStatus == "Not Updated" select ap).ToList();
                //DataTable dtArrearPost = AppLib.LINQResultToDataTable(ArPost);
                //foreach (DataRow dr in dtArrearPost.Rows)
                //{
                //    BF = BF + Convert.ToDecimal(dr["AmountBf"]);
                //    Ins = BF + Convert.ToDecimal(dr["AmountIns"]);
                //    Subs = BF + Convert.ToDecimal(dr["AmtSubs"]);
                //    dTotlMonthsPaid = dTotlMonthsPaid + 1;
                //}
                //var ArPre = (from ap in db.ArrearPreDetails where ap.MemberCode == dMember_Code && ap.UpdatedStatus == "Not Updated" select ap).ToList();
                //DataTable dtArrearPre = AppLib.LINQResultToDataTable(ArPre);
                //foreach (DataRow dr in dtArrearPre.Rows)
                //{
                //    BF = BF + Convert.ToDecimal(dr["AmountBf"]);
                //    Ins = BF + Convert.ToDecimal(dr["AmountIns"]);
                //    Subs = BF + Convert.ToDecimal(dr["AmtSubs"]);
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
                    if (status.TDF != null)
                    {
                        cmbTDF.Text = status.TDF.ToString();
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

                    DateTime dtM = Convert.ToDateTime(status.LASTPAYMENT_DATE);
                    txtRegPaidTill.Text = dtM.ToString("MMM-yyyy");
                    txtRegContributedMonths.Text = Convert.ToInt32(qry.TOTALMONTHSPAID + dTotlMonthsPaid).ToString();

                    txtRegBFContribution.Text = Convert.ToInt32(qry.ACCBF + BF).ToString();
                    txtRegBenefitYear.Text = Convert.ToInt32((qry.TOTALMONTHSPAID + dTotlMonthsPaid) / 12).ToString();

                    TimeSpan ts = Convert.ToDateTime(status.LASTPAYMENT_DATE) - Convert.ToDateTime(status.DATEOFJOINING);

                    int iSerYear = Convert.ToInt32(ts.Days) / 365;
                    txtRegServiceYear.Text = iSerYear.ToString();
                    int totalMonths = ((Convert.ToDateTime(status.LASTPAYMENT_DATE).Year - Convert.ToDateTime(status.DATEOFJOINING).Year) * 12) + Convert.ToDateTime(status.LASTPAYMENT_DATE).Month - Convert.ToDateTime(status.DATEOFJOINING).Month;

                    txtRegAmount.Text = (totalMonths * 3).ToString();
                    txtServicePeriod.Text = iSerYear.ToString();
                    txtRegBenefits.Text = qry.ACCBENEFIT == null ? "0" : qry.ACCBENEFIT.ToString();

                    if (qry.RESIGNED == 1)
                    {
                        lblStatus.Content = "MEMBER IS ALREADY RESIGN";
                    }

                    txtEntranceFee.Text = qry.ENTRANCEFEE.ToString();
                    txtBuildingFund.Text = status.HQFEE.ToString();
                    txtAccBenefit.Text = qry.ACCBENEFIT.ToString();
                    txtMonthlySub.Text = status.MONTHLYSUBSCRIPTION.ToString();
                    txtTotalMonthPaidSubs.Text = (qry.TOTALMONTHSPAID + dTotlMonthsPaid).ToString();
                    txtTotalMonthPaidBF.Text = (qry.TOTALMONTHSPAID + dTotlMonthsPaid).ToString();
                    txtAccSub.Text = (qry.ACCSUBSCRIPTION + Subs).ToString();
                    txtMonthlyBF.Text = status.MONTHLYBF.ToString();
                    txtAccBF.Text = (qry.ACCBF + BF).ToString();
                    txtCurrentYTDBF.Text = BF.ToString();
                    txtCurrentYTDSub.Text = Subs.ToString();

                    dtpLastPay.SelectedDate = Convert.ToDateTime(status.LASTPAYMENT_DATE);
                    dtpLastPay.IsEnabled = false;
                    txtTotalMonthsDueSubs.Text = status.TOTALMOTHSDUE.ToString();
                    txtTotalMonthsDueBF.Text = status.TOTALMOTHSDUE.ToString();
                    txtBadgeAmt.Text = qry.BatchAmt.ToString();
                    if (qry.EMAIL != null)
                    {
                        txtResEmail.Text = qry.EMAIL.ToString();
                    }

                    cmbResCity.SelectedValue = qry.resCitycode;
                    txtResPostalCode.Text = qry.ZIPCODE;
                    cmbResState.SelectedValue = qry.resStatecode;
                    cmbResCountry.Text = qry.COUNTRY;

                    txtCurrentYTDIns.Text = "0";
                    txtTotalMonthsDueIns.Text = "0";
                    txtTotalMonthPaidIns.Text = "0";
                    txtMonthlyIns.Text = "0";
                    txtAccIns.Text = "0";

                    if (status.RESIGNED == 1 || status.MEMBERSTATUSCODE == 6)
                    {
                        lblStatus.Content = "MEMBER IS ALREADY RESIGN";
                    }
                    else
                    {
                        if (status.MEMBERSTATUSCODE == 1)
                        {
                            lblStatus.Content = "Active Member";
                        }
                        else if (status.MEMBERSTATUSCODE == 2)
                        {
                            lblStatus.Content = "Defaulter; Arrears Pending";
                        }
                        else if (status.MEMBERSTATUSCODE == 3)
                        {
                            lblStatus.Content = "Struck Off; Arrears Pending";
                        }
                        else if (status.MEMBERSTATUSCODE == 6)
                        {
                            lblStatus.Content = "MEMBER IS ALREADY RESIGN";
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
                dtpRegResignDate.Focus();

                if (dtResign.Rows.Count > 0)
                {
                    for (int i = 0; i < dtResign.Rows.Count; i++)
                    {
                        cmbRegClaimer.SelectedValue = Convert.ToInt32(dtResign.Rows[i]["RELATION_CODE"]);
                        txtRegClaimerName.Text = dtResign.Rows[i]["CLAIMER_NAME"].ToString();
                        txtRegContributedMonths.Text = dtResign.Rows[i]["MONTHS_CONTRIBUTED"].ToString();
                        txtRegBFContribution.Text = dtResign.Rows[i]["ACCBF"].ToString();
                        txtRegBenefits.Text = qry.ACCBENEFIT.ToString();
                        txtRegAmount.Text = qry.ACCBENEFIT.ToString();
                        txtRegSubTotal.Text = dtResign.Rows[i]["ACCBENEFIT"].ToString();
                        txtRegPaymode.Text = dtResign.Rows[i]["CHEQUENO"].ToString();
                        dtpRegChequeDate.Text = dtResign.Rows[i]["CHEQUEDATE"].ToString();
                        txtRegTotalAmount.Text = dtResign.Rows[i]["AMOUNT"].ToString();
                        dtpRegVData.Text = dtResign.Rows[i]["VOUCHER_DATE"].ToString();
                        txtRegServiceYear.Text = dtResign.Rows[i]["SERVICE_YEAR"].ToString();
                        cmbRegReason.SelectedValue = Convert.ToInt32(dtResign.Rows[i]["REASON_CODE"]);
                        dtpRegResignDate.Text = string.Format("{0:dd/MMM/yyyy}", dtResign.Rows[i]["RESIGNATION_DATE"]);
                    }
                }
                else
                {
                    dtpRegResignDate.SelectedDate = DateTime.Now;
                    dtpRegChequeDate.SelectedDate = DateTime.Now;
                    dtpRegVData.SelectedDate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        void fTotal()
        {
            int iBFContri = Convert.ToInt32(txtRegBFContribution.Text);
            int iBenefits = Convert.ToInt32(txtRegSubTotal.Text);
            txtRegTotalAmount.Text = (iBFContri + iBenefits).ToString();
            //txtRegSubTotal.Text = (iBFContri + iBenefits).ToString();
        }

        #endregion

    }
}
