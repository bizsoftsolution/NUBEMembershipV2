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

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmFeesEntry.xaml
    /// </summary>
    public partial class frmFeesEntryTest : MetroWindow
    {
        nubebfsEntity DB = new nubebfsEntity();
        OpenFileDialog OpenDialogBox = new OpenFileDialog();

        decimal dBankCode = 0;
        decimal dAltrBankCode = 0;
        DataTable dtFeesEntry = new DataTable();
        DataTable dtActive = new DataTable();
        DataTable dtNameNotMatch = new DataTable();
        DataTable dtBankNotMatch = new DataTable();
        DataTable dtICNotMatch = new DataTable();
        DataTable dtNotActiveMember = new DataTable();
        DataTable dtOneOrMore = new DataTable();
        DataTable dtOtherStatus = new DataTable();
        DataTable dtResigned = new DataTable();
        DataTable dtRNameMatch = new DataTable();
        DataTable dtUnPaid = new DataTable();
        Boolean bInsering = true;
        Boolean bValidate = false;

        string sXLShowFileName = "";
        string sXLShowPswd = "";
        string sXLFileName = "";
        string sXLFilePswd = "";

        string qry = "";

        public frmFeesEntryTest()
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
            //try
            //{
            //    bValidate = false;
            //    //BeforeUpdate();

            //    string sourcePath = @"" + sXLFileName;
            //    string SourceFileName = OpenDialogBox.SafeFileName.ToString();
            //    string sDestinationPath = @Environment.CurrentDirectory + "\\Fee Entry Sheets\\" + txtYear.Text + @"\" + txtMonth.Text + @"\" + cmbBankName.Text + @"\";
            //    string sourceFile = System.IO.Path.Combine(sourcePath);
            //    string destinationFile = System.IO.Path.Combine(sDestinationPath, SourceFileName);

            //    int iBnkCode = Convert.ToInt32(cmbBankName.SelectedValue);

            //    if (bInsering == true)
            //    {
            //        if (bValidate == false)
            //        {
            //            //FeesMaster FeesMst = new FeesMaster
            //            //{
            //            //    BankId = iBnkCode,
            //            //    FeeDate = Convert.ToDateTime(String.Format("{0:MMM/dd/yyyy}", txtMonth.Text + "-" + "01" + "-" + txtYear.Text)),
            //            //    XLFileName = destinationFile,
            //            //    XLPassword = txtXLPassword.Password.ToString(),
            //            //    UpdatedStatus = "Not Updated",
            //            //};
            //            //DB.FeesMasters.Add(FeesMst);
            //            TESTFeeMaster FeesMst = new TESTFeeMaster
            //            {
            //                BankId = iBnkCode,
            //                FeeDate = Convert.ToDateTime(String.Format("{0:MMM/dd/yyyy}", txtMonth.Text + "-" + "01" + "-" + txtYear.Text)),
            //                XLFileName = destinationFile,
            //                XLPassword = txtXLPassword.Password.ToString(),
            //                UpdatedStatus = "Not Updated",
            //            };
            //            DB.TESTFeeMasters.Add(FeesMst);
            //            DB.SaveChanges();

            //            //var NewData = new JSonHelper().ConvertObjectToJSon(FeesMst);
            //            //AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "FeesMaster");

            //            //var sFid = DB.FeesMasters.Max(x => x.FeeId).ToString();
            //            var sFid = DB.TESTFeeMasters.Max(x => x.FeeId).ToString();

            //            DataTable dt = new DataTable();
            //            dt = ((DataView)dgFeeDetails.ItemsSource).ToTable();

            //            DateTime YearName = Convert.ToDateTime("01" + "/" + txtMonth.Text + "/" + txtYear.Text);

            //            List<TESTFeesDetail> lstFeesDetls = new List<TESTFeesDetail>();
            //            progressBar1.Minimum = 5;
            //            progressBar1.Maximum = dt.Rows.Count;
            //            progressBar1.Visibility = Visibility.Visible;

            //            for (int i = 0; i < dt.Rows.Count; i++)
            //            {
            //                string IcNo = dt.Rows[i]["NRIC"].ToString();
            //                progressBar1.Value = i;
            //                System.Windows.Forms.Application.DoEvents();

            //                TESTFeesDetail FeesDtl = new TESTFeesDetail
            //                {
            //                    FeeId = Convert.ToInt32(sFid),
            //                    MemberCode = Convert.ToDecimal(dt.Rows[i]["MEMBERCODE"]),
            //                    TotalAmount = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]),
            //                    AmountBF = Convert.ToDecimal(dt.Rows[i]["BF"]),
            //                    AmountIns = Convert.ToDecimal(dt.Rows[i]["INSURANCE"]),
            //                    AmtSubs = Convert.ToDecimal(dt.Rows[i]["SUBSCRIPTION"]),
            //                    Dept = (dt.Rows[i]["DEPARTMENT"]).ToString(),
            //                    UpdatedStatus = "Not Updated",
            //                    FeeYear = YearName.Year,
            //                    FeeMonth = YearName.Month,
            //                    Reason = "",
            //                    IsUnPaid = false,
            //                    IsNotMatch = false,
            //                };
            //                lstFeesDetls.Add(FeesDtl);
            //                //var NewData1 = new JSonHelper().ConvertObjectToJSon(FeesDtl);
            //                //AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData1, "FeesDetail", "MemberCode-" + dt.Rows[i]["MEMBERCODE"].ToString() + " & AMOUNT PAID ");
            //            }

            //            if (lstFeesDetls != null)
            //            {
            //                //DB.FeesDetails.AddRange(lstFeesDetls);
            //                DB.TESTFeesDetails.AddRange(lstFeesDetls);
            //                DB.SaveChanges();
            //            }

            //            dt.Rows.Clear();
            //            dt = ((DataView)dgNotMatch.ItemsSource).ToTable();
            //            lstFeesDetls = new List<TESTFeesDetail>();

            //            progressBar1.Minimum = 5;
            //            progressBar1.Maximum = dt.Rows.Count;
            //            progressBar1.Visibility = Visibility.Visible;

            //            for (int i = 0; i < dt.Rows.Count; i++)
            //            {
            //                progressBar1.Value = i;
            //                System.Windows.Forms.Application.DoEvents();
            //                TESTFeesDetail FeesDtl = new TESTFeesDetail
            //                {
            //                    FeeId = Convert.ToInt32(sFid),
            //                    MemberCode = Convert.ToDecimal(dt.Rows[i]["MEMBERCODE"]),
            //                    TotalAmount = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]),
            //                    AmountBF = Convert.ToDecimal(dt.Rows[i]["BF"]),
            //                    AmountIns = Convert.ToDecimal(dt.Rows[i]["INSURANCE"]),
            //                    AmtSubs = Convert.ToDecimal(dt.Rows[i]["SUBSCRIPTION"]),
            //                    Dept = (dt.Rows[i]["DEPARTMENT"]).ToString(),
            //                    UpdatedStatus = "Not Updated",
            //                    FeeYear = YearName.Year,
            //                    FeeMonth = YearName.Month,
            //                    Reason = dt.Rows[i]["REASON"].ToString(),
            //                    IsUnPaid = false,
            //                    IsNotMatch = true,
            //                    MemberName_Bank = dt.Rows[i]["MEMBERNAME"].ToString(),
            //                    NRIC_Bank = dt.Rows[i]["NRIC"].ToString()
            //                };
            //                lstFeesDetls.Add(FeesDtl);
            //                //var NewData3 = new JSonHelper().ConvertObjectToJSon(FeesDtl);
            //                //AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData3, "FeesDetail", "MemberCode-" + dt.Rows[i]["MEMBERCODE"].ToString() + " & " + dt.Rows[i]["REASON"].ToString());
            //            }

            //            if (lstFeesDetls.Count > 0)
            //            {
            //                DB.TESTFeesDetails.AddRange(lstFeesDetls);
            //                DB.SaveChanges();
            //            }

            //            dt.Rows.Clear();
            //            dt = ((DataView)dgUnPaid.ItemsSource).ToTable();
            //            lstFeesDetls = new List<TESTFeesDetail>();

            //            for (int i = 0; i < dt.Rows.Count; i++)
            //            {
            //                TESTFeesDetail FeesDtl = new TESTFeesDetail
            //                {
            //                    FeeId = Convert.ToInt32(sFid),
            //                    MemberCode = Convert.ToDecimal(dt.Rows[i]["MEMBERCODE"]),
            //                    TotalAmount = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]),
            //                    AmountBF = Convert.ToDecimal(dt.Rows[i]["BF"]),
            //                    AmountIns = Convert.ToDecimal(dt.Rows[i]["INSURANCE"]),
            //                    AmtSubs = Convert.ToDecimal(dt.Rows[i]["SUBSCRIPTION"]),
            //                    Dept = (dt.Rows[i]["DEPARTMENT"]).ToString(),
            //                    UpdatedStatus = "Not Updated",
            //                    FeeYear = YearName.Year,
            //                    FeeMonth = YearName.Month,
            //                    Reason = "",
            //                    IsUnPaid = true,
            //                    IsNotMatch = false
            //                };
            //                lstFeesDetls.Add(FeesDtl);
            //                //var NewData2 = new JSonHelper().ConvertObjectToJSon(FeesDtl);
            //                //AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData2, "FeesDetail", "MemberCode-" + dt.Rows[i]["MEMBERCODE"].ToString() + " & AMOUNT UNPAID ");
            //            }

            //            if (lstFeesDetls.Count > 0)
            //            {
            //                DB.TESTFeesDetails.AddRange(lstFeesDetls);
            //                DB.SaveChanges();
            //            }


            //            if (!System.IO.Directory.Exists(sDestinationPath))
            //            {
            //                System.IO.Directory.CreateDirectory(sDestinationPath);
            //            }
            //            System.IO.File.Copy(sourceFile, destinationFile, true);

            //            MessageBox.Show("Details are Saved Sucessfully");
            //            progressBar1.Visibility = Visibility.Hidden;
            //            cmbBankName.Text = "";
            //            txtMonth.Text = "";
            //            txtYear.Text = "";
            //            txtTotalAmount.Text = "";
            //            txtNotMatch.Text = "";
            //            txtPaidMembers.Text = "";
            //            txtTotalMembers.Text = "";
            //            txtUnPaidMembers.Text = "";
            //            txtXLPassword.Password = "";
            //            dgFeeDetails.ItemsSource = null;
            //            dgNotMatch.ItemsSource = null;
            //            dgUnPaid.ItemsSource = null;
            //        }
            //    }
            //    else
            //    {
            //        DateTime YearName = Convert.ToDateTime("01" + "/" + txtMonth.Text + "/" + txtYear.Text);
            //        FeesMaster FeeMst = DB.FeesMasters.Where(x => x.BankId == iBnkCode && DateTime.Compare(x.FeeDate, YearName) <= 0).FirstOrDefault();
            //        int iFeeId = Convert.ToInt32(FeeMst.FeeId);

            //        FeesMaster FeesMst = (from m in DB.FeesMasters where m.BankId == iFeeId select m).Single();
            //        //var OldData = new JSonHelper().ConvertObjectToJSon(FeesMst);
            //        FeesMst.BankId = iBnkCode;
            //        FeesMst.FeeDate = Convert.ToDateTime("01" + "/" + txtMonth.Text + "/" + txtYear.Text);
            //        FeesMst.XLFileName = destinationFile;
            //        FeesMst.XLPassword = txtXLPassword.Password;
            //        DB.SaveChanges();

            //        //var NewData = new JSonHelper().ConvertObjectToJSon(FeesMst);
            //        //AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "FeesMaster");

            //        var emp = (from CH in DB.FeesDetails where CH.FeeId == iFeeId select CH).ToList();
            //        //var OldData1 = new JSonHelper().ConvertObjectToJSon(emp);
            //        if (emp != null)
            //        {
            //            DB.FeesDetails.RemoveRange(DB.FeesDetails.Where(x => x.FeeId == iFeeId));
            //            DB.SaveChanges();
            //        }

            //        DataTable dt = new DataTable();
            //        dt = ((DataView)dgFeeDetails.ItemsSource).ToTable();
            //        for (int i = 0; i < dt.Rows.Count; i++)
            //        {
            //            string IcNo = dt.Rows[i]["ICNO"].ToString();
            //            var iMember_Code = Convert.ToInt32(DB.MASTERMEMBERs.Where(x => x.ICNO_NEW == IcNo).Select(x => x.MEMBER_CODE).FirstOrDefault());
            //            FeesDetail FeesDtl = new FeesDetail
            //            {
            //                FeeId = Convert.ToInt32(iFeeId),
            //                MemberCode = Convert.ToInt32(iMember_Code),
            //                TotalAmount = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]),
            //                AmountBF = Convert.ToDecimal(dt.Rows[i]["BF"]),
            //                AmountIns = Convert.ToDecimal(dt.Rows[i]["INSURANCE"]),
            //                AmtSubs = Convert.ToDecimal(dt.Rows[i]["SUBSCRIPTION"]),
            //                Dept = (dt.Rows[i]["DEPARTMENT"]).ToString(),
            //                UpdatedStatus = "Not Updated",
            //                FeeYear = YearName.Year,
            //                FeeMonth = YearName.Month,
            //                Reason = "",
            //                IsUnPaid = false
            //            };
            //            DB.FeesDetails.Add(FeesDtl);
            //            DB.SaveChanges();

            //            //var NewData1 = new JSonHelper().ConvertObjectToJSon(FeesDtl);
            //            //AppLib.EventHistory(this.Tag.ToString(), 1, OldData1, NewData1, "FeesDetail", "PAID");
            //        }
            //        if (!System.IO.Directory.Exists(sDestinationPath))
            //        {
            //            System.IO.Directory.CreateDirectory(sDestinationPath);
            //        }
            //        System.IO.File.Copy(sourceFile, destinationFile, true);

            //        MessageBox.Show("Details are Updated Sucessfully");
            //        cmbBankName.Text = "";
            //        txtMonth.Text = "";
            //        txtYear.Text = "";
            //        txtTotalAmount.Text = "";
            //        txtNotMatch.Text = "";
            //        txtPaidMembers.Text = "";
            //        txtTotalMembers.Text = "";
            //        txtUnPaidMembers.Text = "";
            //        txtXLPassword.Password = "";
            //        dgFeeDetails.ItemsSource = null;
            //        dgNotMatch.ItemsSource = null;
            //        dgUnPaid.ItemsSource = null;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ExceptionLogging.SendErrorToText(ex);
            //}
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bInsering = true;
                ImportExcel();
                GridShow();
                //Total();
                MessageBox.Show("Imported Sucessfully");
                progressBar1.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                dgBankNotMatch.ItemsSource = null;
                dgNRICNotMatch.ItemsSource = null;
                dgNotActiveMember.ItemsSource = null;
                //dgUnPaid.ItemsSource = null;
                dtFeesEntry.Rows.Clear();
                dtActive.Rows.Clear();
                dtNameNotMatch.Rows.Clear();
                dtBankNotMatch.Rows.Clear();
                dtICNotMatch.Rows.Clear();
                dtNotActiveMember.Rows.Clear();
                dtOneOrMore.Rows.Clear();
                dtOtherStatus.Rows.Clear();
                dtResigned.Rows.Clear();
                dtRNameMatch.Rows.Clear();
                dtUnPaid.Rows.Clear();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    if (string.IsNullOrEmpty(txtYear.Text))
            //    {
            //        MessageBox.Show("Year Is Empty!");
            //        txtYear.Focus();
            //    }
            //    else if (string.IsNullOrEmpty(txtMonth.Text))
            //    {
            //        MessageBox.Show("Month Is Empty!");
            //        txtMonth.Focus();
            //    }
            //    else if (string.IsNullOrEmpty(cmbBankName.Text))
            //    {
            //        MessageBox.Show("Bank Is Empty!");
            //        cmbBankName.Focus();
            //    }
            //    else
            //    {
            //        bInsering = false;
            //        int iBankCode = Convert.ToInt32(cmbBankName.SelectedValue);
            //        DateTime YearName = Convert.ToDateTime("01" + "/" + txtMonth.Text + "/" + txtYear.Text);
            //        FeesMaster FeeMst = DB.FeesMasters.Where(x => x.BankId == iBankCode && DateTime.Compare(x.FeeDate, YearName) <= 0).FirstOrDefault();
            //        if (FeeMst != null)
            //        {
            //            int iFeeId = Convert.ToInt32(FeeMst.FeeId);
            //            sXLShowFileName = FeeMst.XLFileName;
            //            sXLShowPswd = FeeMst.XLPassword;

            //            var sql = (from f in DB.FeesDetails
            //                       join mm in DB.MASTERMEMBERs on f.MemberCode equals mm.MEMBER_CODE
            //                       where f.FeeId == iFeeId
            //                       select new
            //                       {
            //                           ICNO = mm.ICNO_NEW,
            //                           MEMBERNAME = mm.MEMBER_NAME,
            //                           AMOUNT = f.TotalAmount,
            //                           DEPARTMENT = f.Dept,
            //                           BF = f.AmountBF,
            //                           INSURANCE = f.AmountIns,
            //                           SUBSCRIPTION = f.AmtSubs
            //                       }
            //                       ).ToList();

            //            dgFeeDetails.ItemsSource = sql;
            //            dgFeeDetails.IsReadOnly = false;
            //            dgFeeDetails.CanUserAddRows = true;

            //            DataTable dtNewFee = new DataTable();
            //            if (sXLShowFileName != null)
            //            {
            //                ImportExcel();

            //                //dtInActive = dtFeesEntry.Select("STATUS_CODE=3").CopyToDataTable();
            //                //dtResign = dtFeesEntry.Select("STATUS_CODE=4").CopyToDataTable();
            //                //dtNotMatch = dtFeesEntry.Select("STATUS_CODE=6").CopyToDataTable();

            //                //dgStruckOff.ItemsSource = dtInActive.DefaultView;
            //                //dgDisContinue.ItemsSource = dtResign.DefaultView;
            //                //dgRejoin.ItemsSource = dtNotMatch.DefaultView;

            //                int iPMembers = 0;
            //                int iUPMembers = 0;
            //                int iNotMatch = 0;
            //                int iTotalMember = 0;
            //                int iTotalAmnt = 0;

            //                iNotMatch = dtNotMatch.Rows.Count;
            //                //iTotalMember = dtActive.Rows.Count + dtInActive.Rows.Count + dtResign.Rows.Count + iNotMatch;

            //                //for (int i = 0; i < dtActive.Rows.Count; i++)
            //                //{
            //                //    iTotalAmnt = iTotalAmnt + Convert.ToInt32(dtActive.Rows[i]["Amount"]);
            //                //    if (dtActive.Rows[i]["Amount"] != "0")
            //                //    {
            //                //        iPMembers = iPMembers + 1;
            //                //    }
            //                //    else
            //                //    {
            //                //        iUPMembers = iUPMembers + 1;
            //                //    }
            //                //}
            //                //for (int i = 0; i < dtInActive.Rows.Count; i++)
            //                //{
            //                //    iTotalAmnt = iTotalAmnt + Convert.ToInt32(dtInActive.Rows[i]["Amount"]);
            //                //    if (dtInActive.Rows[i]["Amount"] != "0")
            //                //    {
            //                //        iPMembers = iPMembers + 1;
            //                //    }
            //                //    else
            //                //    {
            //                //        iUPMembers = iUPMembers + 1;
            //                //    }
            //                //}
            //                //for (int i = 0; i < dtResign.Rows.Count; i++)
            //                //{
            //                //    iTotalAmnt = iTotalAmnt + Convert.ToInt32(dtResign.Rows[i]["Amount"]);
            //                //    if (dtResign.Rows[i]["Amount"] != "0")
            //                //    {
            //                //        iPMembers = iPMembers + 1;
            //                //    }
            //                //    else
            //                //    {
            //                //        iUPMembers = iUPMembers + 1;
            //                //    }
            //                //}
            //                //for (int i = 0; i < dtNotMatch.Rows.Count; i++)
            //                //{
            //                //    iTotalAmnt = iTotalAmnt + Convert.ToInt32(dtNotMatch.Rows[i]["Amount"]);
            //                //    if (dtNotMatch.Rows[i]["Amount"] != "0")
            //                //    {
            //                //        iPMembers = iPMembers + 1;
            //                //    }
            //                //    else
            //                //    {
            //                //        iUPMembers = iUPMembers + 1;
            //                //    }
            //                //}

            //                txtPaidMembers.Text = iPMembers.ToString();
            //                txtUnPaidMembers.Text = iUPMembers.ToString();
            //                txtNotMatch.Text = iNotMatch.ToString();
            //                txtTotalMembers.Text = iTotalMember.ToString();
            //                txtTotalAmount.Text = iTotalAmnt.ToString();
            //            }
            //        }
            //        else
            //        {
            //            MessageBox.Show("No Data Found!");
            //            cmbBankName.Focus();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ExceptionLogging.SendErrorToText(ex);
            //}
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
            dgBankNotMatch.ItemsSource = null;
            dgNRICNotMatch.ItemsSource = null;
            dgNotActiveMember.ItemsSource = null;
            dgUnPaid.ItemsSource = null;
            dtFeesEntry.Rows.Clear();
            dtActive.Rows.Clear();
            dtNameNotMatch.Rows.Clear();
            dtBankNotMatch.Rows.Clear();
            dtICNotMatch.Rows.Clear();
            dtNotActiveMember.Rows.Clear();
            dtOneOrMore.Rows.Clear();
            dtOtherStatus.Rows.Clear();
            dtResigned.Rows.Clear();
            dtRNameMatch.Rows.Clear();
            dtUnPaid.Rows.Clear();
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
                progressBar1.Minimum = 5;
                progressBar1.Maximum = excelRange.Rows.Count;
                progressBar1.Visibility = Visibility.Visible;
                progressBar1.Height = 50;
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
                if (rbtnMemberID.IsChecked == true)
                {
                    dtTemp.Columns.Add("NRIC", typeof(string));
                }
                dtTemp.Columns.Add("BF", typeof(string));
                dtTemp.Columns.Add("INSURANCE", typeof(string));
                dtTemp.Columns.Add("SUBSCRIPTION", typeof(string));
                dtTemp.Columns.Add("STATUS_CODE", typeof(string));
                dtTemp.Columns.Add("MEMBERCODE", typeof(decimal));
                dtTemp.Columns.Add("BANKCODE", typeof(decimal));
                dtTemp.Columns.Add("REASON", typeof(string));
                dtTemp.Columns.Add("LAST_PAY_DATE", typeof(string));
                dtTemp.Columns.Add("OLD", typeof(string));
                dtTemp.Columns.Add("OLD2", typeof(string));

                for (rowCnt = 5; rowCnt <= excelRange.Rows.Count; rowCnt++)
                {
                    progressBar1.Value = rowCnt;
                    System.Windows.Forms.Application.DoEvents();
                    string strData = "";
                    Boolean bTotal = true;
                    for (colCnt = 1; colCnt <= excelRange.Columns.Count; colCnt++)
                    {
                        try
                        {
                            strCellData = (string)(excelRange.Cells[rowCnt, colCnt] as Microsoft.Office.Interop.Excel.Range).Value2;
                            if (colCnt == 3)
                            {
                                strData += strCellData.ToUpper() + "|";
                            }
                            else
                            {
                                strData += strCellData + "|";
                            }

                            if (colCnt == 1)
                            {
                                if (string.IsNullOrEmpty(strCellData.ToString()))
                                {
                                    bTotal = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            douCellData = (excelRange.Cells[rowCnt, colCnt] as Microsoft.Office.Interop.Excel.Range).Value2;
                            strData += douCellData.ToString() + "|";

                            if (colCnt == 1)
                            {
                                if (string.IsNullOrEmpty(douCellData.ToString()))
                                {
                                    bTotal = false;
                                }
                            }
                        }
                    }
                    strData = strData.Remove(strData.Length - 1, 1);
                    if (bTotal == true)
                    {
                        dtTemp.Rows.Add(strData.Split('|'));
                    }
                    bTotal = true;
                }
                decimal iAmnt = 0;

                progressBar1.Minimum = 1;
                progressBar1.Maximum = dtTemp.Rows.Count - 1;

                if (AppLib.lstMstMember == null)
                {
                    AppLib.lstMstMember = DB.ViewMasterMembers.OrderByDescending(x => x.DATEOFJOINING).ToList();
                }
                else if (AppLib.lstMstMember.Count == 0)
                {
                    AppLib.lstMstMember = DB.ViewMasterMembers.OrderByDescending(x => x.DATEOFJOINING).ToList();
                }

                if (rbtNRIC.IsChecked == true)
                {
                    for (int i = 0; i < dtTemp.Rows.Count; i++)
                    {
                        progressBar1.Value = i;
                        System.Windows.Forms.Application.DoEvents();
                        decimal iBF = 0;
                        decimal dBank_Code = 0;
                        dBankCode = Convert.ToDecimal(cmbBankName.SelectedValue);

                        string sIcNo = dtTemp.Rows[i]["NRIC"].ToString();
                        //MASTERMEMBER MstMember = DB.MASTERMEMBERs.Where(x => x.ICNO_NEW == sIcNo || x.ICNO_OLD == sIcNo || x.NRIC_ByBank == sIcNo).OrderByDescending(x => x.DATEOFJOINING).FirstOrDefault();
                        var MstMember = AppLib.lstMstMember.Where(x => x.ICNO_NEW == sIcNo || x.ICNO_OLD == sIcNo || x.NRIC_BYBANK == sIcNo).OrderByDescending(x => x.DATEOFJOINING).FirstOrDefault();
                        dAltrBankCode = 0;
                        if (MstMember != null)
                        {
                            iBF = Convert.ToDecimal(MstMember.MONTHLYBF);
                            dtTemp.Rows[i]["BF"] = iBF;
                            dtTemp.Rows[i]["STATUS_CODE"] = MstMember.MEMBERSTATUSCODE;
                            dtTemp.Rows[i]["MEMBERCODE"] = MstMember.MEMBER_CODE;
                            dtTemp.Rows[i]["BANKCODE"] = MstMember.BANK_CODE;
                            dBank_Code = Convert.ToDecimal(MstMember.BANK_CODE);
                            dAltrBankCode = Convert.ToDecimal(MstMember.BANK_CODE);
                            iAmnt = Convert.ToDecimal(dtTemp.Rows[i]["Amount"]);
                            dtTemp.Rows[i]["LAST_PAY_DATE"] = string.Format("{0:dd-MMM-yyyy}", MstMember.LASTPAYMENT_DATE);
                            string sStatus = MstMember.MEMBERSTATUSCODE.ToString();
                            string sReason = "";
                            string sOld = "";

                            if (MstMember.RESIGNED == 1)
                            {
                                sStatus = "12";
                                sReason = "RESIGNED";
                                sOld = string.Format("{0:dd-MMM-yyyy}", MstMember.LASTPAYMENT_DATE);
                            }

                            if (MstMember.MEMBERNAME_BYBANK != null)
                            {
                                if ((dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MstMember.MEMBER_NAME.ToUpper().Replace(" ", "")) && dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MstMember.MEMBERNAME_BYBANK.ToUpper().Replace(" ", ""))
                                {
                                    if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                                    {
                                        sStatus = "10";
                                        sReason = sReason + "- NAME NOT MATCH";
                                        sOld = sOld + "-" + MstMember.MEMBER_NAME.ToString();
                                    }
                                    else
                                    {
                                        sStatus = "6";
                                        sReason = "NAME NOT MATCH";
                                        sOld = MstMember.MEMBER_NAME.ToString();
                                    }
                                }
                            }
                            else if ((dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MstMember.MEMBER_NAME.ToUpper().Replace(" ", "")))
                            {
                                if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                                {
                                    sStatus = "10";
                                    sReason = sReason + "- NAME NOT MATCH";
                                    sOld = sOld + "-" + MstMember.MEMBER_NAME.ToString();
                                }
                                else
                                {
                                    sStatus = "6";
                                    sReason = "NAME NOT MATCH";
                                    sOld = MstMember.MEMBER_NAME.ToString();
                                }
                            }

                            if (dBank_Code != dBankCode && dBankCode != MstMember.BANKCODE_BYBANK)
                            {
                                var st = (from s in DB.MASTERBANKs where s.BANK_CODE == MstMember.BANK_CODE select s).FirstOrDefault();
                                if (st != null)
                                {
                                    if (!string.IsNullOrEmpty(sOld))
                                    {
                                        sOld = sOld + "- " + st.BANK_NAME.ToString();
                                    }
                                    else
                                    {
                                        sOld = st.BANK_NAME.ToString();
                                    }
                                }
                                if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                                {
                                    sStatus = "10";
                                    sReason = sReason + "- BANK NOT MATCH";
                                }
                                else
                                {
                                    sStatus = "7";
                                    sReason = "BANK NOT MATCH";
                                }
                            }

                            if (Convert.ToInt32(MstMember.MEMBERSTATUSCODE) != 1 && Convert.ToInt32(MstMember.RESIGNED) != 1 && Convert.ToInt32(MstMember.MEMBERSTATUSCODE) != 2)
                            {
                                MASTERSTATU st = (from s in DB.MASTERSTATUS where s.STATUS_CODE == MstMember.MEMBERSTATUSCODE select s).FirstOrDefault();
                                if (st != null)
                                {
                                    if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                                    {
                                        sStatus = "10";
                                        sReason = sReason + "- " + st.STATUS_NAME;
                                        sOld = sOld + "-" + string.Format("{0:dd-MMM-yyyy}", MstMember.LASTPAYMENT_DATE);
                                    }
                                    else
                                    {
                                        sStatus = "9";
                                        sReason = st.STATUS_NAME;
                                        sOld = string.Format("{0:dd-MMM-yyyy}", MstMember.LASTPAYMENT_DATE);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                                    {
                                        sStatus = "10";
                                        sReason = sReason + "- NOT ACTIVE-OTHER";
                                        sOld = sOld + "-" + MstMember.STATUS_CODE.ToString();
                                    }
                                    else
                                    {
                                        sStatus = "11";
                                        sReason = "NOT ACTIVE-OTHER";
                                        sOld = MstMember.STATUS_CODE.ToString();
                                    }
                                }
                            }
                            dtTemp.Rows[i]["REASON"] = sReason;
                            dtTemp.Rows[i]["STATUS_CODE"] = sStatus;
                            dtTemp.Rows[i]["OLD"] = sOld;
                        }
                        else
                        {
                            string sName = dtTemp.Rows[i]["MEMBERNAME"].ToString();
                            //MASTERMEMBER mst = (from x in DB.MASTERMEMBERs where x.MEMBER_NAME.Contains(sName) || x.MemberName_ByBank.Contains(sName) select x).FirstOrDefault();
                            var mst = (from x in AppLib.lstMstMember where x.MEMBER_NAME.Contains(sName) || x.MEMBERNAME_BYBANK.Contains(sName) select x).FirstOrDefault();

                            if (mst != null)
                            {
                                dtTemp.Rows[i]["REASON"] = "NRIC NOT MATCH BUT NAME MATCH";
                                dtTemp.Rows[i]["STATUS_CODE"] = "13";
                                if (mst.ICNO_NEW != null)
                                {
                                    dtTemp.Rows[i]["OLD"] = mst.ICNO_NEW.ToString();
                                }
                                if (mst.ICNO_OLD != null)
                                {
                                    dtTemp.Rows[i]["OLD2"] = mst.ICNO_OLD.ToString();
                                }

                                iBF = Convert.ToDecimal(mst.MONTHLYBF);
                                dtTemp.Rows[i]["BF"] = iBF;
                                dtTemp.Rows[i]["MEMBERCODE"] = mst.MEMBER_CODE;
                                dtTemp.Rows[i]["BANKCODE"] = mst.BANK_CODE;
                                dBank_Code = Convert.ToDecimal(mst.BANK_CODE);
                                iAmnt = Convert.ToDecimal(dtTemp.Rows[i]["Amount"]);
                                dtTemp.Rows[i]["LAST_PAY_DATE"] = string.Format("{0:dd-MMM-yyyy}", mst.LASTPAYMENT_DATE);
                            }
                            else
                            {
                                iBF = 0;
                                dtTemp.Rows[i]["BF"] = "3";
                                dtTemp.Rows[i]["STATUS_CODE"] = "8";
                                dtTemp.Rows[i]["MEMBERCODE"] = "0";
                                dtTemp.Rows[i]["BANKCODE"] = "0";
                                dtTemp.Rows[i]["REASON"] = "NRIC NOT MATCH";
                                iAmnt = Convert.ToDecimal(dtTemp.Rows[i]["Amount"]);
                            }
                        }

                        dtTemp.Rows[i]["INSURANCE"] = "0";
                        iAmnt = (iAmnt - (iBF));
                        if (iAmnt < 0)
                        {
                            iAmnt = 0;
                        }
                        dtTemp.Rows[i]["SUBSCRIPTION"] = iAmnt.ToString();
                    }
                }
                else
                {
                    for (int i = 0; i < dtTemp.Rows.Count; i++)
                    {
                        progressBar1.Value = i;
                        System.Windows.Forms.Application.DoEvents();
                        decimal iBF = 0;
                        decimal dBank_Code = 0;
                        decimal dBankCode = Convert.ToDecimal(cmbBankName.SelectedValue);
                        iAmnt = 0;

                        //decimal dMemberId = Convert.ToDecimal(dtTemp.Rows[i]["Member ID"]);
                        ////MASTERMEMBER MstMember = DB.MASTERMEMBERs.Where(x => x.MEMBER_ID == dMemberId).OrderByDescending(x => x.DATEOFJOINING).FirstOrDefault();
                        //var MstMember = AppLib.lstMstMember.Where(x => x.MEMBER_ID == dMemberId).OrderByDescending(x => x.DATEOFJOINING).FirstOrDefault();
                        string sName = dtTemp.Rows[i]["MemberName"].ToString();
                        var MstMember = AppLib.lstMstMember.Where(x => x.MEMBER_NAME.Contains(sName) || x.MEMBERNAME_BYBANK.Contains(sName)).OrderByDescending(x => x.DATEOFJOINING).FirstOrDefault();

                        if (MstMember != null)
                        {
                            dtTemp.Rows[i]["NRIC"] = MstMember.ICNO_NEW.ToString();
                            iBF = Convert.ToDecimal(MstMember.MONTHLYBF);
                            dtTemp.Rows[i]["BF"] = iBF;
                            dtTemp.Rows[i]["STATUS_CODE"] = MstMember.MEMBERSTATUSCODE;
                            dtTemp.Rows[i]["MEMBERCODE"] = MstMember.MEMBER_CODE;
                            dtTemp.Rows[i]["BANKCODE"] = MstMember.BANK_CODE;
                            dBank_Code = Convert.ToDecimal(MstMember.BANK_CODE);
                            iAmnt = Convert.ToDecimal(dtTemp.Rows[i]["Amount"]);
                            dtTemp.Rows[i]["LAST_PAY_DATE"] = string.Format("{0:dd-MMM-yyyy}", MstMember.LASTPAYMENT_DATE);
                            string sStatus = MstMember.MEMBERSTATUSCODE.ToString();
                            string sReason = "";
                            string sOld = "";

                            if (MstMember.RESIGNED == 1)
                            {
                                sStatus = "12";
                                sReason = "RESIGNED";
                                sOld = string.Format("{0:dd-MMM-yyyy}", MstMember.LASTPAYMENT_DATE);
                            }

                            if (MstMember.MEMBERNAME_BYBANK != null)
                            {
                                if ((dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MstMember.MEMBER_NAME.ToUpper().Replace(" ", "")) && dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MstMember.MEMBERNAME_BYBANK.ToUpper().Replace(" ", ""))
                                {
                                    if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                                    {
                                        sStatus = "10";
                                        sReason = sReason + "- NAME NOT MATCH";
                                        sOld = sOld + "- " + MstMember.MEMBER_NAME.ToString();
                                    }
                                    else
                                    {
                                        sStatus = "6";
                                        sReason = "NAME NOT MATCH";
                                        sOld = MstMember.MEMBER_NAME.ToString();
                                    }
                                }
                            }
                            else if ((dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MstMember.MEMBER_NAME.ToUpper().Replace(" ", "")))
                            {
                                if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                                {
                                    sStatus = "10";
                                    sReason = sReason + "- NAME NOT MATCH";
                                    sOld = sOld + "- " + MstMember.MEMBER_NAME.ToString();
                                }
                                else
                                {
                                    sStatus = "6";
                                    sReason = "NAME NOT MATCH";
                                    sOld = MstMember.MEMBER_NAME.ToString();
                                }
                            }

                            //if (dBank_Code != dBankCode && dBank_Code != MstMember.BANKCODE_BYBANK)
                            //{
                            //    var st = (from s in DB.MASTERBANKs where s.BANK_CODE == MstMember.BANK_CODE select s).FirstOrDefault();
                            //    if (st != null)
                            //    {
                            //        if (!string.IsNullOrEmpty(sOld))
                            //        {
                            //            sOld = sOld + "- " + st.BANK_NAME.ToString();
                            //        }
                            //        else
                            //        {
                            //            sOld = st.BANK_NAME.ToString();
                            //        }
                            //    }
                            //    if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                            //    {
                            //        sStatus = "10";
                            //        sReason = sReason + "- BANK NOT MATCH";
                            //    }
                            //    else
                            //    {
                            //        sStatus = "7";
                            //        sReason = "BANK NOT MATCH";
                            //    }
                            //}

                            //if (Convert.ToInt32(MstMember.MEMBERSTATUSCODE) != 1 && Convert.ToInt32(MstMember.RESIGNED) != 1 && Convert.ToInt32(MstMember.MEMBERSTATUSCODE) != 2)
                            //{
                            //    MASTERSTATU st = (from s in DB.MASTERSTATUS where s.STATUS_CODE == MstMember.MEMBERSTATUSCODE select s).FirstOrDefault();
                            //    if (st != null)
                            //    {
                            //        if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                            //        {
                            //            sStatus = "10";
                            //            sReason = sReason + "- " + st.STATUS_NAME;
                            //            sOld = sOld + "- " + string.Format("{0:dd-MMM-yyyy}", MstMember.LASTPAYMENT_DATE);
                            //        }
                            //        else
                            //        {
                            //            sStatus = "9";
                            //            sReason = st.STATUS_NAME;
                            //            sOld = string.Format("{0:dd-MMM-yyyy}", MstMember.LASTPAYMENT_DATE);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                            //        {
                            //            sStatus = "10";
                            //            sReason = sReason + "- NOT ACTIVE-OTHER";
                            //            sOld = sOld + "-" + MstMember.STATUS_CODE.ToString();
                            //        }
                            //        else
                            //        {
                            //            sStatus = "11";
                            //            sReason = "NOT ACTIVE-OTHER";
                            //            sOld = MstMember.STATUS_CODE.ToString();
                            //        }
                            //    }
                            //}
                            dtTemp.Rows[i]["REASON"] = sReason;
                            dtTemp.Rows[i]["STATUS_CODE"] = sStatus;
                            dtTemp.Rows[i]["OLD"] = sOld;
                        }
                        else
                        {
                            //MASTERMEMBER mst = (from x in DB.MASTERMEMBERs where x.MEMBER_NAME.Contains(dtTemp.Rows[i]["MEMBERNAME"].ToString()) select x).FirstOrDefault();

                            //if (mst != null)
                            //{
                            //    dtTemp.Rows[i]["REASON"] = "MEMBERID NOT MATCH BUT NAME MATCH";
                            //    dtTemp.Rows[i]["STATUS_CODE"] = "13";
                            //    dtTemp.Rows[i]["OLD"] = mst.ICNO_NEW.ToString();
                            //    dtTemp.Rows[i]["OLD2"] = mst.ICNO_OLD.ToString();

                            //    iBF = Convert.ToDecimal(mst.MONTHLYBF);
                            //    dtTemp.Rows[i]["BF"] = iBF;
                            //    dtTemp.Rows[i]["MEMBERCODE"] = mst.MEMBER_CODE;
                            //    dtTemp.Rows[i]["BANKCODE"] = mst.BANK_CODE;
                            //    dBank_Code = Convert.ToDecimal(mst.BANK_CODE);
                            //    iAmnt = Convert.ToDecimal(dtTemp.Rows[i]["Amount"]);
                            //    dtTemp.Rows[i]["LAST_PAY_DATE"] = string.Format("{0:dd-MMM-yyyy}", mst.LASTPAYMENT_DATE);
                            //}
                            //else
                            //{
                            iBF = 0;
                            dtTemp.Rows[i]["BF"] = "3";
                            dtTemp.Rows[i]["STATUS_CODE"] = "6";
                            dtTemp.Rows[i]["MEMBERCODE"] = "0";
                            dtTemp.Rows[i]["BANKCODE"] = "0";
                            dtTemp.Rows[i]["REASON"] = "NAME NOT MATCH";
                            iAmnt = Convert.ToDecimal(dtTemp.Rows[i]["Amount"]);
                            //}
                        }
                        dtTemp.Rows[i]["INSURANCE"] = "0";
                        iAmnt = (iAmnt - (iBF));
                        if (iAmnt < 0)
                        {
                            iAmnt = 0;
                        }
                        dtTemp.Rows[i]["SUBSCRIPTION"] = iAmnt.ToString();
                    }
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
            dv.RowFilter = "(STATUS_CODE='1' OR STATUS_CODE='2') AND AMOUNT<>'0' ";
            dtActive = dv.ToTable();

            dv = new DataView(dtFeesEntry);
            dv.RowFilter = "STATUS_CODE='6'";
            dtNameNotMatch = dv.ToTable();

            dv = new DataView(dtFeesEntry);
            dv.RowFilter = "STATUS_CODE='7'";
            dtBankNotMatch = dv.ToTable();

            dv = new DataView(dtFeesEntry);
            dv.RowFilter = "STATUS_CODE='8'";
            dtICNotMatch = dv.ToTable();

            dv = new DataView(dtFeesEntry);
            dv.RowFilter = "STATUS_CODE='9'";
            dtNotActiveMember = dv.ToTable();

            dv = new DataView(dtFeesEntry);
            dv.RowFilter = "STATUS_CODE='10'";
            dtOneOrMore = dv.ToTable();

            dv = new DataView(dtFeesEntry);
            dv.RowFilter = "STATUS_CODE='11'";
            dtOtherStatus = dv.ToTable();

            dv = new DataView(dtFeesEntry);
            dv.RowFilter = "STATUS_CODE='12'";
            dtResigned = dv.ToTable();

            dv = new DataView(dtFeesEntry);
            dv.RowFilter = "STATUS_CODE='13'";
            dtRNameMatch = dv.ToTable();

            dv = new DataView(dtFeesEntry);
            dv.RowFilter = "(STATUS_CODE='1' OR STATUS_CODE='2') AND AMOUNT='0' ";
            dtUnPaid = dv.ToTable();

            int i = 0;
            foreach (DataRow row in dtActive.Rows)
            {
                row["ID"] = i + 1;
                i++;
            }
            dgFeeDetails.ItemsSource = dtActive.DefaultView;

            i = 0;
            foreach (DataRow row in dtNameNotMatch.Rows)
            {
                row["ID"] = i + 1;
                i++;
            }
            dgNotMatch.ItemsSource = dtNameNotMatch.DefaultView;

            i = 0;
            foreach (DataRow row in dtBankNotMatch.Rows)
            {
                row["ID"] = i + 1;
                i++;
            }
            dgBankNotMatch.ItemsSource = dtBankNotMatch.DefaultView;

            i = 0;
            foreach (DataRow row in dtICNotMatch.Rows)
            {
                row["ID"] = i + 1;
                i++;
            }
            dgNRICNotMatch.ItemsSource = dtICNotMatch.DefaultView;

            i = 0;
            foreach (DataRow row in dtNotActiveMember.Rows)
            {
                row["ID"] = i + 1;
                i++;
            }
            dgNotActiveMember.ItemsSource = dtNotActiveMember.DefaultView;

            i = 0;
            foreach (DataRow row in dtOneOrMore.Rows)
            {
                row["ID"] = i + 1;
                i++;
            }
            dgOneOrMore.ItemsSource = dtOneOrMore.DefaultView;

            i = 0;
            foreach (DataRow row in dtOtherStatus.Rows)
            {
                row["ID"] = i + 1;
                i++;
            }
            dgStatusNotMatch.ItemsSource = dtOtherStatus.DefaultView;

            i = 0;
            foreach (DataRow row in dtResigned.Rows)
            {
                row["ID"] = i + 1;
                i++;
            }

            i = 0;
            foreach (DataRow row in dtRNameMatch.Rows)
            {
                row["ID"] = i + 1;
                i++;
            }
            dgNameMatch.ItemsSource = dtRNameMatch.DefaultView;

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
            int iPMembers = 0;
            int iUPMembers = 0;
            int iNotMatch = 0;
            int iTotalMember = 0;
            decimal iTotalAmnt = 0;

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
                iTotalMember = iTotalMember + 1;
                if (!string.IsNullOrEmpty(dtFeesEntry.Rows[i]["Amount"].ToString()))
                {
                    iTotalAmnt = iTotalAmnt + Convert.ToDecimal(dtFeesEntry.Rows[i]["Amount"]);
                    if (Convert.ToDecimal(dtFeesEntry.Rows[i]["Amount"]) != 0)
                    {
                        iPMembers = iPMembers + 1;
                    }
                }
            }
            //iNotMatch = d.Rows.Count;
            iUPMembers = dtUnPaid.Rows.Count;

            txtPaidMembers.Text = iPMembers.ToString();
            txtUnPaidMembers.Text = iUPMembers.ToString();
            txtNotMatch.Text = iNotMatch.ToString();
            txtTotalMembers.Text = iTotalMember.ToString();
            txtTotalAmount.Text = iTotalAmnt.ToString();
        }

        //void BeforeUpdate()
        //{
        //    if (dtNotMatch.Rows.Count > 0)
        //    {
        //        MessageBox.Show("Other Member Detials Not Allowed");
        //        dgNotMatch.Focus();
        //        bValidate = true;
        //        return;
        //    }

        //    if (dtUnPaid.Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in dtUnPaid.Rows)
        //        {
        //            if (string.IsNullOrEmpty(dr["REASON"].ToString()))
        //            {
        //                MessageBox.Show("Un Paid Member Detials Not Allowed");
        //                dgNotMatch.Focus();
        //                bValidate = true;
        //                return;
        //            }
        //        }
        //    }

        //    //MessageBox.Show("This Bank Details already in DB");
        //    //bValidate = true;
        //    //return;

        //    if (dgFeeDetails.Items.Count > 0)
        //    {
        //        //DateTime YearName = Convert.ToDateTime("01" + "/" + txtMonth.Text + "/" + txtYear.Text);
        //        //int iBnkCode = Convert.ToInt32(cmbBankName.SelectedValue);
        //        //FeesMaster FeeMst = DB.FeesMasters.Where(x => x.BankId == iBnkCode && DateTime.Compare(x.FeeDate, YearName) <= 0).FirstOrDefault();
        //        //if (FeeMst != null)
        //        //{
        //        //    MessageBox.Show("This Bank Details already in DB");
        //        //    bValidate = true;
        //        //    return;
        //        //}
        //        MessageBox.Show("This Bank Details already in DB");
        //        bValidate = true;
        //        return;
        //    }
        //    else
        //    {
        //        MessageBox.Show("This Fee Details are Empty");
        //        bValidate = true;
        //        return;
        //    }

        //    MessageBox.Show("This Bank Details already in DB");
        //    bValidate = true;
        //    return;

        //    if (string.IsNullOrEmpty(txtYear.Text))
        //    {
        //        MessageBox.Show("Year Is Empty!");
        //        bValidate = true;
        //        txtYear.Focus();
        //    }
        //    else if (string.IsNullOrEmpty(txtMonth.Text))
        //    {
        //        MessageBox.Show("Month Is Empty!");
        //        bValidate = true;
        //        txtMonth.Focus();
        //    }
        //    else if (string.IsNullOrEmpty(cmbBankName.Text))
        //    {
        //        MessageBox.Show("Bank Is Empty!");
        //        bValidate = true;
        //        cmbBankName.Focus();
        //    }
        //}

        #endregion

        private void CheckBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataRowView drv = (DataRowView)dgNotMatch.SelectedItem;
            var chk = (CheckBox)sender;
            //var row = <DataGridRow>(chk);
            var newValue = !chk.IsChecked.GetValueOrDefault();

            //row.IsSelected = newValue;
            chk.IsChecked = newValue;

            // Mark event as handled so that the default 
            // DataGridPreviewMouseDown doesn't handle the event
            e.Handled = true;
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void dgNotMatch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (dgNotMatch.CurrentCell.Column.Header.ToString() == "Is Acceptable")
            //{
            //    DataRowView drv = (DataRowView)dgNotMatch.SelectedItem;
            //    if (drv != null)
            //    {
            //        if (MessageBox.Show(this, "Are you Sure to Save it? It will Affect Current Data!", "Save Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //        {
            //            decimal dMemberCode = Convert.ToDecimal(drv["MEMBERCODE"]);
            //            var mm = (from x in DB.MASTERMEMBERs where x.MEMBER_CODE == dMemberCode select x).FirstOrDefault();
            //            if (mm != null)
            //            {
            //                mm.MemberName_ByBank = drv["MEMBERNAME"].ToString();
            //                DB.SaveChanges();

            //                DataTable dt = ((DataView)dgNotMatch.ItemsSource).ToTable();
            //                DataTable dtNotMatch = dt.Select("MEMBERCODE<>" + drv["MEMBERCODE"].ToString()).CopyToDataTable();
            //                int i = 0;
            //                foreach (DataRow dr in dtNotMatch.Rows)
            //                {
            //                    i++;
            //                    dr["ID"] = i;
            //                }
            //                DataTable dtFeeDetails = ((DataView)dgFeeDetails.ItemsSource).ToTable();
            //                dtFeeDetails.Rows.Add(dtFeeDetails.Rows.Count + 1, drv["NRIC"], drv["OLD"], drv["AMOUNT"], drv["DEPARTMENT"], drv["BF"], drv["INSURANCE"], drv["SUBSCRIPTION"], drv["STATUS_CODE"], drv["MEMBERCODE"], drv["BANKCODE"], "", drv["LAST_PAY_DATE"], "", "");

            //                dgNotMatch.ItemsSource = dtNotMatch.DefaultView;
            //                dgFeeDetails.ItemsSource = dtFeeDetails.DefaultView;
            //            }
            //            else
            //            {
            //                MessageBox.Show("No Data Found!");
            //            }

            //        }
            //    }
            //}

        }

        private void dgNotMatch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgNotMatch.CurrentCell.Column.Header.ToString() == "Is Acceptable" && dgNotMatch.SelectedItem != null)
            {
                DataRowView drv = (DataRowView)dgNotMatch.SelectedItem;
                if (drv != null)
                {
                    decimal dMemberCode = Convert.ToDecimal(drv["MEMBERCODE"]);
                    var mm = (from x in DB.MASTERMEMBERs where x.MEMBER_CODE == dMemberCode select x).FirstOrDefault();
                    if (mm != null)
                    {
                        mm.MemberName_ByBank = drv["MEMBERNAME"].ToString();
                        DB.SaveChanges();

                        DataTable dt = ((DataView)dgNotMatch.ItemsSource).ToTable();
                        DataTable dtNotMatch = dt.Select("MEMBERCODE<>" + drv["MEMBERCODE"].ToString()).CopyToDataTable();

                        int i = 0;
                        foreach (DataRow dr in dtNotMatch.Rows)
                        {
                            i++;
                            dr["ID"] = i;
                        }

                        DataTable dtFeeDetails = ((DataView)dgFeeDetails.ItemsSource).ToTable();
                        dtFeeDetails.Rows.Add(dtFeeDetails.Rows.Count + 1, drv["NRIC"], drv["OLD"], drv["AMOUNT"], drv["DEPARTMENT"], drv["BF"], drv["INSURANCE"], drv["SUBSCRIPTION"], drv["STATUS_CODE"], drv["MEMBERCODE"], Convert.ToDecimal(drv["BANKCODE"]), "", drv["LAST_PAY_DATE"], "", "");

                        dgNotMatch.ItemsSource = dtNotMatch.DefaultView;
                        dgFeeDetails.ItemsSource = dtFeeDetails.DefaultView;
                    }
                    else
                    {
                        MessageBox.Show("No Data Found!");
                    }
                }
            }
        }

        private void btnMember_Click(object sender, RoutedEventArgs e)
        {
            //frmFeesCheking frm = new frmFeesCheking();
            //frm.Show();
            //this.Close();
        }

        private void dgNameMatch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgNameMatch.CurrentCell.Column.Header.ToString() == "Is Acceptable" && dgNameMatch.SelectedItem != null)
            {
                DataRowView drv = (DataRowView)dgNameMatch.SelectedItem;
                if (drv != null)
                {
                    decimal dMemberCode = Convert.ToDecimal(drv["MEMBERCODE"]);
                    var mm = (from x in DB.MASTERMEMBERs where x.MEMBER_CODE == dMemberCode select x).FirstOrDefault();
                    if (mm != null)
                    {
                        mm.NRIC_ByBank = drv["NRIC"].ToString();
                        DB.SaveChanges();

                        DataTable dt = ((DataView)dgNameMatch.ItemsSource).ToTable();
                        DataTable dtNotMatch = dt.Select("MEMBERCODE<>" + drv["MEMBERCODE"].ToString()).CopyToDataTable();

                        int i = 0;
                        foreach (DataRow dr in dtNotMatch.Rows)
                        {
                            i++;
                            dr["ID"] = i;
                        }

                        DataTable dtFeeDetails = ((DataView)dgFeeDetails.ItemsSource).ToTable();
                        dtFeeDetails.Rows.Add(dtFeeDetails.Rows.Count + 1, drv["NRIC"], drv["MEMBERNAME"], drv["AMOUNT"], drv["DEPARTMENT"], drv["BF"], drv["INSURANCE"], drv["SUBSCRIPTION"], drv["STATUS_CODE"], drv["MEMBERCODE"], Convert.ToDecimal(drv["BANKCODE"]), "", drv["LAST_PAY_DATE"], "", "");

                        dgNameMatch.ItemsSource = dtNotMatch.DefaultView;
                        dgFeeDetails.ItemsSource = dtFeeDetails.DefaultView;
                    }
                    else
                    {
                        MessageBox.Show("No Data Found!");
                    }
                }
            }
        }

        private void dgBankNotMatch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgBankNotMatch.CurrentCell.Column.Header.ToString() == "Is Acceptable" && dgBankNotMatch.SelectedItem != null)
            {
                DataRowView drv = (DataRowView)dgBankNotMatch.SelectedItem;
                if (drv != null)
                {
                    decimal dMemberCode = Convert.ToDecimal(drv["MEMBERCODE"]);
                    var mm = (from x in DB.MASTERMEMBERs where x.MEMBER_CODE == dMemberCode select x).FirstOrDefault();
                    if (mm != null)
                    {
                        mm.BankCode_ByBank = dBankCode;
                        DB.SaveChanges();

                        DataTable dt = ((DataView)dgBankNotMatch.ItemsSource).ToTable();
                        DataTable dtNotMatch = dt.Select("MEMBERCODE<>" + drv["MEMBERCODE"].ToString()).CopyToDataTable();

                        int i = 0;
                        foreach (DataRow dr in dtNotMatch.Rows)
                        {
                            i++;
                            dr["ID"] = i;
                        }

                        DataTable dtFeeDetails = ((DataView)dgFeeDetails.ItemsSource).ToTable();
                        dtFeeDetails.Rows.Add(dtFeeDetails.Rows.Count + 1, drv["NRIC"], drv["MEMBERNAME"], drv["AMOUNT"], drv["DEPARTMENT"], drv["BF"], drv["INSURANCE"], drv["SUBSCRIPTION"], drv["STATUS_CODE"], drv["MEMBERCODE"], Convert.ToDecimal(drv["BANKCODE"]), "", drv["LAST_PAY_DATE"], "", "");

                        dgBankNotMatch.ItemsSource = dtNotMatch.DefaultView;
                        dgFeeDetails.ItemsSource = dtFeeDetails.DefaultView;
                    }
                    else
                    {
                        MessageBox.Show("No Data Found!");
                    }
                }
            }
        }
    }
}


