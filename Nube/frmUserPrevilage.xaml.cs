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

namespace Nube
{
    /// <summary>
    /// Interaction logic for frmUserPrevilage.xaml
    /// </summary>
    public partial class frmUserPrevilage : MetroWindow
    {
        UserPrevilage userPrevilage;
        public frmUserPrevilage()
        {
            InitializeComponent();
            UserRights();
        }
        void UserRights()
        {
            try
            {
                userPrevilage = new UserPrevilage(this.btnUserRights.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnUserRights.IsEnabled = true;
                }
                else
                {
                    btnUserRights.IsEnabled = false;
                }

                userPrevilage = new UserPrevilage(this.btnUserAccount.Tag.ToString());
                if (userPrevilage.Show == true)
                {
                    btnUserAccount.IsEnabled = true;
                }
                else
                {
                    btnUserAccount.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnUserType_Click(object sender, RoutedEventArgs e)
        {
            frmUserType frm = new frmUserType();
            userPrevilage = new UserPrevilage(this.btnUserType.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                frm.btnDelete.IsEnabled = Convert.ToBoolean(userPrevilage.Remove);
                frm.bIsEdit = Convert.ToBoolean(userPrevilage.Edit);
                this.Close();
                frm.ShowDialog();
            }                     
        }

        private void btnUserAccount_Click(object sender, RoutedEventArgs e)
        {
            MasterSetup.frmUserAccounts frm = new MasterSetup.frmUserAccounts();
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

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            frmMain frm = new frmMain();
            this.Close();
            frm.ShowDialog();
        }
    }
}
