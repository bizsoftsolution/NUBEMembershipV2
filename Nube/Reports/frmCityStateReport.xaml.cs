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
    /// Interaction logic for frmCityStateReport.xaml
    /// </summary>
    public partial class frmCityStateReport : MetroWindow
    {

        string connStr = AppLib.connStr;
        nubebfsEntity db = new nubebfsEntity();

        public frmCityStateReport()
        {
            InitializeComponent();
            var State = db.MASTERSTATEs.ToList();
            cmbState.ItemsSource = State.ToList();
            cmbState.SelectedValuePath = "STATE_NAME";
            cmbState.DisplayMemberPath = "STATE_NAME";
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
                Report.Reset();
                DataTable dt = GetData();
                ReportDataSource masterData = new ReportDataSource("MasterState", dt);

                Report.LocalReport.DataSources.Add(masterData);
                Report.LocalReport.ReportEmbeddedResource = "Nube.Reports.StateReport.rdlc";
                Report.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(CityDetails);

                Report.RefreshReport();


            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private DataTable GetData1()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {

                SqlCommand cmd = new SqlCommand("Select * from MASTERCITY order by CITY_NAME", conn);
                SqlDataAdapter sdp = new SqlDataAdapter(cmd);
                sdp.Fill(dt);

            }
            return dt;
        }

        private void CityDetails(object sender, SubreportProcessingEventArgs e)
        {
            int c = int.Parse(e.Parameters["StateCode"].Values[0]);
            DataTable dt = GetDetails(c);
            ReportDataSource rs = new ReportDataSource("MasterCity", dt);
            e.DataSources.Add(rs);
        }

        private DataTable GetData()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                if (cmbState.Text != "")
                {
                    string c = cmbState.Text;
                    SqlCommand cmd = new SqlCommand("Select * from MASTERSTATE where STATE_NAME='" + c + "' order by STATE_NAME", conn);
                    SqlDataAdapter sdp = new SqlDataAdapter(cmd);
                    sdp.Fill(dt);
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("Select * from MASTERSTATE order by STATE_NAME", conn);
                    SqlDataAdapter sdp = new SqlDataAdapter(cmd);
                    sdp.Fill(dt);
                }
            }
            return dt;
        }

        private DataTable GetDetails(int c)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string qry = "Select * from MasterCity as MC Left join MasterCity as MS on MC.STATE_CODE=MS.STATE_CODE where MS.STATE_CODE=" + c + " order by MC.CITY_NAME";
                SqlCommand cmd = new SqlCommand(qry, conn);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
            }
            return dt;
        }

    }

}
