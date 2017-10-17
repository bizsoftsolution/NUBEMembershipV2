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
                if (sForm_Name == "NewMemberReport")
                {
                    txtHeading.Text = "New Member Report";
                }
                else
                {
                    txtHeading.Text = "Active Member Report";
                }
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
            if (sForm_Name == "NewMemberReport" && string.IsNullOrEmpty(dtpFromDate.Text))
            {
                MessageBox.Show("Date is Empty!");
                dtpFromDate.Focus();
                return;
            }

            else if (sForm_Name != "NewMemberReport" && dtpFromDate.Text != "" && string.IsNullOrEmpty(dtpToDate.Text))
            {
                MessageBox.Show("Date is Empty!");
                dtpFromDate.Focus();
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
            using (SqlConnection con = new SqlConnection(AppLib.connStr))
            {
                SqlCommand cmd;
                if (sForm_Name == "NewMemberReport")
                {
                    cmd = new SqlCommand(" SELECT ROW_NU-bMBER() OVER(ORDER BY MEMBER_NAME ASC) AS RNO,MEMBER_ID,MEMBER_NAME,ISNULL(MEMBERTYPE_NAME,'')MEMBERTYPE_NAME,\r" +
                                     " CASE WHEN ISNULL(ICNO_NEW, '')<>'' THEN ISNULL(ICNO_NEW,'') ELSE ISNULL(ICNO_OLD,'') END ICNO_NEW,\r" +
                                     " BANK_USERCODE+'/'+BRANCHUSERCODE BANK_USERCODE,DATEOFJOINING,LEVY,TDF,LASTPAYMENT_DATE,\r" +
                                     " ISNULL(SEX,'')SEX,DATEOFJOINING \r" +
                                     " FROM TEMPVIEWMASTERMEMBER MM(NOLOCK) " +
                                     " WHERE " + qry +
                                     " ORDER BY MEMBER_NAME", con);
                }
                else
                {
                    int i = ((Convert.ToDateTime(DateTime.Now).Year - Convert.ToDateTime(dtpFromDate.SelectedDate).Year) * 12) + Convert.ToDateTime(DateTime.Now).Month - Convert.ToDateTime(dtpFromDate.SelectedDate).Month;

                    AlterView.ViewMemberTotalMonthsDue(i);
                    AlterView.ViewMasterMember(i);
                    AlterView.ExecuteSPREFRESH();
                    AlterView.DefaultMemberTotalMonthsDue();
                    AlterView.DefaultMasterMember();

                    cmd = new SqlCommand(" SELECT ROW_NUMBER() OVER(ORDER BY MEMBER_NAME ASC) AS RNO,MEMBER_ID,MEMBER_NAME,ISNULL(MEMBERTYPE_NAME,'')MEMBERTYPE_NAME,\r" +
                                                         " CASE WHEN ISNULL(ICNO_NEW, '')<>'' THEN ISNULL(ICNO_NEW,'') ELSE ISNULL(ICNO_OLD,'') END ICNO_NEW,\r" +
                                                         " BANK_USERCODE+'/'+BRANCHUSERCODE BANK_USERCODE,DATEOFJOINING,LEVY,TDF,LASTPAYMENT_DATE,\r" +
                                                         " ISNULL(SEX,'')SEX,DATEOFJOINING\r" +
                                                         " FROM TEMPVIEWMASTERMEMBER(NOLOCK) WHERE " + qry +
                                                         " ORDER BY MEMBER_NAME", con);
                }

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
            if (sForm_Name == "NewMemberReport" && !string.IsNullOrEmpty(dtpFromDate.Text) && !string.IsNullOrEmpty(qry))
            {
                qry = qry + string.Format(" AND DATEOFJOINING BETWEEN '{0:dd/MMM/yyyy}' AND '{1:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
            }
            else if (sForm_Name == "NewMemberReport" && !string.IsNullOrEmpty(dtpFromDate.Text) && string.IsNullOrEmpty(qry))
            {
                qry = qry + string.Format(" DATEOFJOINING BETWEEN '{0:dd/MMM/yyyy}' AND '{1:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
            }

            if (sForm_Name != "NewMemberReport" && !string.IsNullOrEmpty(qry))
            {
                qry = qry + " AND MEMBERSTATUSCODE=1 ";
            }
            else if (sForm_Name != "NewMemberReport" && string.IsNullOrEmpty(qry))
            {
                qry = qry + " MEMBERSTATUSCODE=1 ";
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

            if (!string.IsNullOrEmpty(cmbNubeBranch.Text))
            {
                if (!string.IsNullOrEmpty(qry))
                {
                    qry = qry + " AND NUBEBRANCH=" + cmbNubeBranch.SelectedValue;
                }
                else
                {
                    qry = qry + " NUBEBRANCH=" + cmbNubeBranch.SelectedValue;
                }
            }

            if (!string.IsNullOrEmpty(cmbBank.Text))
            {
                if (!string.IsNullOrEmpty(qry))
                {
                    qry = qry + " AND BANK_CODE=" + cmbBank.SelectedValue;
                }
                else
                {
                    qry = qry + " BANK_CODE=" + cmbBank.SelectedValue;
                }
            }

            if (!string.IsNullOrEmpty(cmbBranch.Text))
            {
                if (!string.IsNullOrEmpty(qry))
                {
                    qry = qry + " AND BRANCH_CODE=" + cmbBranch.SelectedValue;
                }
                else
                {
                    qry = qry + " BRANCH_CODE=" + cmbBranch.SelectedValue;
                }
            }

            return qry;
        }

        #endregion

    }
}
