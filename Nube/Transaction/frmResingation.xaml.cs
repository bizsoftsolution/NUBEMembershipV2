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
            //LoadTempViewMaster();
        }
        void IRCHeadingLoad()
        {
            txtIRCResignMemberName.Text = txtMemberName.Text;
            string SheOrHe;
            if (cmbGender.Text.ToLower() == "male")
            {
                SheOrHe = "He";
            }
            else if (cmbGender.Text.ToLower() == "female")
            {
                SheOrHe = "She";
            }
            else
            {
                SheOrHe = "She/he";
            }
            string rMemberType;
            if (string.IsNullOrWhiteSpace(cmbMemberType.Text))
            {
                rMemberType = "Clerical/Non clerical";
            }
            else
            {
                rMemberType = cmbMemberType.Text;
            }

            cbxPromotedTo.Content = string.Format("{0} was promoted to", SheOrHe);
            cbxBeforePromotion.Content = string.Format("{0} was a {1} before promotion [Delete which is not applicable]", SheOrHe, rMemberType);
            cbxHereByConfirm.Content = string.Format("I hereby confirm that {0} got promoted {0} is no longer doing any clerical job function.", SheOrHe);
            cbxFilledBy.Content = string.Format("The {0} position has been filled by", rMemberType);
            cbxBranchCommitteeVerification1.Content = String.Format("I have verified the above and confirm that the declaration by the IRC is correct. The {0} position has been filled by another {0} And;", rMemberType);
            cbxBranchCommitteeVerification2.Content = String.Format("The promoted member is no longer doing {0} job functions", rMemberType);
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
            //frmMemberQuery frm = new frmMemberQuery("Resingation");
            //this.Close();
            //frm.ShowDialog();
            frmIRCConfirmationSearch frm = new frmIRCConfirmationSearch();
            frm.cbxPending.IsChecked = false;
            frm.ShowDialog();
            dMember_Code = frm.selectedIRC.MemberCode;
            ViewIRC(dMember_Code);
            FormFill();
            tbiIRC.IsEnabled = true;
            tbiBank.IsEnabled = false;
            tbiResidentialAddress.IsEnabled = false;
            tbiFundDetail.IsEnabled = false;
            tbiNomineeDetails.IsEnabled = false;
            tbiGaurdian.IsEnabled = false;
            tbiResignationDetail.IsEnabled = false;
            tbiIRC.Focus();


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
                        var mm = (from mas in db.MASTERMEMBERs where mas.MEMBER_CODE == dMember_Code select mas).FirstOrDefault();
                        //var OldData = new JSonHelper().ConvertObjectToJSon(mm);
                        if (mm != null)
                        {
                            mm.RESIGNED = 1;
                            //db.SaveChanges();

                            RESIGNATION rg = (from mas in db.RESIGNATIONs where mas.MEMBER_CODE == dMember_Code select mas).FirstOrDefault();
                            if (rg != null)
                            {
                                db.RESIGNATIONs.Remove(rg);
                                //db.SaveChanges();
                            }

                            RESIGNATION rsg = new RESIGNATION();
                            rsg.MEMBER_CODE = dMember_Code;
                            rsg.RESIGNATION_DATE = dtpRegResignDate.SelectedDate;
                            rsg.RESIGNSTATUS_CODE = iStatusCode;
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
                            rsg.InsuranceAmount = Convert.ToDecimal(txtRegUnionContri.Text);
                            rsg.CHEQUENO = txtRegPaymode.Text.ToString();
                            rsg.PayMode = cmbPaymode.Text.ToString();
                            rsg.CHEQUEDATE = dtpRegChequeDate.SelectedDate;
                            rsg.AMOUNT = Convert.ToDecimal(txtRegTotalAmount.Text);
                            rsg.TOTALARREARS = Convert.ToDecimal(txtTotalMonthsDueSubs.Text);
                            rsg.USER_CODE = Convert.ToDecimal(AppLib.iUserCode);
                            rsg.ENTRY_DATE = DateTime.Now.Date;
                            rsg.ENTRY_TIME = string.Format(@"{0:hh\:mm\:ss}", DateTime.Now.TimeOfDay).ToString();
                            rsg.CURRENTDATE = DateTime.Now;
                            rsg.VOUCHER_DATE = dtpRegVData.SelectedDate;
                            rsg.SERVICE_YEAR = Convert.ToDecimal(txtRegServiceYear.Text);
                            db.RESIGNATIONs.Add(rsg);

                            db.SaveChanges();

                            //var NewData = new JSonHelper().ConvertObjectToJSon(mm);
                            //AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "MASTERMEMBER");

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
                BenefitCalculation();
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
                int totalMonths = AppLib.MonthDiff(dtpDOJ.SelectedDate.Value, dtpRegResignDate.SelectedDate.Value);
                //txtRegContributedMonths.Text = totalMonths.ToString();
                txtRegAmount.Text = (totalMonths * 3).ToString();
                //txtRegBFContribution.Text = (totalMonths * 3).ToString();
                if (iSerYear > 0)
                {
                    txtTotalMonthsDueSubs.Text = (totalMonths - Convert.ToInt32("0"+ txtTotalMonthPaidSubs.Text)).ToString();
                    txtTotalMonthsDueBF.Text = (totalMonths - Convert.ToInt32("0" + txtTotalMonthPaidBF.Text)).ToString();
                }
                BenefitCalculation();
            }
        }

        private void txtRegContributedMonths_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtRegContributedMonths.Text))
            {
              //  txtRegBenefitYear.Text = (Convert.ToDecimal(txtRegContributedMonths.Text) / 12).ToString();
            }
        }

        private void txtMemberNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(txtMemberNo.Text))
            {
                decimal dMemberId = Convert.ToDecimal(txtMemberNo.Text);
                var mm = (from x in db.MASTERMEMBERs where x.MEMBER_ID == dMemberId && x.IsCancel == false select x).FirstOrDefault();
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

            cmbGE_Insurance.Text = "";
            cmbAI_Insurance.Text = "";
            txtTakaful.Text = "";
            txtTDFInsurance.Text = "";

            txtRegContributedMonths.Text = "";
            cmbRegClaimer.Text = "";
            txtRegBenefitYear.Text = "";
            txtRegBenefits.Text = "";
            dtpRegChequeDate.Text = "";
            dtpRegVData.Text = "";
            cmbPaymode.Text = "";
            txtRegSubTotal.Text = "";
            iStatusCode = 0;
            txtRegServiceYear.Text = "";
            txtRegBenefits.Text = "";
            txtRegSubTotal.Text = "";
            FormLoad();
        }

        void LoadTempViewMaster()
        {
            try
            {
                if (AppLib.lstMstMember.Count == 0)
                {
                    var lstMM = (from x in db.MemberStatusLogs select x).ToList();
                    AppLib.lstMstMember = lstMM;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
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
        void ViewIRC(decimal Member_Code)
        {
            var d = db.IRCConfirmations.FirstOrDefault(x => x.MemberCode == Member_Code);
            if (d != null)
            {
                txtIRCMemberNo.Text = d.IRCMembershipNo;
                txtIRCName.Text = d.IRCName;
                txtIRCBankName.Text = d.IRCBank;
                txtIRCBankAddress.Text = d.IRCBankAddress;
                txtIRCTelephoneNo.Text = d.IRCTelephoneNo;
                txtIRCMobileNo.Text = d.IRCMobileNo;
                txtIRCFax.Text = d.IRCFaxNo;
                txtMemberName.Text = d.ResignMemberName;
                txtIRCPromotedTo.Text = d.PromotedTo;
                txtBranchCommitteeName.Text = d.BranchCommitteeName;
                txtBranchCommitteeZone.Text = d.BranchCommitteeZone;
                txtRemarks.Text = d.Remarks;
                dtpGrade.SelectedDate = d.GradeWEF;
                dtpBranchCommitteeDate.SelectedDate = d.BranchCommitteeDate;
                cbxNameOfPerson.IsChecked = d.NameOfPerson;
                cbxPromotedTo.IsChecked = d.WasPromoted;
                cbxBeforePromotion.IsChecked = d.BeforePromotion;
                cbxAttached.IsChecked = d.Attached;
                cbxHereByConfirm.IsChecked = d.HereByConfirm;
                cbxFilledBy.IsChecked = d.FilledBy;
                cbxBranchCommitteeVerification1.IsChecked = d.BranchCommitteeVerification1;
                cbxBranchCommitteeVerification2.IsChecked = d.BranchCommitteeVerification2;


                if (d.IRCPosition == "Chairman")
                {
                    rbtChariman.IsChecked = true;
                }
                else if (d.IRCPosition == "Secretary")
                {
                    rbtSecretary.IsChecked = true;
                }
                else
                {
                    rbtCommitteMember.IsChecked = true;
                }

            }
        }
        private void FormFill()
        {
            try
            {
                var qry = (from x in db.MASTERMEMBERs where x.MEMBER_CODE == dMember_Code && x.IsCancel == false orderby x.DATEOFJOINING descending select x).FirstOrDefault();
                if (qry != null)
                {
                    DataTable dtResign = new DataTable();
                    DataTable dtFee = new DataTable();
                    using (SqlConnection con = new SqlConnection(AppLib.connStr))
                    {
                        SqlCommand cmd;
                        cmd = new SqlCommand(string.Format("SELECT * FROM VIEWRESIGN(NOLOCK) WHERE MEMBER_CODE={0}", dMember_Code), con);
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);

                        dtResign.Rows.Clear();
                        adp.SelectCommand.CommandTimeout = 0;
                        adp.Fill(dtResign);

                        string str = " SELECT ISNULL(AMOUNTBF,0)AMOUNTBF,ISNULL(UNIONCONTRIBUTION,0)UNIONCONTRIBUTION,ISNULL(AMOUNTINS,0)AMOUNTINS, \r" +
                                     " ISNULL(AMTSUBS,0)AMTSUBS,ISNULL(TOTALMONTHSPAID,0)TOTALMONTHSPAID,ISNULL(TOTALMONTHSPAIDINS, 0)TOTALMONTHSPAIDINS \r" +
                                     " FROM FEESDETAILS(NOLOCK)" +
                                     " WHERE UPDATEDSTATUS = 'NOT UPDATED' AND ISNOTMATCH=0  AND MEMBERCODE=" + dMember_Code;

                        cmd = new SqlCommand(str, con);
                        adp = new SqlDataAdapter(cmd);
                        adp.Fill(dtFee);
                    }

                    //var status = (from x in AppLib.lstTVMasterMember where x.MEMBER_CODE == dMember_Code select x).FirstOrDefault();
                    //var status = (from x in AppLib.lstMstMember where x.MEMBER_CODE == dMember_Code select x).FirstOrDefault();

                    var status = (from x in db.MemberStatusLogs where x.MEMBER_CODE == dMember_Code select x).FirstOrDefault();

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

                    //var nominee = (from nm in db.MASTERNOMINEEs
                    //               join ct in db.MASTERCITies on nm.CITY_CODE equals ct.CITY_CODE
                    //               join st in db.MASTERSTATEs on nm.STATE_CODE equals st.STATE_CODE
                    //               join rl in db.MASTERRELATIONs on nm.RELATION_CODE equals rl.RELATION_CODE
                    //               where nm.MEMBER_CODE == dMember_Code
                    //               orderby nm.Id descending
                    //               select new
                    //               {
                    //                   nm.NAME,
                    //                   nm.AGE,
                    //                   nm.SEX,
                    //                   rl.RELATION_NAME,
                    //                   nm.ICNO_NEW,
                    //                   nm.ICNO_OLD,
                    //                   nm.ADDRESS1,
                    //                   nm.ADDRESS2,
                    //                   nm.ADDRESS3,
                    //                   ct.CITY_NAME,
                    //                   st.STATE_NAME,
                    //                   nm.MOBILE,
                    //                   nm.PHONE,
                    //                   nm.MEMBER_CODE,
                    //                   nm.CITY_CODE,
                    //                   nm.STATE_CODE,
                    //                   nm.RELATION_CODE
                    //               }).ToList();
                    var nominee = db.MASTERNOMINEEs.Where(x=> x.MEMBER_CODE==dMember_Code).ToList();
                    if (nominee != null)
                    {
                        DataTable dt = new DataTable();
                        dt = AppLib.LINQResultToDataTable(nominee);
                        if (dt.Rows.Count > 0)
                        {
                            dgNomination.ItemsSource = dt.DefaultView;
                        }
                    }

                    //var gurdian = (from gr in db.MASTERGUARDIANs
                    //               where gr.MEMBER_CODE == dMember_Code
                    //               select new
                    //               {
                    //                   Name = gr.NAME,
                    //                   gr.RELATION_CODE,
                    //                   gr.ADDRESS1,
                    //                   gr.ADDRESS2,
                    //                   gr.ADDRESS3,
                    //                   gr.AGE,
                    //                   gr.ICNO_NEW,
                    //                   gr.ICNO_OLD,
                    //                   gr.CITY_CODE,
                    //                   gr.STATE_CODE,
                    //                   gr.COUNTRY,
                    //                   gr.ZIPCODE,
                    //                   gr.PHONE,
                    //                   gr.MOBILE,
                    //                   gr.SEX
                    //               }
                    //               ).FirstOrDefault();

                    var gurdian = db.MASTERGUARDIANs.FirstOrDefault(x => x.MEMBER_CODE == dMember_Code);

                    //var fees = (from fs in db.FeesDetails where fs.MemberCode == dMember_Code && fs.UpdatedStatus == "Not Updated" select fs).ToList();

                    decimal BF = 0, UC = 0, Ins = 0, Subs = 0, dTotlMonthsPaid = 0, dTotlMonthsPaidUC = 0;

                    foreach (DataRow dr in dtFee.Rows)
                    {
                        BF = BF + Convert.ToDecimal(dr["AMOUNTBF"]);
                        UC = UC + Convert.ToDecimal(dr["UNIONCONTRIBUTION"]);
                        Ins = Ins + Convert.ToDecimal(dr["AMOUNTINS"]);
                        Subs = Subs + Convert.ToDecimal(dr["AMTSUBS"]);
                        dTotlMonthsPaid = dTotlMonthsPaid + Convert.ToDecimal(dr["TOTALMONTHSPAID"]);
                        dTotlMonthsPaidUC = dTotlMonthsPaidUC + Convert.ToDecimal(dr["TOTALMONTHSPAIDINS"]);
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
                        txtSalary.Text = qry.Salary==null?"": qry.Salary.ToString();
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

                        if (status.GE_Insurance == true)
                        {
                            cmbGE_Insurance.Text = "Available";
                        }
                        else
                        {
                            cmbGE_Insurance.Text = "N/A";
                        }

                        if (status.AI_Insurance == true)
                        {
                            cmbAI_Insurance.Text = "Available";
                        }
                        else
                        {
                            cmbAI_Insurance.Text = "N/A";
                        }
                        txtTakaful.Text = dTotlMonthsPaidUC.ToString();
                        if (qry.TDF != null && qry.TDF.ToUpper() == "YES")
                        {
                            if (qry.TDF_AMOUNT != null)
                            {
                                txtTDFInsurance.Text = qry.TDF_AMOUNT.ToString();
                            }
                            else
                            {
                                txtTDFInsurance.Text = "0";
                            }
                        }
                        else
                        {
                            txtTDFInsurance.Text = "NO";
                        }

                        DateTime dtM = Convert.ToDateTime(status.LASTPAYMENT_DATE);
                        txtRegPaidTill.Text = dtM.ToString("MMM-yyyy");
                        txtRegContributedMonths.Text = Convert.ToInt32(qry.TotalPaid + dTotlMonthsPaid).ToString();

                        txtRegBFContribution.Text = Convert.ToInt32(qry.ACCBF).ToString();
                        txtRegBenefitYear.Text = Convert.ToInt32((qry.TotalPaid + dTotlMonthsPaid) / 12).ToString();

                        try
                        {
                            using (SqlConnection cn = new SqlConnection(AppLib.connstatus))
                            {
                                cn.Open();
                                var cmd = new SqlCommand(string.Format("select * from status052017 where member_code={0}", dMember_Code), cn);
                                var dr = cmd.ExecuteReader();
                                if (dr.Read())
                                {
                                    txtRegContributedMonths.Text = Convert.ToInt32(dr["TOTALMONTHSPAID"]).ToString();
                                    txtRegBFContribution.Text = Convert.ToInt32(dr["ACCBF"]).ToString();
                                }
                                dr.Close();
                                cn.Close();
                            }
                        }catch(Exception EX) { }

                        TimeSpan ts = Convert.ToDateTime(status.LASTPAYMENT_DATE) - Convert.ToDateTime(status.DATEOFJOINING);

                        int iSerYear = Convert.ToInt32(ts.Days) / 365;
                        txtRegServiceYear.Text = iSerYear.ToString();
                        int totalMonths = Convert.ToInt32(status.LASTPAYMENT_DATE.Value.Subtract(status.DATEOFJOINING.Value).Days / (365.25 / 12));

                        txtRegAmount.Text = (totalMonths * 3).ToString();
                        txtServicePeriod.Text = iSerYear.ToString();

                        if (qry.RESIGNED == 1)
                        {
                            lblStatus.Content = "MEMBER IS ALREADY RESIGNED";
                        }

                        txtEntranceFee.Text = qry.ENTRANCEFEE.ToString();
                        txtBuildingFund.Text = qry.HQFEE.ToString();
                        txtAccBenefit.Text = qry.ACCBENEFIT.ToString();
                        dtpLastPay.SelectedDate = Convert.ToDateTime(status.LASTPAYMENT_DATE);
                        dtpLastPay.IsEnabled = false;

                        txtMonthlySub.Text = qry.MONTHLYSUBSCRIPTION.ToString();
                        txtMonthlyBF.Text = qry.MONTHLYBF.ToString();
                        txtMonthlyUC.Text = "7";
                        txtMonthlyIns.Text = "10";

                        txtAccSub.Text = (status.AccSubs).ToString();
                        txtAccBF.Text = (status.AccBF).ToString();
                        txtAccUC.Text = (dTotlMonthsPaidUC * 7).ToString();
                        txtAccIns.Text = Ins.ToString();

                        txtCurrentYTDSub.Text = Subs.ToString();
                        txtCurrentYTDBF.Text = BF.ToString();
                        txtCurrentYTDUC.Text = (dTotlMonthsPaidUC * 7).ToString();
                        txtCurrentYTDIns.Text = Ins.ToString();

                        txtTotalMonthPaidSubs.Text = (status.TOTALMONTHSPAID).ToString();
                        txtTotalMonthPaidBF.Text = (status.TOTALMONTHSPAID).ToString();
                        txtTotalMonthPaidUC.Text = (dTotlMonthsPaidUC).ToString();
                        txtTotalMonthPaidIns.Text = dTotlMonthsPaidUC.ToString();

                        txtTotalMonthsDueSubs.Text = status.TOTALMOTHSDUE.ToString();
                        txtTotalMonthsDueBF.Text = status.TOTALMOTHSDUE.ToString();
                        txtTotalMonthsDueUC.Text = "0";
                        txtTotalMonthsDueIns.Text = "0";

                        txtBadgeAmt.Text = qry.BatchAmt.ToString();
                        if (qry.EMAIL != null)
                        {
                            txtResEmail.Text = qry.EMAIL.ToString();
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
                            lblStatus.Content = "MEMBER IS ALREADY RESIGNED";
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
                                lblStatus.Content = "MEMBER IS ALREADY RESIGNED";
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
                        txGurName.Text = gurdian.NAME;
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
                            cmbRegClaimer.SelectedValue = Convert.ToInt32("0"+ dtResign.Rows[i]["RELATION_CODE"]);
                            txtRegClaimerName.Text = dtResign.Rows[i]["CLAIMER_NAME"].ToString();
                            txtRegContributedMonths.Text = dtResign.Rows[i]["MONTHS_CONTRIBUTED"].ToString();
                            txtRegBFContribution.Text = dtResign.Rows[i]["ACCBF"].ToString();
                            txtRegBenefits.Text = dtResign.Rows[i]["ACCBENEFIT"].ToString();
                            txtRegAmount.Text = dtResign.Rows[i]["ACCBF"].ToString();
                            txtRegSubTotal.Text = dtResign.Rows[i]["ACCBENEFIT"].ToString();
                            cmbPaymode.Text = dtResign.Rows[i]["PAYMODE"].ToString();
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
                else
                {
                    MessageBox.Show("No Records Found!");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        void fTotal()
        {
            decimal iBFContri = Convert.ToDecimal(txtRegBFContribution.Text);
            decimal iBenefits = Convert.ToDecimal(txtRegSubTotal.Text);
            decimal iUnionContri = Convert.ToDecimal(txtRegUnionContri.Text);
            txtRegTotalAmount.Text = (iBFContri + iBenefits).ToString();
            txtRegGrandTotal.Text = (iBFContri + iBenefits + iUnionContri).ToString();
            //txtRegSubTotal.Text = (iBFContri + iBenefits).ToString();
        }

        void BenefitCalculation()
        {
            if (dMember_Code != 0)
            {
                int iBenefitAmt = 0;
                int iUnionContribbution = 0;
                decimal dReason = Convert.ToDecimal(cmbRegReason.SelectedValue);
                int iTotalmonth = AppLib.MonthDiff(dtpRegResignDate.SelectedDate.Value, dtpDOJ.SelectedDate.Value) + 1;

                if (dReason != 0)
                {
                    var rr = (from d in db.MASTERRESIGNSTATUS where d.RESIGNSTATUS_CODE == dReason && d.IsBenefitValid == true select d).FirstOrDefault();
                    if (rr != null)
                    {
                        int iBenefitYrs = Convert.ToInt32(txtRegServiceYear.Text);
                        iUnionContribbution = Convert.ToInt32(txtAccUC.Text);
                        if ((Convert.ToInt32(txtTotalMonthPaidSubs.Text) + 3) >= iTotalmonth && iBenefitYrs >= rr.MinimumYear)
                        {
                            int AmtPerYear1 = Convert.ToInt32(rr.AmtPerYear1);
                            int AmtPerYear2 = Convert.ToInt32(rr.AmtPerYear2);

                            iBenefitAmt = Convert.ToInt32((AmtPerYear1) + ((iBenefitYrs - 5) * AmtPerYear2));

                            if (iBenefitAmt < rr.MinimumRefund)
                            {
                                iBenefitAmt = Convert.ToInt32(rr.MinimumRefund);
                            }
                            if (iBenefitAmt > rr.MaximumRefund)
                            {
                                iBenefitAmt = Convert.ToInt32(rr.MaximumRefund);
                            }
                        }
                    }
                    else
                    {
                        iUnionContribbution = 0;
                    }
                }
                txtRegBenefits.Text = iBenefitAmt.ToString();
                txtRegUnionContri.Text = iUnionContribbution.ToString();
                //txtRegContributedMonths.Text = iTotalmonth.ToString();
                txtRegSubTotal.Text = iBenefitAmt.ToString();
                fTotal();
            }
        }

        #endregion

        private void txtIRCMemberNo_KeyUp(object sender, KeyEventArgs e)
        {
         if(e.Key==Key.Enter)
            {
                LoadIRCMember();
            }
        }

        void LoadIRCMember()
        {
            try {
                var id = Convert.ToDecimal(txtIRCMemberNo.Text);
                var mm = db.ViewMasterMembers.FirstOrDefault(x => x.MEMBER_ID == id);
                if (mm != null)
                {
                    txtIRCName.Text = mm.MEMBER_NAME;
                    txtIRCBankName.Text = mm.BANK_NAME;
                    txtIRCBankAddress.Text = string.Format("{0}, {1}, {2} {3}", mm.BRANCHADR1, mm.BRANCHADR2, mm.BRANCHZIPCODE, mm.BRANCHCITY);                    
                }
            } catch (Exception ex) { }
            
        }

        private void txtIRCMemberNo_LostFocus(object sender, RoutedEventArgs e)
        {
            LoadIRCMember();
        }

        private void txtMemberName_TextChanged(object sender, TextChangedEventArgs e)
        {
            IRCHeadingLoad();
        }

        private void btnIRCPrint_Click(object sender, RoutedEventArgs e)
        {
            frmIRCPrint frm = new frmIRCPrint();
            frm.loadData(this);
            frm.ShowDialog();
        }

        private void btnNextIRC_Click(object sender, RoutedEventArgs e)
        {
            tbiBank.IsEnabled = true;
            tbiBank.Focus();
        }

        private void btnNextBank_Click(object sender, RoutedEventArgs e)
        {
            tbiResidentialAddress.IsEnabled = true;
            tbiResidentialAddress.Focus();
        }

        private void btnNextResidentialAddress_Click(object sender, RoutedEventArgs e)
        {
            tbiFundDetail.IsEnabled = true;
            tbiFundDetail.Focus();
        }

        private void btnNextNomineeDetails_Click(object sender, RoutedEventArgs e)
        {
            tbiGaurdian.IsEnabled = true;
            tbiGaurdian.Focus();
        }

        private void btnNextFund_Click(object sender, RoutedEventArgs e)
        {
            tbiNomineeDetails.IsEnabled = true;
            tbiNomineeDetails.Focus();
        }

        private void btnNextGaurdian_Click(object sender, RoutedEventArgs e)
        {
            tbiResignationDetail.IsEnabled = true;
            tbiResignationDetail.Focus();
        }
    }
}
