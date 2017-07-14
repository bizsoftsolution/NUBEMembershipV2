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
    /// Interaction logic for frmResignedMemberReport.xaml
    /// </summary>
    public partial class frmResignedMemberReport : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        string BankName, BranchName, NubeBranch;
        string qry = "";
        string connStr = AppLib.connStr;

        public frmResignedMemberReport()
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

            var resn = db.MASTERRESIGNSTATUS.OrderBy(x => x.RESIGNSTATUS_NAME).ToList();
            cmbReasonBranch.ItemsSource = resn;
            cmbReasonBranch.SelectedValuePath = "RESIGNSTATUS_CODE";
            cmbReasonBranch.DisplayMemberPath = "RESIGNSTATUS_NAME";

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

        //Button  events
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (chkPrint.IsChecked == true)
            {
                if (string.IsNullOrEmpty(txtMemberNoFrom.Text) && string.IsNullOrEmpty(txtMemberNoTo.Text))
                {
                    MessageBox.Show("Membership No Empty!");
                    txtMemberNoFrom.Focus();
                }
                else
                {
                    DataTable dt = getData();
                    if (dt.Rows.Count > 0)
                    {
                        frmResignReport frm = new frmResignReport(Convert.ToDecimal(dt.Rows[0]["MEMBER_CODE"]), "RTEST");
                        frm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("No Records Found!");
                    }

                }
            }
            else
            {
                LoadReport();
            }

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cmbNubeBranch.Text = "";
            cmbBank.Text = "";
            cmbBranch.Text = "";
            dtpFromDate.Text = "";
            dtpToDate.Text = "";
            cmbReasonBranch.Text = "";
            txtMemberNoFrom.Text = "";
            txtMemberNoTo.Text = "";
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            frmHomeReports frm = new frmHomeReports();
            this.Close();
            frm.ShowDialog();
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
                    ReportDataSource masterData = new ReportDataSource("Resign", dt);

                    MemberReport.LocalReport.DataSources.Add(masterData);
                    MemberReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.rptResign.rdlc";
                    ReportParameter RP = new ReportParameter("TotalMember", dt.Rows.Count.ToString());
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
                SqlCommand cmd;
                //if (qry != "")
                //{
                //    qry = qry + "and RESIGNED='1'";
                //}
                //else
                //{
                //    qry = "RESIGNED='1'";
                //}
                if (!string.IsNullOrEmpty(dtpFromDate.Text))
                {
                    if (!string.IsNullOrEmpty(qry))
                    {
                        qry = qry + string.Format(" AND MONTH(VOUCHER_DATE) BETWEEN MONTH('{0:dd/MMM/yyyy}') AND MONTH('{1:dd/MMM/yyyy}') AND YEAR(VOUCHER_DATE) BETWEEN YEAR('{0:dd/MMM/yyyy}') AND YEAR('{1:dd/MMM/yyyy}') ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                    }
                    else
                    {
                        qry = qry + string.Format(" MONTH(VOUCHER_DATE) BETWEEN MONTH('{0:dd/MMM/yyyy}') AND MONTH('{1:dd/MMM/yyyy}') AND YEAR(VOUCHER_DATE) BETWEEN YEAR('{0:dd/MMM/yyyy}') AND YEAR('{1:dd/MMM/yyyy}') ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                    }
                }
                if (!string.IsNullOrEmpty(cmbReasonBranch.Text))
                {
                    if (!string.IsNullOrEmpty(qry))
                    {
                        qry = qry + string.Format(" AND RESIGNSTATUS_CODE=" + cmbReasonBranch.SelectedValue);
                    }
                    else
                    {
                        qry = qry + string.Format(" RESIGNSTATUS_CODE=" + cmbReasonBranch.SelectedValue);
                    }
                }

                if (!string.IsNullOrEmpty(txtMemberNoFrom.Text) && !string.IsNullOrEmpty(txtMemberNoTo.Text))
                {
                    if (!string.IsNullOrEmpty(qry))
                    {
                        qry = qry + string.Format(" AND MEMBER_ID BETWEEN {0} AND {1} ", txtMemberNoFrom.Text, txtMemberNoTo.Text);
                    }
                    else
                    {
                        qry = qry + string.Format(" MEMBER_ID BETWEEN {0} AND {1} ", txtMemberNoFrom.Text, txtMemberNoTo.Text);
                    }

                }
                else if (!string.IsNullOrEmpty(txtMemberNoFrom.Text))
                {
                    if (!string.IsNullOrEmpty(qry))
                    {
                        qry = qry + string.Format(" AND MEMBER_ID ={0} ", txtMemberNoFrom.Text);
                    }
                    else
                    {
                        qry = qry + string.Format(" MEMBER_ID ={0} ", txtMemberNoFrom.Text);
                    }
                }

                else if (!string.IsNullOrEmpty(txtMemberNoTo.Text))
                {
                    if (!string.IsNullOrEmpty(qry))
                    {
                        qry = qry + string.Format(" AND MEMBER_ID ={0} ", txtMemberNoTo.Text);
                    }
                    else
                    {
                        qry = qry + string.Format(" MEMBER_ID ={0} ", txtMemberNoTo.Text);
                    }
                }

                cmd = new SqlCommand("SELECT * FROM VIEWRESIGNREPORT(NOLOCK) WHERE " + qry + " order by MEMBER_NAME", con);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.SelectCommand.CommandTimeout = 0;
                adp.Fill(dt);
                int i = 0;
                foreach (DataRow row in dt.Rows)
                {
                    row["RNO"] = i + 1;
                    i++;
                }
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
                if (qry != "")
                {
                    qry = qry + "and RESIGNED='1'";
                }
                else
                {
                    qry = "RESIGNED='1'";
                }

                if (!string.IsNullOrEmpty(dtpFromDate.Text))
                {
                    qry = qry + string.Format(" AND MONTH(VOUCHER_DATE) BETWEEN MONTH('{0:dd/MMM/yyyy}') AND MONTH('{1:dd/MMM/yyyy}') AND YEAR(VOUCHER_DATE) BETWEEN YEAR('{0:dd/MMM/yyyy}') AND YEAR('{1:dd/MMM/yyyy}') ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                }

                cmd = new SqlCommand("SELECT * FROM TEMPVIEWMASTERMEMBER(NOLOCK)  where " + qry + " ORDER BY MEMBER_NAME", con);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.SelectCommand.CommandTimeout = 0;
                adp.Fill(dt);
                qry = "";
            }
            return dt;

        }

        public string Wqry()
        {
            qry = "";
            if (cmbNubeBranch.Text != "")
            {
                NubeBranch = cmbNubeBranch.Text;
                if (qry != "")
                {

                    qry = qry + " AND NUBEBANCHNAME='" + NubeBranch + "'";
                }
                else
                {
                    qry = " NUBEBANCHNAME='" + NubeBranch + "'";
                }
            }


            if (cmbBank.Text != "")
            {
                BankName = cmbBank.Text;
                if (qry != "")
                {
                    qry = qry + " AND BANK_NAME='" + BankName + "'";
                }
                else
                {
                    qry = " BANK_NAME='" + BankName + "'";
                }

            }

            if (cmbBranch.Text != "")
            {
                BranchName = cmbBranch.Text;
                if (qry != "")
                {
                    qry = qry + " AND BRANCHNAME='" + BranchName + "'";
                }
                else
                {
                    qry = " BRANCHNAME='" + BranchName + "'";
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

