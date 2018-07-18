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
using Nube.MasterSetup;
using System.Data.SqlClient;
using System.Data;

namespace Nube
{
    /// <summary>
    /// Interaction logic for frmHomeMaster.xaml
    /// </summary>
    public partial class frmHomeMaster : MetroWindow
    {
        UserPrevilage userPrevilage;
        public frmHomeMaster()
        {
            InitializeComponent();
            //btnMonthEndClosing.Visibility = Visibility.Hidden;
            //btnYearEndClosing.Visibility = Visibility.Hidden;
            btnCountry.Visibility = Visibility.Hidden;
            btnUserAccount.Visibility = Visibility.Hidden;
            btnUserRights.Visibility = Visibility.Hidden;
            UserRights();
        }

        void UserRights()
        {
            try
            {
                userPrevilage = new UserPrevilage(this.btnBank.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnBank.IsEnabled = true;

                }
                else
                {
                    btnBank.IsEnabled = false;
                }

                userPrevilage = new UserPrevilage(this.btnBranch.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnBranch.IsEnabled = true;
                }
                else
                {
                    btnBranch.IsEnabled = false;
                }

                userPrevilage = new UserPrevilage(this.btnNUBEBranch.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnNUBEBranch.IsEnabled = true;
                }
                else
                {
                    btnNUBEBranch.IsEnabled = false;
                }

                userPrevilage = new UserPrevilage(this.btnPersonTitle.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnPersonTitle.IsEnabled = true;
                }
                else
                {
                    btnPersonTitle.IsEnabled = false;
                }

                userPrevilage = new UserPrevilage(this.btnSalutation.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnSalutation.IsEnabled = true;
                }
                else
                {
                    btnSalutation.IsEnabled = false;
                }

                userPrevilage = new UserPrevilage(this.btnRelationSetup.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnRelationSetup.IsEnabled = true;
                }
                else
                {
                    btnRelationSetup.IsEnabled = false;
                }

                userPrevilage = new UserPrevilage(this.btnReasonSetup.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnReasonSetup.IsEnabled = true;
                }
                else
                {
                    btnReasonSetup.IsEnabled = false;
                }

                userPrevilage = new UserPrevilage(this.btnNameSetup.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnNameSetup.IsEnabled = true;
                }
                else
                {
                    btnNameSetup.IsEnabled = false;
                }

                userPrevilage = new UserPrevilage(this.btnCity.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnCity.IsEnabled = true;
                }
                else
                {
                    btnCity.IsEnabled = false;
                }

                userPrevilage = new UserPrevilage(this.btnStateSetup.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnStateSetup.IsEnabled = true;
                }
                else
                {
                    btnStateSetup.IsEnabled = false;
                }

                userPrevilage = new UserPrevilage(this.btnMonthEndClosing.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnMonthEndClosing.IsEnabled = true;
                }
                else
                {
                    btnMonthEndClosing.IsEnabled = false;
                }

                userPrevilage = new UserPrevilage(this.btnYearEndClosing.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnYearEndClosing.IsEnabled = true;
                }
                else
                {
                    btnYearEndClosing.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnRelationSetup_Click(object sender, RoutedEventArgs e)
        {
            frmRelationSetup frm = new frmRelationSetup("HOME");
            userPrevilage = new UserPrevilage(this.btnRelationSetup.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                frm.btnDelete.IsEnabled = Convert.ToBoolean(userPrevilage.Remove);
                frm.bIsEdit = Convert.ToBoolean(userPrevilage.Edit);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnStateCitySetup_Click(object sender, RoutedEventArgs e)
        {
            frmBranchSetup frm = new frmBranchSetup("HOME");
            userPrevilage = new UserPrevilage(this.btnStateSetup.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                frm.btnDelete.IsEnabled = Convert.ToBoolean(userPrevilage.Remove);
                frm.bIsEdit = Convert.ToBoolean(userPrevilage.Edit);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnReasonSetup_Click(object sender, RoutedEventArgs e)
        {
            frmReasonSetup frm = new frmReasonSetup("HOME");
            userPrevilage = new UserPrevilage(this.btnReasonSetup.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                frm.btnDelete.IsEnabled = Convert.ToBoolean(userPrevilage.Remove);
                frm.bIsEdit = Convert.ToBoolean(userPrevilage.Edit);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            frmHomeMembership frm = new frmHomeMembership();
            this.Close();
            frm.ShowDialog();
        }

        private void btnCountry_Click(object sender, RoutedEventArgs e)
        {
            frmCountrySetup frm = new frmCountrySetup("HOME");
            userPrevilage = new UserPrevilage(this.btnCountry.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                frm.btnDelete.IsEnabled = Convert.ToBoolean(userPrevilage.Remove);
                frm.bIsEdit = Convert.ToBoolean(userPrevilage.Edit);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnCitySetup_Copy_Click(object sender, RoutedEventArgs e)
        {
            frmCitySetup frm = new frmCitySetup("HOME");
            userPrevilage = new UserPrevilage(this.btnCity.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                frm.btnDelete.IsEnabled = Convert.ToBoolean(userPrevilage.Remove);
                frm.bIsEdit = Convert.ToBoolean(userPrevilage.Edit);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnBank_Click(object sender, RoutedEventArgs e)
        {
            frmBankSetup frm = new frmBankSetup("HOME");
            userPrevilage = new UserPrevilage(this.btnBank.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                frm.btnDelete.IsEnabled = Convert.ToBoolean(userPrevilage.Remove);
                frm.bIsEdit = Convert.ToBoolean(userPrevilage.Edit);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnBranch_Click(object sender, RoutedEventArgs e)
        {
            frmBranchSetup frm = new frmBranchSetup("HOME");
            userPrevilage = new UserPrevilage(this.btnBranch.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                frm.btnDelete.IsEnabled = Convert.ToBoolean(userPrevilage.Remove);
                frm.bIsEdit = Convert.ToBoolean(userPrevilage.Edit);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnNUBEBranch_Click(object sender, RoutedEventArgs e)
        {
            frmNUBEBranch frm = new frmNUBEBranch("HOME");
            userPrevilage = new UserPrevilage(this.btnNUBEBranch.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                frm.btnDelete.IsEnabled = Convert.ToBoolean(userPrevilage.Remove);
                frm.bIsEdit = Convert.ToBoolean(userPrevilage.Edit);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnPersonTitle_Click(object sender, RoutedEventArgs e)
        {
            frmPersonTitleSetup frm = new frmPersonTitleSetup();
            userPrevilage = new UserPrevilage(this.btnPersonTitle.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                frm.btnDelete.IsEnabled = Convert.ToBoolean(userPrevilage.Remove);
                frm.bIsEdit = Convert.ToBoolean(userPrevilage.Edit);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnSalutation_Click(object sender, RoutedEventArgs e)
        {
            frmSalutationSetup frm = new frmSalutationSetup("HOME");
            userPrevilage = new UserPrevilage(this.btnSalutation.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                frm.btnDelete.IsEnabled = Convert.ToBoolean(userPrevilage.Remove);
                frm.bIsEdit = Convert.ToBoolean(userPrevilage.Edit);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnNameSetup_Click(object sender, RoutedEventArgs e)
        {
            frmNameSetup frm = new frmNameSetup("HOME");
            userPrevilage = new UserPrevilage(this.btnNameSetup.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnStateSetup_Click(object sender, RoutedEventArgs e)
        {
            frmStateSetup frm = new frmStateSetup("HOME");
            userPrevilage = new UserPrevilage(this.btnStateSetup.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                frm.btnDelete.IsEnabled = Convert.ToBoolean(userPrevilage.Remove);
                frm.bIsEdit = Convert.ToBoolean(userPrevilage.Edit);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnMonthEndClosing_Click(object sender, RoutedEventArgs e)
        {
            frmMonthEndClosing frm = new frmMonthEndClosing();
            this.Close();
            frm.ShowDialog();
        }

        private void btnYearEndClosing_Click(object sender, RoutedEventArgs e)
        {
            //frmYearEnd frm = new frmYearEnd();
            //frmAccountsExpenceReport frm = new frmAccountsExpenceReport();
            frmDataAuditing frm = new frmDataAuditing();
            this.Close();
            frm.ShowDialog();
        }

        private void btnUserRights_Click(object sender, RoutedEventArgs e)
        {
            frmUserRights frm = new frmUserRights(0, "HOME");
            userPrevilage = new UserPrevilage(this.btnUserRights.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnUserAccount_Click(object sender, RoutedEventArgs e)
        {
            frmUserAccounts frm = new frmUserAccounts();
            userPrevilage = new UserPrevilage(this.btnUserAccount.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                frm.btnDelete.IsEnabled = Convert.ToBoolean(userPrevilage.Remove);
                frm.bIsEdit = Convert.ToBoolean(userPrevilage.Edit);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //AlterView.DefaultMemberTotalMonthsDue();
                //AlterView.DefaultMasterMember();
                using (SqlConnection con = new SqlConnection(AppLib.connStr))
                {
                    System.Windows.Forms.Application.DoEvents();
                    SqlCommand cmd = new SqlCommand("SPMONTHENDCLOSING", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    System.Windows.Forms.Application.DoEvents();
                    cmd.Connection.Open();
                    cmd.CommandTimeout = 0;
                    int i = cmd.ExecuteNonQuery();
                    if (i == 0)
                    {
                        MessageBox.Show("Data Not Refreshed Contact Administrator!", "Error");
                    }
                    else
                    {
                        MessageBox.Show("Data Refreshed Sucessfully !", "Sucessfully");
                    }
                    cmd.Connection.Close();                    
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }
    }
}
