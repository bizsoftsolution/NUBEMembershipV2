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
    /// Interaction logic for frmVariationReport.xaml
    /// </summary>
    public partial class frmVariationReport : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        DataTable dt = new DataTable();
        public frmVariationReport()
        {
            InitializeComponent();
            fFormLoad();
        }

        #region Events

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(dtpFromDate.Text))
                {
                    MessageBox.Show("From Date is Empty!");
                    dtpFromDate.Focus();
                    return;
                }
                else if (string.IsNullOrEmpty(dtpToDate.Text))
                {
                    MessageBox.Show("To Date is Empty!");
                    dtpToDate.Focus();
                    return;
                }
                else if (Convert.ToDateTime(dtpFromDate.SelectedDate).Month == Convert.ToDateTime(dtpToDate.SelectedDate).Month)
                {
                    MessageBox.Show("From Date and To Date Must be Different");
                    dtpToDate.Focus();
                    return;
                }

                else
                {
                    fGetData();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                fClear();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnExportXL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dt = ((DataView)dgVariationReport.ItemsSource).ToTable();
                if (dt.Rows.Count > 0)
                {
                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.InitialDirectory = Environment.CurrentDirectory;
                    dlg.Title = "Variation Report";
                    dlg.DefaultExt = ".xlsx";
                    dlg.Filter = "XL files|*.xls;*.xlsx|All files|*.*";
                    dlg.FileName = "VariationReport";
                    if (dlg.ShowDialog() == true)
                    {
                        Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
                        Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
                        Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
                        app.Visible = true;
                        worksheet = workbook.Sheets["Sheet1"];

                        worksheet = workbook.ActiveSheet;
                        worksheet.Name = "Variation Report";

                        worksheet.Cells[3, 4] = "VARIATION REPORT ";
                        worksheet.Cells[3, 4].Font.Bold = true;
                        for (int i = 1; i < dt.Columns.Count + 1; i++)
                        {
                            if (i == 3)
                            {
                                worksheet.Cells[5, i + 2] = string.Format("{0:MMM} A Member", dtpFromDate.SelectedDate);
                            }
                            else if (i == 4)
                            {
                                worksheet.Cells[5, i + 2] = string.Format("{0:MMM} A Amount", dtpFromDate.SelectedDate);
                            }
                            else if (i == 5)
                            {
                                worksheet.Cells[5, i + 2] = string.Format("{0:MMM} S Member", dtpFromDate.SelectedDate);
                            }
                            else if (i == 6)
                            {
                                worksheet.Cells[5, i + 2] = string.Format("{0:MMM} S Amount", dtpFromDate.SelectedDate);
                            }
                            else if (i == 7)
                            {
                                worksheet.Cells[5, i + 2] = string.Format("{0:MMM} Tot Member", dtpFromDate.SelectedDate);
                            }
                            else if (i == 8)
                            {
                                worksheet.Cells[5, i + 2] = string.Format("{0:MMM} Tot Amount", dtpFromDate.SelectedDate);
                            }
                            else if (i == 9)
                            {
                                worksheet.Cells[5, i + 2] = string.Format("{0:MMM} A Member", dtpToDate.SelectedDate);
                            }
                            else if (i == 10)
                            {
                                worksheet.Cells[5, i + 2] = string.Format("{0:MMM} A Amount", dtpToDate.SelectedDate);
                            }
                            else if (i == 11)
                            {
                                worksheet.Cells[5, i + 2] = string.Format("{0:MMM} S Member", dtpToDate.SelectedDate);
                            }
                            else if (i == 12)
                            {
                                worksheet.Cells[5, i + 2] = string.Format("{0:MMM} S Amount", dtpToDate.SelectedDate);
                            }
                            else if (i == 13)
                            {
                                worksheet.Cells[5, i + 2] = string.Format("{0:MMM} Tot Member", dtpToDate.SelectedDate);
                            }
                            else if (i == 14)
                            {
                                worksheet.Cells[5, i + 2] = string.Format("{0:MMM} Tot Amount", dtpToDate.SelectedDate);
                            }
                            else if (i != 1)
                            {
                                worksheet.Cells[5, i + 2] = dt.Columns[i - 1].ColumnName;
                            }
                            worksheet.Cells[5, i + 2].Font.Bold = true;
                        }

                        for (int i = 0; i < dt.Rows.Count - 1; i++)
                        {
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                if (j != 0)
                                {
                                    worksheet.Cells[i + 6, j + 3] = dt.Rows[i][j].ToString();
                                }
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

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            frmHomeReports frm = new frmHomeReports();
            this.Close();
            frm.ShowDialog();
        }

        private void cmbBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                decimal dBankCode = Convert.ToDecimal(cmbBank.SelectedValue);
                if (dBankCode != 0)
                {
                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANK_CODE == dBankCode).OrderBy(x => x.BANKBRANCH_NAME).ToList();
                    cmbBranch.ItemsSource = mbr;
                    cmbBranch.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranch.DisplayMemberPath = "BANKBRANCH_NAME";
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void dgVariationReport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion

        #region Functiond

        void fFormLoad()
        {
            cmbBank.ItemsSource = db.MASTERBANKs.ToList();
            cmbBank.SelectedValuePath = "BANK_CODE";
            cmbBank.DisplayMemberPath = "BANK_NAME";

            cmbNUBEBranch.ItemsSource = db.MASTERNUBEBRANCHes.ToList();
            cmbNUBEBranch.SelectedValuePath = "NUBE_BRANCH_CODE";
            cmbNUBEBranch.DisplayMemberPath = "NUBE_BRANCH_NAME";
        }

        void fClear()
        {
            cmbBank.ItemsSource = null;
            cmbBranch.ItemsSource = null;
            cmbNUBEBranch.ItemsSource = null;
            dtpFromDate.Text = "";
            dtpToDate.Text = "";
            dgVariationReport.ItemsSource = null;
            dt.Rows.Clear();
            fFormLoad();
        }

        void fGetData()
        {
            dt.Rows.Clear();
            using (SqlConnection conn = new SqlConnection(AppLib.connStr))
            {
                //----------------------->> New Member & Resignation Vation Report <<-----------------------------

                //DateTime dt2 = Convert.ToDateTime(dtpFromDate.SelectedDate);
                //dt2 = new DateTime(dt2.Year, dt2.Month, 1);
                //DateTime FfromDate = dt2.AddDays(-1);//daec31
                //DateTime FToDate = dt2.AddMonths(1);//feb

                //DateTime dtS = Convert.ToDateTime(dtpToDate.SelectedDate);
                //dtS = new DateTime(dtS.Year, dtS.Month, 1);
                //DateTime SfromDate = dtS.AddDays(-1);//daec31
                //DateTime SToDate = dtS.AddMonths(1);//feb    

                //string sQry = string.Format("SELECT ISNULL(MB.BANK_USERCODE,'')BANK,ISNULL(SUM(FNEW),0)F_NEW,ISNULL(SUM(FREG),0)F_REG,ISNULL(SUM(SNEW),0)S_NEW,\r" +
                //    "ISNULL(SUM(SREG),0)S_REG,ISNULL(SUM(SNEW)-SUM(FNEW),0) VARIATION_NEW,ISNULL(SUM(SREG)-SUM(FREG),0) VARIATION_REG FROM \r" +
                //    "(SELECT BANK_CODE, SUM(NEWMEMBER)FNEW, SUM(RESINGED)FREG, 0SNEW, 0SREG FROM\r" +
                //    "(SELECT ST.BANK_CODE, COUNT(*) NEWMEMBER, 0 RESINGED FROM MASTERMEMBER MM\r" +
                //    "LEFT JOIN MASTERMEMBER ST ON ST.MEMBER_CODE = MM.MEMBER_CODE\r" +
                //    " WHERE MM.DATEOFJOINING > '" + string.Format("{0:dd/MMM/yyyy}", FfromDate) + "' AND MM.DATEOFJOINING < '" + string.Format("{0:dd/MMM/yyyy}", FToDate) + "'\r" +
                //    "GROUP BY ST.BANK_CODE\r" +
                //    "UNION ALL\r" +
                //    "SELECT ST.BANK_CODE, 0 NEWMEMBER, COUNT(*)RESINGED FROM RESIGNATION MM\r" +
                //    "LEFT JOIN MASTERMEMBER ST ON ST.MEMBER_CODE = MM.MEMBER_CODE\r" +
                //    "WHERE VOUCHER_DATE > '" + string.Format("{0:dd/MMM/yyyy}", FfromDate) + "' AND VOUCHER_DATE < '" + string.Format("{0:dd/MMM/yyyy}", FToDate) + "'\r" +
                //    "GROUP BY ST.BANK_CODE\r" +
                //    ")TEMP\r" +
                //    "GROUP BY BANK_CODE\r" +
                //    "UNION ALL\r" +
                //    "SELECT BANK_CODE, 0 FNEW, 0 FREG, SUM(NEWMEMBER)SNEW, SUM(RESINGED)SREG FROM\r" +
                //    "(SELECT ST.BANK_CODE, COUNT(*) NEWMEMBER, 0 RESINGED FROM MASTERMEMBER MM\r" +
                //    "LEFT JOIN MASTERMEMBER ST ON ST.MEMBER_CODE = MM.MEMBER_CODE\r" +
                //    " WHERE MM.DATEOFJOINING > '" + string.Format("{0:dd/MMM/yyyy}", SfromDate) + "' AND MM.DATEOFJOINING < '" + string.Format("{0:dd/MMM/yyyy}", SToDate) + "'\r" +
                //    "GROUP BY ST.BANK_CODE\r" +
                //    "UNION ALL\r" +
                //    "SELECT ST.BANK_CODE, 0 NEWMEMBER, COUNT(*)RESINGED FROM RESIGNATION MM\r" +
                //    "LEFT JOIN MASTERMEMBER ST ON ST.MEMBER_CODE = MM.MEMBER_CODE\r" +
                //    "WHERE VOUCHER_DATE > '" + string.Format("{0:dd/MMM/yyyy}", SfromDate) + "' AND VOUCHER_DATE < '" + string.Format("{0:dd/MMM/yyyy}", SToDate) + "'\r" +
                //    "GROUP BY ST.BANK_CODE\r" +
                //    ")TEMP\r" +
                //    "GROUP BY BANK_CODE)TE\r" +
                //    "LEFT JOIN MASTERBANK MB(NOLOCK) ON MB.BANK_CODE = TE.BANK_CODE\r" +
                //    "GROUP BY MB.BANK_USERCODE\r" +
                //    "ORDER BY MB.BANK_USERCODE", FToDate, SToDate);

                //----------------------->> New Member & Resignation Vation Report <<-----------------------------

                string sJOIN = "";
                string sSelectQry = "";
                string sWhere = "";

                if (!string.IsNullOrEmpty(cmbNUBEBranch.Text) || !string.IsNullOrEmpty(cmbBank.Text) || !string.IsNullOrEmpty(cmbBranch.Text))
                {
                    sJOIN = sJOIN + " LEFT JOIN MEMBERSTATUSLOG MM(NOLOCK) ON MM.MEMBER_CODE=FD.MEMBERCODE \r" +
                                    " LEFT JOIN FEESMASTER FM(NOLOCK) ON FM.FEEID=FD.FEEID ";
                }

                if (!string.IsNullOrEmpty(cmbNUBEBranch.Text))
                {
                    sWhere = sWhere + " AND MM.NUBEBRANCH_CODE=" + cmbNUBEBranch.SelectedValue;
                }

                if (!string.IsNullOrEmpty(cmbBank.Text))
                {
                    sWhere = sWhere + " AND FM.BANKID=" + cmbBank.SelectedValue;
                }

                if (!string.IsNullOrEmpty(cmbBranch.Text))
                {
                    sWhere = sWhere + " AND MM.BRANCH_CODE=" + cmbBranch.SelectedValue;
                }

                if (chkIncludeArrear.IsChecked == false)
                {
                    sWhere = sWhere + " AND STATUS='FEES ENTRY' ";
                }

                if (rbtnActiveMember.IsChecked == true)
                {
                    sSelectQry = sSelectQry + ",SUM(FA_AMT)+SUM(SA_AMT)TOTALAMOUNT,SUM(FA_MEM)+SUM(SA_MEM)TOTALMEMBERS \r" +
                                              ",SUM(FA_AMT)-SUM(SA_AMT)VARIATIONAMOUNT,SUM(FA_MEM)-SUM(SA_MEM)VARIATIONMEMBER \r";
                }
                else
                {
                    sSelectQry = sSelectQry + ",SUM(FT_AMT)+SUM(ST_AMT)TOTALAMOUNT,SUM(FT_MEM)+SUM(ST_MEM)TOTALMEMBERS \r" +
                                              ",SUM(FT_AMT)-SUM(ST_AMT)VARIATIONAMOUNT,SUM(FT_MEM)-SUM(ST_MEM)VARIATIONMEMBER \r";
                }

                string sQry = string.Format("SELECT BANKID,CASE WHEN ISNULL(MB.BANK_USERCODE,'')<>'' THEN MB.BANK_USERCODE ELSE 'Arrear Entry' END BANK_USERCODE, \r" +
                    " SUM(FA_AMT)FA_AMT,SUM(FA_MEM)FA_MEM,SUM(FS_AMT)FS_AMT,SUM(FS_MEM)FS_MEM,SUM(FT_AMT)FT_AMT,SUM(FT_MEM)FT_MEM, \r" +
                    " SUM(SA_AMT)SA_AMT,SUM(SA_MEM)SA_MEM,SUM(SS_AMT)SS_AMT,SUM(SS_MEM)SS_MEM,SUM(ST_AMT)ST_AMT,SUM(ST_MEM)ST_MEM \r" + sSelectQry +
                    " FROM (SELECT FEEYEAR FY,FEEMONTH FM,FM.BANKID,TMP.FEEID FI,SUM(AC_AMOUNT) FA_AMT,SUM(AC_MEMBERS)FA_MEM,SUM(SD_AMOUNT)FS_AMT,SUM(SD_MEMBERS)FS_MEM, \r" +
                    " SUM(AC_AMOUNT)+SUM(SD_AMOUNT)FT_AMT,SUM(AC_MEMBERS)+SUM(SD_MEMBERS)FT_MEM, \r" +
                    " 0 SY,0 SM,0 SI,0 SA_AMT,0 SA_MEM,0 SS_AMT,0 SS_MEM,0 ST_AMT,0 ST_MEM \r" +
                    " FROM(SELECT FD.FEEYEAR,FD.FEEMONTH,FD.FEEID,SUM(TOTALAMOUNT)AC_AMOUNT,COUNT(*)AC_MEMBERS,0 SD_AMOUNT,0 SD_MEMBERS \r" +
                    " FROM FEESDETAILS FD(NOLOCK) \r" + sJOIN +
                    " WHERE FD.FEEYEAR=YEAR('{0:dd/MMM/yyyy}') AND FEEMONTH=MONTH('{0:dd/MMM/yyyy}') AND ISNOTMATCH=0 " + sWhere + " \r" +
                    " GROUP BY FD.FEEYEAR,FD.FEEMONTH,FD.FEEID \r" +
                    " UNION ALL \r" +
                    " SELECT FD.FEEYEAR,FD.FEEMONTH,FD.FEEID,0 AC_AMOUNT,0 AC_MEMBERS,SUM(TOTALAMOUNT) SD_AMOUNT,COUNT(*) SD_MEMBERS \r" +
                    " FROM FEESDETAILSNOTMATCH FD(NOLOCK) \r" + sJOIN +
                    " WHERE FD.FEEYEAR=YEAR('{0:dd/MMM/yyyy}') AND FD.FEEMONTH=MONTH('{0:dd/MMM/yyyy}') AND ISNOTMATCH=1 " + sWhere + " \r" +
                    " GROUP BY FD.FEEYEAR,FD.FEEMONTH,FD.FEEID)TMP \r" +
                    " LEFT JOIN FEESMASTER FM(NOLOCK) ON FM.FEEID = TMP.FEEID \r" +
                    " GROUP BY FEEYEAR,FEEMONTH,FM.BANKID,TMP.FEEID \r" +
                    " UNION ALL \r" +
                    " SELECT 0 FY,0 FM,FM.BANKID,0 FI,0 FA_AMT,0 FA_MEM,0 FS_AMT,0 FS_MEM,0 FT_AMT,0 FT_MEM, \r" +
                    " FEEYEAR SY,FEEMONTH SM,TMP.FEEID SI,SUM(AC_AMOUNT) SA_AMT,SUM(AC_MEMBERS)SA_MEM,SUM(SD_AMOUNT)SS_AMT,SUM(SD_MEMBERS)SS_MEM, \r" +
                    " SUM(AC_AMOUNT)+SUM(SD_AMOUNT)ST_AMT,SUM(AC_MEMBERS)+SUM(SD_MEMBERS)ST_MEM \r" +
                    " FROM(SELECT FD.FEEYEAR,FD.FEEMONTH,FD.FEEID,SUM(TOTALAMOUNT)AC_AMOUNT,COUNT(*)AC_MEMBERS,0 SD_AMOUNT,0 SD_MEMBERS \r" +
                    " FROM FEESDETAILS FD(NOLOCK) \r" + sJOIN +
                    " WHERE FD.FEEYEAR=YEAR('{1:dd/MMM/yyyy}') AND FD.FEEMONTH=MONTH('{1:dd/MMM/yyyy}') AND ISNOTMATCH=0 " + sWhere + " \r" +
                    " GROUP BY FD.FEEYEAR, FD.FEEMONTH, FD.FEEID \r" +
                    " UNION ALL \r" +
                    " SELECT FD.FEEYEAR, FD.FEEMONTH, FD.FEEID,0 AC_AMOUNT,0 AC_MEMBERS,SUM(TOTALAMOUNT) SD_AMOUNT,COUNT(*) SD_MEMBERS \r" +
                    " FROM FEESDETAILSNOTMATCH FD(NOLOCK) \r" + sJOIN +
                    " WHERE FD.FEEYEAR=YEAR('{1:dd/MMM/yyyy}') AND FD.FEEMONTH=MONTH('{1:dd/MMM/yyyy}') AND ISNOTMATCH=1 " + sWhere + " \r" +
                    " GROUP BY FD.FEEYEAR,FD.FEEMONTH,FD.FEEID)TMP \r" +
                    " LEFT JOIN FEESMASTER FM(NOLOCK) ON FM.FEEID = TMP.FEEID \r" +
                    " GROUP BY FEEYEAR, FEEMONTH, FM.BANKID, TMP.FEEID)TMP \r" +
                    " LEFT JOIN MASTERBANK MB(NOLOCK) ON MB.BANK_CODE = TMP.BANKID \r" +
                    " GROUP BY BANKID, MB.BANK_USERCODE \r" +
                    " ORDER BY MB.BANK_USERCODE ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);

                SqlCommand cmd = new SqlCommand(sQry, conn);
                SqlDataAdapter sdp = new SqlDataAdapter(cmd);
                sdp.SelectCommand.CommandTimeout = 0;
                sdp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dgVariationReport.ItemsSource = dt.DefaultView;
                    dgVariationReport.Columns[1].Header = string.Format("{0:MMM} A Amount", dtpFromDate.SelectedDate);
                    dgVariationReport.Columns[2].Header = string.Format("{0:MMM} A Member", dtpFromDate.SelectedDate);
                    dgVariationReport.Columns[3].Header = string.Format("{0:MMM} S Amount", dtpFromDate.SelectedDate);
                    dgVariationReport.Columns[4].Header = string.Format("{0:MMM} S Member", dtpFromDate.SelectedDate);
                    dgVariationReport.Columns[5].Header = string.Format("{0:MMM} Tot Amount", dtpFromDate.SelectedDate);
                    dgVariationReport.Columns[6].Header = string.Format("{0:MMM} Tot Member", dtpFromDate.SelectedDate);
                    dgVariationReport.Columns[7].Header = string.Format("{0:MMM} A Amount", dtpToDate.SelectedDate);
                    dgVariationReport.Columns[8].Header = string.Format("{0:MMM} A Member", dtpToDate.SelectedDate);
                    dgVariationReport.Columns[9].Header = string.Format("{0:MMM} S Amount", dtpToDate.SelectedDate);
                    dgVariationReport.Columns[10].Header = string.Format("{0:MMM} S Member", dtpToDate.SelectedDate);
                    dgVariationReport.Columns[11].Header = string.Format("{0:MMM} Tot Amount", dtpToDate.SelectedDate);
                    dgVariationReport.Columns[12].Header = string.Format("{0:MMM} Tot Member", dtpToDate.SelectedDate);

                    VariationReport.Reset();
                    ReportDataSource masterdata = new ReportDataSource("DataSet1", dt);
                    VariationReport.LocalReport.DataSources.Add(masterdata);
                    VariationReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.rptVariationReport.rdlc";
                    ReportParameter[] RP = new ReportParameter[2];
                    string fm = string.Format("{0:MMM}", dtpFromDate.SelectedDate);
                    string sm = string.Format("{0:MMM}", dtpToDate.SelectedDate);
                    RP[0] = new ReportParameter("FirstMonth", fm);
                    RP[1] = new ReportParameter("SecondMonth", sm);
                    VariationReport.LocalReport.SetParameters(RP);
                    VariationReport.RefreshReport();
                }

                else
                {
                    dgVariationReport.ItemsSource = null;
                    MessageBox.Show("No Records Found!", "Empty");
                    dtpFromDate.Focus();
                }
            }
        }
        #endregion
    }
}
