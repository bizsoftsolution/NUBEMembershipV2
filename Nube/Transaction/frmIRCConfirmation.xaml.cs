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
                if (mm == null)
                {
                    mm = new ViewMasterMember();
                }
                dtpDOB.Text = "";
                dtpDOJ.Text = "";
                txtAge.Text = "";

                txtMemberName.Text = mm.MEMBER_NAME??"";
                cmbBankName.Text = mm.BANK_NAME ?? "";
                cmbMemberType.Text = mm.MEMBERTYPE_NAME ?? "";
                cmbMemberInit.Text = mm.MEMBER_INITIAL ?? "";
                cmbBankBranchName.Text = mm.BRANCHNAME ?? "";
                cmbGender.Text = mm.SEX ?? "";
                cmbRace.Text = mm.RACE_NAME ?? "";                
                dtpDOB.SelectedDate = mm.DATEOFBIRTH;
                dtpDOJ.SelectedDate = mm.DATEOFJOINING;
                txtNewIC.Text = string.IsNullOrWhiteSpace(mm.ICNO_NEW) ? mm.ICNO_OLD : mm.ICNO_NEW;
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

        private void btnIRCSubmit_Click(object sender, RoutedEventArgs e)
        {
            var mm = db.IRCConfirmations.FirstOrDefault(x => x.ResignMemberICNo == txtMemberNo.Text);
            MessageBox.Show("Saved");
        }

        private void btnIRCSerch_Click(object sender, RoutedEventArgs e)
        {
            frmIRCConfirmationSearch frm = new frmIRCConfirmationSearch();
            frm.ShowDialog();
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
    }
}
