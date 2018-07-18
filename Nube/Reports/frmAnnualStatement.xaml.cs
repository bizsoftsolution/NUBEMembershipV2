using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;
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

namespace Nube.Reports
{
    /// <summary>
    /// Interaction logic for frmAnnualStatement.xaml
    /// </summary>
    public partial class frmAnnualStatement : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        public frmAnnualStatement(decimal dMemberCode = 0)
        {
            InitializeComponent();
            try
            {
                if (dMemberCode != 0)
                {
                    var mm = (from x in db.MASTERMEMBERs where x.MEMBER_CODE == dMemberCode orderby x.DATEOFJOINING descending select x).FirstOrDefault();
                    if (mm.MEMBER_ID != null)
                    {
                        txtMemberNo.Text = mm.MEMBER_ID.ToString();
                    }
                }

                var NUBE = db.MASTERNUBEBRANCHes.OrderBy(x => x.NUBE_BRANCH_NAME).ToList();
                cmbNubeBranch.ItemsSource = NUBE;
                cmbNubeBranch.SelectedValuePath = "NUBE_BRANCH_CODE";
                cmbNubeBranch.DisplayMemberPath = "NUBE_BRANCH_NAME";

                var bank = db.MASTERBANKs.OrderBy(x => x.BANK_NAME).ToList();
                cmbBank.ItemsSource = bank;
                cmbBank.SelectedValuePath = "BANK_CODE";
                cmbBank.DisplayMemberPath = "BANK_NAME";

                dtpDOB.SelectedDate = Convert.ToDateTime("31/MAR/2017");
                dtpDOB.IsEnabled = false;
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #region BUTTON EVENTS

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(dtpDOB.Text))
                {
                    MessageBox.Show("Date is Empty!");
                    dtpDOB.Focus();
                    return;
                }
                else if (chkSummary.IsChecked == true)
                {
                    ReportSummary();
                }                              
                else
                {
                    ReportMember();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cmbNubeBranch.Text = "";
                cmbBank.Text = "";
                cmbBranch.Text = "";
                txtMemberNo.Text = "";
                ReportViewer.Clear();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            frmHomeReports frm = new frmHomeReports();
            this.Close();
            frm.ShowDialog();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            Transaction.frmMemberQuery frm = new Transaction.frmMemberQuery("Annual Statement");
            this.Close();
            frm.ShowDialog();
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
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion

        #region FUNCTIONS

        void ReportMember()
        {
            DataTable dt = GetData();
            if (dt.Rows.Count > 0)
            {               
                ReportViewer.Reset();
                ReportDataSource masterData = new ReportDataSource("AnnualStatement", dt);
                ReportViewer.LocalReport.ReportEmbeddedResource = "Nube.Reports.AnnualStatememnt.rdlc";              
                ReportViewer.LocalReport.DataSources.Add(masterData);               
                ReportViewer.RefreshReport();                   
            }
            else
            {
                MessageBox.Show("No Records Found!");
            }
        }

        void ReportSummary()
        {
            DataTable dt = GetData();

            if (dt.Rows.Count > 0)
            {
                ReportViewer.Reset();
                ReportDataSource masterData = new ReportDataSource("AnnualStatementSummary", dt);
                ReportViewer.LocalReport.DataSources.Add(masterData);
                ReportViewer.LocalReport.ReportEmbeddedResource = "Nube.Reports.AnnualStatementSummary.rdlc";

                ReportParameter[] NB = new ReportParameter[3];
                if (!string.IsNullOrEmpty(cmbNubeBranch.Text))
                {
                    NB[0] = new ReportParameter("NubeBranchName", cmbNubeBranch.Text.ToString());
                }
                else
                {
                    NB[0] = new ReportParameter("NubeBranchName", "");
                }

                if (!string.IsNullOrEmpty(cmbBank.Text))
                {
                    NB[1] = new ReportParameter("BankName", cmbBank.Text.ToString());
                }
                else
                {
                    NB[1] = new ReportParameter("BankName", "");
                }

                if (!string.IsNullOrEmpty(cmbBranch.Text))
                {
                    NB[2] = new ReportParameter("BranchName", cmbBranch.Text.ToString());
                }
                else
                {
                    NB[2] = new ReportParameter("BranchName", "");
                }
                ReportViewer.LocalReport.SetParameters(NB);
                ReportViewer.RefreshReport();
            }
            else
            {
                MessageBox.Show("No Records Found!");
            }
        }

        private DataTable GetData()
        {
            DataTable dt = new DataTable();
            DataTable dtTemp = new DataTable();

            if (AppLib.dtAnnualStatement.Rows.Count > 0)
            {
                dtTemp = AppLib.dtAnnualStatement.Copy();
            }
            else
            {
                using (SqlConnection conn = new SqlConnection(AppLib.connStr))
                {
                    SqlCommand cmd;
                    cmd = new SqlCommand("SPANNUALSTATEMENT", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Add(new SqlParameter("@MEMBER_CODE", dMember_code));
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.SelectCommand.CommandTimeout = 0;
                    adp.Fill(dtTemp);
                }
                AppLib.dtAnnualStatement = dtTemp.Copy();
            }

            DataView dv = new DataView(dtTemp);
            string sWhere = "";

            if (!string.IsNullOrEmpty(txtMemberNo.Text))
            {
                sWhere = " MEMBER_ID='" + txtMemberNo.Text.ToString() + "'";
            }

            if (!string.IsNullOrEmpty(cmbNubeBranch.Text))
            {
                if (sWhere != "")
                {
                    sWhere = sWhere + " AND NUBE='" + cmbNubeBranch.Text + "'";
                }
                else
                {
                    sWhere = " NUBE='" + cmbNubeBranch.Text + "'";
                }
            }

            if (!string.IsNullOrEmpty(cmbBank.Text))
            {
                if (sWhere != "")
                {
                    sWhere = sWhere + " AND BANK_NAME='" + cmbBank.Text + "'";
                }
                else
                {
                    sWhere = " BANK_NAME='" + cmbBank.Text + "'";
                }
            }

            if (!string.IsNullOrEmpty(cmbBranch.Text))
            {
                if (sWhere != "")
                {
                    sWhere = sWhere + " AND BRANCHNAME='" + cmbBranch.Text + "'";
                }
                else
                {
                    sWhere = " BRANCHNAME='" + cmbBranch.Text + "'";
                }
            }

            if (rbActive.IsChecked==true)
            {
                if (sWhere != "")
                {
                    sWhere = sWhere + " AND MEMBERSTATUSCODE=1";
                }
                else
                {
                    sWhere = " MEMBERSTATUSCODE=1";
                }
            }
            else if (rbDefaulter.IsChecked == true)
            {
                if (sWhere != "")
                {
                    sWhere = sWhere + " AND MEMBERSTATUSCODE=2";
                }
                else
                {
                    sWhere = " MEMBERSTATUSCODE=2";
                }
            }


            if (!string.IsNullOrEmpty(sWhere))
            {
                dv.RowFilter = sWhere;
            }
            dt = dv.ToTable();
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                row["RNO"] = i + 1;
                i++;
            }
            return dt;
        }

        #endregion

    }
}
