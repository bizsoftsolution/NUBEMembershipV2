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

namespace Nube.Reports
{
    /// <summary>
    /// Interaction logic for frmStateReport.xaml
    /// </summary>
    public partial class frmStateReport : MetroWindow
    {
        string connStr =AppLib.connStr;
        nubebfsEntity db = new nubebfsEntity();
        public frmStateReport()
        {
            InitializeComponent();
            var country = db.CountrySetups.ToList();
            cmbCountry.ItemsSource = country.ToList();
            cmbCountry.SelectedValuePath = "CountryName";
            cmbCountry.DisplayMemberPath = "CountryName";
            //LoadReport();
            this.KeyDown += new System.Windows.Input.KeyEventHandler(Window_KeyDown);
        }

        //Closing Events
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                this.Close();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure to exit?", "Exit Confirmation", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }

        //Button Events

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //User defined 
        private void LoadReport()
        {
            try
            {
                ReportViewer.Reset();
                DataTable dt = GetData();
                ReportDataSource masterData = new ReportDataSource("CountrySetup", dt);

                ReportViewer.LocalReport.DataSources.Add(masterData);
                ReportViewer.LocalReport.ReportEmbeddedResource = "Nube.Reports.rptStateReport.rdlc";
                ReportViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(StateDetail);

                ReportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private DataTable GetData()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                if (cmbCountry.Text != "")
                {
                    string c = cmbCountry.Text;
                    SqlCommand cmd = new SqlCommand("Select * from CountrySetup where CountryName='" + c + "'", conn);
                    SqlDataAdapter sdp = new SqlDataAdapter(cmd);
                    sdp.Fill(dt);
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("Select * from CountrySetup", conn);
                    SqlDataAdapter sdp = new SqlDataAdapter(cmd);
                    sdp.Fill(dt);
                }
            }
            return dt;
        }

        private void StateDetail(object sender, SubreportProcessingEventArgs e)
        {
            int c = int.Parse(e.Parameters["Country"].Values[0]);
            DataTable dt = GetDetails(c);
            ReportDataSource rs = new ReportDataSource("State", dt);
            e.DataSources.Add(rs);
        }

        private DataTable GetDetails(int c)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string qry = "Select * from MasterState as MS Left join CountrySetup as CS on MS.Country=CS.ID where MS.Country=" + c + "";
                SqlCommand cmd = new SqlCommand(qry, conn);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
            }
            return dt;
        }
      
    }
}


