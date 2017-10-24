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
            try
            {
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
                //cmd = new SqlCommand(" SELECT ROW_NUMBER() OVER(ORDER BY MEMBER_NAME ASC) AS RNO,MEMBER_ID,MEMBER_NAME,ISNULL(MEMBERTYPE_NAME,'')MEMBERTYPE_NAME,\r" +
                //                                           " CASE WHEN ISNULL(ICNO_NEW, '')<>'' THEN ISNULL(ICNO_NEW,'') ELSE ISNULL(ICNO_OLD,'') END ICNO_NEW,\r" +
                //                                           " BANK_USERCODE+'/'+BRANCHUSERCODE BANK_USERCODE,DATEOFJOINING,LEVY,TDF,LASTPAYMENT_DATE,\r" +
                //                                           " ISNULL(TOTALMOTHSDUE,0)TOTALMOTHSDUE,ISNULL(SEX,'')SEX,DATEOFJOINING\r" +
                //                                           " FROM TEMPVIEWMASTERMEMBER(NOLOCK) WHERE " + qry +
                //                                           " ORDER BY MEMBER_NAME", con);
                cmd = new SqlCommand("SPMEMBERSTATUS", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ENTRYDATE", dtpToDate.SelectedDate);
                cmd.Parameters.AddWithValue("@MEMBERSTATUSCODE", 3);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);

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
            return dt;
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


