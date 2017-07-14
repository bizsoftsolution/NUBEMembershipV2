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
    /// Interaction logic for frmMemberCard.xaml
    /// </summary>
    public partial class frmMemberCard : MetroWindow
    {
        string connStr =AppLib.connStr;
        public frmMemberCard()
        {
            InitializeComponent();
            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                ReportViewer.Reset();
                DataTable dt = GetData();
                ReportDataSource masterData = new ReportDataSource("MasterMember", dt);

                ReportViewer.LocalReport.DataSources.Add(masterData);
                ReportViewer.LocalReport.ReportEmbeddedResource = "Nube.MemberCard.rdlc";
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

                string qry = "Select * from TEMPVIEWMASTERMEMBER";
                SqlCommand cmd = new SqlCommand(qry, conn);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
            }

            return dt;
        }
    }
}
