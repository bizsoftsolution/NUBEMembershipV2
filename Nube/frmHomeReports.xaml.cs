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
using Nube.Reports;

namespace Nube
{
    /// <summary>
    /// Interaction logic for frmHomeReports.xaml
    /// </summary>
    public partial class frmHomeReports : MetroWindow
    {
        UserPrevilage userPrevilage;
        public frmHomeReports()
        {
            InitializeComponent();
            UserRights();
        }

        void UserRights()
        {
            userPrevilage = new UserPrevilage(this.btnActiveMember.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                btnActiveMember.IsEnabled = true;
            }
            else
            {
                btnActiveMember.IsEnabled = false;
            }

            userPrevilage = new UserPrevilage(this.btnBranchAdvice.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                btnBranchAdvice.IsEnabled = true;
            }
            else
            {
                btnBranchAdvice.IsEnabled = false;
            }

            userPrevilage = new UserPrevilage(this.btnDefaultMember.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                btnDefaultMember.IsEnabled = true;
            }
            else
            {
                btnDefaultMember.IsEnabled = false;
            }

            //userPrevilage = new UserPrevilage(this.btnMonthlyStmt.Tag.ToString());
            //if (userPrevilage.Show == true)
            //{
            //    btnMonthlyStmt.IsEnabled = true;
            //}
            //else
            //{
            //    btnMonthlyStmt.IsEnabled = false;
            //}

            userPrevilage = new UserPrevilage(this.btnStruckOff.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                btnStruckOff.IsEnabled = true;
            }
            else
            {
                btnStruckOff.IsEnabled = false;
            }

            userPrevilage = new UserPrevilage(this.btnHalfShareReport.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                btnHalfShareReport.IsEnabled = true;
            }
            else
            {
                btnHalfShareReport.IsEnabled = false;
            }

            userPrevilage = new UserPrevilage(this.btnResign.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                btnResign.IsEnabled = true;
            }
            else
            {
                btnResign.IsEnabled = false;
            }

            userPrevilage = new UserPrevilage(this.btnBankStaticsToBranch.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                btnBankStaticsToBranch.IsEnabled = true;
            }
            else
            {
                btnBankStaticsToBranch.IsEnabled = false;
            }

            userPrevilage = new UserPrevilage(this.btnComparitionReport.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                btnComparitionReport.IsEnabled = true;
            }
            else
            {
                btnComparitionReport.IsEnabled = false;
            }

            userPrevilage = new UserPrevilage(this.btnFeeReport.Tag.ToString());
            if (userPrevilage.Show == true)
            {
                btnFeeReport.IsEnabled = true;
            }
            else
            {
                btnFeeReport.IsEnabled = false;
            }
        }

        private void btnActiveMember_Click(object sender, RoutedEventArgs e)
        {
            frmActiveMemberReport frm = new frmActiveMemberReport("ActiveMemberReport");
            this.Close();
            frm.ShowDialog();
        }

        private void btnBranchAdvice_Click(object sender, RoutedEventArgs e)
        {
            frmBranchAdviceList frm = new frmBranchAdviceList();
            this.Close();
            frm.ShowDialog();
        }

        private void btnDefaultMember_Click(object sender, RoutedEventArgs e)
        {
            frmDefaultMemberReport frm = new frmDefaultMemberReport();
            this.Close();
            frm.ShowDialog();
        }

        //private void btnMonthlyStmt_Click(object sender, RoutedEventArgs e)
        //{
        //    frmMonthlyStatement frm = new frmMonthlyStatement();
        //    this.Close();
        //    frm.ShowDialog();
        //}

        private void btnStruckOff_Click(object sender, RoutedEventArgs e)
        {
            frmStruckOffMemberReport frm = new frmStruckOffMemberReport();
            this.Close();
            frm.ShowDialog();
        }

        private void btnHalfShareReport_Click(object sender, RoutedEventArgs e)
        {
            frmFinancialHalfShareReport frm = new frmFinancialHalfShareReport();
            this.Close();
            frm.ShowDialog();
        }

        private void btnResign_Click(object sender, RoutedEventArgs e)
        {
            frmResignedMemberReport frm = new frmResignedMemberReport();
            this.Close();
            frm.ShowDialog();
        }

        private void btnBankStaticsToBranch_Click(object sender, RoutedEventArgs e)
        {
            frmBranchStatisticsReport frm = new frmBranchStatisticsReport();
            this.Close();
            frm.ShowDialog();
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            frmHomeMembership frm = new frmHomeMembership();
            this.Close();
            frm.ShowDialog();
        }

        private void btnComparitionReport_Click(object sender, RoutedEventArgs e)
        {
            frmComparitionReport frm = new frmComparitionReport();
            this.Close();
            frm.ShowDialog();
        }

        private void btnFeeReport_Click(object sender, RoutedEventArgs e)
        {
            frmFeeReport frm = new frmFeeReport();
            this.Close();
            frm.ShowDialog();
        }

        private void btnAnnualStatement_Click(object sender, RoutedEventArgs e)
        {
            frmAnnualStatement frm = new frmAnnualStatement();
            this.Close();
            frm.ShowDialog();
        }

        private void btnNewMemberReport_Click(object sender, RoutedEventArgs e)
        {
            frmActiveMemberReport frm = new frmActiveMemberReport("NewMemberReport");
            this.Close();
            frm.ShowDialog();
        }

        private void btnBranchStatusReport_Click(object sender, RoutedEventArgs e)
        {
            frmBranchStatusReport frm = new frmBranchStatusReport();
            this.Close();
            frm.ShowDialog();
        }
    }
}
