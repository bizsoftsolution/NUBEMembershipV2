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
    /// Interaction logic for frmHalfShareReport.xaml
    /// </summary>
    public partial class frmHalfShareReport : MetroWindow
    {      
        string mon = "";
        string connStr =AppLib.connStr;
        nubebfsEntity db = new nubebfsEntity();
        
        public frmHalfShareReport()
        {
            InitializeComponent();           
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {         
            dtpDate.SelectedDate = Convert.ToDateTime(DateTime.Now);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            frmHomeReports frm = new frmHomeReports();
            this.Close();
            frm.ShowDialog();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {           
            if (dtpDate.Text == "")
            {
                MessageBox.Show("Enter date");
            }
            else
            {
                try
                {
                    mon = dtpDate.SelectedDate.Value.Month.ToString();
                    MemberReport.Reset();
                    DataTable dt = getData();
                    ReportDataSource masterData = new ReportDataSource("HalfShare", dt);

                    MemberReport.LocalReport.DataSources.Add(masterData);
                    MemberReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.HalfShare.rdlc";
                    ReportParameter rp = new ReportParameter("Month", String.Format("{0:MMM-yyyy}", dtpDate.SelectedDate.Value));
                    MemberReport.LocalReport.SetParameters(rp);
                  
                    MemberReport.RefreshReport();
                }
                catch (Exception ex)
                {
                    ExceptionLogging.SendErrorToText(ex);
                }
            }
        }

        private DataTable getData()
        {

            DateTime date = dtpDate.SelectedDate.Value;
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd;
                string FEEDATE = string.Format("{0:ddMMMyyyy}", dtpDate.SelectedDate.Value);
                cmd = new SqlCommand("SPHALFSHARE", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@FEEDATE", FEEDATE));
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.SelectCommand.CommandTimeout = 0;
                adp.Fill(dt);                              
               // qry = String.Format("SELECT NubeBanchName,SUM(AmtSubs) AS Subs,SUM(AmtBF) AS BF FROM ViewMasterFeeDetails WHERE (FeeMonth = N'{0:MM}') AND (FeeYear = N'{1:yyyy}') GROUP BY NubeBanchName", dtpDate.SelectedDate.Value, dtpDate.SelectedDate.Value);                               
            }
            return dt;

        }        
        
    }
}
