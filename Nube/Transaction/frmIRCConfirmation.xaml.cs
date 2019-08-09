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

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmIRCConfirmation.xaml
    /// </summary>
    public partial class frmIRCConfirmation : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();

        enum IRC{
            Confirmation = 7,
            BranchCommittee = 8
        }

        public frmIRCConfirmation()
        {
            InitializeComponent();
            FormLoad();
            if(AppLib.iUsertypeId == (int)IRC.Confirmation)
            {
                txtIRCMemberNo.IsEnabled = true;
                txtIRCName.IsEnabled = true;
                rbtChariman.IsEnabled = true;
                rbtSecretary.IsEnabled = true;
                rbtCommitteMember.IsEnabled = true;
                txtIRCBankName.IsEnabled = true;
                txtIRCBankAddress.IsEnabled = true;
                txtIRCTelephoneNo.IsEnabled = true;
                txtIRCMobileNo.IsEnabled = true;
                txtIRCFax.IsEnabled = true;
                

                cbxNameOfPerson.IsEnabled = true;
                txtIRCResignMemberName.IsEnabled = true;
                cbxPromotedTo.IsEnabled = true;
                cmbRegReason.IsEnabled = true;
                txtIRCPromotedTo.IsEnabled = true;
                cbxBeforePromotion.IsEnabled = true;
                cbxAttached.IsEnabled = true;
                txtIRCJobFunction.IsEnabled = true;
                cbxHereByConfirm.IsEnabled = true;
                cbxFilledBy.IsEnabled = true;
                txtIRCFilledBy.IsEnabled = true;
                txtIRCFilledByPosition.IsEnabled = true;
                txtIRCFilledByContact.IsEnabled = true;
                cbxTransferTo.IsEnabled = true;
                txtIRCTransferTo.IsEnabled = true;
                cbxContact.IsEnabled = true;
                txtIRCContact.IsEnabled = true;
                txtIRCContactMobileNo.IsEnabled = true;
                txtIRCContactFax.IsEnabled = true;


                cbxBranchCommitteeVerification1.IsEnabled = false;
                cbxBranchCommitteeVerification2.IsEnabled = false;
                txtBranchCommitteeName.IsEnabled = false;
                txtBranchCommitteeZone.IsEnabled = false;
            }
            else
            {
                txtIRCMemberNo.IsEnabled = false;
                txtIRCName.IsEnabled = false;
                rbtChariman.IsEnabled = false;
                rbtSecretary.IsEnabled = false;
                rbtCommitteMember.IsEnabled = false;
                txtIRCBankName.IsEnabled = false;
                txtIRCBankAddress.IsEnabled = false;
                txtIRCTelephoneNo.IsEnabled = false;
                txtIRCMobileNo.IsEnabled = false;
                txtIRCFax.IsEnabled = false;

                cbxNameOfPerson.IsEnabled = false;
                txtIRCResignMemberName.IsEnabled = false;
                cbxPromotedTo.IsEnabled = false;
                cmbRegReason.IsEnabled = false;
                txtIRCPromotedTo.IsEnabled = false;
                cbxBeforePromotion.IsEnabled = false;
                cbxAttached.IsEnabled = false;
                txtIRCJobFunction.IsEnabled = false;
                cbxHereByConfirm.IsEnabled = false;
                cbxFilledBy.IsEnabled = false;
                txtIRCFilledBy.IsEnabled = false;
                txtIRCFilledByPosition.IsEnabled = false;
                txtIRCFilledByContact.IsEnabled = false;
                cbxTransferTo.IsEnabled = false;
                txtIRCTransferTo.IsEnabled = false;
                cbxContact.IsEnabled = false;
                txtIRCContact.IsEnabled = false;
                txtIRCContactMobileNo.IsEnabled = false;
                txtIRCContactFax.IsEnabled = false;

                cbxBranchCommitteeVerification1.IsEnabled = true;
                cbxBranchCommitteeVerification2.IsEnabled = true;
                txtBranchCommitteeName.IsEnabled = true;
                txtBranchCommitteeZone.IsEnabled = true;
            }
        }

        void FormLoad()
        {
            try
            {
                var city = db.MASTERCITies.ToList();
                var state = db.MASTERSTATEs.ToList();
                var relation = db.MASTERRELATIONs.ToList();

                cmbMemberType.ItemsSource = db.MASTERMEMBERTYPEs.ToList();
                cmbMemberType.SelectedValuePath = "MEMBERTYPE_CODE";
                cmbMemberType.DisplayMemberPath = "MEMBERTYPE_NAME";

                cmbMemberInit.ItemsSource = db.NameTitleSetups.ToList();
                cmbMemberInit.SelectedValuePath = "ID";
                cmbMemberInit.DisplayMemberPath = "TitleName";

                cmbBankName.ItemsSource = db.MASTERBANKs.ToList();
                cmbBankName.SelectedValuePath = "BANK_CODE";
                cmbBankName.DisplayMemberPath = "BANK_NAME";

                cmbBankBranchName.ItemsSource = db.MASTERBANKBRANCHes.ToList();
                cmbBankBranchName.SelectedValuePath = "BANKBRANCH_CODE";
                cmbBankBranchName.DisplayMemberPath = "BANKBRANCH_NAME";

                cmbMemberType.ItemsSource = db.MASTERMEMBERTYPEs.ToList();
                cmbMemberType.SelectedValuePath = "MEMBERTYPE_CODE";
                cmbMemberType.DisplayMemberPath = "MEMBERTYPE_NAME";

                cmbRace.ItemsSource = db.MASTERRACEs.ToList();
                cmbRace.SelectedValuePath = "RACE_CODE";
                cmbRace.DisplayMemberPath = "RACE_NAME";

                cmbRegReason.ItemsSource = db.MASTERRESIGNSTATUS.ToList();
                cmbRegReason.SelectedValuePath = "RESIGNSTATUS_CODE";
                cmbRegReason.DisplayMemberPath = "RESIGNSTATUS_NAME";

            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        void LoadResignMember()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMemberNo.Text)) return;
                var id = Convert.ToDecimal("0" + txtMemberNo.Text);
                var mm = db.ViewMasterMembers.FirstOrDefault(x => x.MEMBER_ID == id || x.ICNO_NEW==txtMemberNo.Text || x.ICNO_OLD == txtMemberNo.Text || x.NRIC_BYBANK == txtMemberNo.Text);
                ClearIRC();
                if (mm != null)                
                {
                    txtMemberNo.Text = mm.MEMBER_ID.ToString();
					cmbMemberInit.Text = mm.MEMBER_TITLE ?? "";
                    txtMemberName.Text = mm.MEMBER_NAME ?? "";
                    cmbBankName.Text = mm.BANK_NAME ?? "";
				    cmbMemberType.Text = mm.MEMBERTYPE_NAME ?? "";
                    cmbMemberInit.Text = mm.MEMBER_INITIAL ?? "";
                    cmbBankBranchName.Text = mm.BRANCHNAME ?? "";
                    cmbGender.Text = mm.SEX ?? "";
                    cmbRace.Text = mm.RACE_NAME ?? "";
                    dtpDOB.SelectedDate = mm.DATEOFBIRTH;
                    dtpDOJ.SelectedDate = mm.DATEOFJOINING;
                    txtNewIC.Text = string.IsNullOrWhiteSpace(mm.ICNO_NEW) ? mm.ICNO_OLD : mm.ICNO_NEW;
                    ViewIRC(mm.MEMBER_CODE);
                }                
            }
            catch (Exception ex) { }

        }

        void LoadIRCMember()
        {
            try
            {
                var id = Convert.ToDecimal(txtIRCMemberNo.Text);
                var mm = db.ViewMasterMembers.FirstOrDefault(x => x.MEMBER_ID == id);
                if (mm != null)
                {
                    txtIRCName.Text = mm.MEMBER_NAME;
                    txtIRCBankName.Text = mm.BANK_NAME;
                    txtIRCBankAddress.Text = string.Format("{0}, {1}, {2} {3}", mm.BRANCHADR1, mm.BRANCHADR2, mm.BRANCHZIPCODE, mm.BRANCHCITY);
                }
            }
            catch (Exception ex) { }

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

            cbxPromotedTo.Content = string.Format("{0} was ", SheOrHe);
            cbxBeforePromotion.Content = string.Format("{0} was a {1} before promotion [Delete which is not applicable]", SheOrHe, rMemberType);
            cbxHereByConfirm.Content = string.Format("I hereby confirm that {0} got {1} {0} is no longer doing any clerical job function.", SheOrHe,cmbRegReason.Text.ToLower());
            cbxFilledBy.Content = string.Format("The {0} position has been filled by", rMemberType);
            cbxBranchCommitteeVerification1.Content = String.Format("I have verified the above and confirm that the declaration by the IRC is correct. The {0} position has been filled by another {0} And;",rMemberType);
            cbxBranchCommitteeVerification2.Content = String.Format("The promoted member is no longer doing {0} job functions", rMemberType);
            cbxTransferTo.Content = string.Format("{0} promoted and transfer to new place",SheOrHe);
        }

        private void txtMemberName_TextChanged(object sender, TextChangedEventArgs e)
        {
            IRCHeadingLoad();
        }

        private void txtIRCMemberNo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadIRCMember();
            }
        }

        private void txtIRCMemberNo_LostFocus(object sender, RoutedEventArgs e)
        {
            LoadIRCMember();
        }

        private void dtpDOB_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
				if (dtpDOB.SelectedDate != null)
				{
					DateTime now = DateTime.Today;
					TimeSpan ts = DateTime.Now - Convert.ToDateTime(dtpDOB.SelectedDate);
					int age = Convert.ToInt32(ts.Days) / 365;
					txtAge.Text = age.ToString();
				}
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        bool IsValid()
        {
            bool rv = true;
			if (cbxPromotedTo.IsChecked == true && string.IsNullOrWhiteSpace(txtIRCPromotedTo.Text))
			{
				rv = false;
				MessageBox.Show("Please enter the Promoted to");
				txtIRCPromotedTo.Focus();
			}
			else if (cbxPromotedTo.IsChecked == true && dtpGrade.SelectedDate == null)
			{
				rv = false;
				MessageBox.Show("Please selected Grade w.e.f");
				dtpGrade.Focus();
			}
			else if (cbxFilledBy.IsChecked == true && string.IsNullOrWhiteSpace(txtIRCFilledBy.Text))
			{
				rv = false;
				MessageBox.Show("Please enter Filled by");
				txtIRCFilledBy.Focus();
			}
            return rv;
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
				txtIRCFilledBy.Text = d.NameforFilledBy;
                txtIRCJobFunction.Text = d.IRCJobFunction;
                txtIRCFilledByPosition.Text = d.IRCFilledByPosition;
                txtIRCFilledByContact.Text = d.IRCFilledByContact;
                txtIRCTransferTo.Text = d.IRCTransferTo;
                txtIRCContact.Text = d.IRCContact;
                txtIRCContactMobileNo.Text = d.IRCContactMobileNo;
                txtIRCContactFax.Text = d.IRCContactFax;
                dtpGrade.SelectedDate = d.GradeWEF;
                dtpBranchCommitteeDate.SelectedDate = d.BranchCommitteeDate;
				dtpFileSubmit.SelectedDate = d.SubmittedAt;
                cbxNameOfPerson.IsChecked = d.NameOfPerson;
                cbxPromotedTo.IsChecked = d.WasPromoted;
                cbxBeforePromotion.IsChecked = d.BeforePromotion;
                cbxAttached.IsChecked = d.Attached;
                cbxHereByConfirm.IsChecked = d.HereByConfirm;
                cbxFilledBy.IsChecked = d.FilledBy;
                cbxTransferTo.IsChecked = d.TransferTo;
                cbxContact.IsChecked = d.IsContact;
                cbxBranchCommitteeVerification1.IsChecked = d.BranchCommitteeVerification1;
                cbxBranchCommitteeVerification2.IsChecked = d.BranchCommitteeVerification2;
                cmbRegReason.Text = d.ResignReason;

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

        void ClearIRC()
        {
            txtMemberNo.Text = "";
            txtMemberName.Text = "";
            cmbBankName.Text = "";
            cmbMemberType.Text =  "";
            cmbMemberInit.Text = "";
            cmbBankBranchName.Text = "";
            cmbGender.Text = "";
            cmbRace.Text = "";
			txtAge.Text = "";
            txtNewIC.Text = "";                      

            txtIRCMemberNo.Text = "";
            txtIRCName.Text = "";
            txtIRCBankName.Text = "";
            txtIRCBankAddress.Text = "";
            txtIRCTelephoneNo.Text = "";
            txtIRCMobileNo.Text = "";
            txtIRCFax.Text = "";
            txtMemberName.Text = "";
            txtIRCPromotedTo.Text ="";
            txtBranchCommitteeName.Text = "";
            txtBranchCommitteeZone.Text = "";
            txtRemarks.Text = "";
			txtIRCFilledBy.Text = "";
            txtIRCJobFunction.Text = "";
            txtIRCFilledByPosition.Text = "";
            txtIRCFilledByContact.Text = "";
            txtIRCTransferTo.Text = "";
            txtIRCContact.Text = "";
            txtIRCContactMobileNo.Text = "";
            txtIRCContactFax.Text = "";
            cmbRegReason.Text = "PROMOTED";

            dtpDOB.Text = "";
            dtpDOJ.Text = "";
            dtpGrade.Text = "";
            dtpBranchCommitteeDate.Text = "";
			dtpFileSubmit.Text = "";

            dtpDOB.SelectedDate = null;
            dtpDOJ.SelectedDate = null;
            dtpGrade.SelectedDate = null;
            dtpBranchCommitteeDate.SelectedDate = null;
			dtpFileSubmit.SelectedDate = null;
            cbxNameOfPerson.IsChecked = false;
            cbxPromotedTo.IsChecked = false;
            cbxBeforePromotion.IsChecked = false;
            cbxAttached.IsChecked = false;
            cbxHereByConfirm.IsChecked = false;
            cbxFilledBy.IsChecked = false;
            cbxBranchCommitteeVerification1.IsChecked = false;
            cbxBranchCommitteeVerification2.IsChecked = false;
            cbxTransferTo.IsChecked = false;
            cbxContact.IsChecked = false;
            rbtChariman.IsChecked = false;
            rbtSecretary.IsChecked = false;
            rbtCommitteMember.IsChecked = false;
        }
        private void btnIRCSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsValid())
                {
                    var mm = db.MASTERMEMBERs.FirstOrDefault(x => x.MEMBER_ID.ToString() == txtMemberNo.Text);
                    var IRC = db.IRCConfirmations.FirstOrDefault(x => x.MemberCode == mm.MEMBER_CODE);
                    if (IRC == null)
                    {
                        IRC = new IRCConfirmation();
                        db.IRCConfirmations.Add(IRC);
                        IRC.CreatedAt = DateTime.Now;
                    }

                    IRC.MemberCode = mm.MEMBER_CODE;
                    IRC.ResignMemberNo = txtMemberNo.Text;
                    IRC.ResignMemberICNo = txtNewIC.Text;
                    IRC.ResignMemberName = txtMemberName.Text;
                    IRC.ResignMemberBankName = cmbBankName.Text;
                    IRC.ResignMemberBranchName = cmbBankBranchName.Text;
                    IRC.ResignReason = cmbRegReason.Text;
                    IRC.IRCPosition = rbtChariman.IsChecked == true ? "Chairman" : rbtSecretary.IsChecked == true ? "Secretary" : "Commitee Member";
                    IRC.IRCMembershipNo = txtIRCMemberNo.Text;
                    IRC.IRCName = txtIRCName.Text;
                    IRC.PromotedTo = txtIRCPromotedTo.Text;
                    IRC.IRCBank = txtIRCBankName.Text;
                    IRC.IRCBankAddress = txtIRCBankAddress.Text;
                    IRC.IRCTelephoneNo = txtIRCTelephoneNo.Text;
                    IRC.IRCMobileNo = txtIRCMobileNo.Text;
                    IRC.IRCFaxNo = txtIRCFax.Text;
                    IRC.IRCJobFunction = txtIRCJobFunction.Text;
                    IRC.IRCFilledByPosition = txtIRCFilledByPosition.Text;
                    IRC.IRCFilledByContact = txtIRCFilledByContact.Text;
                    IRC.IRCTransferTo = txtIRCTransferTo.Text;
                    IRC.IRCContact = txtIRCContact.Text;
                    IRC.IRCContactMobileNo = txtIRCContactMobileNo.Text;
                    IRC.IRCContactFax = txtIRCContactFax.Text;
                    IRC.GradeWEF = dtpGrade.SelectedDate;
                    IRC.NameOfPerson = cbxNameOfPerson.IsChecked;
                    IRC.WasPromoted = cbxPromotedTo.IsChecked;
                    IRC.BeforePromotion = cbxBeforePromotion.IsChecked;
                    IRC.Attached = cbxAttached.IsChecked;
                    IRC.HereByConfirm = cbxHereByConfirm.IsChecked;
                    IRC.FilledBy = cbxFilledBy.IsChecked;
                    IRC.TransferTo = cbxTransferTo.IsChecked;
                    IRC.IsContact = cbxContact.IsChecked;
					IRC.NameforFilledBy = txtIRCFilledBy.Text;
                    IRC.BranchCommitteeVerification1 = cbxBranchCommitteeVerification1.IsChecked;
                    IRC.BranchCommitteeVerification2 = cbxBranchCommitteeVerification2.IsChecked;
                    IRC.BranchCommitteeName = txtBranchCommitteeName.Text;
                    IRC.BranchCommitteeZone = txtBranchCommitteeZone.Text;
                    IRC.BranchCommitteeDate = dtpBranchCommitteeDate.SelectedDate;
					IRC.SubmittedAt = dtpFileSubmit.SelectedDate;
                    IRC.Remarks = txtRemarks.Text;
                    IRC.Status = cbxNameOfPerson.IsChecked == true
                              && cbxPromotedTo.IsChecked == true
                              && cbxHereByConfirm.IsChecked == true
                              && cbxTransferTo.IsChecked == true
                              && cbxContact.IsChecked==true
                              && !string.IsNullOrWhiteSpace(txtIRCMemberNo.Text) == true
                              && (rbtChariman.IsChecked == true || rbtSecretary.IsChecked == true || rbtCommitteMember.IsChecked == true)
                              && (  
                                    cmbRegReason.Text != "PROMOTED"  
                                    ||   (      cmbRegReason.Text == "PROMOTED" 
                                                && cbxBeforePromotion.IsChecked == true
                                                && cbxAttached.IsChecked == true
                                                && cbxFilledBy.IsChecked == true
                                                && cbxBranchCommitteeVerification1.IsChecked == true
                                                && cbxBranchCommitteeVerification2.IsChecked == true
                                          )
                                  )
                              
							  
                              ? "Confirm" : "Pending";
                    IRC.UpdatedAt = DateTime.Now;

                    db.SaveChanges();
                    MessageBox.Show(IRC.Status=="Confirm"? "Submitted Successfully" : "Saved to Draft");
                    ClearIRC();
                }                
            }
            catch(Exception ex) { }
            
        }

        private void btnIRCSerch_Click(object sender, RoutedEventArgs e)
        {
            frmIRCConfirmationSearch frm = new frmIRCConfirmationSearch();
            frm.ShowDialog();
            if(!string.IsNullOrWhiteSpace(frm.selectedIRC.ResignMemberNo))
            {
                txtMemberNo.Text = frm.selectedIRC.ResignMemberNo;
                LoadResignMember();
            }
        }

        private void btnIRCSignOut_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtMemberNo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadResignMember();
            }
        }

        private void txtMemberNo_LostFocus(object sender, RoutedEventArgs e)
        {
            LoadResignMember();
        }

        private void cmbMemberType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IRCHeadingLoad();
        }

        private void cmbGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IRCHeadingLoad();
        }

        private void cmbGender_LostFocus(object sender, RoutedEventArgs e)
        {
            IRCHeadingLoad();
        }

        private void cmbMemberType_LostFocus(object sender, RoutedEventArgs e)
        {
            IRCHeadingLoad();
        }

        private void cmbBankBranchName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dtpGrade_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

		private void btnIRCClear_Click(object sender, RoutedEventArgs e)
		{
			ClearIRC();
		}

        private void cmbRegReason_KeyUp(object sender, KeyEventArgs e)
        {
            ResignReasonChanged();
        }

      

        void ResignReasonChanged()
        {
            if (cmbRegReason.Text == "PROMOTED")
            {
                cbxBeforePromotion.Visibility = Visibility.Visible;
                cbxAttached.Visibility = Visibility.Visible;
                cbxFilledBy.Visibility = Visibility.Visible;
				txtIRCFilledBy.Visibility = Visibility.Visible;
                splBRANCHCOMMITTEEVERIFICATION.Visibility = Visibility.Visible;
            }
            else
            {
                cbxBeforePromotion.Visibility = Visibility.Collapsed;
                cbxAttached.Visibility = Visibility.Collapsed;
                cbxFilledBy.Visibility = Visibility.Collapsed;
				txtIRCFilledBy.Visibility = Visibility.Collapsed;
                splBRANCHCOMMITTEEVERIFICATION.Visibility = Visibility.Collapsed;
            }

            IRCHeadingLoad();
        }

        private void cmbRegReason_LostFocus(object sender, RoutedEventArgs e)
        {
            ResignReasonChanged();
        }
    }
}
