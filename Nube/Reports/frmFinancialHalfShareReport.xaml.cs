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

namespace Nube.Reports
{
    /// <summary>
    /// Interaction logic for frmFinancialHalfShareReport.xaml
    /// </summary>
    public partial class frmFinancialHalfShareReport : MetroWindow
    {
        DataTable dt = new DataTable();
        public frmFinancialHalfShareReport()
        {
            InitializeComponent();
        }

        #region Button Events

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(dtpDate.Text))
                {
                    MessageBox.Show("Date is Empty!");
                    dtpDate.Focus();
                }
                else
                {
                    dt.Rows.Clear();

                    progressBar1.Minimum = 1;
                    progressBar1.Maximum = 10;
                    progressBar1.Visibility = Visibility.Visible;
                    DateTime date = dtpDate.SelectedDate.Value;
                    using (SqlConnection con = new SqlConnection(AppLib.connStr))
                    {
                        SqlCommand cmd;
                        DateTime FEEDATE = Convert.ToDateTime(dtpDate.SelectedDate);
                        FEEDATE = new DateTime(FEEDATE.Year, FEEDATE.Month, 1);
                        progressBar1.Value = 5;
                        System.Windows.Forms.Application.DoEvents();
                        //                string str=string.Format("DECLARE @FEEDATE DATETIME='{0:dd/MMM/yyyy}' \r" +
                        //" SELECT NUBEBANCHNAME [NUBEBANCHNAME],ROUND(SUM(TOTALAMOUNT),2)TOTAL,ROUND(SUM(AMOUNTBF),2)BF,ROUND(SUM(AmtIns),2)AmtIns, \r" +
                        //                        " ROUND(SUM(AMTSUBS),2)SUBS, ROUND((ROUND(SUM(AMTSUBS),2) / 2),2)[HALFSHARE], \r" +
                        //                        " ROUND((ROUND((ROUND(SUM(AMTSUBS),2) / 2),2) * 0.1),2)[FUND], ROUND(ROUND((ROUND(SUM(AMTSUBS),2) / 2),2) - ROUND((ROUND((ROUND(SUM(AMTSUBS),2) / 2),2) * 0.1),2),2)[TOTALAMOUNT] \r" +
                        //                        " FROM( \r" +
                        //                        " SELECT DISTINCT T.FEEID, T.DETAILID, T.MEMBERCODE, \r" +
                        //                        " CASE WHEN ISNULL(NB.NUBE_BRANCH_NAME, '') <> '' THEN ISNULL(NB.NUBE_BRANCH_NAME, '') ELSE ISNULL(ST.NUBEBRANCH_NAME, '') END \r" +
                        //" NUBEBANCHNAME,T.FEEDATE, ISNULL(T.STATUS, '')STATUS, TOTALAMOUNT, AMTSUBS, AMOUNTBF,AmtIns \r" +
                        //                        " FROM(SELECT FM.FEEID, FD.DETAILID, FD.MEMBERCODE, FM.FEEDATE, FD.STATUS, \r" +
                        //" SUM(FD.TOTALAMOUNT)TOTALAMOUNT, SUM(FD.AMOUNTBF)AMOUNTBF,SUM(ISNULL(FD.UNIONCONTRIBUTION, 0)) AmtIns, (SUM(FD.AMTSUBS))AMTSUBS \r" +
                        //                        " FROM FEESDETAILS FD(NOLOCK) \r" +
                        //                        " LEFT JOIN FEESMASTER FM(NOLOCK) ON FM.FEEID = FD.FEEID \r" +
                        //                        " WHERE FM.FEEDATE=@FEEDATE AND FD.ISNOTMATCH = 0 \r" +
                        //                        " GROUP BY FM.FEEID, FD.DETAILID, FD.MEMBERCODE, FM.FEEDATE, FD.STATUS \r" +
                        //                        " ) T \r" +
                        //                        " LEFT JOIN NUBESTATUS..STATUS{0:MMyyyy} MM(NOLOCK) ON MM.MEMBER_CODE = T.MEMBERCODE \r" +
                        //                        " LEFT JOIN MASTERBANKBRANCH BB(NOLOCK) ON BB.BANKBRANCH_CODE = BRANCH_CODE \r" +
                        //                        " LEFT JOIN MASTERNUBEBRANCH NB(NOLOCK) ON NB.NUBE_BRANCH_CODE = BB.NUBE_BRANCH_CODE \r" +
                        //                        " LEFT JOIN MEMBERSTATUSLOG ST(NOLOCK) ON ST.MEMBER_CODE = T.MEMBERCODE \r" +
                        //                        " )T \r" +
                        //                        " GROUP BY NUBEBANCHNAME \r" + 
                        //                        " ORDER BY NUBEBANCHNAME", FEEDATE);

                        string str = $"DECLARE @HSDate DATETIME='{FEEDATE:yyyy/MM/dd}' " +
                                     " select NUBEBANCHNAME, TotalSubs+TotalBF + TotalIns as Total, TotalBF as BF, TotalIns as AmtIns,TotalSubs As Subs, round(TotalSubs / 2,2) as HalfShare,round((round(TotalSubs / 2,2)) / 10,2) as Fund, round(TotalSubs / 2,2) - round((round(TotalSubs / 2,2)) / 10,2) as TotalAmount from(select nb.NUBE_BRANCH_NAME as NUBEBANCHNAME, sum(mmes.TOTALSUBCRP_AMOUNT) as TotalSubs, sum(mmes.TOTALBF_AMOUNT) as TotalBF, sum(mmes.TotalInsurance_Amount) as TotalIns   from MemberMonthEndStatus mmes " +
                                     " left join MasterBankBranch mbb on mmes.BRANCH_CODE = mbb.BANKBRANCH_CODE " +
                                     " left join MASTERNUBEBRANCH nb on nb.NUBE_BRANCH_CODE = mbb.NUBE_BRANCH_CODE " +
                                     " where year(mmes.StatusMonth) = year(@HSDate) and month(mmes.statusMonth) = MONTH(@HSDate) and STATUS_CODE in (1, 2) " +
                                     " group by nb.nube_branch_name) HS";
                        cmd = new SqlCommand(str, con);
                        //cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.Add(new SqlParameter("@FEEDATE", FEEDATE));
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        progressBar1.Value = 7;
                        System.Windows.Forms.Application.DoEvents();
                        adp.SelectCommand.CommandTimeout = 0;
                        adp.Fill(dt);
                        if (dt.Rows.Count != 0)
                        {
                            HalfShareReport.Reset();
                            ReportDataSource masterData = new ReportDataSource("HalfShare", dt);
                            HalfShareReport.LocalReport.DataSources.Add(masterData);//Month
                            HalfShareReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.HalfShare.rdlc";
                            ReportParameter[] rp = new ReportParameter[1];
                            rp[0] = new ReportParameter("Month", string.Format("{0:MMMyyyy}", FEEDATE));
                            HalfShareReport.LocalReport.SetParameters(rp);
                            HalfShareReport.RefreshReport();

                            dgvHalfShare.ItemsSource = dt.DefaultView;

                            progressBar1.Value = 10;
                            System.Windows.Forms.Application.DoEvents();
                        }
                        else
                        {
                            MessageBox.Show("No records found");
                            progressBar1.Value = 0;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {
                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.InitialDirectory = Environment.CurrentDirectory;
                    dlg.Title = "Half Share";
                    dlg.DefaultExt = ".xlsx";
                    dlg.Filter = "XL files|*.xls;*.xlsx|All files|*.*";
                    dlg.FileName = "HalfShare";
                    if (dlg.ShowDialog() == true)
                    {
                        Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
                        Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
                        Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
                        app.Visible = true;
                        worksheet = workbook.Sheets["Sheet1"];

                        worksheet = workbook.ActiveSheet;
                        worksheet.Name = "Half Share Report";

                        worksheet.Cells[3, 4] = "HALF SHARE REPORT ";
                        worksheet.Cells[3, 4].Font.Bold = true;
                        worksheet.Cells[3, 4].Font.Size = 14;
                        worksheet.Columns["C:C"].ColumnWidth = 18.43;
                        worksheet.Columns["G:G"].ColumnWidth = 10.43;
                        worksheet.Columns["I:I"].ColumnWidth = 14.29;
                        worksheet.Range["c3: i3"].Merge();
                        worksheet.Cells[3, 4].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        for (int i = 1; i < dt.Columns.Count + 1; i++)
                        {
                            worksheet.Cells[5, i + 2] = dt.Columns[i - 1].ColumnName;
                            worksheet.Cells[5, i + 2].Font.Bold = true;
                            if (worksheet.Cells[5, i + 2].text == "NUBEBANCHNAME")
                            {
                                worksheet.Cells[5, i + 2].Replace("NUBEBANCHNAME", "NUBE BRANCH NAME");
                            }
                        }

                        for (int i = 0; i < dt.Rows.Count ; i++)
                        {
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                worksheet.Cells[i + 6, j + 3] = dt.Rows[i][j].ToString();
                            }
                        }

                        workbook.SaveAs(dlg.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                        app.Quit();
                        MessageBox.Show("Exported Sucessfully !", "Sucess");
                    }
                }
                else
                {
                    MessageBox.Show("No Data Found !", "Empty");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            dtpDate.Text = "";
            dt.Rows.Clear();
            dgvHalfShare.ItemsSource = null;
            HalfShareReport.Reset();
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            frmHomeReports frm = new frmHomeReports();
            this.Close();
            frm.ShowDialog();
        }

        #endregion

    }
}
