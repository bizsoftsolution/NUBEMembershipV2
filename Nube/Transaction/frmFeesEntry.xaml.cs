using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using Microsoft.Win32;
using System.Data.SqlClient;
using Nube.MasterSetup;
using ci = System.Globalization.CultureInfo;

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmFeesEntry.xaml
    /// </summary>
    public partial class frmFeesEntry : MetroWindow
    {
        nubebfsEntity DB = new nubebfsEntity();
        OpenFileDialog OpenDialogBox = new OpenFileDialog();

        DataTable dtFeesEntry = new DataTable();
        DataTable dtActive = new DataTable();
        DataTable dtNotMatch = new DataTable();
        DataTable dtUnPaid = new DataTable();
        Boolean bInsering = true;
        Boolean bValidate = false;

        string sXLShowFileName = "";
        string sXLShowPswd = "";
        string sXLFileName = "";
        string sXLFilePswd = "";
        
        string connectionstring = AppLib.connStr;

        public frmFeesEntry()
        {
            InitializeComponent();
            try
            {
                cmbBankName.ItemsSource = DB.MASTERBANKs.ToList();
                cmbBankName.SelectedValuePath = "BANK_CODE";
                cmbBankName.DisplayMemberPath = "BANK_NAME";
                btnShow.Visibility = Visibility.Hidden;
                progressBar1.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #region "BUTTON EVENTS"       

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            frmHomeMembership frm = new frmHomeMembership();
            this.Close();
            frm.ShowDialog();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bValidate = false;
                BeforeUpdate();

                string sourcePath = @"" + sXLFileName;
                string SourceFileName = OpenDialogBox.SafeFileName.ToString();
                string sDestinationPath = @Environment.CurrentDirectory + "\\Fee Entry Sheets\\" + txtYear.Text + @"\" + txtMonth.Text + @"\" + cmbBankName.Text + @"\";
                string sourceFile = System.IO.Path.Combine(sourcePath);
                string destinationFile = System.IO.Path.Combine(sDestinationPath, SourceFileName);

                int iBnkCode = Convert.ToInt32(cmbBankName.SelectedValue);

                if (bInsering == true)
                {
                    if (bValidate == false)
                    {
                        DateTime YearName = Convert.ToDateTime("01" + "/" + txtMonth.Text + "/" + txtYear.Text);
                        FeesMaster FeeMst = DB.FeesMasters.Where(x => x.BankId == iBnkCode && x.FeeDate == YearName).FirstOrDefault();
                        if (FeeMst != null)
                        {
                            if (MessageBox.Show("This Bank Details are already in DB, Do You want to Save Once Again ?", "Save Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                DB.FeesMasters.Remove(FeeMst);
                                DB.SaveChanges();

                                var fdtl = (from x in DB.FeesDetails where x.FeeId == FeeMst.FeeId select x).ToList();
                                if (fdtl != null)
                                {
                                    DB.FeesDetails.RemoveRange(DB.FeesDetails.Where(x => x.FeeId == FeeMst.FeeId));
                                    DB.SaveChanges();
                                }
                            }
                        }

                        FeesMaster FeesMst = new FeesMaster
                        {
                            BankId = iBnkCode,
                            FeeDate = Convert.ToDateTime(String.Format("{0:MMM/dd/yyyy}", txtMonth.Text + "-" + "01" + "-" + txtYear.Text)),
                            XLFileName = destinationFile,
                            XLPassword = txtXLPassword.Password.ToString(),
                            UpdatedStatus = "Not Updated",
                        };
                        DB.FeesMasters.Add(FeesMst);
                        DB.SaveChanges();

                        var NewData = new JSonHelper().ConvertObjectToJSon(FeesMst);
                        AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "FeesMaster");

                        var sFid = DB.FeesMasters.Max(x => x.FeeId).ToString();

                        DataTable dt = new DataTable();
                        dt = ((DataView)dgFeeDetails.ItemsSource).ToTable();

                        List<FeesDetail> lstFeesDetls = new List<FeesDetail>();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string IcNo = dt.Rows[i]["NRIC"].ToString();
                            FeesDetail FeesDtl = new FeesDetail
                            {
                                FeeId = Convert.ToInt32(sFid),
                                MemberCode = Convert.ToDecimal(dt.Rows[i]["MEMBERCODE"]),
                                TotalAmount = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]),
                                AmountBF = Convert.ToDecimal(dt.Rows[i]["BF"]),
                                AmountIns = Convert.ToDecimal(dt.Rows[i]["INSURANCE"]),
                                AmtSubs = Convert.ToDecimal(dt.Rows[i]["SUBSCRIPTION"]),
                                Dept = (dt.Rows[i]["DEPARTMENT"]).ToString(),
                                UpdatedStatus = "Not Updated",
                                FeeYear = YearName.Year,
                                FeeMonth = YearName.Month,
                                Reason = "",
                                IsUnPaid = false,
                                IsNotMatch = false
                            };
                            lstFeesDetls.Add(FeesDtl);
                            var NewData1 = new JSonHelper().ConvertObjectToJSon(FeesDtl);
                            AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData1, "FeesDetail", "MemberCode-" + dt.Rows[i]["MEMBERCODE"].ToString() + " & AMOUNT PAID ");
                        }

                        if (lstFeesDetls != null)
                        {
                            DB.FeesDetails.AddRange(lstFeesDetls);
                            DB.SaveChanges();
                        }

                        dt.Rows.Clear();
                        dt = ((DataView)dgNotMatch.ItemsSource).ToTable();
                        lstFeesDetls = new List<FeesDetail>();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            FeesDetail FeesDtl = new FeesDetail
                            {
                                FeeId = Convert.ToInt32(sFid),
                                MemberCode = Convert.ToDecimal(dt.Rows[i]["MEMBERCODE"]),
                                TotalAmount = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]),
                                AmountBF = Convert.ToDecimal(dt.Rows[i]["BF"]),
                                AmountIns = Convert.ToDecimal(dt.Rows[i]["INSURANCE"]),
                                AmtSubs = Convert.ToDecimal(dt.Rows[i]["SUBSCRIPTION"]),
                                Dept = (dt.Rows[i]["DEPARTMENT"]).ToString(),
                                UpdatedStatus = "Not Updated",
                                FeeYear = YearName.Year,
                                FeeMonth = YearName.Month,
                                Reason = dt.Rows[i]["REASON"].ToString(),
                                IsUnPaid = false,
                                IsNotMatch = true
                            };
                            lstFeesDetls.Add(FeesDtl);
                            var NewData3 = new JSonHelper().ConvertObjectToJSon(FeesDtl);
                            AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData3, "FeesDetail", "MemberCode-" + dt.Rows[i]["MEMBERCODE"].ToString() + " & " + dt.Rows[i]["REASON"].ToString());
                        }

                        if (lstFeesDetls.Count > 0)
                        {
                            DB.FeesDetails.AddRange(lstFeesDetls);
                            DB.SaveChanges();
                        }

                        dt.Rows.Clear();
                        dt = ((DataView)dgUnPaid.ItemsSource).ToTable();
                        lstFeesDetls = new List<FeesDetail>();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            FeesDetail FeesDtl = new FeesDetail
                            {
                                FeeId = Convert.ToInt32(sFid),
                                MemberCode = Convert.ToDecimal(dt.Rows[i]["MEMBERCODE"]),
                                TotalAmount = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]),
                                AmountBF = Convert.ToDecimal(dt.Rows[i]["BF"]),
                                AmountIns = Convert.ToDecimal(dt.Rows[i]["INSURANCE"]),
                                AmtSubs = Convert.ToDecimal(dt.Rows[i]["SUBSCRIPTION"]),
                                Dept = (dt.Rows[i]["DEPARTMENT"]).ToString(),
                                UpdatedStatus = "Not Updated",
                                FeeYear = YearName.Year,
                                FeeMonth = YearName.Month,
                                Reason = "",
                                IsUnPaid = true,
                                IsNotMatch = false
                            };
                            lstFeesDetls.Add(FeesDtl);
                            var NewData2 = new JSonHelper().ConvertObjectToJSon(FeesDtl);
                            AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData2, "FeesDetail", "MemberCode-" + dt.Rows[i]["MEMBERCODE"].ToString() + " & AMOUNT UNPAID ");
                        }

                        if (lstFeesDetls.Count > 0)
                        {
                            DB.FeesDetails.AddRange(lstFeesDetls);
                            DB.SaveChanges();
                        }


                        if (!System.IO.Directory.Exists(sDestinationPath))
                        {
                            System.IO.Directory.CreateDirectory(sDestinationPath);
                        }
                        System.IO.File.Copy(sourceFile, destinationFile, true);

                        MessageBox.Show("Details are Saved Sucessfully");
                        cmbBankName.Text = "";
                        txtMonth.Text = "";
                        txtYear.Text = "";
                        txtTotalAmount.Text = "";
                        txtNotMatch.Text = "";
                        txtPaidMembers.Text = "";
                        txtTotalMembers.Text = "";
                        txtUnPaidMembers.Text = "";
                        txtXLPassword.Password = "";
                        dgFeeDetails.ItemsSource = null;
                        dgNotMatch.ItemsSource = null;
                        dgUnPaid.ItemsSource = null;
                    }
                }
                else
                {
                    DateTime YearName = Convert.ToDateTime("01" + "/" + txtMonth.Text + "/" + txtYear.Text);
                    FeesMaster FeeMst = DB.FeesMasters.Where(x => x.BankId == iBnkCode && DateTime.Compare(x.FeeDate, YearName) <= 0).FirstOrDefault();
                    int iFeeId = Convert.ToInt32(FeeMst.FeeId);

                    FeesMaster FeesMst = (from m in DB.FeesMasters where m.BankId == iFeeId select m).Single();
                    var OldData = new JSonHelper().ConvertObjectToJSon(FeesMst);
                    FeesMst.BankId = iBnkCode;
                    FeesMst.FeeDate = Convert.ToDateTime("01" + "/" + txtMonth.Text + "/" + txtYear.Text);
                    FeesMst.XLFileName = destinationFile;
                    FeesMst.XLPassword = txtXLPassword.Password;
                    DB.SaveChanges();

                    var NewData = new JSonHelper().ConvertObjectToJSon(FeesMst);
                    AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "FeesMaster");

                    var emp = (from CH in DB.FeesDetails where CH.FeeId == iFeeId select CH).ToList();
                    var OldData1 = new JSonHelper().ConvertObjectToJSon(emp);
                    if (emp != null)
                    {
                        DB.FeesDetails.RemoveRange(DB.FeesDetails.Where(x => x.FeeId == iFeeId));
                        DB.SaveChanges();
                    }

                    DataTable dt = new DataTable();
                    dt = ((DataView)dgFeeDetails.ItemsSource).ToTable();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string IcNo = dt.Rows[i]["ICNO"].ToString();
                        var iMember_Code = Convert.ToInt32(DB.MASTERMEMBERs.Where(x => x.ICNO_NEW == IcNo).Select(x => x.MEMBER_CODE).FirstOrDefault());
                        FeesDetail FeesDtl = new FeesDetail
                        {
                            FeeId = Convert.ToInt32(iFeeId),
                            MemberCode = Convert.ToInt32(iMember_Code),
                            TotalAmount = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]),
                            AmountBF = Convert.ToDecimal(dt.Rows[i]["BF"]),
                            AmountIns = Convert.ToDecimal(dt.Rows[i]["INSURANCE"]),
                            AmtSubs = Convert.ToDecimal(dt.Rows[i]["SUBSCRIPTION"]),
                            Dept = (dt.Rows[i]["DEPARTMENT"]).ToString(),
                            UpdatedStatus = "Not Updated",
                            FeeYear = YearName.Year,
                            FeeMonth = YearName.Month,
                            Reason = "",
                            IsUnPaid = false
                        };
                        DB.FeesDetails.Add(FeesDtl);
                        DB.SaveChanges();

                        var NewData1 = new JSonHelper().ConvertObjectToJSon(FeesDtl);
                        AppLib.EventHistory(this.Tag.ToString(), 1, OldData1, NewData1, "FeesDetail", "PAID");
                    }
                    if (!System.IO.Directory.Exists(sDestinationPath))
                    {
                        System.IO.Directory.CreateDirectory(sDestinationPath);
                    }
                    System.IO.File.Copy(sourceFile, destinationFile, true);

                    MessageBox.Show("Details are Updated Sucessfully");
                    cmbBankName.Text = "";
                    txtMonth.Text = "";
                    txtYear.Text = "";
                    txtTotalAmount.Text = "";
                    txtNotMatch.Text = "";
                    txtPaidMembers.Text = "";
                    txtTotalMembers.Text = "";
                    txtUnPaidMembers.Text = "";
                    txtXLPassword.Password = "";
                    dgFeeDetails.ItemsSource = null;
                    dgNotMatch.ItemsSource = null;
                    dgUnPaid.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bInsering = true;
                ImportExcel();
                GridShow();
                Total();
                progressBar1.Visibility = Visibility.Hidden;
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
                cmbBankName.Text = "";
                txtMonth.Text = "";
                txtYear.Text = "";
                txtTotalAmount.Text = "";
                txtNotMatch.Text = "";
                txtPaidMembers.Text = "";
                txtTotalMembers.Text = "";
                txtUnPaidMembers.Text = "";
                txtXLPassword.Password = "";
                dgFeeDetails.ItemsSource = null;
                dgNotMatch.ItemsSource = null;
                dgUnPaid.ItemsSource = null;
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtYear.Text))
                {
                    MessageBox.Show("Year Is Empty!");
                    txtYear.Focus();
                }
                else if (string.IsNullOrEmpty(txtMonth.Text))
                {
                    MessageBox.Show("Month Is Empty!");
                    txtMonth.Focus();
                }
                else if (string.IsNullOrEmpty(cmbBankName.Text))
                {
                    MessageBox.Show("Bank Is Empty!");
                    cmbBankName.Focus();
                }
                else
                {
                    bInsering = false;
                    int iBankCode = Convert.ToInt32(cmbBankName.SelectedValue);
                    DateTime YearName = Convert.ToDateTime("01" + "/" + txtMonth.Text + "/" + txtYear.Text);
                    FeesMaster FeeMst = DB.FeesMasters.Where(x => x.BankId == iBankCode && DateTime.Compare(x.FeeDate, YearName) <= 0).FirstOrDefault();
                    if (FeeMst != null)
                    {
                        int iFeeId = Convert.ToInt32(FeeMst.FeeId);
                        sXLShowFileName = FeeMst.XLFileName;
                        sXLShowPswd = FeeMst.XLPassword;

                        var sql = (from f in DB.FeesDetails
                                   join mm in DB.MASTERMEMBERs on f.MemberCode equals mm.MEMBER_CODE
                                   where f.FeeId == iFeeId
                                   select new
                                   {
                                       ICNO = mm.ICNO_NEW,
                                       MEMBERNAME = mm.MEMBER_NAME,
                                       AMOUNT = f.TotalAmount,
                                       DEPARTMENT = f.Dept,
                                       BF = f.AmountBF,
                                       INSURANCE = f.AmountIns,
                                       SUBSCRIPTION = f.AmtSubs
                                   }
                                   ).ToList();

                        dgFeeDetails.ItemsSource = sql;
                        dgFeeDetails.IsReadOnly = false;
                        dgFeeDetails.CanUserAddRows = true;

                        DataTable dtNewFee = new DataTable();
                        if (sXLShowFileName != null)
                        {
                            ImportExcel();

                            //dtInActive = dtFeesEntry.Select("STATUS_CODE=3").CopyToDataTable();
                            //dtResign = dtFeesEntry.Select("STATUS_CODE=4").CopyToDataTable();
                            //dtNotMatch = dtFeesEntry.Select("STATUS_CODE=6").CopyToDataTable();

                            //dgStruckOff.ItemsSource = dtInActive.DefaultView;
                            //dgDisContinue.ItemsSource = dtResign.DefaultView;
                            //dgRejoin.ItemsSource = dtNotMatch.DefaultView;

                            int iPMembers = 0;
                            int iUPMembers = 0;
                            int iNotMatch = 0;
                            int iTotalMember = 0;
                            int iTotalAmnt = 0;

                            iNotMatch = dtNotMatch.Rows.Count;
                            //iTotalMember = dtActive.Rows.Count + dtInActive.Rows.Count + dtResign.Rows.Count + iNotMatch;

                            //for (int i = 0; i < dtActive.Rows.Count; i++)
                            //{
                            //    iTotalAmnt = iTotalAmnt + Convert.ToInt32(dtActive.Rows[i]["Amount"]);
                            //    if (dtActive.Rows[i]["Amount"] != "0")
                            //    {
                            //        iPMembers = iPMembers + 1;
                            //    }
                            //    else
                            //    {
                            //        iUPMembers = iUPMembers + 1;
                            //    }
                            //}
                            //for (int i = 0; i < dtInActive.Rows.Count; i++)
                            //{
                            //    iTotalAmnt = iTotalAmnt + Convert.ToInt32(dtInActive.Rows[i]["Amount"]);
                            //    if (dtInActive.Rows[i]["Amount"] != "0")
                            //    {
                            //        iPMembers = iPMembers + 1;
                            //    }
                            //    else
                            //    {
                            //        iUPMembers = iUPMembers + 1;
                            //    }
                            //}
                            //for (int i = 0; i < dtResign.Rows.Count; i++)
                            //{
                            //    iTotalAmnt = iTotalAmnt + Convert.ToInt32(dtResign.Rows[i]["Amount"]);
                            //    if (dtResign.Rows[i]["Amount"] != "0")
                            //    {
                            //        iPMembers = iPMembers + 1;
                            //    }
                            //    else
                            //    {
                            //        iUPMembers = iUPMembers + 1;
                            //    }
                            //}
                            //for (int i = 0; i < dtNotMatch.Rows.Count; i++)
                            //{
                            //    iTotalAmnt = iTotalAmnt + Convert.ToInt32(dtNotMatch.Rows[i]["Amount"]);
                            //    if (dtNotMatch.Rows[i]["Amount"] != "0")
                            //    {
                            //        iPMembers = iPMembers + 1;
                            //    }
                            //    else
                            //    {
                            //        iUPMembers = iUPMembers + 1;
                            //    }
                            //}

                            txtPaidMembers.Text = iPMembers.ToString();
                            txtUnPaidMembers.Text = iUPMembers.ToString();
                            txtNotMatch.Text = iNotMatch.ToString();
                            txtTotalMembers.Text = iTotalMember.ToString();
                            txtTotalAmount.Text = iTotalAmnt.ToString();
                        }
                    }
                    else
                    {
                        MessageBox.Show("No Data Found!");
                        cmbBankName.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion

        #region "USER DEFINEND FUNCTION"

        void fNew()
        {
            cmbBankName.Text = "";
            txtMonth.Text = "";
            txtYear.Text = "";
            txtTotalAmount.Text = "";
            txtNotMatch.Text = "";
            txtPaidMembers.Text = "";
            txtTotalMembers.Text = "";
            txtUnPaidMembers.Text = "";
            txtXLPassword.Password = "";
            dgFeeDetails.ItemsSource = null;
            dgNotMatch.ItemsSource = null;
            dgUnPaid.ItemsSource = null;
        }

        void ImportExcel()
        {
            DataTable dtTemp = new DataTable();
            OpenDialogBox.DefaultExt = ".xlsx";
            OpenDialogBox.Filter = "XL files|*.xls;*.xlsx|All files|*.*";
            string sFileName = "";
            string sFilePswd = "";
            bool bImportXl = false;
            if (bInsering == true)
            {
                var browsefile = OpenDialogBox.ShowDialog();
                if (browsefile == true)
                {
                    bImportXl = true;
                }
                else
                {
                    bImportXl = false;
                }
            }
            else
            {
                bImportXl = true;
            }

            if (bImportXl == true)
            {
                if (bInsering == true)
                {
                    sFileName = OpenDialogBox.FileName.ToString();
                    sXLFileName = OpenDialogBox.FileName.ToString();
                    sXLFilePswd = txtXLPassword.Password;
                    sFilePswd = txtXLPassword.Password;
                }
                else
                {
                    sFileName = sXLShowFileName;
                    sFilePswd = sXLShowPswd;
                }
                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook excelBook = excelApp.Workbooks.Open(sFileName, 0, true, 5, sFilePswd, "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                Microsoft.Office.Interop.Excel.Worksheet excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.Worksheets.get_Item(1);
                Microsoft.Office.Interop.Excel.Range excelRange = excelSheet.UsedRange;

                string strCellData = "";
                double douCellData;
                int rowCnt = 0;
                int colCnt = 0;
                progressBar1.Minimum = 1;
                progressBar1.Maximum = excelRange.Rows.Count;
                progressBar1.Visibility = Visibility.Visible;
                if (bInsering == true)
                {
                    try
                    {
                        txtYear.Text = (string)(excelRange.Cells[2, 2] as Microsoft.Office.Interop.Excel.Range).Value2;
                    }
                    catch (Exception ex)
                    {
                        douCellData = (excelRange.Cells[2, 2] as Microsoft.Office.Interop.Excel.Range).Value2;
                        txtYear.Text = douCellData.ToString();
                    }
                    try
                    {
                        txtMonth.Text = (string)(excelRange.Cells[2, 4] as Microsoft.Office.Interop.Excel.Range).Value2;
                    }
                    catch (Exception ex)
                    {
                        douCellData = (excelRange.Cells[2, 4] as Microsoft.Office.Interop.Excel.Range).Value2;
                        txtMonth.Text = douCellData.ToString();
                    }

                    try
                    {
                        string sBankName = (string)(excelRange.Cells[1, 2] as Microsoft.Office.Interop.Excel.Range).Value2;
                        var qry = (from m in DB.MASTERBANKs where m.BANK_NAME == sBankName select m).FirstOrDefault();
                        if (qry != null)
                        {
                            cmbBankName.SelectedValue = qry.BANK_CODE;
                        }
                    }
                    catch (Exception ex)
                    {
                        douCellData = (excelRange.Cells[1, 2] as Microsoft.Office.Interop.Excel.Range).Value2;
                        string sBankName = douCellData.ToString();
                        var qry = (from m in DB.MASTERBANKs where m.BANK_NAME == sBankName select m).FirstOrDefault();
                        if (qry != null)
                        {
                            cmbBankName.SelectedValue = qry.BANK_CODE;
                        }
                    }
                }
                for (colCnt = 1; colCnt <= excelRange.Columns.Count; colCnt++)
                {
                    string strColumn = "";
                    strColumn = (string)(excelRange.Cells[4, colCnt] as Microsoft.Office.Interop.Excel.Range).Value2;
                    dtTemp.Columns.Add(strColumn.ToUpper(), typeof(string));
                }
                dtTemp.Columns.Add("BF", typeof(string));
                dtTemp.Columns.Add("INSURANCE", typeof(string));
                dtTemp.Columns.Add("SUBSCRIPTION", typeof(string));
                dtTemp.Columns.Add("STATUS_CODE", typeof(string));
                dtTemp.Columns.Add("MEMBERCODE", typeof(decimal));
                dtTemp.Columns.Add("MEMBERID", typeof(decimal));
                dtTemp.Columns.Add("BANKCODE", typeof(decimal));
                dtTemp.Columns.Add("REASON", typeof(string));
                dtTemp.Columns.Add("LAST_PAY_DATE", typeof(string));

                for (rowCnt = 5; rowCnt <= excelRange.Rows.Count; rowCnt++)
                {
                    progressBar1.Value = rowCnt;
                    System.Windows.Forms.Application.DoEvents();
                    string strData = "";
                    for (colCnt = 1; colCnt <= excelRange.Columns.Count; colCnt++)
                    {
                        try
                        {
                            strCellData = (string)(excelRange.Cells[rowCnt, colCnt] as Microsoft.Office.Interop.Excel.Range).Value2;
                            strData += strCellData + "|";
                        }
                        catch (Exception ex)
                        {
                            douCellData = (excelRange.Cells[rowCnt, colCnt] as Microsoft.Office.Interop.Excel.Range).Value2;
                            strData += douCellData.ToString() + "|";
                        }
                    }
                    strData = strData.Remove(strData.Length - 1, 1);
                    dtTemp.Rows.Add(strData.Split('|'));
                }
                decimal dAmnt = 0;

                progressBar1.Minimum = 1;
                progressBar1.Maximum = dtTemp.Rows.Count - 1;

                for (int i = 0; i < dtTemp.Rows.Count; i++)
                {
                    progressBar1.Value = i;
                    System.Windows.Forms.Application.DoEvents();
                    int iBF = 0;
                    decimal dBank_Code = 0;
                    decimal dBankCode = Convert.ToDecimal(cmbBankName.SelectedValue);
                    string sIcNo = dtTemp.Rows[i]["NRIC"].ToString();

                    MASTERMEMBER MstMember = DB.MASTERMEMBERs.Where(x => x.ICNO_NEW == sIcNo || x.ICNO_OLD == sIcNo).OrderByDescending(x => x.DATEOFJOINING).FirstOrDefault();
                    if (MstMember != null)
                    {
                        iBF = Convert.ToInt32(MstMember.MONTHLYBF);
                        dtTemp.Rows[i]["BF"] = iBF;
                        dtTemp.Rows[i]["STATUS_CODE"] = MstMember.STATUS_CODE;
                        dtTemp.Rows[i]["MEMBERCODE"] = MstMember.MEMBER_CODE;
                        dtTemp.Rows[i]["MEMBERID"] = MstMember.MEMBER_ID;
                        dtTemp.Rows[i]["BANKCODE"] = MstMember.BANK_CODE;
                        dBank_Code = Convert.ToInt32(MstMember.BANK_CODE);
                        dAmnt = Convert.ToDecimal(dtTemp.Rows[i]["Amount"]);

                        dtTemp.Rows[i]["LAST_PAY_DATE"] = string.Format("{0:dd-MMM-yyyy}", MstMember.LASTPAYMENT_DATE);


                        if (dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MstMember.MEMBER_NAME.ToUpper().Replace(" ", ""))
                        {
                            dtTemp.Rows[i]["REASON"] = "NAME NOT MATCH";
                            dtTemp.Rows[i]["STATUS_CODE"] = "7";
                        }
                        else if (dBank_Code != dBankCode)
                        {
                            dtTemp.Rows[i]["REASON"] = "BANK NOT MATCH";
                        }
                        else if (Convert.ToInt32(dtTemp.Rows[i]["STATUS_CODE"]) != 1 && Convert.ToInt32(dtTemp.Rows[i]["STATUS_CODE"]) != 2)
                        {
                            MASTERSTATU st = (from s in DB.MASTERSTATUS where s.STATUS_CODE == MstMember.STATUS_CODE select s).FirstOrDefault();
                            dtTemp.Rows[i]["REASON"] = "NOT ACTIVE-" + st.STATUS_NAME;
                        }
                        //if (iAmnt == 0)
                        //{
                        //    dtTemp.Rows[i]["REASON"] = "NOT PAY";
                        //}
                    }
                    else
                    {
                        iBF = 0;
                        dtTemp.Rows[i]["BF"] = "0";
                        dtTemp.Rows[i]["STATUS_CODE"] = "6";
                        dtTemp.Rows[i]["MEMBERCODE"] = "0";
                        dtTemp.Rows[i]["MEMBERCODE"] = "0";
                        dtTemp.Rows[i]["BANKCODE"] = "0";
                        dtTemp.Rows[i]["REASON"] = "NRIC NOT MATCH";
                    }

                    dtTemp.Rows[i]["INSURANCE"] = "0";
                    dAmnt = (dAmnt - (iBF));
                    if (dAmnt < 0)
                    {
                        dAmnt = 0;
                    }
                    dtTemp.Rows[i]["SUBSCRIPTION"] = dAmnt.ToString();
                }
                excelBook.Close(true, null, null);
                excelApp.Quit();
                dtFeesEntry = dtTemp.Copy();
            }
        }

        void GridShow()
        {
            decimal dBankCode = Convert.ToDecimal(cmbBankName.SelectedValue);

            DataView dv = new DataView(dtFeesEntry);
            dv.RowFilter = "(STATUS_CODE=1 OR STATUS_CODE=2) AND AMOUNT>0.0 AND BANKCODE=" + dBankCode;
            dtActive = dv.ToTable();

            dv = new DataView(dtFeesEntry);
            dv.RowFilter = "STATUS_CODE<>1 AND STATUS_CODE<>2";
            dtNotMatch = dv.ToTable();

            dv = new DataView(dtFeesEntry);
            dv.RowFilter = "(STATUS_CODE=1 OR STATUS_CODE=2) AND AMOUNT<=0.0 AND BANKCODE=" + dBankCode;
            dtUnPaid = dv.ToTable();

            int i = 0;
            foreach (DataRow row in dtActive.Rows)
            {
                row["ID"] = i + 1;
                i++;
            }
            dgFeeDetails.ItemsSource = dtActive.DefaultView;

            i = 0;
            foreach (DataRow row in dtNotMatch.Rows)
            {
                row["ID"] = i + 1;
                i++;
            }
            dgNotMatch.ItemsSource = dtNotMatch.DefaultView;

            i = 0;
            foreach (DataRow row in dtUnPaid.Rows)
            {
                row["ID"] = i + 1;
                i++;
            }
            dgUnPaid.ItemsSource = dtUnPaid.DefaultView;
        }

        void Total()
        {
            decimal dPMembers = 0;
            decimal dUPMembers = 0;
            decimal dNotMatch = 0;
            decimal dTotalMember = 0;
            decimal dTotalAmnt = 0;

            decimal dBankCode = Convert.ToDecimal(cmbBankName.SelectedValue);
            DataTable dtTotal = new DataTable();
            DataView dv = new DataView(dtFeesEntry);
            dv.RowFilter = "BANKCODE=" + dBankCode;
            dtTotal = dv.ToTable();

            progressBar1.Minimum = 1;
            progressBar1.Maximum = dtFeesEntry.Rows.Count;

            for (int i = 0; i < dtFeesEntry.Rows.Count; i++)
            {
                progressBar1.Value = i;
                System.Windows.Forms.Application.DoEvents();
                dTotalMember = dTotalMember + 1;
                if (!string.IsNullOrEmpty(dtFeesEntry.Rows[i]["Amount"].ToString()))
                {
                    dTotalAmnt = dTotalAmnt + Convert.ToDecimal(dtFeesEntry.Rows[i]["Amount"]);
                    if (Convert.ToDecimal(dtFeesEntry.Rows[i]["Amount"]) != 0)
                    {
                        dPMembers = dPMembers + 1;
                    }
                }
            }
            dNotMatch = dtNotMatch.Rows.Count;
            dUPMembers = dtUnPaid.Rows.Count;


            txtPaidMembers.Text = dPMembers.ToString();
            txtUnPaidMembers.Text = dUPMembers.ToString();
            txtNotMatch.Text = dNotMatch.ToString();
            txtTotalMembers.Text = dTotalMember.ToString();
            txtTotalAmount.Text = dTotalAmnt.ToString();
        }

        void BeforeUpdate()
        {
            if (dtNotMatch.Rows.Count > 0)
            {
                foreach (DataRow dr in dtNotMatch.Rows)
                {
                    if (dr["REASON"].ToString() != "NAME NOT MATCH")
                    {
                        MessageBox.Show("Other Member Detials Not Allowed");
                        dgNotMatch.Focus();
                        bValidate = true;
                        return;
                    }
                }
            }

            if (dtUnPaid.Rows.Count > 0)
            {
                foreach (DataRow dr in dtUnPaid.Rows)
                {
                    if (string.IsNullOrEmpty(dr["REASON"].ToString()))
                    {
                        MessageBox.Show("Un Paid Member Detials Not Allowed");
                        dgNotMatch.Focus();
                        bValidate = true;
                        return;
                    }
                }
            }

            //MessageBox.Show("This Bank Details already in DB");
            //bValidate = true;
            //return;

            if (!(dgFeeDetails.Items.Count > 0))
            {
                MessageBox.Show("This Fee Details are Empty");
                bValidate = true;
                return;
            }

            if (string.IsNullOrEmpty(txtYear.Text))
            {
                MessageBox.Show("Year Is Empty!");
                bValidate = true;
                txtYear.Focus();
            }
            else if (string.IsNullOrEmpty(txtMonth.Text))
            {
                MessageBox.Show("Month Is Empty!");
                bValidate = true;
                txtMonth.Focus();
            }
            else if (string.IsNullOrEmpty(cmbBankName.Text))
            {
                MessageBox.Show("Bank Is Empty!");
                bValidate = true;
                cmbBankName.Focus();
            }
        }

        #endregion       
    }
}
