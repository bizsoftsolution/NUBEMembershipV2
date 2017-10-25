using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Runtime.InteropServices;
using System.Reflection;
using Microsoft.Reporting.WinForms;

namespace Nube.Reports
{
    /// <summary>
    /// Interaction logic for frmBranchStatisticsReport.xaml
    /// </summary>
    public partial class frmBranchStatisticsReport : MetroWindow
    {

        nubebfsEntity db = new nubebfsEntity();
        string Qry = "";
        DataTable dtBankStatistic = new DataTable();
        DataTable dtEXBANK = new DataTable();
        DataTable dtEXNUBE = new DataTable();

        ObservableCollection<branchStatistics> lstBranchStatistic = new ObservableCollection<branchStatistics>();

        public frmBranchStatisticsReport()
        {
            InitializeComponent();
            try
            {
                cmbBankName.ItemsSource = (from x in db.MASTERNUBEBRANCHes select x).ToList();
                cmbBankName.SelectedValuePath = "NUBE_BRANCH_CODE";
                cmbBankName.DisplayMemberPath = "NUBE_BRANCH_NAME";
                dgvBankStatistics.ItemsSource = lstBranchStatistic;

                lblState.Visibility = Visibility.Collapsed;
                chkMelaka.Visibility = Visibility.Collapsed;
                chkNegeriSembilan.Visibility = Visibility.Collapsed;

                var bank = db.MASTERBANKs.OrderBy(x => x.BANK_NAME).ToList();
                cmbBank.ItemsSource = bank;
                cmbBank.SelectedValuePath = "BANK_CODE";
                cmbBank.DisplayMemberPath = "BANK_NAME";
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        #region BUTTON EVENTS

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            GetDetails();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            frmHomeReports frm = new frmHomeReports();
            this.Close();
            frm.ShowDialog();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtEXBANK.Rows.Count > 0)
                {
                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.InitialDirectory = Environment.CurrentDirectory;
                    dlg.Title = "Branch Statistics Report";
                    dlg.DefaultExt = ".xlsx";
                    dlg.Filter = "XL files|*.xls;*.xlsx|All files|*.*";
                    dlg.FileName = "Branch Statistics Report BANK BRANCH";
                    if (dlg.ShowDialog() == true)
                    {
                        Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
                        Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
                        Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
                        app.Visible = true;
                        worksheet = workbook.Sheets["Sheet1"];

                        worksheet = workbook.ActiveSheet;
                        worksheet.Name = " BANK BRANCH";
                        worksheet.Cells[2, 4] = string.Format("STATISTICS REPORT FOR {0:MMM-yy}", dtpDOB.SelectedDate);

                        if (!string.IsNullOrEmpty(cmbBankName.Text))
                        {
                            worksheet.Cells[3, 4] = "NUBE BRANCH - " + cmbBankName.Text;
                        }
                        else
                        {
                            worksheet.Cells[3, 4] = "OVER ALL BANK BRANCH";
                        }

                        worksheet.Cells[3, 4].Font.Bold = true;
                        for (int i = 1; i < dtEXBANK.Columns.Count + 1; i++)
                        {
                            if (i == 1)
                            {
                                worksheet.Cells[5, i + 2] = dtEXBANK.Columns[i - 1].ColumnName;
                                worksheet.Cells[5, i + 2].Font.Bold = true;
                            }
                            else if (i == 3)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "MM";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 4)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "MC";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 5)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "MI";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 6)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "MO";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 7)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "S.Total";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 8)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "FM";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 9)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "FC";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 10)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "FI";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 11)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "FO";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 12)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "S.Total";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 13)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "Total";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 14)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "MM";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 15)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "MC";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 16)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "MI";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 17)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "MO";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 18)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "S.Total";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 19)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "FM";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 20)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "FC";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 21)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "FI";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 22)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "FO";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 23)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "S.Total";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 24)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "Total";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                            else if (i == 25)
                            {
                                worksheet.Cells[5, (i + 2) - 1] = "G.Total";
                                worksheet.Cells[5, (i + 2) - 1].Font.Bold = true;
                            }
                        }

                        for (int i = 0; i < dtEXBANK.Rows.Count - 1; i++)
                        {

                            for (int j = 0; j < dtEXBANK.Columns.Count; j++)
                            {
                                if (j == 0)
                                {

                                    worksheet.Cells[i + 6, j + 3] = dtEXBANK.Rows[i][j].ToString();
                                }
                                else if (j != 1)
                                {
                                    worksheet.Cells[i + 6, j + 2] = dtEXBANK.Rows[i][j].ToString();
                                }
                            }
                        }
                        workbook.SaveAs(dlg.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                        app.Quit();
                        MessageBox.Show("Exported Sucessfully !", "Sucess");
                    }


                    if (string.IsNullOrEmpty(cmbBankName.Text))
                    {
                        if (MessageBox.Show("Do You want to Print NUBE Branch Report?", " Export Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {

                            Microsoft.Win32.SaveFileDialog dlg2 = new Microsoft.Win32.SaveFileDialog();
                            dlg2.InitialDirectory = Environment.CurrentDirectory;
                            dlg2.Title = "Branch Statistics Report";
                            dlg2.DefaultExt = ".xlsx";
                            dlg2.Filter = "XL files|*.xls;*.xlsx|All files|*.*";
                            dlg2.FileName = "Branch Statistics Report NUBE Branch";
                            if (dlg2.ShowDialog() == true)
                            {

                                Microsoft.Office.Interop.Excel._Application app2 = new Microsoft.Office.Interop.Excel.Application();
                                Microsoft.Office.Interop.Excel._Workbook workbook2 = app2.Workbooks.Add(Type.Missing);
                                Microsoft.Office.Interop.Excel._Worksheet worksheet2 = null;
                                app2.Visible = true;
                                worksheet2 = workbook2.Sheets["Sheet1"];

                                worksheet2 = workbook2.ActiveSheet;
                                worksheet2.Name = " NUBE BRANCH";
                                worksheet2.Cells[2, 4] = string.Format("STATISTICS REPORT FOR {0:MMM-yy}", dtpDOB.SelectedDate);

                                if (!string.IsNullOrEmpty(cmbBankName.Text))
                                {
                                    worksheet2.Cells[3, 4] = "NUBE BRANCH - " + cmbBankName.Text;
                                }
                                else
                                {
                                    worksheet2.Cells[3, 4] = "OVER ALL BANK BRANCH";
                                }

                                worksheet2.Cells[3, 4].Font.Bold = true;
                                for (int i = 1; i < dtEXNUBE.Columns.Count + 1; i++)
                                {
                                    //worksheet2.Cells[5, i + 2] = dtEXNUBE.Columns[i - 1].ColumnName;
                                    //worksheet2.Cells[5, i + 2].Font.Bold = true;

                                    if (i == 1)
                                    {
                                        worksheet2.Cells[5, i + 2] = dtEXNUBE.Columns[i - 1].ColumnName;
                                        worksheet2.Cells[5, i + 2].Font.Bold = true;
                                    }
                                    else if (i == 3)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "MM";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 4)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "MC";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 5)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "MI";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 6)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "MO";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 7)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "S.Total";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 8)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "FM";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 9)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "FC";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 10)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "FI";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 11)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "FO";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 12)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "S.Total";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 13)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "Total";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 14)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "MM";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 15)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "MC";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 16)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "MI";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 17)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "MO";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 18)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "S.Total";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 19)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "FM";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 20)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "FC";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 21)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "FI";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 22)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "FO";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 23)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "S.Total";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 24)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "Total";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                    else if (i == 25)
                                    {
                                        worksheet2.Cells[5, (i + 2) - 1] = "G.Total";
                                        worksheet2.Cells[5, (i + 2) - 1].Font.Bold = true;
                                    }
                                }

                                for (int i = 0; i < dtEXNUBE.Rows.Count - 1; i++)
                                {

                                    for (int j = 0; j < dtEXNUBE.Columns.Count; j++)
                                    {
                                        if (j == 0)
                                        {

                                            worksheet2.Cells[i + 6, j + 3] = dtEXNUBE.Rows[i][j].ToString();
                                        }
                                        else if (j != 1)
                                        {
                                            worksheet2.Cells[i + 6, j + 2] = dtEXNUBE.Rows[i][j].ToString();
                                        }
                                    }
                                }

                                workbook2.SaveAs(dlg2.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                                app2.Quit();
                                MessageBox.Show("NUBE BRANCH Exported Sucessfully !", "Sucess");
                            }
                        }
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

        #endregion

        #region USERDEFINED FUNCTIONS

        private void GetDetails()
        {
            DataTable dt = new DataTable();
            dtEXBANK.Rows.Clear();
            dtEXNUBE.Rows.Clear();

            string dateMonth = string.Format("{0:MMyyyy}", dtpDOB.SelectedDate.Value);
            using (SqlConnection con = new SqlConnection(AppLib.connStr))
            {
                con.Open();
                string sWhere = "";
                if (!string.IsNullOrEmpty(cmbBankName.Text))
                {
                    sWhere = " AND ST.NUBEBANCHCODE=" + cmbBankName.SelectedValue;
                }
                else
                {
                    sWhere = "";
                }

                if (!string.IsNullOrEmpty(cmbBank.Text))
                {
                    sWhere = sWhere + " AND ST.BANK_NAME='" + cmbBank.Text + "'";
                }


                if (!string.IsNullOrEmpty(cmbBranch.Text))
                {
                    sWhere = sWhere + " AND ST.BRANCHNAME='" + cmbBranch.Text + "'";
                }


                if (Convert.ToInt32(cmbBankName.SelectedValue) == 11)
                {
                    if (chkMelaka.IsChecked == true && chkNegeriSembilan.IsChecked == false)
                    {
                        sWhere = sWhere + " AND (BRANCHSTATE LIKE '%MELAKA%') ";
                    }
                    else if (chkMelaka.IsChecked == false && chkNegeriSembilan.IsChecked == true)
                    {
                        sWhere = sWhere + " AND (BRANCHSTATE NOT LIKE '%MELAKA%') ";
                    }
                }
                //Qry = String.Format("select isnull(nb.NUBE_BRANCH_NAME,'') AS NUBE_BRANCH_NAME,isnull(bk.BANK_NAME,'') AS BANK_NAME,isnull(bk.BANK_USERCODE,'')+'_'+isnull(br.BANKBRANCH_USERCODE,'') as BankBranch,isnull(mm.SEX,'') as SEX,isnull(mm.RACE_CODE,0) as RACE_CODE,isnull(mm.MEMBERTYPE_CODE,0) as MEMBERTYPE_CODE from {1}.dbo.STATUS{2} as st left join {0}.dbo.MASTERMEMBER as mm on mm.MEMBER_CODE=st.MEMBER_CODE left join {0}.dbo.MASTERBANK as bk on st.BANK_CODE = bk.BANK_CODE left join {0}.dbo.MASTERNUBEBRANCH as nb on nb.NUBE_BRANCH_CODE=st.NUBE_BRANCH_CODE left join {0}.dbo.MASTERBANKBRANCH as br on br.BANKBRANCH_CODE=st.BRANCH_CODE  where st.RESIGNED=0 " + sWhere , AppLib.DBBFS, AppLib.DBStatus, dateMonth);

                //Qry = String.Format("SELECT ISNULL(NB.NUBE_BRANCH_NAME,'') AS NUBE_BRANCH_NAME,ISNULL(BK.BANK_NAME,'') AS BANK_NAME, \r" +
                //    " ISNULL(BK.BANK_USERCODE, '') + '_' + ISNULL(BR.BANKBRANCH_USERCODE, '') AS BANKBRANCH, \r" +
                //    " ISNULL(MM.SEX, '') AS SEX, ISNULL(MM.RACE_CODE, 0) AS RACE_CODE,\r" +
                //    " (CASE WHEN ST.TOTALDUEPAY <= 5 THEN 1.0 ELSE 2.0  END) STATUS \r" +
                //    " FROM VIEWMEMBERTOTALMONTHSDUE ST (NOLOCK) \r" +
                //    " LEFT JOIN DBO.MASTERMEMBER AS MM(NOLOCK) ON MM.MEMBER_CODE = ST.MEMBER_CODE \r" +
                //    " LEFT JOIN DBO.MASTERBANK AS BK(NOLOCK) ON MM.BANK_CODE = BK.BANK_CODE \r" +
                //    " LEFT JOIN DBO.MASTERBANKBRANCH AS BR(NOLOCK) ON BR.BANKBRANCH_CODE = MM.BRANCH_CODE \r" +
                //    " LEFT JOIN DBO.MASTERNUBEBRANCH AS NB(NOLOCK) ON NB.NUBE_BRANCH_CODE = BR.NUBE_BRANCH_CODE " + sWhere, dateMonth);

                Qry = string.Format(" SELECT ISNULL(ST.NUBEBANCHNAME,'') AS NUBE_BRANCH_NAME,ISNULL(ST.BANK_NAME,'') AS BANK_NAME, \r" +
                " ISNULL(ST.BANK_USERCODE, '') + '_' + ISNULL(ST.BRANCHUSERCODE, '') AS BANKBRANCH, \r" +
                " ISNULL(ST.SEX, '') AS SEX, ISNULL(ST.RACE_CODE, 0) AS RACE_CODE, \r" +
                " (CASE WHEN ST.MEMBERSTATUSCODE = 1 THEN 1.0 ELSE 2.0  END) STATUS \r" +
                " FROM TEMPVIEWMASTERMEMBER ST (NOLOCK) \r" +
                " --FROM VIEWMASTERMEMBER ST (NOLOCK) \r" +
                " WHERE ST.MEMBERSTATUSCODE IN(1, 2) AND ST.DATEOFJOINING < '2017-10-01' " + sWhere);
                //

                //Qry = String.Format("select isnull(nb.NUBE_BRANCH_NAME,'') AS NUBE_BRANCH_NAME,isnull(bk.BANK_NAME,'') AS BANK_NAME,isnull(bk.BANK_USERCODE,'')+'_'+isnull(br.BANKBRANCH_USERCODE,'') as BankBranch,isnull(mm.SEX,'') as SEX,isnull(mm.RACE_CODE,0) as RACE_CODE,isnull(mm.MEMBERTYPE_CODE,0) as MEMBERTYPE_CODE from {1}.dbo.STATUS{2} as st left join {0}.dbo.MASTERMEMBER as mm on mm.MEMBER_CODE=st.MEMBER_CODE left join {0}.dbo.MASTERBANK as bk on st.BANK_CODE = bk.BANK_CODE left join {0}.dbo.MASTERNUBEBRANCH as nb on nb.NUBE_BRANCH_CODE=br.NUBE_BRANCH_CODE left join {0}.dbo.MASTERBANKBRANCH as br on br.BANKBRANCH_CODE=mm.BRANCH_CODE  where mm.RESIGNED=0", AppLib.DBBFS);
                SqlCommand cmd = new SqlCommand(Qry, con);
                SqlDataAdapter sdp = new SqlDataAdapter(cmd);
                sdp.SelectCommand.CommandTimeout = 0;
                sdp.Fill(dt);
            }
            if (dt.Rows.Count > 0)
            {
                var datas = (from DataRow row in dt.Rows
                             select new
                             {
                                 Bank_Name = (string)(row["BANK_NAME"] ?? ""),
                                 Nube_branch_code = (string)(row["NUBE_BRANCH_NAME"] ?? ""),
                                 Branch_Code = (string)(row["BANKBRANCH"] ?? ""),
                                 SEX = (string)(row["SEX"] ?? ""),
                                 RACE_CODE = (decimal)(row["RACE_CODE"]),
                                 MEMBERTYPE_CODE = (decimal)(row["STATUS"])
                             }).ToList();

                var BankBranchDatas1 = datas.GroupBy(x => x.Branch_Code).ToList();
                var BankBranchdatas2 = BankBranchDatas1.Select(x => new branchStatistics
                {

                    BranchCode = x.Key.ToString(),

                    CMM = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 1 && y.MEMBERTYPE_CODE == 1).Count(),
                    CMI = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 2 && y.MEMBERTYPE_CODE == 1).Count(),
                    CMC = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 3 && y.MEMBERTYPE_CODE == 1).Count(),
                    CMO = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 4 && y.MEMBERTYPE_CODE == 1).Count(),

                    CFM = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 1 && y.MEMBERTYPE_CODE == 1).Count(),
                    CFI = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 2 && y.MEMBERTYPE_CODE == 1).Count(),
                    CFC = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 3 && y.MEMBERTYPE_CODE == 1).Count(),
                    CFO = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 4 && y.MEMBERTYPE_CODE == 1).Count(),

                    NFM = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 1 && y.MEMBERTYPE_CODE == 2).Count(),
                    NFI = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 2 && y.MEMBERTYPE_CODE == 2).Count(),
                    NFC = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 3 && y.MEMBERTYPE_CODE == 2).Count(),
                    NFO = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 4 && y.MEMBERTYPE_CODE == 2).Count(),

                    NMM = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 1 && y.MEMBERTYPE_CODE == 2).Count(),
                    NMI = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 2 && y.MEMBERTYPE_CODE == 2).Count(),
                    NMC = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 3 && y.MEMBERTYPE_CODE == 2).Count(),
                    NMO = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 4 && y.MEMBERTYPE_CODE == 2).Count()

                });
                var BankBranchDatas3 = BankBranchdatas2.Select(x => new branchStatistics
                {
                    BranchCode = x.BranchCode,
                    CMM = x.CMM,
                    CMI = x.CMI,
                    CMC = x.CMC,
                    CMO = x.CMO,
                    CMSTOT = x.CMM + x.CMI + x.CMC + x.CMO,
                    CFC = x.CFC,
                    CFI = x.CFI,
                    CFM = x.CFM,
                    CFO = x.CFO,
                    CFSTOT = x.CFO + x.CFM + x.CFI + x.CFC,
                    CTOTAL = x.CMM + x.CMI + x.CMC + x.CMO + x.CFO + x.CFM + x.CFI + x.CFC,

                    NMM = x.NMM,
                    NMI = x.NMI,
                    NMO = x.NMO,
                    NMC = x.NMC,
                    NMSTOT = x.NMM + x.NMI + x.NMO + x.NMC,
                    NFC = x.NFC,
                    NFM = x.NFM,
                    NFI = x.NFI,
                    NFO = x.NFO,
                    NFSTOT = x.NFC + x.NFM + x.NFI + x.NFO,

                    NTOTAL = x.NMM + x.NMI + x.NMO + x.NMC + x.NFC + x.NFM + x.NFI + x.NFO,
                    GTOTAL = x.CMM + x.CMI + x.CMC + x.CMO + x.CFO + x.CFM + x.CFI + x.CFC + x.NMM + x.NMI + x.NMO + x.NMC + x.NFC + x.NFM + x.NFI + x.NFO

                }).ToList();
                dgvBankStatistics.ItemsSource = BankBranchDatas3;
                dtEXBANK = AppLib.LINQResultToDataTable(BankBranchDatas3);

                MemberReport.Reset();
                if (dtEXBANK.Rows.Count > 0)
                {
                    ReportDataSource masterData = new ReportDataSource("BANKBRANCHSTATISTICAL", dtEXBANK);

                    MemberReport.LocalReport.DataSources.Add(masterData);
                    MemberReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.rptBranchStaticsReport.rdlc";
                    ReportParameter[] NB = new ReportParameter[2];

                    NB[0] = new ReportParameter("Month", string.Format("{0:MMM - yyyy}", dtpDOB.SelectedDate));

                    if (!string.IsNullOrEmpty(cmbBank.Text))
                    {
                        NB[1] = new ReportParameter("ReportName", "NUBE BRANCH - " + cmbBank.Text.ToString());
                    }
                    else
                    {
                        NB[1] = new ReportParameter("ReportName", "OVER ALL BANK BRANCH");
                    }
                    MemberReport.LocalReport.SetParameters(NB);
                    MemberReport.RefreshReport();

                    //using (SqlConnection con = new SqlConnection(AppLib.connStr))
                    //{
                    //    SqlCommand cmd;
                    //    string str = "";
                    //    if (!string.IsNullOrEmpty(cmbBank.Text))
                    //    {
                    //        str = "";
                    //    }
                    //    else
                    //    {

                    //    }
                    //    cmd = new SqlCommand("SELECT ST.MEMBERTYPE_NAME,COUNT(*) TOTAL \r" +
                    //        " FROM TEMPVIEWMASTERMEMBER ST(NOLOCK)\r" +
                    //        " WHERE ST.MEMBERSTATUSCODE IN(1,2) AND ST.DATEOFJOINING < '2017-03-01' AND--ST.NUBEBANCHCODE = 11\r" +
                    //        " GROUP BY ST.MEMBERTYPE_NAME\r" +
                    //        " ORDER BY ST.MEMBERTYPE_NAME", con);
                    //}
                }
                //dgvBranch
                var NubeBranchDatas1 = datas.GroupBy(x => x.Nube_branch_code).ToList();
                var NubeBranchdatas2 = NubeBranchDatas1.Select(x => new branchStatistics
                {
                    BranchCode = x.Key.ToString(),

                    CMM = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 1 && y.MEMBERTYPE_CODE == 1).Count(),
                    CMI = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 2 && y.MEMBERTYPE_CODE == 1).Count(),
                    CMC = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 3 && y.MEMBERTYPE_CODE == 1).Count(),
                    CMO = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 4 && y.MEMBERTYPE_CODE == 1).Count(),

                    CFM = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 1 && y.MEMBERTYPE_CODE == 1).Count(),
                    CFI = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 2 && y.MEMBERTYPE_CODE == 1).Count(),
                    CFC = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 3 && y.MEMBERTYPE_CODE == 1).Count(),
                    CFO = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 4 && y.MEMBERTYPE_CODE == 1).Count(),

                    NFM = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 1 && y.MEMBERTYPE_CODE == 2).Count(),
                    NFI = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 2 && y.MEMBERTYPE_CODE == 2).Count(),
                    NFC = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 3 && y.MEMBERTYPE_CODE == 2).Count(),
                    NFO = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 4 && y.MEMBERTYPE_CODE == 2).Count(),

                    NMM = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 1 && y.MEMBERTYPE_CODE == 2).Count(),
                    NMI = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 2 && y.MEMBERTYPE_CODE == 2).Count(),
                    NMC = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 3 && y.MEMBERTYPE_CODE == 2).Count(),
                    NMO = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 4 && y.MEMBERTYPE_CODE == 2).Count()

                });
                var NubeBranchDatas3 = NubeBranchdatas2.Select(x => new branchStatistics
                {
                    BranchCode = x.BranchCode,
                    CMM = x.CMM,
                    CMI = x.CMI,
                    CMC = x.CMC,
                    CMO = x.CMO,
                    CMSTOT = x.CMM + x.CMI + x.CMC + x.CMO,
                    CFC = x.CFC,
                    CFI = x.CFI,
                    CFM = x.CFM,
                    CFO = x.CFO,
                    CFSTOT = x.CFO + x.CFM + x.CFI + x.CFC,
                    CTOTAL = x.CMM + x.CMI + x.CMC + x.CMO + x.CFO + x.CFM + x.CFI + x.CFC,

                    NMM = x.NMM,
                    NMI = x.NMI,
                    NMO = x.NMO,
                    NMC = x.NMC,
                    NMSTOT = x.NMM + x.NMI + x.NMO + x.NMC,
                    NFC = x.NFC,
                    NFM = x.NFM,
                    NFI = x.NFI,
                    NFO = x.NFO,
                    NFSTOT = x.NFC + x.NFM + x.NFI + x.NFO,

                    NTOTAL = x.NMM + x.NMI + x.NMO + x.NMC + x.NFC + x.NFM + x.NFI + x.NFO,
                    GTOTAL = x.CMM + x.CMI + x.CMC + x.CMO + x.CFO + x.CFM + x.CFI + x.CFC + x.NMM + x.NMI + x.NMO + x.NMC + x.NFC + x.NFM + x.NFI + x.NFO

                }).ToList();
                dgvNubeStatistics.ItemsSource = NubeBranchDatas3;
                dtEXNUBE = AppLib.LINQResultToDataTable(NubeBranchDatas3);

                MemberReport2.Reset();
                if (dtEXNUBE.Rows.Count > 0)
                {
                    ReportDataSource masterData = new ReportDataSource("NUBEBRANCHSTATISTICAL", dtEXNUBE);

                    MemberReport2.LocalReport.DataSources.Add(masterData);
                    MemberReport2.LocalReport.ReportEmbeddedResource = "Nube.Reports.rptBranchStaticsReportNubeBranch.rdlc";
                    ReportParameter[] NB2 = new ReportParameter[2];

                    NB2[0] = new ReportParameter("Month", string.Format("{0:MMM - yyyy}", dtpDOB.SelectedDate));

                    if (!string.IsNullOrEmpty(cmbBank.Text))
                    {
                        NB2[1] = new ReportParameter("ReportName", "NUBE BRANCH - " + cmbBank.Text.ToString());
                    }
                    else
                    {
                        NB2[1] = new ReportParameter("ReportName", "OVER ALL - NUBE BRANCH");
                    }
                    MemberReport2.LocalReport.SetParameters(NB2);
                    MemberReport2.RefreshReport();
                }

                //Bank

                var BankDatas1 = datas.GroupBy(x => x.Bank_Name).ToList();
                var BankDatas2 = BankDatas1.Select(x => new branchStatistics
                {

                    Bank = x.Key.ToString(),

                    CMM = x.Where(y => y.SEX == "Male" && y.MEMBERTYPE_CODE == 1).Count(),

                    CFM = x.Where(y => y.SEX == "Female" && y.MEMBERTYPE_CODE == 1).Count(),

                    NFM = x.Where(y => y.SEX == "Female" && y.MEMBERTYPE_CODE == 2).Count(),

                    NMM = x.Where(y => y.SEX == "Male" && y.MEMBERTYPE_CODE == 2).Count(),

                });
                var BankDatas3 = BankDatas2.Select(x => new branchStatistics
                {
                    Bank = x.Bank,
                    CMM = x.CMM,

                    CFM = x.CFM,

                    NMM = x.NMM,
                    NFM = x.NFM,

                    GTOTAL = x.CMM + x.CFM + x.NMM + x.NFM

                }).ToList();
                dgvBankList.ItemsSource = BankDatas3;
            }
            else
            {
                MessageBox.Show("No Records Found!");
            }


        }

        private void LoadReport()
        {
            dgvBankStatistics.ItemsSource = dtBankStatistic.DefaultView;
            var nubeBranch = db.MASTERNUBEBRANCHes.Select(x => x.NUBE_BRANCH_CODE).ToList();
        }

        private int GetData(string Gender, string Race, string BranchCode, Boolean IsBenefit, Boolean IsNubeBranch)
        {
            string dateMonth = string.Format("{0:MMyyyy}", dtpDOB.SelectedDate.Value);
            using (SqlConnection con = new SqlConnection(AppLib.connStr))
            {
                con.Open();
                Qry = String.Format("select isnull(Count(*),0) from {0}.dbo.Status{1:MMyyyy} AS ST Left Join MasterMember As MM On ST.Member_Code=MM.Member_Code where MM.Sex='{2}' And MM.Race_Code={3} And ST.Status_Code{4}{5} and ST.{6}={7}", AppLib.DBStatus, dtpDOB.SelectedDate.Value, Gender, Race, "=", (IsBenefit == true) ? 1 : 2, "Branch_Code", BranchCode);
                SqlCommand cmd = new SqlCommand(Qry, con);
                Int32 count = (Int32)cmd.ExecuteScalar();
                return count;
            }
        }

        public class MemberStatistic
        {
            public string Bank_Name { get; set; }

            public decimal Member_code { get; set; }
            public decimal Nube_branch_code { get; set; }
            public decimal Branch_Code { get; set; }
            public string SEX { get; set; }
            public decimal RACE_CODE { get; set; }
            public decimal STATUS_CODE { get; set; }
            public decimal MEMBERTYPE_CODE { get; set; }

        }

        private void dgBranchReport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        class branchStatistics
        {
            public string BranchCode { get; set; }
            public string Bank { get; set; }
            public double CMM { get; set; }
            public double CMC { get; set; }
            public double CMI { get; set; }
            public double CMO { get; set; }
            public double CMSTOT { get; set; }

            public double CFM { get; set; }
            public double CFC { get; set; }
            public double CFI { get; set; }
            public double CFO { get; set; }
            public double CFSTOT { get; set; }
            public double CTOTAL { get; set; }

            public double NMM { get; set; }
            public double NMC { get; set; }
            public double NMI { get; set; }
            public double NMO { get; set; }
            public double NMSTOT { get; set; }

            public double NFM { get; set; }
            public double NFI { get; set; }
            public double NFC { get; set; }
            public double NFO { get; set; }
            public double NFSTOT { get; set; }
            public double NTOTAL { get; set; }

            public double GTOTAL { get; set; }
        }

        #endregion

        private void cmbBankName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (Convert.ToInt32(cmbBankName.SelectedValue) == 11)
                {
                    lblState.Visibility = Visibility.Visible;
                    chkMelaka.Visibility = Visibility.Visible;
                    chkNegeriSembilan.Visibility = Visibility.Visible;
                }
                else
                {
                    lblState.Visibility = Visibility.Collapsed;
                    chkMelaka.Visibility = Visibility.Collapsed;
                    chkNegeriSembilan.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
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

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cmbBank.Text = "";
                cmbBankName.Text = "";
                cmbBranch.Text = "";
                dtpDOB.Text = "";
                lblState.Visibility = Visibility.Collapsed;
                chkMelaka.Visibility = Visibility.Collapsed;
                chkNegeriSembilan.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }
    }
}
