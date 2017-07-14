using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using Microsoft.Reporting.WinForms;

namespace Nube
{
    /// <summary>
    /// Interaction logic for frmStruckOffMemberReport.xaml
    /// </summary>
    public partial class frmStruckOffMemberReport : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        string BankName, BranchName, NubeBranch;
        string qry = "";
        string connStr = AppLib.connStr;

        public frmStruckOffMemberReport()
        {
            InitializeComponent();
            var NUBE = db.MASTERNUBEBRANCHes.OrderBy(x => x.NUBE_BRANCH_NAME).ToList();
            cmbNubeBranch.ItemsSource = NUBE;
            cmbNubeBranch.SelectedValuePath = "NUBE_BRANCH_CODE";
            cmbNubeBranch.DisplayMemberPath = "NUBE_BRANCH_NAME";

            var bank = db.MASTERBANKs.OrderBy(x => x.BANK_NAME).ToList();
            cmbBank.ItemsSource = bank;
            cmbBank.SelectedValuePath = "BANK_CODE";
            cmbBank.DisplayMemberPath = "BANK_NAME";

            this.KeyDown += new System.Windows.Input.KeyEventHandler(Window_KeyDown);
        }

        //Closing events
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                this.Close();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //if (MessageBox.Show(this, "Are you sure to exit?", "Exit Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //{
            //    e.Cancel = false;
            //}
            //else
            //{
            //    e.Cancel = true;
            //}
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            frmHomeReports frm = new frmHomeReports();
            this.Close();
            frm.ShowDialog();
        }

        //Button  events
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cmbNubeBranch.Text = "";
            cmbBank.Text = "";
            cmbBranch.Text = "";
            dtpFromDate.Text = "";
            dtpToDate.Text = "";
        }

        //User defined
        private void LoadReport()
        {
            try
            {
                MemberReport.Reset();
                DataTable dt = getData();
                if (dt.Rows.Count > 0)
                {
                    ReportDataSource masterData = new ReportDataSource("ViewMasterMember", dt);

                    MemberReport.LocalReport.DataSources.Add(masterData);
                    MemberReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.MemberReport.rdlc";
                    ReportParameter RP = new ReportParameter("Title", "STRUCKOFF MEMBER REPORT");
                    MemberReport.LocalReport.SetParameters(RP);

                    MemberReport.RefreshReport();
                    LoadBankReport();
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

        private void LoadBankReport()
        {
            try
            {
                NUBEMemberReport.Reset();
                DataTable dt1 = getNUBEData();
                ReportDataSource Data = new ReportDataSource("ViewMasterMember", dt1);

                NUBEMemberReport.LocalReport.DataSources.Add(Data);
                NUBEMemberReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.NUBEBranchMemberReport.rdlc";
                ReportParameter RP1 = new ReportParameter("Title", "NUBE BRANCH MEMBER REPORT");
                NUBEMemberReport.LocalReport.SetParameters(RP1);

                NUBEMemberReport.RefreshReport();
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }

        }

        private DataTable getData()
        {
            Wqry();
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                int i = ((Convert.ToDateTime(DateTime.Now).Year - Convert.ToDateTime(dtpFromDate.SelectedDate).Year) * 12) + Convert.ToDateTime(DateTime.Now).Month - Convert.ToDateTime(dtpFromDate.SelectedDate).Month;

                AlterView.ViewMemberTotalMonthsDue(i);
                AlterView.ViewMasterMember(i);
                AlterView.ExecuteSPREFRESH();
                AlterView.DefaultMemberTotalMonthsDue();
                AlterView.DefaultMasterMember();

                SqlCommand cmd;
                cmd = new SqlCommand(" SELECT ROW_NUMBER() OVER(ORDER BY MEMBER_NAME ASC) AS RNO,MEMBER_ID,MEMBER_NAME,ISNULL(MEMBERTYPE_NAME,'')MEMBERTYPE_NAME,\r" +
                                                           " CASE WHEN ISNULL(ICNO_NEW, '')<>'' THEN ISNULL(ICNO_NEW,'') ELSE ISNULL(ICNO_OLD,'') END ICNO_NEW,\r" +
                                                           " BANK_USERCODE+'/'+BRANCHUSERCODE BANK_USERCODE,DATEOFJOINING,LEVY,TDF,LASTPAYMENT_DATE,\r" +
                                                           " ISNULL(TOTALMOTHSDUE,0)TOTALMOTHSDUE,ISNULL(SEX,'')SEX,DATEOFJOINING\r" +
                                                           " FROM TEMPVIEWMASTERMEMBER(NOLOCK) WHERE " + qry +
                                                           " ORDER BY MEMBER_NAME", con);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                qry = "";
            }
            return dt;
        }

        private DataTable getNUBEData()
        {
            Wqry();
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd;
                cmd = new SqlCommand("SELECT * FROM TEMPVIEWMASTERMEMBER(NOLOCK)  WHERE " + qry + " ORDER BY MEMBER_NAME", con);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                qry = "";
            }
            return dt;

        }       

        public string Wqry()
        {
            //if (!string.IsNullOrEmpty(dtpFromDate.Text) && !string.IsNullOrEmpty(qry))
            //{
            //    qry = qry + string.Format(" AND MONTH(DATEOFJOINING) BETWEEN MONTH('{0:dd/MMM/yyyy}') AND MONTH('{1:dd/MMM/yyyy}') AND YEAR(DATEOFJOINING) BETWEEN YEAR('{0:dd/MMM/yyyy}') AND YEAR('{1:dd/MMM/yyyy}') ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
            //}
            //else if (!string.IsNullOrEmpty(dtpFromDate.Text) && string.IsNullOrEmpty(qry))
            //{
            //    qry = qry + string.Format(" MONTH(DATEOFJOINING) BETWEEN MONTH('{0:dd/MMM/yyyy}') AND MONTH('{1:dd/MMM/yyyy}') AND YEAR(DATEOFJOINING) BETWEEN YEAR('{0:dd/MMM/yyyy}') AND YEAR('{1:dd/MMM/yyyy}') ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
            //}

            if (!string.IsNullOrEmpty(qry))
            {
                qry = qry + " AND MEMBERSTATUSCODE=3 ";
            }
            else
            {
                qry = qry + " MEMBERSTATUSCODE=3 ";
            }


            if (!string.IsNullOrEmpty(cmbNubeBranch.Text))
            {
                if (!string.IsNullOrEmpty(qry))
                {
                    qry = qry + " AND NUBEBANCHNAME='" + cmbNubeBranch.Text + "' ";
                }
                else
                {
                    qry = qry + " NUBEBANCHNAME='" + cmbNubeBranch.Text + "' ";
                }
            }

            if (!string.IsNullOrEmpty(cmbBank.Text))
            {
                if (!string.IsNullOrEmpty(qry))
                {
                    qry = qry + " AND BANK_NAME='" + cmbBank.Text + "' ";
                }
                else
                {
                    qry = qry + " BANK_NAME='" + cmbBank.Text + "' ";
                }
            }

            if (!string.IsNullOrEmpty(cmbBranch.Text))
            {
                if (!string.IsNullOrEmpty(qry))
                {
                    qry = qry + " AND BRANCHNAME='" + cmbBranch.Text + "' ";
                }
                else
                {
                    qry = qry + " BRANCHNAME='" + cmbBranch.Text + "' ";
                }
            }
            return qry;
        }

        private void cmbBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Decimal d = Convert.ToDecimal(cmbBank.SelectedValue);
                var branch = db.MASTERBANKBRANCHes.Where(x => x.BANK_CODE == d).OrderBy(x => x.BANKBRANCH_NAME).ToList();
                cmbBranch.ItemsSource = branch;
                cmbBranch.SelectedValuePath = "BANKBRANCH_CODE";
                cmbBranch.DisplayMemberPath = "BANKBRANCH_NAME";
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

    }
}


