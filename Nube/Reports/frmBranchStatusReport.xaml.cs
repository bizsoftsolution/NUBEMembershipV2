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
            ReportViewer.Clear();
        }


        #endregion

        #region FUNCTIONS

        void GetDetails()
        {
            if (string.IsNullOrEmpty(dtpDOB.Text))
            {
                MessageBox.Show("Date is empty", "Empty");
                dtpDOB.Focus();

            }
            else
            {
                try
                {
                    //if (dtpDOB.SelectedDate > Convert.ToDateTime("28/FEB/2018").Date)
                    //{
                        //string dateMonth = string.Format("{0:MMyyyy}", dtpDOB.SelectedDate.Value);
                        DataTable dt = new DataTable();
                        using (SqlConnection con = new SqlConnection(AppLib.connStr))
                        {
                            con.Open();
                            string sWhere = "";
                            string Qry = "";

                            DateTime dtfirstDayOfNextMonth = new DateTime(Convert.ToDateTime(dtpDOB.SelectedDate).Year, Convert.ToDateTime(dtpDOB.SelectedDate).Month, 1).AddMonths(1);

                            if (!string.IsNullOrEmpty(cmbNubeBranch.Text))
                            {
                                sWhere = " AND BB.NUBE_BRANCH_CODE=" + cmbNubeBranch.SelectedValue;
                            }

                            if (!string.IsNullOrEmpty(cmbBank.Text))
                            {
                                sWhere = sWhere + " AND MB.BANK_NAME='" + cmbBank.Text + "'";
                            }

                            if (!string.IsNullOrEmpty(cmbBranch.Text))
                            {
                                sWhere = sWhere + " AND BB.BANKBRANCH_NAME='" + cmbBranch.Text + "'";
                            }


                            if (chkMelaka.IsChecked == true && chkNegeriSembilan.IsChecked == false)
                            {
                                sWhere = sWhere + " AND (MS.STATE_NAME LIKE '%MELAKA%') ";
                            }
                            else if (chkMelaka.IsChecked == false && chkNegeriSembilan.IsChecked == true)
                            {
                                sWhere = sWhere + " AND (MS.STATE_NAME NOT LIKE '%MELAKA%') ";
                            }
                        if ((Convert.ToDateTime(dtpDOB.SelectedDate).Year >= 2016 && Convert.ToDateTime(dtpDOB.SelectedDate).Month > 3) || Convert.ToDateTime(dtpDOB.SelectedDate).Year > 2016)
                        {
                            Qry = string.Format(" SELECT ROW_NUMBER() OVER(ORDER BY MB.BANK_NAME,MM.MEMBER_NAME ASC) AS RNO,MM.MEMBER_ID,ISNULL(MM.MEMBER_NAME,'')MEMBER_NAME,  \r" +
                                                " MB.BANK_USERCODE+'/'+BB.BANKBRANCH_USERCODE BANKBRANCH_NAME,MB.BANK_NAME,CASE WHEN MM.SEX = 'MALE' THEN 'M' ELSE 'F' END SEX, \r" +
                                                " CASE WHEN ISNULL(MM.ICNO_NEW, '') <> '' THEN ISNULL(MM.ICNO_NEW, '') ELSE ISNULL(ICNO_OLD, '') END ICNO_NEW,\r" +
                                                " CASE WHEN MM.MEMBERTYPE_CODE = 1 THEN 'C' ELSE 'N' END MEMBERTYPE_NAME,\r" +
                                                " MM.DATEOFJOINING, CASE WHEN ST.STATUS_CODE = 1 THEN 'A' ELSE 'D' END STATUS_NAME\r" +
                                                " FROM NUBESTATUS..STATUS{0:MMyyyy} ST(NOLOCK)\r" +
                                                " LEFT JOIN MASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE = ST.MEMBER_CODE \r" +
                                                " LEFT JOIN MASTERBANK MB(NOLOCK) ON MB.BANK_CODE = MM.BANK_CODE \r" +
                                                " LEFT JOIN MASTERBANKBRANCH BB(NOLOCK) ON BB.BANKBRANCH_CODE = MM.BRANCH_CODE \r" +
                                                " LEFT JOIN MASTERNUBEBRANCH NB(NOLOCK) ON NB.NUBE_BRANCH_CODE = BB.NUBE_BRANCH_CODE \r" +
                                                " LEFT JOIN MASTERSTATE MS(NOLOCK) ON MS.STATE_CODE = BB.BANKBRANCH_STATE_CODE   \r" +
                                                " WHERE ST.STATUS_CODE IN(1,2) " + sWhere, dtpDOB.SelectedDate);
                        }
                        else
                        {
                            Qry = string.Format(" SELECT ROW_NUMBER() OVER(ORDER BY MB.BANK_NAME,MM.MEMBER_NAME ASC) AS RNO,MM.MEMBER_ID,ISNULL(MM.MEMBER_NAME,'')MEMBER_NAME,  \r" +
                                                " MB.BANK_USERCODE+'/'+BB.BANKBRANCH_USERCODE BANKBRANCH_NAME,MB.BANK_NAME,CASE WHEN MM.SEX = 'MALE' THEN 'M' ELSE 'F' END SEX, \r" +
                                                " CASE WHEN ISNULL(MM.ICNO_NEW, '') <> '' THEN ISNULL(MM.ICNO_NEW, '') ELSE ISNULL(ICNO_OLD, '') END ICNO_NEW,\r" +
                                                " CASE WHEN MM.MEMBERTYPE_CODE = 1 THEN 'C' ELSE 'N' END MEMBERTYPE_NAME,\r" +
                                                " MM.DATEOFJOINING, CASE WHEN ST.STATUS_CODE = 1 THEN 'A' ELSE 'D' END STATUS_NAME\r" +
                                                " FROM NUBESTATUS..STATUS{0:MMyyyy} ST(NOLOCK)\r" +
                                                " LEFT JOIN MASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE = ST.MEMBER_CODE \r" +
                                                " LEFT JOIN MASTERBANK MB(NOLOCK) ON MB.BANK_CODE = ST.BANK_CODE \r" +
                                                " LEFT JOIN MASTERBANKBRANCH BB(NOLOCK) ON BB.BANKBRANCH_CODE = ST.BRANCH_CODE \r" +
                                                " LEFT JOIN MASTERNUBEBRANCH NB(NOLOCK) ON NB.NUBE_BRANCH_CODE = ST.NUBE_BRANCH_CODE \r" +
                                                " LEFT JOIN MASTERSTATE MS(NOLOCK) ON MS.STATE_CODE = BB.BANKBRANCH_STATE_CODE   \r" +
                                                " WHERE ST.STATUS_CODE IN(1,2) " + sWhere, dtpDOB.SelectedDate);
                        }

                            

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
                                NB[4] = new ReportParameter("NUBEBRANCH", "NUBE BRANCH : " + cmbNubeBranch.Text.ToString());
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
                    //}
                    //else
                    //{
                    //    MessageBox.Show("No Records Found!");
                    //}
                }
                catch (Exception ex)
                {
                    ExceptionLogging.SendErrorToText(ex);
                }
            }
        }

        #endregion

    }
}
