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

        public frmIRCConfirmation()
        {
            InitializeComponent();
            FormLoad();
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
                
                var id = Convert.ToDecimal("0" + txtMemberNo.Text);
                var mm = db.ViewMasterMembers.FirstOrDefault(x => x.MEMBER_ID == id);
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

            cbxPromotedTo.Content = string.Format("{0} was promoted to", SheOrHe);
            cbxBeforePromotion.Content = string.Format("{0} was a {1} before promotion [Delete which is not applicable]", SheOrHe, rMemberType);
            cbxHereByConfirm.Content = string.Format("I hereby confirm that {0} got promoted {0} is no longer doing any clerical job function.", SheOrHe);
            cbxFilledBy.Content = string.Format("The {0} position has been filled by", rMemberType);
            cbxBranchCommitteeVerification1.Content = String.Format("I have verified the above and confirm that the declaration by the IRC is correct. The {0} position has been filled by another {0} And;",rMemberType);
            cbxBranchCommitteeVerification2.Content = String.Format("The promoted member is no longer doing {0} job functions", rMemberType);
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
            if(cbxPromotedTo.IsChecked==true && string.IsNullOrWhiteSpace(txtIRCPromotedTo.Text))
            {
                rv = false;
                MessageBox.Show("Please enter the Promoted to");
                txtIRCPromotedTo.Focus();
            }
            else if(cbxPromotedTo.IsChecked==true && dtpGrade.SelectedDate == null)
            {
                rv = false;
                MessageBox.Show("Please selected Grade w.e.f");
                dtpGrade.Focus();
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

            dtpDOB.Text = "";
            dtpDOJ.Text = "";
            dtpGrade.Text = "";
            dtpBranchCommitteeDate.Text = "";

            dtpDOB.SelectedDate = null;
            dtpDOJ.SelectedDate = null;
            dtpGrade.SelectedDate = null;
            dtpBranchCommitteeDate.SelectedDate = null;
            
            cbxNameOfPerson.IsChecked = false;
            cbxPromotedTo.IsChecked = false;
            cbxBeforePromotion.IsChecked = false;
            cbxAttached.IsChecked = false;
            cbxHereByConfirm.IsChecked = false;
            cbxFilledBy.IsChecked = false;
            cbxBranchCommitteeVerification1.IsChecked = false;
            cbxBranchCommitteeVerification2.IsChecked = false;
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
                    IRC.IRCPosition = rbtChariman.IsChecked == true ? "Chairman" : rbtSecretary.IsChecked == true ? "Secretary" : "Commitee Member";
                    IRC.IRCMembershipNo = txtIRCMemberNo.Text;
                    IRC.IRCName = txtIRCName.Text;
                    IRC.PromotedTo = txtIRCPromotedTo.Text;
                    IRC.IRCBank = txtIRCBankName.Text;
                    IRC.IRCBankAddress = txtIRCBankAddress.Text;
                    IRC.IRCTelephoneNo = txtIRCTelephoneNo.Text;
                    IRC.IRCMobileNo = txtIRCMobileNo.Text;
                    IRC.IRCFaxNo = txtIRCFax.Text;
                    IRC.GradeWEF = dtpGrade.SelectedDate;
                    IRC.NameOfPerson = cbxNameOfPerson.IsChecked;
                    IRC.WasPromoted = cbxPromotedTo.IsChecked;
                    IRC.BeforePromotion = cbxBeforePromotion.IsChecked;
                    IRC.Attached = cbxAttached.IsChecked;
                    IRC.HereByConfirm = cbxHereByConfirm.IsChecked;
                    IRC.FilledBy = cbxFilledBy.IsChecked;
                    IRC.BranchCommitteeVerification1 = cbxBranchCommitteeVerification1.IsChecked;
                    IRC.BranchCommitteeVerification2 = cbxBranchCommitteeVerification2.IsChecked;
                    IRC.BranchCommitteeName = txtBranchCommitteeName.Text;
                    IRC.BranchCommitteeZone = txtBranchCommitteeZone.Text;
                    IRC.BranchCommitteeDate = dtpBranchCommitteeDate.SelectedDate;
                    IRC.Remarks = txtRemarks.Text;
                    IRC.Status = cbxNameOfPerson.IsChecked == true
                              && cbxPromotedTo.IsChecked == true
                              && cbxBeforePromotion.IsChecked == true
                              && cbxAttached.IsChecked == true
                              && cbxHereByConfirm.IsChecked == true
                              && cbxFilledBy.IsChecked == true
                              && cbxBranchCommitteeVerification1.IsChecked == true
                              && cbxBranchCommitteeVerification2.IsChecked == true
							  && !string.IsNullOrWhiteSpace(txtIRCMemberNo.Text)==true
							  && (rbtChariman.IsChecked==true || rbtSecretary.IsChecked==true || rbtCommitteMember.IsChecked==true )
                              ? "Confirm" : "Pending";
                    IRC.UpdatedAt = DateTime.Now;

                    db.SaveChanges();
                    MessageBox.Show(IRC.Status=="Confirm"? "Submitted Successfully" : "Stored to Draft");
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
	}
}
