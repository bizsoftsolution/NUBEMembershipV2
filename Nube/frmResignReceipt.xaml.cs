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
    /// Interaction logic for frmResignReport.xaml
    /// </summary>
    public partial class frmResignReport : MetroWindow
    {
        decimal dMemberID = 0;
        string sReason = "";
        string connStr = AppLib.connStr;
        public frmResignReport(decimal dMember, string Reason)
        {
            InitializeComponent();
            dMemberID = dMember;
            sReason = Reason;
            LoadReport();
        }
        private void LoadReport()
        {
            try
            {
                ReportViewer.Reset();
                DataTable dt = GetData();
                ReportDataSource masterData = new ReportDataSource("Resign", dt);

                ReportViewer.LocalReport.DataSources.Add(masterData);
                ReportViewer.LocalReport.ReportEmbeddedResource = "Nube.Reports.ResignReport.rdlc";
                ReportParameter insAmt = new ReportParameter("InsAmt", (dt.Rows[0]["InsuranceAmount"]).ToString());
                ReportViewer.LocalReport.SetParameters(insAmt);

                if (Convert.ToInt32(dt.Rows[0]["ACCBENEFIT"]) != 0)
                {
                    ReportParameter RSN = new ReportParameter("Benifit", (dt.Rows[0]["ACCBENEFIT"]).ToString());
                    ReportViewer.LocalReport.SetParameters(RSN);
                }
                else
                {
                    ReportParameter RSN = new ReportParameter("Benifit", "");
                    ReportViewer.LocalReport.SetParameters(RSN);
                }
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
                string qry = "SELECT * FROM VIEWRESIGN(NOLOCK) WHERE MEMBER_CODE='" + dMemberID + "'";
                SqlCommand cmd = new SqlCommand(qry, conn);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
            }
            return dt;
        }
    }
}
