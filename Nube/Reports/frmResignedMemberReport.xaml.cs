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
                if (string.IsNullOrEmpty(txtMemberNo.Text))
                {
                    MessageBox.Show("Membership No Empty!");
                    txtMemberNo.Focus();
                }
               
                else
                {
                    //DataTable dt = getData();
                    var m = db.VIEWRESIGNREPORTs.ToList();
                    int n = Convert.ToInt32(txtMemberNo.Text);
                    var vm = m.Where(x => x.MEMBER_ID == n).FirstOrDefault();
                    if (vm!=null)
                    {
                        frmResignReport frm = new frmResignReport(Convert.ToDecimal( vm.MEMBER_CODE), "RTEST");
                        frm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("No Records Found!");
                    }
                }
            }
            else if (string.IsNullOrEmpty(dtpFromDate.Text) )
            {
                MessageBox.Show("From Date is Empty!", "Empty");
                dtpFromDate.Focus();
            }
            else if (string.IsNullOrEmpty(dtpToDate.Text))
            {
                MessageBox.Show("To Date is Empty!", "Empty");
                dtpToDate.Focus();
            }
            else if (dtpFromDate.SelectedDate > dtpToDate.SelectedDate)
            {
                MessageBox.Show("From Date is Greater than To Date!");
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
            NUBEMemberReport.Clear();
            MemberReport.Clear();
            txtMemberNo.Text="";
            rbtResingDate.IsChecked = true;
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
                DataTable rt = getResignData();
                if (dt.Rows.Count > 0)
                {
                    //if (chkSimple.IsChecked == true)
                    //{
                    //    dgvResigned.ItemsSource = dt.DefaultView;
                    //}
                    //else
                    //{
                    ReportDataSource masterData = new ReportDataSource("Resign", dt);

                    MemberReport.LocalReport.DataSources.Add(masterData);
                    ReportDataSource master = new ReportDataSource("ResignStatus", rt);

                    MemberReport.LocalReport.DataSources.Add(master);
                    MemberReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.rptResign.rdlc";
                    ReportParameter RP = new ReportParameter("TotalMember", dt.Rows.Count.ToString());
                    MemberReport.LocalReport.SetParameters(RP);

                    MemberReport.RefreshReport();
                    LoadBankReport();
                    //}
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
                NUBEMemberReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.NubeResignedReport.rdlc";
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
                //if (chkSimple.IsChecked == true)
                //{
                //    if (!string.IsNullOrEmpty(dtpFromDate.Text))
                //    {
                //        qry = qry + string.Format(" RG.RESIGNATION_DATE BETWEEN '{0:dd/MMM/yyyy}' AND '{1:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                //    }
                //    if (Convert.ToInt32(cmbBank.SelectedValue) != 0)
                //    {
                //        decimal dBankCode = Convert.ToDecimal(cmbBank.SelectedValue);
                //        var st = (from x in db.MASTERBANKs where x.BANK_CODE == dBankCode || x.HEADER_BANK_CODE == dBankCode select x).ToList();
                //        if (st != null)
                //        {
                //            DataTable dtBank = AppLib.LINQResultToDataTable(st);
                //            string str = "";
                //            foreach (DataRow dr in dtBank.Rows)
                //            {
                //                if (string.IsNullOrEmpty(str))
                //                {
                //                    str = dr["BANK_CODE"].ToString();
                //                }
                //                else
                //                {
                //                    str = str + "," + dr["BANK_CODE"].ToString();
                //                }
                //                if (Convert.ToInt32(dr["HEADER_BANK_CODE"]) > 0)
                //                {
                //                    str = str + "," + dr["HEADER_BANK_CODE"].ToString();
                //                }
                //            }
                //            qry = qry + " AND MM.BANK_CODE IN (" + str + ") ";
                //        }
                //    }

                //    string strg = "SELECT ROW_NUMBER() OVER(ORDER BY MM.MEMBER_NAME ASC) AS RNO,MM.MEMBER_ID,MM.MEMBER_NAME,MM.SEX,\r" +
                //                 "CASE WHEN ISNULL(MM.ICNO_NEW,'')<>'' THEN MM.ICNO_NEW ELSE ISNULL(MM.ICNO_OLD,'') END NRIC,\r" +
                //                 "MM.BANK_USERCODE+'/'+MM.BRANCHUSERCODE BANK,RG.RESIGNATION_DATE,ST.RESIGNSTATUS_NAME,MM.LASTPAYMENT_DATE,RG.CLAIMER_NAME\r" +
                //                 "FROM RESIGNATION RG(NOLOCK)\r" +
                //                 "LEFT JOIN VIEWMASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE=RG.MEMBER_CODE\r" +
                //                 "LEFT JOIN MASTERRESIGNSTATUS ST(NOLOCK) ON ST.RESIGNSTATUS_CODE=RG.RESIGNSTATUS_CODE\r" +
                //                 "WHERE" + qry;
                //    cmd = new SqlCommand(strg, con);
                //    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                //    adp.SelectCommand.CommandTimeout = 0;
                //    adp.Fill(dt);
                //    qry = "";
                //}
                //else
                //{
                Wqry();
                if (!string.IsNullOrEmpty(dtpFromDate.Text))
                {
                    if (rbtPaymentDate.IsChecked == true)
                    {
                        if (!string.IsNullOrEmpty(qry))
                        {
                            qry = qry + string.Format(" AND VOUCHER_DATE BETWEEN '{0:dd/MMM/yyyy}' AND '{1:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                        }
                        else
                        {
                            qry = qry + string.Format(" VOUCHER_DATE BETWEEN '{0:dd/MMM/yyyy}' AND '{1:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(qry))
                        {
                            qry = qry + string.Format(" AND RESIGNATION_DATE BETWEEN '{0:dd/MMM/yyyy}' AND '{1:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                        }
                        else
                        {
                            qry = qry + string.Format(" RESIGNATION_DATE BETWEEN '{0:dd/MMM/yyyy}' AND '{1:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(cmbReasonBranch.Text))
                {
                    if (!string.IsNullOrEmpty(qry))
                    {
                        qry = qry + string.Format(" AND STATUSCODE=" + cmbReasonBranch.SelectedValue);
                    }
                    else
                    {
                        qry = qry + string.Format(" STATUSCODE=" + cmbReasonBranch.SelectedValue);
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

                //}
                return dt;

            }
        }
        private DataTable getResignData()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd;
                Wqry();
                if (!string.IsNullOrEmpty(dtpFromDate.Text))
                {
                    if (rbtPaymentDate.IsChecked == true)
                    {
                        if (!string.IsNullOrEmpty(qry))
                        {
                            qry = qry + string.Format(" AND VOUCHER_DATE BETWEEN '{0:dd/MMM/yyyy}' AND '{1:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                        }
                        else
                        {
                            qry = qry + string.Format(" VOUCHER_DATE BETWEEN '{0:dd/MMM/yyyy}' AND '{1:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(qry))
                        {
                            qry = qry + string.Format(" AND RESIGNATION_DATE BETWEEN '{0:dd/MMM/yyyy}' AND '{1:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                        }
                        else
                        {
                            qry = qry + string.Format(" RESIGNATION_DATE BETWEEN '{0:dd/MMM/yyyy}' AND '{1:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(cmbReasonBranch.Text))
                {
                    if (!string.IsNullOrEmpty(qry))
                    {
                        qry = qry + string.Format(" AND STATUSCODE=" + cmbReasonBranch.SelectedValue);
                    }
                    else
                    {
                        qry = qry + string.Format(" STATUSCODE=" + cmbReasonBranch.SelectedValue);
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


                cmd = new SqlCommand("Select distinct(ms.RESIGNSTATUS_SHORTCODE)RESIGNSTATUS_CODE,ms.RESIGNSTATUS_NAME from MASTERRESIGNSTATUS(nolock) ms\r" +
                                      "left join VIEWRESIGNREPORT(nolock) vp on vp.STATUSCODE = ms.RESIGNSTATUS_CODE\r" + "" +
                                      "WHERE " + qry + " order by ms.RESIGNSTATUS_SHORTCODE", con);
                SqlDataAdapter adp1 = new SqlDataAdapter(cmd);
                adp1.SelectCommand.CommandTimeout = 0;
                adp1.Fill(dt);
                //}
                return dt;

            }
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
                    if (rbtPaymentDate.IsChecked == true)
                    {
                        qry = qry + string.Format(" AND VOUCHER_DATE BETWEEN '{0:dd/MMM/yyyy}' AND '{1:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                    }
                    else
                    {
                        qry = qry + string.Format(" AND RESIGNATION_DATE BETWEEN '{0:dd/MMM/yyyy}' AND '{1:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                    }
                }

                cmd = new SqlCommand("SELECT * FROM VIEWMASTERMEMBER(NOLOCK)  where " + qry + " ORDER BY MEMBER_NAME", con);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.SelectCommand.CommandTimeout = 0;
                adp.Fill(dt);
                qry = "";
            }
            return dt;

        }

        private void chkPrint_Click(object sender, RoutedEventArgs e)
        {
            if (chkPrint.IsChecked == true)
            {
                lblDate.Visibility = Visibility.Hidden;
                lblToDate.Visibility = Visibility.Hidden;
                lblReason.Visibility = Visibility.Hidden;
                dtpFromDate.Visibility = Visibility.Hidden;
                dtpToDate.Visibility = Visibility.Hidden;
                lblToDate_Copy.Visibility = Visibility.Hidden;
                chkSimple.Visibility = Visibility.Hidden;
                rbtResingDate.Visibility = Visibility.Hidden;
                rbtPaymentDate.Visibility = Visibility.Hidden;
                txtMemberNoTo.Visibility = Visibility.Hidden;
                cmbReasonBranch.Visibility = Visibility.Hidden;
                txtMemberNoFrom.Visibility = Visibility.Collapsed;
                txtMemberNo.Visibility = Visibility.Visible;
            }
            else 
            {
                lblDate.Visibility = Visibility.Visible;
                lblToDate.Visibility = Visibility.Visible;
                lblReason.Visibility = Visibility.Visible;
                dtpFromDate.Visibility = Visibility.Visible;
                dtpToDate.Visibility = Visibility.Visible;
                lblToDate_Copy.Visibility = Visibility.Visible;
                
                rbtResingDate.Visibility = Visibility.Visible;
                rbtPaymentDate.Visibility = Visibility.Visible;
                txtMemberNoTo.Visibility = Visibility.Visible;
                cmbReasonBranch.Visibility = Visibility.Visible;
                txtMemberNo.Visibility = Visibility.Collapsed;
                txtMemberNoFrom.Visibility = Visibility.Visible;
                txtMemberNo.Text = "";
            }
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

