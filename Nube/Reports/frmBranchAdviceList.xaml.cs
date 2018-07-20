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
    /// Interaction logic for frmBranchAdviceList.xaml
    /// </summary>
    public partial class frmBranchAdviceList : MetroWindow
    {
        string connStr = AppLib.connStr;
        nubebfsEntity dbBFS = new nubebfsEntity();
        //nubestatusEntities dbStatis = new nubestatusEntities();
        string mon = "";
        string qry = "";
        public frmBranchAdviceList()
        {
            InitializeComponent();
            var branch = dbBFS.MASTERNUBEBRANCHes.ToList();
            cmbBranch.ItemsSource = branch.ToList();
            cmbBranch.SelectedValuePath = "NUBE_BRANCH_USERCODE";
            cmbBranch.DisplayMemberPath = "NUBE_BRANCH_NAME";
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrEmpty(cmbBranch.Text))
            //{
            //    MessageBox.Show("Branch Name is Empty!");
            //    cmbBranch.Focus();
            //}
            //else
            if (dtpDateFrom.SelectedDate == null || dtpDateTo.SelectedDate == null)
            {
                MessageBox.Show("Select Date Range");
                dtpDateFrom.Focus();
            }
            else
            {
                try
                {
                    NewMemberReport.Reset();
                    DataTable dt = getData();
                    if (dt.Rows.Count > 0)
                    {
                        ReportDataSource masterData = new ReportDataSource("BranchAdviceList", dt);
                        NewMemberReport.LocalReport.DataSources.Add(masterData);
                        NewMemberReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.NUBEBranchAdviceList.rdlc";
                        ReportParameter[] rp = new ReportParameter[3];
                        if (!string.IsNullOrEmpty(cmbBranch.Text))
                        {
                            rp[0] = new ReportParameter("BranchName","Branch Name : " +cmbBranch.Text);
                        }
                        else
                        {
                            rp[0] = new ReportParameter("BranchName", "");
                        }
                        rp[1] = new ReportParameter("Month", String.Format("{0:dd MMM yyyy} - {1:dd MMM yyyy}", dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value));
                        if (!string.IsNullOrEmpty(cmbBranch.Text))
                        {
                            rp[2] = new ReportParameter("BranchCode", cmbBranch.SelectedValue.ToString());
                        }
                        else
                        {
                            rp[2] = new ReportParameter("BranchCode", "");
                        }

                        NewMemberReport.LocalReport.SetParameters(rp);
                        NewMemberReport.RefreshReport();
                    }
                    else
                    {
                        MessageBox.Show("New Member's Not Found !");
                    }
                    LoadResignMember();
                }
                catch (Exception ex)
                {
                    ExceptionLogging.SendErrorToText(ex);
                }
            }
        }

        private void LoadResignMember()
        {
            ResignMemberReport.Reset();
            DataTable dt = getResignData();
            if (dt.Rows.Count > 0)
            {
                ReportDataSource masterData = new ReportDataSource("BranchAdviceList", dt);

                ResignMemberReport.LocalReport.DataSources.Add(masterData);
                ResignMemberReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.NUBEBranchAdviceList.rdlc";
                ReportParameter[] rp = new ReportParameter[3];
                if (!string.IsNullOrEmpty(cmbBranch.Text))
                {
                    rp[0] = new ReportParameter("BranchName", "Branch Name : " +cmbBranch.Text);
                }
                else
                {
                    rp[0] = new ReportParameter("BranchName", "");
                }
                rp[1] = new ReportParameter("Month", String.Format("{0:dd MMM yyyy} - {1:dd MMM yyyy}", dtpDateFrom.SelectedDate.Value, dtpDateTo.SelectedDate.Value));
                if (!string.IsNullOrEmpty(cmbBranch.Text))
                {
                    rp[2] = new ReportParameter("BranchCode", cmbBranch.SelectedValue.ToString());
                }
                else
                {
                    rp[2] = new ReportParameter("BranchCode", "");
                }
                ResignMemberReport.LocalReport.SetParameters(rp);

                ResignMemberReport.RefreshReport();
            }
            else
            {
                MessageBox.Show("Resigned Member's Not Found !");
            }
        }

        private DataTable getResignData()
        {
            Wqry();
            if (!string.IsNullOrEmpty(qry))
            {
                qry = qry + " AND RESIGNED=1";
            }
            else
            {
                qry = qry + " RESIGNED=1";
            }

            if (rbtPaymentDate.IsChecked == true)
            {
                if (!string.IsNullOrEmpty(qry))
                {
                    qry = qry + string.Format(" AND MONTH(VOUCHER_DATE)=MONTH('{0:dd/MMM/yyyy}') AND YEAR(VOUCHER_DATE)=YEAR('{0:dd/MMM/yyyy}') ", dtpDateFrom.SelectedDate);
                }
                else
                {
                    qry = qry + string.Format(" MONTH(VOUCHER_DATE)=MONTH('{0:dd/MMM/yyyy}') AND YEAR(VOUCHER_DATE)=YEAR('{0:dd/MMM/yyyy}') ", dtpDateFrom.SelectedDate);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(qry))
                {
                    qry += " and ";
                }
                qry += string.Format("RESIGNATION_DATE>='{0:dd/MMM/yyyy}' AND RESIGNATION_DATE<='{1:dd/MMM/yyyy}'", dtpDateFrom.SelectedDate, dtpDateTo.SelectedDate);
            }

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connStr))
            {

                SqlCommand cmd = new SqlCommand(" SELECT ROW_NUMBER() OVER(ORDER BY MEMBER_NAME ASC) AS RNO,MEMBER_NAME,BANKUSER_CODE BANK_USERCODE,MEMBER_ID, \r" +
                                               " CASE WHEN ISNULL(ICNO_NEW, '')<>'' THEN ISNULL(ICNO_NEW,'') ELSE ISNULL(ICNO_OLD,'') END ICNO_NEW, Branch_Name  As BRANCHNAME, \r" +
                                               " DATEOFJOINING,CASE WHEN ISNULL(MEMBERTYPE_NAME,'')='CLERICAL' THEN 'Y' ELSE 'N' END MEMBERTYPE_NAME,\r" +
                                               " ENTRANCEFEE,MONTHLYSUBSCRIPTION,MONTHLYBF,HQFEE,CASE WHEN ISNULL(REJOINED,0)=0 THEN 'N' ELSE 'R' END REJOINED\r" +
                                               " FROM MEMBERSTATUSLOG(NOLOCK) WHERE " + qry +
                                               "  ORDER BY Branch_Name, MEMBER_NAME", con);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                adp.Fill(dt);

            }
            return dt;
        }

        private DataTable getData()
        {
            Wqry();
            if (!string.IsNullOrEmpty(dtpDateFrom.Text))
            {
                if (!string.IsNullOrEmpty(qry))
                {
                    qry += " and ";                    
                }
                qry = qry + string.Format("DATEOFJOINING>='{0:dd/MMM/yyyy}' AND DATEOFJOINING<='{1:dd/MMM/yyyy}'", dtpDateFrom.SelectedDate,dtpDateTo.SelectedDate);
            }

            if (!string.IsNullOrEmpty(qry))
            {
                qry = qry + " AND ISCANCEL=0 ";
            }
            else
            {
                qry = qry + " ISCANCEL=0 ";
            }

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(" SELECT ROW_NUMBER() OVER(ORDER BY MEMBER_NAME ASC) AS RNO,MEMBER_NAME,BANKUSER_CODE BANK_USERCODE,MEMBER_ID, \r" +
                                                " CASE WHEN ISNULL(ICNO_NEW, '')<>'' THEN ISNULL(ICNO_NEW,'') ELSE ISNULL(ICNO_OLD,'') END ICNO_NEW, Branch_Name  As BRANCHNAME, \r" +
                                                " DATEOFJOINING,CASE WHEN ISNULL(MEMBERTYPE_NAME,'')='CLERICAL' THEN 'Y' ELSE 'N' END MEMBERTYPE_NAME,\r" +
                                                " ENTRANCEFEE,MONTHLYSUBSCRIPTION,MONTHLYBF,HQFEE,CASE WHEN ISNULL(REJOINED,0)=0 THEN 'N' ELSE 'R' END REJOINED\r" +
                                                " FROM MEMBERSTATUSLOG(NOLOCK) WHERE " + qry +
                                                " ORDER BY Branch_Name,MEMBER_NAME", con);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                adp.Fill(dt);

            }
            return dt;
        }

        public string Wqry()
        {
            qry = "";
            if (!string.IsNullOrEmpty(cmbBranch.Text))
            {
                if (!string.IsNullOrEmpty(qry))
                {
                    qry = qry + " AND NUBEBRANCH_NAME='" + cmbBranch.Text + "' ";
                }
                else
                {
                    qry = qry + " NUBEBRANCH_NAME='" + cmbBranch.Text + "' ";
                }
            }

            return qry;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            frmHomeReports frm = new frmHomeReports();
            this.Close();
            frm.ShowDialog();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cmbBranch.Text = "";
            dtpDateFrom.Text ="";
            dtpDateTo.Text = "";
            NewMemberReport.Clear();
            ResignMemberReport.Clear();
        }

    }
}
