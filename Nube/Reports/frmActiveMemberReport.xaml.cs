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
    /// Interaction logic for frmActiveMemberReport.xaml
    /// </summary>
    public partial class frmActiveMemberReport : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        string qry = "";
        string connStr = AppLib.connStr;
        string sForm_Name = "";

        public frmActiveMemberReport(string sFormName = "")
        {
            InitializeComponent();
            try
            {
                sForm_Name = sFormName;
                this.KeyDown += new System.Windows.Input.KeyEventHandler(Window_KeyDown);
                FormLoad();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #region CLOSING EVENTS

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

        #endregion        

        #region BUTTON  EVENTS

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (sForm_Name == "NewMemberReport" && (string.IsNullOrEmpty(dtpFromDate.Text) || string.IsNullOrEmpty(dtpToDate.Text)))
            {
                MessageBox.Show("Date is Empty!");

                return;
            }
            else if (dtpFromDate.SelectedDate > dtpToDate.SelectedDate)
            {
                MessageBox.Show("From Date is Greater than To Date!");
            }
            else if (sForm_Name != "NewMemberReport" && string.IsNullOrEmpty(dtpToDate.Text))
            {
                MessageBox.Show("Date is Empty!");
                dtpToDate.Focus();
                return;
            }
            else
            {
                LoadReport();
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cmbNubeBranch.Text = "";
                cmbBank.Text = "";
                cmbBranch.Text = "";
                dtpFromDate.Text = "";
                dtpToDate.Text = "";
                txtMemberNoFrom.Text = "";
                txtMemberNoTo.Text = "";
                chkRejoin.IsChecked = false;
                MemberReport.Clear();
                NUBEMemberReport.Clear();
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

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            frmHomeReports frm = new frmHomeReports();
            this.Close();
            frm.ShowDialog();
        }

        #endregion

        #region USER DEFINED FUNCTION

        void FormLoad()
        {
            if (sForm_Name == "NewMemberReport")
            {
                txtHeading.Text = "New Member Report";
                lblDate.Text = "To Date";
                dtpFromDate.Visibility = Visibility.Visible;
                lblFromDate.Visibility = Visibility.Visible;
                chkRejoin.Visibility = Visibility.Visible;
            }
            else
            {
                txtHeading.Text = "Active Member Report";
                lblDate.Text = "Entry Date";
                dtpFromDate.Visibility = Visibility.Collapsed;
                lblFromDate.Visibility = Visibility.Collapsed;
                chkRejoin.Visibility = Visibility.Collapsed;
            }

            var NUBE = db.MASTERNUBEBRANCHes.OrderBy(x => x.NUBE_BRANCH_NAME).ToList();
            cmbNubeBranch.ItemsSource = NUBE;
            cmbNubeBranch.SelectedValuePath = "NUBE_BRANCH_CODE";
            cmbNubeBranch.DisplayMemberPath = "NUBE_BRANCH_NAME";

            var bank = db.MASTERBANKs.OrderBy(x => x.BANK_NAME).ToList();
            cmbBank.ItemsSource = bank;
            cmbBank.SelectedValuePath = "BANK_CODE";
            cmbBank.DisplayMemberPath = "BANK_NAME";
        }

        private void LoadReport()
        {
            try
            {
                MemberReport.Reset();
                DataTable dt = new DataTable();
                dt = getData();
                if (dt.Rows.Count > 0)
                {
                    string Total = dt.Rows.Count.ToString();
                    ReportDataSource masterData = new ReportDataSource("ActiveMember", dt);

                    MemberReport.LocalReport.DataSources.Add(masterData);
                    ReportDataSource Data = new ReportDataSource("DataSet1", dt);

                    MemberReport.LocalReport.DataSources.Add(Data);
                    MemberReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.frmActiveMemberReport.rdlc";
                    ReportParameter[] NB = new ReportParameter[5];
                    if (!string.IsNullOrEmpty(cmbNubeBranch.Text))
                    {
                        NB[0] = new ReportParameter("NubeBranchName", "NUBE BRANCH - " + cmbNubeBranch.Text.ToString());
                    }
                    else
                    {
                        NB[0] = new ReportParameter("NubeBranchName", "");
                    }

                    if (!string.IsNullOrEmpty(cmbBank.Text))
                    {
                        NB[1] = new ReportParameter("BankName", "BANK - " + cmbBank.Text.ToString());
                    }
                    else
                    {
                        NB[1] = new ReportParameter("BankName", "");
                    }


                    if (!string.IsNullOrEmpty(cmbBranch.Text))
                    {
                        NB[2] = new ReportParameter("BranchName", "BANK BRANCH - " + cmbBranch.Text.ToString());
                    }
                    else
                    {
                        NB[2] = new ReportParameter("BranchName", "");
                    }
                    NB[3] = new ReportParameter("TotalMember", Total);

                    if (sForm_Name == "NewMemberReport")
                    {
                        NB[4] = new ReportParameter("Title", "NEW MEMBER REPORT");
                    }
                    else
                    {
                        NB[4] = new ReportParameter("Title", "ACTIVE MEMBER REPORT");
                    }

                    MemberReport.LocalReport.SetParameters(NB);
                    MemberReport.RefreshReport();
                    LoadBankReport(dt);
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

        private void LoadBankReport(DataTable dtBranch)
        {
            try
            {
                NUBEMemberReport.Reset();
                ReportDataSource Data = new ReportDataSource("ViewMasterMember", dtBranch);
                NUBEMemberReport.LocalReport.DataSources.Add(Data);
                NUBEMemberReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.NUBEBranchMemberReport.rdlc";
                ReportParameter RP1 = new ReportParameter();
                if (!string.IsNullOrEmpty(cmbNubeBranch.Text))
                {
                    RP1 = new ReportParameter("Title", "NUBE " + cmbNubeBranch.Text + " BRANCH MEMBER REPORT");
                }
                else
                {
                    RP1 = new ReportParameter("Title", "NUBE BRANCH MEMBER REPORT");
                }
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
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(AppLib.connStr))
            {
                SqlCommand cmd;
                if (sForm_Name == "NewMemberReport")
                {
                    string sDate = "";
                    if (!string.IsNullOrEmpty(dtpFromDate.Text) && !string.IsNullOrEmpty(dtpToDate.Text))
                    {
                        sDate = string.Format(" MM.DATEOFJOINING BETWEEN '{0:dd/MMM/yyyy}' AND '{1:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                    }
                    else if (!string.IsNullOrEmpty(dtpFromDate.Text))
                    {
                        sDate = string.Format(" MM.DATEOFJOINING ='{0:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate);
                    }
                    else if (!string.IsNullOrEmpty(dtpToDate.Text))
                    {
                        sDate = string.Format(" MM.DATEOFJOINING='{0:dd/MMM/yyyy}' ", dtpToDate.SelectedDate);
                    }

                    if (chkRejoin.IsChecked == true)
                    {
                        sDate = sDate + " AND MM.REJOINED=1 ";
                    }

                    cmd = new SqlCommand(" SELECT ROW_NUMBER() OVER(ORDER BY MM.MEMBER_NAME ASC) AS RNO,MM.MEMBER_ID,MM.MEMBER_NAME,  \r" +
                                         " ISNULL(MM.MEMBERTYPE_NAME, '')MEMBERTYPE_NAME, ISNULL(MM.LEVY, '')LEVY, ISNULL(MM.TDF, '')TDF, ISNULL(MM.SEX, '')SEX, \r" +
                                         " CASE WHEN ISNULL(MM.ICNO_NEW, '') <> '' THEN ISNULL(MM.ICNO_NEW, '') ELSE ISNULL(MM.ICNO_OLD, '') END ICNO_NEW, \r" +
                                         " MM.BANKUSER_CODE + '/' + MM.BRANCH_USER_CODE BANK_USERCODE, MM.DATEOFJOINING, MM.BANKUSER_CODE BANK, MM.BRANCH_USER_CODE BANKBRANCH_USERCODE, \r" +
                                         " MM.LASTPAYMENT_DATE LASTPAYMENT_DATE,MM.BANK_CODE,MM.BRANCH_CODE,MM.NUBEBRANCH_CODE NUBE_BRANCH_CODE \r" +
                                         " FROM MEMBERSTATUSLOG MM(NOLOCK) \r" +
                                         " WHERE MM.ISCANCEL=0 AND " + sDate +
                                         " ORDER BY MEMBER_NAME", con);

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.SelectCommand.CommandTimeout = 0;
                    adp.Fill(dt);
                }
                else
                {
                    string str = string.Format(" SELECT ROW_NUMBER() OVER(ORDER BY MM.MEMBER_NAME ASC) AS RNO,ST.MEMBER_CODE,MM.MEMBER_ID,MM.MEMBER_NAME, \r" +
                                               " CASE WHEN ST.MEMBERTYPE_CODE = 1 THEN 'C' ELSE 'N' END MEMBERTYPE_NAME, \r" +
                                               " CASE  WHEN ISNULL(MM.ICNO_NEW, '') <> '' THEN MM.ICNO_NEW ELSE MM.ICNO_OLD END ICNO_NEW, \r" +
                                               " MB.BANK_USERCODE BANK_USERCODE, ST.BANK_CODE, ST.BRANCH_CODE, BB.BANKBRANCH_USERCODE BANKBRANCH_USERCODE, \r" +
                                               " BB.NUBE_BRANCH_CODE, MB.BANK_USERCODE BANK, MM.LEVY, MM.TDF, ST.LASTPAYMENTDATE LASTPAYMENT_DATE, \r" +
                                               " MM.SEX, MM.DATEOFJOINING, MS.STATUS_NAME, ST.STATUS_CODE MEMBERSTATUSCODE, ST.TOTALMONTHSDUE TOTALMOTHSDUE \r" +
                                               " FROM NUBESTATUS..STATUS{0:MMyyyy} ST(NOLOCK) \r" +
                                               " LEFT JOIN MASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE = ST.MEMBER_CODE \r" +
                                               " LEFT JOIN MASTERBANK MB(NOLOCK) ON MB.BANK_CODE = ST.BANK_CODE \r" +
                                               " LEFT JOIN MASTERBANKBRANCH BB(NOLOCK) ON BB.BANKBRANCH_CODE = ST.BRANCH_CODE \r" +
                                               " LEFT JOIN MASTERNUBEBRANCH NB(NOLOCK) ON NB.NUBE_BRANCH_CODE = ST.NUBE_BRANCH_CODE \r" +
                                               " LEFT JOIN MASTERSTATUS MS(NOLOCK) ON MS.STATUS_CODE = ST.STATUS_CODE \r" +
                                               " WHERE ST.STATUS_CODE = 1 AND ISCANCEL = 0 \r" +
                                               " GROUP BY ST.MEMBER_CODE, MM.MEMBER_ID, MM.MEMBER_NAME, \r" +
                                               " ST.MEMBERTYPE_CODE, MM.ICNO_NEW, MM.ICNO_OLD, MB.BANK_USERCODE, ST.BANK_CODE, ST.BRANCH_CODE, BB.BANKBRANCH_USERCODE, ST.STATUS_CODE, \r" +
                                               " BB.NUBE_BRANCH_CODE, MB.BANK_USERCODE, MM.LEVY, MM.TDF, ST.LASTPAYMENTDATE, MM.SEX, MM.DATEOFJOINING, MS.STATUS_NAME, ST.TOTALMONTHSDUE", dtpToDate.SelectedDate);
                    cmd = new SqlCommand(str, con);                    
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.SelectCommand.CommandTimeout = 0;
                    adp.Fill(dt);
                }

                if (dt.Rows.Count > 0)
                {
                    string sWhere = "";

                    if (!string.IsNullOrEmpty(cmbBank.Text))
                    {
                        sWhere = sWhere + " BANK_CODE=" + cmbBank.SelectedValue;
                    }

                    if (!string.IsNullOrEmpty(cmbBranch.Text) && !string.IsNullOrEmpty(sWhere))
                    {
                        sWhere = sWhere + " AND BRANCH_CODE=" + cmbBranch.SelectedValue;
                    }
                    else if (!string.IsNullOrEmpty(cmbBranch.Text))
                    {
                        sWhere = sWhere + " BRANCH_CODE=" + cmbBranch.SelectedValue;
                    }

                    if (!string.IsNullOrEmpty(cmbNubeBranch.Text) && !string.IsNullOrEmpty(sWhere))
                    {
                        sWhere = sWhere + " AND NUBE_BRANCH_CODE=" + cmbNubeBranch.SelectedValue;
                    }
                    else if (!string.IsNullOrEmpty(cmbNubeBranch.Text))
                    {
                        sWhere = sWhere + " NUBE_BRANCH_CODE=" + cmbNubeBranch.SelectedValue;
                    }

                    if (!string.IsNullOrEmpty(txtMemberNoFrom.Text) && !string.IsNullOrEmpty(txtMemberNoTo.Text) && !string.IsNullOrEmpty(sWhere))
                    {
                        sWhere = sWhere + string.Format(" AND MEMBER_ID >={0} AND MEMBER_ID<={1} ", txtMemberNoFrom.Text, txtMemberNoTo.Text);
                    }
                    else if (!string.IsNullOrEmpty(txtMemberNoFrom.Text) && !string.IsNullOrEmpty(txtMemberNoTo.Text))
                    {
                        sWhere = sWhere + string.Format(" MEMBER_ID >= {0} AND MEMBER_ID<={1} ", txtMemberNoFrom.Text, txtMemberNoTo.Text);
                    }
                    else if (!string.IsNullOrEmpty(txtMemberNoFrom.Text) && !string.IsNullOrEmpty(qry))
                    {
                        sWhere = sWhere + string.Format(" AND MEMBER_ID ={0} ", txtMemberNoFrom.Text);
                    }
                    else if (!string.IsNullOrEmpty(txtMemberNoFrom.Text))
                    {
                        sWhere = sWhere + string.Format(" MEMBER_ID ={0} ", txtMemberNoFrom.Text);
                    }
                    else if (!string.IsNullOrEmpty(txtMemberNoTo.Text) && !string.IsNullOrEmpty(sWhere))
                    {
                        sWhere = sWhere + string.Format(" AND MEMBER_ID ={0} ", txtMemberNoTo.Text);
                    }
                    else if (!string.IsNullOrEmpty(txtMemberNoTo.Text))
                    {
                        sWhere = sWhere + string.Format(" MEMBER_ID ={0} ", txtMemberNoTo.Text);
                    }

                    if (!string.IsNullOrEmpty(sWhere))
                    {
                        DataView dv = new DataView(dt);
                        dv.RowFilter = sWhere;
                        dt = dv.ToTable();
                        int i = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            row["RNO"] = i + 1;
                            i++;
                        }
                    }
                }
            }
            return dt;
        }

        #endregion

    }
}
