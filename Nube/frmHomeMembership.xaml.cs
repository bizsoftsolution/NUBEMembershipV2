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
using Nube.Transaction;
using System.Data;
using System.Data.SqlClient;

namespace Nube
{
    /// <summary>
    /// Interaction logic for frmHomeMembership.xaml
    /// </summary>
    public partial class frmHomeMembership : MetroWindow
    {
        UserPrevilage userPrevilage;
        public frmHomeMembership()
        {
            InitializeComponent();
            UserRights();
        }

        #region FUNCTIONS 

        void UserRights()
        {
            userPrevilage = new UserPrevilage(this.btnMemberRegistration.Tag.ToString());

            if (userPrevilage.Show == true)
            {
                btnMemberRegistration.IsEnabled = true;
            }
            else
            {
                btnMemberRegistration.IsEnabled = false;
            }

            userPrevilage = new UserPrevilage(this.btnFeeEntry.Tag.ToString());

            if (userPrevilage.Show == true)
            {
                btnFeeEntry.IsEnabled = true;
            }
            else
            {
                btnFeeEntry.IsEnabled = false;
            }

            userPrevilage = new UserPrevilage(this.btnResingation.Tag.ToString());

            if (userPrevilage.Show == true)
            {
                btnResingation.IsEnabled = true;
            }
            else
            {
                btnResingation.IsEnabled = false;
            }

            userPrevilage = new UserPrevilage(this.btnMemberQuery.Tag.ToString());

            if (userPrevilage.Show == true)
            {
                btnMemberQuery.IsEnabled = true;
            }
            else
            {
                btnMemberQuery.IsEnabled = false;
            }

            userPrevilage = new UserPrevilage(this.btnPreApr16.Tag.ToString());

            if (userPrevilage.Show == true)
            {
                btnPreApr16.IsEnabled = true;
            }
            else
            {
                btnPreApr16.IsEnabled = false;
            }

            userPrevilage = new UserPrevilage(this.btnPostApr16.Tag.ToString());

            if (userPrevilage.Show == true)
            {
                btnPostApr16.IsEnabled = true;
            }
            else
            {
                btnPostApr16.IsEnabled = false;
            }

            userPrevilage = new UserPrevilage(this.btnLevy.Tag.ToString());

            if (userPrevilage.Show == true)
            {
                btnLevy.IsEnabled = true;
            }
            else
            {
                btnLevy.IsEnabled = false;
            }

            userPrevilage = new UserPrevilage(this.btnTDF.Tag.ToString());

            if (userPrevilage.Show == true)
            {
                btnTDF.IsEnabled = true;
            }
            else
            {
                btnTDF.IsEnabled = false;
            }

            userPrevilage = new UserPrevilage(this.btnTransfer.Tag.ToString());

            if (userPrevilage.Show == true)
            {
                btnTransfer.IsEnabled = true;
            }
            else
            {
                btnTransfer.IsEnabled = false;
            }

        }

        #endregion

        #region BUTTON EVENTS     

        private void btnMaster_Click(object sender, RoutedEventArgs e)
        {
            frmHomeMaster frm = new frmHomeMaster();
            this.Close();
            frm.ShowDialog();
        }

        private void btnMemberRegistration_Click(object sender, RoutedEventArgs e)
        {
            frmMemberRegistration frm = new frmMemberRegistration();
            userPrevilage = new UserPrevilage(this.btnMemberRegistration.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                frm.btnDelete.IsEnabled = Convert.ToBoolean(userPrevilage.Remove);
                frm.btnSearch.IsEnabled = Convert.ToBoolean(userPrevilage.Edit);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnFeeEntry_Click(object sender, RoutedEventArgs e)
        {
            frmFeesEntry frm = new frmFeesEntry();
            userPrevilage = new UserPrevilage(this.btnFeeEntry.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnResingation_Click(object sender, RoutedEventArgs e)
        {
            frmResingation frm = new frmResingation();
            userPrevilage = new UserPrevilage(this.btnResingation.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnMemberQuery_Click(object sender, RoutedEventArgs e)
        {
            frmMemberQuery frm = new frmMemberQuery("HomeMember");
            this.Close();
            frm.ShowDialog();            
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            frmHomeReports frm = new frmHomeReports();
            this.Close();
            frm.ShowDialog();
        }

        private void btnTransfer_Click(object sender, RoutedEventArgs e)
        {
            frmTransfer frm = new frmTransfer();
            userPrevilage = new UserPrevilage(this.btnTransfer.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnPreApr16_Click(object sender, RoutedEventArgs e)
        {
            frmArrearPre16 frm = new frmArrearPre16();
            userPrevilage = new UserPrevilage(this.btnPreApr16.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                frm.btnEditOldDue.IsEnabled = Convert.ToBoolean(userPrevilage.Edit);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnPostApr16_Click(object sender, RoutedEventArgs e)
        {
            frmArrearPost16 frm = new frmArrearPost16();
            userPrevilage = new UserPrevilage(this.btnPostApr16.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                frm.btnEditOldDue.IsEnabled = Convert.ToBoolean(userPrevilage.Edit);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnLevy_Click(object sender, RoutedEventArgs e)
        {
            frmLevy frm = new frmLevy();
            userPrevilage = new UserPrevilage(this.btnLevy.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                frm.btnSave.IsEnabled = Convert.ToBoolean(userPrevilage.AddNew);
                this.Close();
                frm.ShowDialog();
            }
        }

        private void btnFeeCheck_Click(object sender, RoutedEventArgs e)
        {
            frmFeesEntryTest frm = new frmFeesEntryTest();
            this.Close();
            frm.ShowDialog();
        }

        private void btnTDF_Click(object sender, RoutedEventArgs e)
        {
            frmTDF frm = new frmTDF();
            userPrevilage = new UserPrevilage(this.btnTDF.Tag.ToString());
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


        #endregion


    }
}
