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
    /// Interaction logic for frmBranchStatusReport.xaml
    /// </summary>
    public partial class frmBranchStatusReport : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        public frmBranchStatusReport()
        {
            InitializeComponent();
            try
            {
                lblState.Visibility = Visibility.Hidden;
                chkMelaka.Visibility = Visibility.Hidden;
                chkNegeriSembilan.Visibility = Visibility.Hidden;

                var NUBE = db.MASTERNUBEBRANCHes.OrderBy(x => x.NUBE_BRANCH_NAME).ToList();
                cmbNubeBranch.ItemsSource = NUBE;
                cmbNubeBranch.SelectedValuePath = "NUBE_BRANCH_CODE";
                cmbNubeBranch.DisplayMemberPath = "NUBE_BRANCH_NAME";

                var bank = db.MASTERBANKs.OrderBy(x => x.BANK_NAME).ToList();
                cmbBank.ItemsSource = bank;
                cmbBank.SelectedValuePath = "BANK_CODE";
                cmbBank.DisplayMemberPath = "BANK_NAME";
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        #region BUTTON EVENTS

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            GetDetails();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            frmHomeReports frm = new frmHomeReports();
            this.Close();
            frm.ShowDialog();
        }

        private void cmbNubeBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (Convert.ToInt32(cmbNubeBranch.SelectedValue) == 11)
                {
                    lblState.Visibility = Visibility.Visible;
                    chkMelaka.Visibility = Visibility.Visible;
                    chkNegeriSembilan.Visibility = Visibility.Visible;
                }
                else
                {
                    lblState.Visibility = Visibility.Collapsed;
                    chkMelaka.Visibility = Visibility.Collapsed;
                    chkNegeriSembilan.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
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

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cmbBank.Text = "";
            cmbBranch.Text = "";
            cmbNubeBranch.Text = "";
            dtpDOB.Text = "";
            lblState.Visibility = Visibility.Collapsed;
            chkMelaka.Visibility = Visibility.Collapsed;
            chkNegeriSembilan.Visibility = Visibility.Collapsed;
        }


        #endregion

        #region FUNCTIONS

        void GetDetails()
        {
            try
            {
                //string dateMonth = string.Format("{0:MMyyyy}", dtpDOB.SelectedDate.Value);
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(AppLib.connStr))
                {
                    con.Open();
                    string sWhere = "";
                    string Qry = "";

                    if (!string.IsNullOrEmpty(cmbNubeBranch.Text))
                    {
                        sWhere = " AND NUBEBANCHCODE=" + cmbNubeBranch.SelectedValue;
                    }

                    if (!string.IsNullOrEmpty(cmbBank.Text))
                    {
                        sWhere = sWhere + " AND BANK_NAME='" + cmbBank.Text + "'";
                    }

                    if (!string.IsNullOrEmpty(cmbBranch.Text))
                    {
                        sWhere = sWhere + " AND BRANCHNAME='" + cmbBranch.Text + "'";
                    }


                    if (chkMelaka.IsChecked == true && chkNegeriSembilan.IsChecked == false)
                    {
                        sWhere = sWhere + " AND(BRANCHSTATE LIKE '%MELAKA%') ";
                    }
                    else if (chkMelaka.IsChecked == false && chkNegeriSembilan.IsChecked == true)
                    {
                        sWhere = sWhere + " AND(BRANCHSTATE NOT LIKE '%MELAKA%') ";
                    }


                    Qry = string.Format(" SELECT ROW_NUMBER() OVER(ORDER BY BANK_NAME,MEMBER_NAME ASC) AS RNO,MEMBER_ID,ISNULL(MEMBER_NAME,'')MEMBER_NAME, \r" +
                " BANK_USERCODE+'/'+BRANCHUSERCODE BANKBRANCH_NAME,BANK_NAME,CASE WHEN SEX = 'MALE' THEN 'M' ELSE 'F' END SEX, \r" +
                " CASE WHEN ISNULL(ICNO_NEW,'')<>'' THEN ISNULL(ICNO_NEW,'') ELSE ISNULL(ICNO_OLD,'') END ICNO_NEW, \r" +
                " CASE WHEN MEMBERTYPE_NAME='CLERICAL' THEN 'C' ELSE 'N' END MEMBERTYPE_NAME, \r" +
                " DATEOFJOINING,CASE WHEN MEMBERSTATUS='ACTIVE' THEN 'A' ELSE 'D' END STATUS_NAME \r" +
                " FROM TEMPVIEWMASTERMEMBER (NOLOCK) \r" +
                " WHERE MEMBERSTATUSCODE IN (1,2) AND DATEOFJOINING<'2017-05-01' " + sWhere + " \r" +
                " ORDER BY BANK_NAME,MEMBER_NAME");
                    SqlCommand cmd = new SqlCommand(Qry, con);
                    SqlDataAdapter sdp = new SqlDataAdapter(cmd);
                    sdp.SelectCommand.CommandTimeout = 0;
                    sdp.Fill(dt);
                }

                if (dt.Rows.Count > 0)
                {
                    DataTable dtActive = new DataTable();
                    DataTable dtDefaulter = new DataTable();
                    DataView dvAc = new DataView(dt);
                    dvAc.RowFilter = " STATUS_NAME='A' ";
                    dtActive = dvAc.ToTable();
                    DataView dvDF = new DataView(dt);
                    dvDF.RowFilter = " STATUS_NAME='D' ";
                    dtDefaulter = dvDF.ToTable();

                    ReportViewer.Reset();
                    ReportDataSource masterData = new ReportDataSource("STATUSREPORT", dt);
                    ReportViewer.LocalReport.DataSources.Add(masterData);
                    ReportViewer.LocalReport.ReportEmbeddedResource = "Nube.Reports.BranchStatusReport.rdlc";

                    ReportParameter[] NB = new ReportParameter[6];
                    NB[0] = new ReportParameter("DATE", string.Format("{0:MMM-yyyy}", dtpDOB.SelectedDate));
                    NB[1] = new ReportParameter("ACTIVE", dtActive.Rows.Count.ToString());
                    NB[2] = new ReportParameter("DEFAULTER", dtDefaulter.Rows.Count.ToString());
                    NB[3] = new ReportParameter("TOTAL", dt.Rows.Count.ToString());

                    if (!string.IsNullOrEmpty(cmbNubeBranch.Text))
                    {
                        NB[4] = new ReportParameter("NUBEBRANCH", cmbNubeBranch.Text.ToString());
                    }
                    else
                    {
                        NB[4] = new ReportParameter("NUBEBRANCH", "");
                    }

                    if (chkMelaka.IsChecked == true && chkNegeriSembilan.IsChecked == false)
                    {
                        NB[5] = new ReportParameter("STATE", "MELAKA");
                    }
                    else if (chkMelaka.IsChecked == false && chkNegeriSembilan.IsChecked == true)
                    {
                        NB[5] = new ReportParameter("STATE", "NEGERI SEMBILAN");
                    }
                    else
                    {
                        NB[5] = new ReportParameter("STATE", "");
                    }

                    ReportViewer.LocalReport.SetParameters(NB);
                    ReportViewer.RefreshReport();
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





        #endregion


    }
}
