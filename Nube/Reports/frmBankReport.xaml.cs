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
using Nube.MasterSetup;

namespace Nube.Reports
{
    /// <summary>
    /// Interaction logic for frmBankReport.xaml
    /// </summary>
    public partial class frmBankReport : MetroWindow
    {
        string connStr =AppLib.connStr;
        nubebfsEntity db = new nubebfsEntity();

        public frmBankReport()
        {
            InitializeComponent();
            var bank = db.MASTERBANKs.ToList();
            cmbBank.ItemsSource = bank.ToList();
            cmbBank.SelectedValuePath = "BANK_NAME";
            cmbBank.DisplayMemberPath = "BANK_NAME";
            LoadReport();
            this.KeyDown += new System.Windows.Input.KeyEventHandler(Window_KeyDown);
        }

        //User defined 
        private void LoadReport()
        {
            try
            {
                ReportViewer.Reset();
                DataTable dt = GetData();
                ReportDataSource masterData = new ReportDataSource("ViewBank", dt);

                ReportViewer.LocalReport.DataSources.Add(masterData);
                ReportViewer.LocalReport.ReportEmbeddedResource = "Nube.Reports.rptBankReport.rdlc";
              
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
            try
            {
                
               
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    if (cmbBank.Text != "")
                    {
                        string b = cmbBank.Text;
                        SqlCommand cmd1 = new SqlCommand("Select * from ViewBank where BANK_NAME='" + b + "'", conn);
                        SqlDataAdapter sdp1 = new SqlDataAdapter(cmd1);
                        sdp1.Fill(dt);
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("Select * from ViewBank ", conn);
                        SqlDataAdapter sdp = new SqlDataAdapter(cmd);
                        sdp.Fill(dt);
                    }

                }
                
            }
            catch(Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
            return dt;
        }

        //button events
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            //frmBankSetup frm = new frmBankSetup();
            //frm.Show();
            this.Close();
        }

        //Closing Events
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                this.Close();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure to exit?", "Exit Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

    }
}
