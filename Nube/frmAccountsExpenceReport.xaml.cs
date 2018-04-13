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

namespace Nube
{
    /// <summary>
    /// Interaction logic for frmAccountsExpenceReport.xaml
    /// </summary>
    public partial class frmAccountsExpenceReport : MetroWindow
    {
        DataTable dtLedgerName = new DataTable();
        DataTable dtPayto = new DataTable();
        List<DDL_Ledger> objLedgerList;
   
        public frmAccountsExpenceReport()
        {
            InitializeComponent();

            try
            {
                using (SqlConnection con = new SqlConnection(AppLib.connStr))
                {
                    SqlCommand cmd;
                    string str = string.Format(" SELECT LEDGERNAME FROM NUBE_New..LEDGER WHERE ACCOUNTGROUPID IN (10102,10146,10190) \r " +
                                                " GROUP BY LEDGERNAME \r " +
                                                " ORDER BY LEDGERNAME ");
                    cmd = new SqlCommand(str, con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dtLedgerName);
                    str = string.Format(" SELECT DISTINCT PM.PAYTO LEDGERNAME  \r " +
                                        " FROM NUBE_NEW..LEDGER LD(NOLOCK)\r " +
                                        " LEFT JOIN NUBE_NEW..PAYMENTDETAIL PD(NOLOCK) ON PD.LEDGERID = LD.ID \r " +
                                        " LEFT JOIN NUBE_NEW..PAYMENT PM(NOLOCK) ON PM.ID = PD.PAYMENTID \r " +
                                        " WHERE LD.ACCOUNTGROUPID IN(10102, 10146, 10190) AND ISNULL(PM.PAYTO, '') <> '' \r " +
                                        " GROUP BY PM.PAYTO\r " +
                                        " ORDER BY PM.PAYTO ");
                    cmd = new SqlCommand(str, con);
                    adp = new SqlDataAdapter(cmd);
                    adp.Fill(dtPayto);
                }
                cmbLedgerName.ItemsSource = dtLedgerName.DefaultView;
                cmbLedgerName.SelectedValuePath = "LEDGERNAME";
                cmbLedgerName.DisplayMemberPath = "LEDGERNAME";

                objLedgerList = new List<DDL_Ledger>();
                AddElementsInList();
                BindCountryDropDown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void BindCountryDropDown()
        {
            ddlLedger.ItemsSource = objLedgerList;
        }
        void AddElementsInList()
        {
            DDL_Ledger obj = new DDL_Ledger();
            if (rbtnLedgerName.IsChecked == true)
            {
                foreach (DataRow drrow in dtLedgerName.Rows)
                {
                    obj = new DDL_Ledger();
                    obj.LEDGERNAME = drrow["LEDGERNAME"].ToString();
                    obj.Check_Status = false;
                    objLedgerList.Add(obj);
                    obj = new DDL_Ledger();
                }
            }
            foreach (DataRow drrow in dtPayto.Rows)
            {
                obj = new DDL_Ledger();
                obj.LEDGERNAME = drrow["LEDGERNAME"].ToString();
                obj.Check_Status = false;
                objLedgerList.Add(obj);
                obj = new DDL_Ledger();
            }
        }
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
      
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(dtpFromDate.Text) || string.IsNullOrEmpty(dtpToDate.Text))
            {
                MessageBox.Show("Date is Empty!");
                dtpFromDate.Focus();
                return;
            }
            else
            {
                string sLedger ;

                if(rbtnLedgerName.IsChecked == true)
                {
                    sLedger = "AND LD.LedgerName in(";
                }
                else
                {
                    sLedger = "AND PM.PAYTO in(";
                }
                //if (!string.IsNullOrEmpty(cmbLedgerName.Text))
                //{
                //    if (rbtnLedgerName.IsChecked == true)
                //    {
                //        sLedger = " AND LD.LedgerName='" + ddlLedger.Text + "'";
                //    }
                //    else
                //    {
                //        sLedger = " AND PM.PayTo='" + ddlLedger.Text + "'";
                //    }
                //}

                var d = objLedgerList.Where(x => x.Check_Status == true).ToList();
                if (d.Count > 1)
                {
                    foreach (var d1 in d)
                    {
                        sLedger += "'" + d1.LEDGERNAME + "',";
                    }
                    sLedger = sLedger.Remove(sLedger.Length - 1);
                }
                else
                {
                    sLedger += "'" + d.FirstOrDefault().LEDGERNAME + "'";
                }
                sLedger += ")";
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(AppLib.connStr))
                {
                    SqlCommand cmd;
                    string str = "";
                    if (rbtnLedgerName.IsChecked == true)
                    {
                        str = string.Format("SELECT LD.LedgerName,FORMAT(PAYMENTDATE,'MMM-yyyy', 'en-US')PaymentDate,ISNULL(SUM(PD.AMOUNT),0)Amount \r " +
                       " FROM NUBE_New..LEDGER LD(NOLOCK) \r " +
                       " LEFT JOIN NUBE_New..PAYMENTDETAIL PD(NOLOCK) ON PD.LEDGERID = LD.ID \r " +
                       " LEFT JOIN NUBE_New..PAYMENT PM(NOLOCK) ON PM.ID = PD.PAYMENTID \r " +
                       " WHERE LD.ACCOUNTGROUPID IN(10102,10146,10190) \r " +
                       " AND PAYMENTDATE BETWEEN '{0:dd/MMM/yyyy}' AND '{1:dd/MMM/yyyy}'  \r " + sLedger +
                       " GROUP BY LD.LEDGERNAME, FORMAT(PAYMENTDATE, 'MMM-yyyy', 'en-US') \r " +
                       " HAVING ISNULL(SUM(PD.AMOUNT), 0) <> 0 \r " +
                       " ORDER BY PAYMENTDATE ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                    }
                    else
                    {
                        str = string.Format("SELECT DISTINCT PM.PAYTO LedgerName,FORMAT(PAYMENTDATE,'MMM-yyyy', 'en-US')PaymentDate,ISNULL(SUM(PD.AMOUNT),0)Amount \r " +
                       " FROM NUBE_New..LEDGER LD(NOLOCK) \r " +
                       " LEFT JOIN NUBE_New..PAYMENTDETAIL PD(NOLOCK) ON PD.LEDGERID = LD.ID \r " +
                       " LEFT JOIN NUBE_New..PAYMENT PM(NOLOCK) ON PM.ID = PD.PAYMENTID \r " +
                       " WHERE LD.ACCOUNTGROUPID IN(10102,10146,10190) \r " +
                       " AND PAYMENTDATE BETWEEN '{0:dd/MMM/yyyy}' AND '{1:dd/MMM/yyyy}'  \r " + sLedger +
                       " GROUP BY PM.PayTo, FORMAT(PAYMENTDATE, 'MMM-yyyy', 'en-US') \r " +
                       " HAVING ISNULL(SUM(PD.AMOUNT), 0) <> 0 \r " +
                       " ORDER BY PAYMENTDATE ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);
                    }

                    cmd = new SqlCommand(str, con);

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                }
                if (dt.Rows.Count > 0)
                {
                    ExpencesReport.Reset();
                    ReportDataSource masterData = new ReportDataSource("AccountsExpenceReport", dt);

                    ExpencesReport.LocalReport.DataSources.Add(masterData);
                    ExpencesReport.LocalReport.ReportEmbeddedResource = "Nube.rptAccountsExpenceReport.rdlc";
                    //ReportParameter[] NB1 = new ReportParameter[1];
                    //NB1[0] = new ReportParameter("LedgerName", cmbLedgerName.Text.ToString());
                    //ExpencesReport.LocalReport.SetParameters(NB1);

                    ExpencesReport.RefreshReport();

                    //ExpencesReport2.Reset();
                    //ReportDataSource masterData2 = new ReportDataSource("AccountReport", dt);

                    //ExpencesReport2.LocalReport.DataSources.Add(masterData2);
                    //ExpencesReport2.LocalReport.ReportEmbeddedResource = "Nube.rptAccountsExpenceReport2.rdlc";
                    //ReportParameter[] NB2 = new ReportParameter[1];
                    //NB2[0] = new ReportParameter("LedgerName", cmbLedgerName.Text.ToString());
                    //ExpencesReport.LocalReport.SetParameters(NB2);
                    //ExpencesReport2.RefreshReport();

                    //ExpencesReport3.Reset();
                    //ReportDataSource masterData3 = new ReportDataSource("AccountReport", dt);

                    //ExpencesReport3.LocalReport.DataSources.Add(masterData3);
                    //ExpencesReport3.LocalReport.ReportEmbeddedResource = "Nube.rptAccountsExpenceReport3.rdlc";
                    //ReportParameter[] NB3 = new ReportParameter[1];
                    //NB3[0] = new ReportParameter("LedgerName", cmbLedgerName.Text.ToString());
                    //ExpencesReport.LocalReport.SetParameters(NB3);
                    //ExpencesReport3.RefreshReport();
                }

                else
                {
                    MessageBox.Show("No Records Found!");
                }
            }
        }
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ExpencesReport.Clear();
            ddlLedger.Text = "";
            dtpFromDate.Text = "";
            dtpToDate.Text = "";
            rbtnLedgerName.IsChecked = true;
            fFilterType();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            frmHomeMaster frm = new frmHomeMaster();
            this.Close();
            frm.ShowDialog();
        }

        private void rbtnLedgerName_Click(object sender, RoutedEventArgs e)
        {
            ddlLedger.Text = "";
            ExpencesReport.Clear();
            objLedgerList = new List<DDL_Ledger>();
            fFilterType();
            AddElementsInList();
            BindCountryDropDown();
        }

        private void rbtnPayto_Click(object sender, RoutedEventArgs e)
        {
            ddlLedger.Text = "";
            ExpencesReport.Clear();
            objLedgerList = new List<DDL_Ledger>();
            fFilterType();
            AddElementsInList();
            BindCountryDropDown();
        }
        void fFilterType()
        {
            if (rbtnLedgerName.IsChecked == true)
            {
                cmbLedgerName.ItemsSource = dtLedgerName.DefaultView;
                cmbLedgerName.SelectedValuePath = "LEDGERNAME";
                cmbLedgerName.DisplayMemberPath = "LEDGERNAME";
            }
            else
            {
                cmbLedgerName.ItemsSource = dtPayto.DefaultView;
                cmbLedgerName.SelectedValuePath = "LEDGERNAME";
                cmbLedgerName.DisplayMemberPath = "LEDGERNAME";
            }
        }

        private void ddlLedger_TextChanged(object sender, RoutedEventArgs e)
        {
            ddlLedger.ItemsSource = objLedgerList.Where(x => x.LEDGERNAME.StartsWith(ddlLedger.Text.Trim()));
        }

        private void ddlLedger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AllCheckbocx_CheckedAndUnchecked(object sender, RoutedEventArgs e)
        {
            // BindListBOX();

            var v = ((CheckBox)sender).Tag as DDL_Ledger;
            v.Check_Status = true;
            ddlLedger.Text += v.LEDGERNAME + "";
            //ddlLedger.Text = ddlLedger.Text.Remove(ddlLedger.Text.Length - 1);
        }

        void BindListBOX()
        {
            //foreach (var country in objLedgerList)
            //{

            //    ddlLedger.Text = country.LEDGERNAME.ToString();
            //    if (country.Check_Status == true)
            //    {
            //        //testListbox.Items.Add(country.Country_Name);

            //    }
            //}

        }

        public class DDL_Ledger
        {
            public string LEDGERNAME { get; set; }
            public Boolean Check_Status { get; set; }
            }
        private void chkLedger_Unchecked(object sender, RoutedEventArgs e)
        {
            var v = ((CheckBox)sender).Tag as DDL_Ledger;
            v.Check_Status = false;
            //ddlLedger.Text=ddlLedger.Text.ToLower().Trim().Replace(v.LEDGERNAME.ToLower().Trim(), "");
        }

        private void ddlLedger_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
