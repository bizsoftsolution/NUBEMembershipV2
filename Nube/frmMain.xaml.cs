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
using MahApps.Metro.Controls;

namespace Nube
{
    /// <summary>
    /// Interaction logic for frmMain.xaml
    /// </summary>
    public partial class frmMain : MetroWindow
    {
        UserPrevilage userPrevilage;
        public frmMain()
        {
            InitializeComponent();
            UserRights();
        }

        #region FUNCTIONS

        void UserRights()
        {
            try
            {
                AppLib.sProjectName = btnUser.Tag.ToString();
                userPrevilage = new UserPrevilage(this.btnUser.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnUser.Visibility = Visibility.Visible;
                    ICNUserRight.Visibility = Visibility.Visible;
                }
                else
                {
                    btnUser.Visibility = Visibility.Hidden;
                    ICNUserRight.Visibility = Visibility.Hidden;
                }

                AppLib.sProjectName = btnDataBase.Tag.ToString();
                userPrevilage = new UserPrevilage(this.btnDataBase.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnDataBase.Visibility = Visibility.Visible;
                    ICNBackup.Visibility = Visibility.Visible;
                }
                else
                {
                    btnDataBase.Visibility = Visibility.Hidden;
                    ICNBackup.Visibility = Visibility.Hidden;
                }

                AppLib.sProjectName = btnMember.Tag.ToString();
                userPrevilage = new UserPrevilage(this.btnMember.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnMember.IsEnabled = true;
                }
                else
                {
                    btnMember.IsEnabled = false;
                }

                AppLib.sProjectName = btnAccounts.Tag.ToString();
                userPrevilage = new UserPrevilage(this.btnAccounts.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnAccounts.IsEnabled = true;
                }
                else
                {
                    btnAccounts.IsEnabled = false;
                }
                AppLib.sProjectName = "";

            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion

        #region BUTTON EVENTS

        private void btnAccounts_Click(object sender, RoutedEventArgs e)
        {
            //AppLib.sProjectName = btnAccounts.Tag.ToString();
            //Nube.Accounts.frmFundChange frm = new Nube.Accounts.frmFundChange();
            ////Nube.Accounts.frmHomeAccounts frm = new Nube.Accounts.frmHomeAccounts();                        
            ////NubeHomeAccount frm = new NubeHomeAccount();           
            //userPrevilage = new UserPrevilage(this.btnAccounts.Tag.ToString());
            //if (userPrevilage.Show == true)
            //{
            //    frm.Show();
            //    this.Close();
            //}
            MessageBox.Show("Accounts Under Process!","Sorry");
        }

        private void btnMember_Click(object sender, RoutedEventArgs e)
        {
            AppLib.sProjectName = btnMember.Tag.ToString();
            frmHomeMembership frm = new frmHomeMembership();

            userPrevilage = new UserPrevilage(this.btnMember.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.Show();
                this.Close();
            }
        }

        private void btnInsurance_Click(object sender, RoutedEventArgs e)
        {
            AppLib.sProjectName = btnInsurance.Tag.ToString();
            MessageBox.Show("Insurance Under Process!", "Sorry");
        }

        private void btnUser_Click(object sender, RoutedEventArgs e)
        {
            AppLib.sProjectName = btnUser.Tag.ToString();
            frmUserPrevilage frm = new frmUserPrevilage();
            userPrevilage = new UserPrevilage(this.btnUser.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.Show();
                this.Close();
            }
        }

        private void btnHome_Click_1(object sender, RoutedEventArgs e)
        {
            frmLogin frm = new frmLogin();
            frm.Show();
            this.Close();           
        }

        private void btnDataBase_Click(object sender, RoutedEventArgs e)
        {
            frmBackUpDB frm = new frmBackUpDB();
            frm.Show();
            this.Close();
        }

        #endregion
    }
}
