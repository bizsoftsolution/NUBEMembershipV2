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

        decimal dBankCode = 0;
        decimal dAltrBankCode = 0;
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

        string qry = "";      

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
                        DataTable dtBankGroup = new DataTable();
                        dtBankGroup = dtFeesEntry.AsEnumerable()
                            .GroupBy(r => new { Col1 = r["FEEYEAR"], Col2 = r["FEEMONTH"], Col3 = r["BANK_CODE"] })
                            .Select(g => g.OrderBy(r => r["BANK_CODE"]).First())
                            .CopyToDataTable();

                        progressBar1.Minimum = 10;
                        progressBar1.Maximum = dtBankGroup.Rows.Count;
                        progressBar1.Visibility = Visibility.Visible;

                        DataTable dtWholeMatched = ((DataView)dgFeeDetails.ItemsSource).ToTable();
                        int irow = 0;
                        foreach (DataRow drFees in dtBankGroup.Rows)
                        {
                            progressBar1.Value = irow + 1;
                            System.Windows.Forms.Application.DoEvents();
                            iBnkCode = Convert.ToInt32(drFees["BANK_CODE"]);
                            DateTime YearName = new DateTime(Convert.ToInt32(txtYear.Text), Convert.ToInt32(txtMonth.Text), 1); //Convert.ToDateTime(String.Format("{0:MMM/dd/yyyy}", "01" + "-" + txtMonth.Text + "-" + txtYear.Text));
                            var FeeMst = (from x in DB.FeesMasters where x.BankId == iBnkCode && x.FeeDate == YearName select x).FirstOrDefault();

                            DataView dv = new DataView(dtWholeMatched);
                            dv.RowFilter = "BANK_CODE=" + Convert.ToInt32(drFees["BANK_CODE"]);
                            DataTable dt = dv.ToTable();

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

                                    var fdtlNt = (from x in DB.FeesDetailsNotMatches where x.FeeId == FeeMst.FeeId select x).ToList();
                                    if (fdtlNt != null)
                                    {
                                        DB.FeesDetailsNotMatches.RemoveRange(DB.FeesDetailsNotMatches.Where(x => x.FeeId == FeeMst.FeeId));
                                        DB.SaveChanges();
                                    }

                                    FeesMaster FeesMst = new FeesMaster
                                    {
                                        BankId = iBnkCode,
                                        FeeDate = YearName,
                                        XLFileName = drFees["BANK_CODE"].ToString(),
                                        XLPassword = txtXLPassword.Password.ToString(),
                                        UpdatedStatus = "Not Updated",
                                    };
                                    DB.FeesMasters.Add(FeesMst);
                                    DB.SaveChanges();

                                    var sFid = DB.FeesMasters.Max(x => x.FeeId).ToString();
                                    dv = new DataView(dt);
                                    dv.RowFilter = "STATUS_CODE=3 AND AMOUNT>0.0 ";
                                    dtNotMatch = dv.ToTable();

                                    List<FeesDetail> lstFeesDetls = new List<FeesDetail>();
                                    progressBar1.Minimum = 0;
                                    progressBar1.Maximum = dt.Rows.Count;
                                    progressBar1.Visibility = Visibility.Visible;
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
                                            FeeYear = Convert.ToInt32(txtYear.Text),
                                            FeeMonth = Convert.ToInt32(txtMonth.Text),
                                            Reason = "",
                                            IsUnPaid = false,
                                            IsNotMatch = false,
                                            MemberId = Convert.ToDecimal(dt.Rows[i]["MEMBERID"]),
                                            Status = "Fees Entry",
                                            TotalMonthsPaid = 1
                                        };
                                        lstFeesDetls.Add(FeesDtl);
                                    }

                                    if (lstFeesDetls != null)
                                    {
                                        DB.FeesDetails.AddRange(lstFeesDetls);
                                        DB.SaveChanges();
                                    }

                                    dt.Rows.Clear();
                                    dt = ((DataView)dgNotMatch.ItemsSource).ToTable();
                                    dv.RowFilter = "BANK_CODE=" + Convert.ToInt32(drFees["BANK_CODE"]);
                                    dt = dv.ToTable();

                                    lstFeesDetls = new List<FeesDetail>();
                                    List<FeesDetailsNotMatch> lstFeesDetlsNotMatch = new List<FeesDetailsNotMatch>();

                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        progressBar1.Value = i;
                                        System.Windows.Forms.Application.DoEvents();
                                        if (dt.Rows[i]["REASON"].ToString().ToUpper() == "YES" || dt.Rows[i]["REASON"].ToString().ToUpper() == "S")
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
                                                FeeYear = Convert.ToInt32(txtYear.Text),
                                                FeeMonth = Convert.ToInt32(txtMonth.Text),
                                                Reason = "IS ACCEPT BY NUBE",
                                                IsUnPaid = false,
                                                IsNotMatch = false,
                                                MemberId = Convert.ToDecimal(dt.Rows[i]["MEMBERID"]),
                                                IsAccept_ByNube = true,
                                                IsStruckOff = false,
                                                Status = "Fees Entry",
                                                TotalMonthsPaid = 1

                                            };
                                            lstFeesDetls.Add(FeesDtl);
                                        }
                                        else if (dt.Rows[i]["REASON"].ToString().ToUpper() == "INACTIVE")
                                        {
                                            FeesDetailsNotMatch FeesDtl = new FeesDetailsNotMatch
                                            {
                                                FeeId = Convert.ToInt32(sFid),
                                                MemberCode = Convert.ToDecimal(dt.Rows[i]["MEMBERCODE"]),
                                                TotalAmount = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]),
                                                AmountBF = Convert.ToDecimal(dt.Rows[i]["BF"]),
                                                AmountIns = Convert.ToDecimal(dt.Rows[i]["INSURANCE"]),
                                                AmtSubs = Convert.ToDecimal(dt.Rows[i]["SUBSCRIPTION"]),
                                                Dept = (dt.Rows[i]["DEPARTMENT"]).ToString(),
                                                UpdatedStatus = "Not Updated",
                                                FeeYear = Convert.ToInt32(txtYear.Text),
                                                FeeMonth = Convert.ToInt32(txtMonth.Text),
                                                Reason = dt.Rows[i]["REASON"].ToString(),
                                                IsUnPaid = false,
                                                IsNotMatch = true,
                                                MemberId = Convert.ToDecimal(dt.Rows[i]["MEMBERID"]),
                                                IsAccept_ByNube = false,
                                                IsStruckOff = true,
                                                Status = "Fees Entry",
                                                TotalMonthsPaid = 1
                                            };
                                            lstFeesDetlsNotMatch.Add(FeesDtl);
                                        }
                                        else
                                        {
                                            FeesDetailsNotMatch FeesDtl = new FeesDetailsNotMatch
                                            {
                                                FeeId = Convert.ToInt32(sFid),
                                                MemberCode = Convert.ToDecimal(dt.Rows[i]["MEMBERCODE"]),
                                                TotalAmount = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]),
                                                AmountBF = Convert.ToDecimal(dt.Rows[i]["BF"]),
                                                AmountIns = Convert.ToDecimal(dt.Rows[i]["INSURANCE"]),
                                                AmtSubs = Convert.ToDecimal(dt.Rows[i]["SUBSCRIPTION"]),
                                                Dept = (dt.Rows[i]["DEPARTMENT"]).ToString(),
                                                UpdatedStatus = "Not Updated",
                                                FeeYear = Convert.ToInt32(txtYear.Text),
                                                FeeMonth = Convert.ToInt32(txtMonth.Text),
                                                Reason = dt.Rows[i]["REASON"].ToString(),
                                                IsUnPaid = false,
                                                IsNotMatch = true,
                                                MemberId = 0,
                                                IsAccept_ByNube = false,
                                                IsStruckOff = false,
                                                Status = "Fees Entry",
                                                TotalMonthsPaid = 1
                                            };
                                            lstFeesDetlsNotMatch.Add(FeesDtl);
                                        }
                                    }

                                    if (lstFeesDetlsNotMatch.Count > 0)
                                    {
                                        DB.FeesDetailsNotMatches.AddRange(lstFeesDetlsNotMatch);
                                        DB.SaveChanges();
                                    }

                                    if (lstFeesDetls.Count > 0)
                                    {
                                        DB.FeesDetails.AddRange(lstFeesDetls);
                                        DB.SaveChanges();
                                    }

                                    dt.Rows.Clear();
                                }
                            }
                            else
                            {
                                FeesMaster FeesMst = new FeesMaster
                                {
                                    BankId = iBnkCode,
                                    FeeDate = YearName,
                                    XLFileName = drFees["BANK_NAME"].ToString(),
                                    XLPassword = txtXLPassword.Password.ToString(),
                                    UpdatedStatus = "Not Updated",
                                };
                                DB.FeesMasters.Add(FeesMst);
                                DB.SaveChanges();

                                var sFid = DB.FeesMasters.Max(x => x.FeeId).ToString();

                                //DataTable dt = new DataTable();
                                //dt = ((DataView)dgFeeDetails.ItemsSource).ToTable();

                                List<FeesDetail> lstFeesDetls = new List<FeesDetail>();
                                //progressBar1.Minimum = 0;
                                //progressBar1.Maximum = dt.Rows.Count;
                                progressBar1.Visibility = Visibility.Visible;
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    //progressBar1.Value = i;
                                    //System.Windows.Forms.Application.DoEvents();
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
                                        FeeYear = Convert.ToInt32(txtYear.Text),
                                        FeeMonth = Convert.ToInt32(txtMonth.Text),
                                        Reason = "",
                                        IsUnPaid = false,
                                        IsNotMatch = false,
                                        MemberId = Convert.ToDecimal(dt.Rows[i]["MEMBERID"]),
                                        Status = "Fees Entry",
                                        TotalMonthsPaid = 1
                                    };
                                    lstFeesDetls.Add(FeesDtl);
                                }

                                if (lstFeesDetls != null)
                                {
                                    DB.FeesDetails.AddRange(lstFeesDetls);
                                    DB.SaveChanges();
                                }

                                dt.Rows.Clear();
                                dt = ((DataView)dgNotMatch.ItemsSource).ToTable();
                                dv = new DataView(dt);
                                dv.RowFilter = "BANK_CODE=" + Convert.ToInt32(drFees["BANK_CODE"]);
                                dt = dv.ToTable();


                                lstFeesDetls = new List<FeesDetail>();
                                List<FeesDetailsNotMatch> lstFeesDetlsNotMatch = new List<FeesDetailsNotMatch>();

                                //progressBar1.Minimum = 0;
                                //progressBar1.Maximum = dt.Rows.Count;
                                //progressBar1.Visibility = Visibility.Visible;
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    //progressBar1.Value = i;
                                    //System.Windows.Forms.Application.DoEvents();
                                    if (dt.Rows[i]["REASON"].ToString().ToUpper() == "YES" || dt.Rows[i]["REASON"].ToString().ToUpper() == "S")
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
                                            FeeYear = Convert.ToInt32(txtYear.Text),
                                            FeeMonth = Convert.ToInt32(txtMonth.Text),
                                            Reason = "IS ACCEPT BY NUBE",
                                            IsUnPaid = false,
                                            IsNotMatch = false,
                                            MemberId = Convert.ToDecimal(dt.Rows[i]["MEMBERID"]),
                                            IsAccept_ByNube = true,
                                            IsStruckOff = false,
                                            Status = "Fees Entry",
                                            TotalMonthsPaid = 1

                                        };
                                        lstFeesDetls.Add(FeesDtl);
                                    }
                                    else if (dt.Rows[i]["REASON"].ToString().ToUpper() == "INACTIVE")
                                    {
                                        FeesDetailsNotMatch FeesDtl = new FeesDetailsNotMatch
                                        {
                                            FeeId = Convert.ToInt32(sFid),
                                            MemberCode = Convert.ToDecimal(dt.Rows[i]["MEMBERCODE"]),
                                            TotalAmount = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]),
                                            AmountBF = Convert.ToDecimal(dt.Rows[i]["BF"]),
                                            AmountIns = Convert.ToDecimal(dt.Rows[i]["INSURANCE"]),
                                            AmtSubs = Convert.ToDecimal(dt.Rows[i]["SUBSCRIPTION"]),
                                            Dept = (dt.Rows[i]["DEPARTMENT"]).ToString(),
                                            UpdatedStatus = "Not Updated",
                                            FeeYear = Convert.ToInt32(txtYear.Text),
                                            FeeMonth = Convert.ToInt32(txtMonth.Text),
                                            Reason = dt.Rows[i]["REASON"].ToString(),
                                            IsUnPaid = false,
                                            IsNotMatch = true,
                                            MemberId = Convert.ToDecimal(dt.Rows[i]["MEMBERID"]),
                                            IsAccept_ByNube = false,
                                            IsStruckOff = true,
                                            Status = "Fees Entry",
                                            TotalMonthsPaid = 1
                                        };
                                        lstFeesDetlsNotMatch.Add(FeesDtl);
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(dt.Rows[i]["MEMBERCODE"].ToString()))
                                        {
                                            FeesDetailsNotMatch FeesDtl = new FeesDetailsNotMatch
                                            {
                                                FeeId = Convert.ToInt32(sFid),
                                                MemberCode = Convert.ToDecimal(dt.Rows[i]["MEMBERCODE"]),
                                                TotalAmount = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]),
                                                AmountBF = Convert.ToDecimal(dt.Rows[i]["BF"]),
                                                AmountIns = Convert.ToDecimal(dt.Rows[i]["INSURANCE"]),
                                                AmtSubs = Convert.ToDecimal(dt.Rows[i]["SUBSCRIPTION"]),
                                                Dept = (dt.Rows[i]["DEPARTMENT"]).ToString(),
                                                UpdatedStatus = "Not Updated",
                                                FeeYear = Convert.ToInt32(txtYear.Text),
                                                FeeMonth = Convert.ToInt32(txtMonth.Text),
                                                Reason = dt.Rows[i]["REASON"].ToString(),
                                                IsUnPaid = false,
                                                IsNotMatch = true,
                                                MemberId = 0,
                                                IsAccept_ByNube = false,
                                                IsStruckOff = false,
                                                Status = "Fees Entry",
                                                TotalMonthsPaid = 1
                                            };
                                            lstFeesDetlsNotMatch.Add(FeesDtl);
                                        }
                                        else
                                        {
                                            FeesDetailsNotMatch FeesDtl = new FeesDetailsNotMatch
                                            {
                                                FeeId = Convert.ToInt32(sFid),
                                                MemberCode = 0,
                                                TotalAmount = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]),
                                                AmountBF = Convert.ToDecimal(dt.Rows[i]["BF"]),
                                                AmountIns = Convert.ToDecimal(dt.Rows[i]["INSURANCE"]),
                                                AmtSubs = Convert.ToDecimal(dt.Rows[i]["SUBSCRIPTION"]),
                                                Dept = (dt.Rows[i]["DEPARTMENT"]).ToString(),
                                                UpdatedStatus = "Not Updated",
                                                FeeYear = Convert.ToInt32(txtYear.Text),
                                                FeeMonth = Convert.ToInt32(txtMonth.Text),
                                                Reason = dt.Rows[i]["REASON"].ToString(),
                                                IsUnPaid = false,
                                                IsNotMatch = true,
                                                MemberId = 0,
                                                IsAccept_ByNube = false,
                                                IsStruckOff = false,
                                                Status = "Fees Entry",
                                                TotalMonthsPaid = 1
                                            };
                                            lstFeesDetlsNotMatch.Add(FeesDtl);
                                        }

                                    }
                                }

                                if (lstFeesDetlsNotMatch.Count > 0)
                                {
                                    DB.FeesDetailsNotMatches.AddRange(lstFeesDetlsNotMatch);
                                    DB.SaveChanges();
                                }

                                if (lstFeesDetls.Count > 0)
                                {
                                    DB.FeesDetails.AddRange(lstFeesDetls);
                                    DB.SaveChanges();
                                }

                                dt.Rows.Clear();
                            }
                        }
                    }
                }
                else
                {
                    DateTime YearName = Convert.ToDateTime("01" + "/" + txtMonth.Text + "/" + txtYear.Text);
                    FeesMaster FeeMst = DB.FeesMasters.Where(x => x.BankId == iBnkCode && DateTime.Compare(x.FeeDate, YearName) <= 0).FirstOrDefault();
                    int iFeeId = Convert.ToInt32(FeeMst.FeeId);

                    FeesMaster FeesMst = (from m in DB.FeesMasters where m.BankId == iFeeId select m).Single();

                    FeesMst.BankId = iBnkCode;
                    FeesMst.FeeDate = Convert.ToDateTime("01" + "/" + txtMonth.Text + "/" + txtYear.Text);
                    FeesMst.XLFileName = destinationFile;
                    FeesMst.XLPassword = txtXLPassword.Password;
                    DB.SaveChanges();



                    var emp = (from CH in DB.FeesDetails where CH.FeeId == iFeeId select CH).ToList();

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bInsering = true;
                ImportExcelNew();
                //ImportExcel();
                GridShow();
                Total();
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
                dgUnPaid.ItemsSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbBankName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbBankName.SelectedItem != null)
                {
                    int iBankCode = Convert.ToInt32(cmbBankName.SelectedValue);

                    DataView dv = new DataView(dtFeesEntry);
                    dv.RowFilter = "(STATUS_CODE=1 OR STATUS_CODE=2) AND AMOUNT>0.0 AND BANK_CODE=" + iBankCode;
                    dtActive = dv.ToTable();

                    dv = new DataView(dtFeesEntry);
                    dv.RowFilter = "STATUS_CODE=3 AND AMOUNT>0.0 AND BANK_CODE=" + iBankCode;
                    dtNotMatch = dv.ToTable();

                    dv = new DataView(dtFeesEntry);
                    dv.RowFilter = "AMOUNT<=0.0 OR STATUS_CODE=4 AND BANK_CODE=" + iBankCode;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region "USER DEFINEND FUNCTION"

        void dbRefresh()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-M0M7RA2\DENARIUSOFT;Initial Catalog=nubebfs17072017;Integrated Security=True"))
                {
                    System.Windows.Forms.Application.DoEvents();
                    SqlCommand cmd = new SqlCommand("SPREFRESH", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    System.Windows.Forms.Application.DoEvents();
                    cmd.Connection.Open();
                    cmd.CommandTimeout = 0;
                    int i = cmd.ExecuteNonQuery();
                    if (i == 0)
                    {
                        MessageBox.Show("Data Not Refresh Contact Administrator!", "Error");
                    }
                    else
                    {
                        MessageBox.Show("Data Refreshed Sucessfully !", "Sucessfully");
                    }
                    cmd.Connection.Close();
                    MessageBox.Show("No Data Found");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

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

        void FormLoad()
        {
            try
            {
                cmbBankName.ItemsSource = DB.MASTERBANKs.OrderBy(x => x.BANK_NAME).ToList();
                cmbBankName.SelectedValuePath = "BANK_CODE";
                cmbBankName.DisplayMemberPath = "BANK_NAME";
                btnShow.Visibility = Visibility.Hidden;
                progressBar1.Visibility = Visibility.Hidden;

                if (AppLib.lstMstMember == null)
                {
                    AppLib.lstMstMember = DB.MemberStatusLogs.OrderByDescending(x => x.DATEOFJOINING).ToList();
                }
                else if (AppLib.lstMstMember.Count == 0)
                {
                    AppLib.lstMstMember = DB.MemberStatusLogs.OrderByDescending(x => x.DATEOFJOINING).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void ImportExcelNew()
        {
            OpenDialogBox.DefaultExt = ".xlsx";
            OpenDialogBox.Filter = "XL files|*.xls;*.xlsx|All files|*.*";
            OpenDialogBox.Multiselect = true;

            var browsefile = OpenDialogBox.ShowDialog();
            if (browsefile == true)
            {
                cmbBankName.IsEnabled = false;
                progressBar1.Minimum = 3;
                progressBar1.Maximum = 100;
                progressBar1.Visibility = Visibility.Visible;

                dtFeesEntry.Rows.Clear();
                DataTable dtFees = new DataTable();
                dtFees.Columns.Add("FEEYEAR", typeof(int));
                dtFees.Columns.Add("FEEMONTH", typeof(int));
                dtFees.Columns.Add("BANK_CODE", typeof(int));
                dtFees.Columns.Add("BANK_NAME", typeof(int));
                dtFees.Columns.Add("ID", typeof(int));
                dtFees.Columns.Add("NRIC", typeof(string));
                dtFees.Columns.Add("MemberName", typeof(string));
                dtFees.Columns.Add("Amount", typeof(decimal));
                dtFees.Columns.Add("Department", typeof(string));


                DataTable dt = new DataTable();
                dt.Columns.Add("FEEYEAR", typeof(int));
                dt.Columns.Add("FEEMONTH", typeof(int));
                dt.Columns.Add("BANK_CODE", typeof(int));
                dt.Columns.Add("ID", typeof(int));
                dt.Columns.Add("NRIC", typeof(string));
                dt.Columns.Add("MemberName", typeof(string));
                dt.Columns.Add("Amount", typeof(decimal));
                dt.Columns.Add("Department", typeof(string));

                int iProgress = 5;
                progressBar1.Value = iProgress;
                System.Windows.Forms.Application.DoEvents();
                foreach (string filename in OpenDialogBox.FileNames)
                {
                    iProgress = iProgress + 5;
                    string sFile_Name = filename.Substring(filename.LastIndexOf("\\") + 1);
                    String Year = sFile_Name.Substring(3, 4);
                    String Month = sFile_Name.Substring(0, 3);
                    String BankName = sFile_Name.Substring(7, sFile_Name.Length - 12);
                    DateTime dtFeedate = Convert.ToDateTime("01/" + Month + "/" + Year).Date;
                    txtYear.Text = dtFeedate.Year.ToString();
                    txtMonth.Text = dtFeedate.Month.ToString();
                    //sFile_Name = "D:\\MAR 2018" + sFile_Name;
                    using (SqlConnection con = new SqlConnection(connectionstring))
                    {
                        SqlCommand cmd;
                        string qry = " DECLARE @BANK_CODE INT \r " +
                                     " DECLARE @BANK_NAME VARCHAR(50) \r " +
                                     " SELECT @BANK_CODE=BANK_CODE,@BANK_NAME=BANK_USERCODE FROM MASTERBANK(NOLOCK) WHERE BANK_NAME='" + BankName + "' AND DELETED=0 \r \r " +
                                     " SELECT " + dtFeedate.Year + " FEEYEAR," + dtFeedate.Month + " FEEMONTH,@BANK_NAME BANK_NAME,@BANK_CODE BANK_CODE,* FROM OPENROWSET('MICROSOFT.ACE.OLEDB.12.0','EXCEL 12.0; \r " +
                                     " DATABASE=" + filename + "; \r " +
                                     " HDR = NO; IMEX = 1', \r " +
                                     " 'SELECT * FROM [SHEET1$]') XL \r ";

                        //string qry = " DECLARE @BANK_CODE INT \r " +
                        //      " DECLARE @BANK_NAME VARCHAR(50) \r " +
                        //      " SELECT @BANK_CODE=BANK_CODE,@BANK_NAME=BANK_USERCODE FROM MASTERBANK(NOLOCK) WHERE BANK_NAME='" + BankName + "' AND DELETED=0 \r \r " +
                        //      " SELECT " + dtFeedate.Year + " FEEYEAR," + dtFeedate.Month + " FEEMONTH,@BANK_NAME BANK_NAME,@BANK_CODE BANK_CODE,* FROM OPENROWSET('MICROSOFT.ACE.OLEDB.12.0','EXCEL 12.0; \r " +
                        //      " DATABASE=" + sFile_Name + "; \r " +
                        //      " HDR = NO; IMEX = 1', \r " +
                        //      " 'SELECT * FROM [SHEET1$]') XL \r ";

                        cmd = new SqlCommand(qry, con);
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        dt.Rows.Clear();
                        adp.Fill(dt);
                        if (dt.Rows.Count > 0 && dtFees.Rows.Count > 0)
                        {
                            dtFees.Merge(dt);
                        }
                        else if (dt.Rows.Count > 0)
                        {
                            dtFees = dt.Copy();
                        }
                    }
                    progressBar1.Value = iProgress;
                    System.Windows.Forms.Application.DoEvents();
                }

                dtFees.Columns.Add("BF", typeof(string));
                dtFees.Columns.Add("INSURANCE", typeof(string));
                dtFees.Columns.Add("SUBSCRIPTION", typeof(string));
                dtFees.Columns.Add("STATUS_CODE", typeof(string));
                dtFees.Columns.Add("MEMBERCODE", typeof(decimal));
                dtFees.Columns.Add("MEMBERID", typeof(decimal));
                dtFees.Columns.Add("REASON", typeof(string));
                dtFees.Columns.Add("LAST_PAY_DATE", typeof(string));

                if (dtFees.Rows.Count > 0)
                {
                    int iRow = 0;
                    foreach (DataRow row in dtFees.Rows)
                    {
                        row["ID"] = iRow + 1;
                        row["BF"] = "3";
                        row["INSURANCE"] = "7";
                        iRow++;
                    }
                    progressBar1.Value = 100;
                    System.Windows.Forms.Application.DoEvents();
                    dgOverAllMember.ItemsSource = dtFees.DefaultView;
                    txtTotalMembers.Text = dtFees.Rows.Count.ToString();
                }

                progressBar1.Minimum = 5;
                progressBar1.Maximum = dtFees.Rows.Count;
                progressBar1.Visibility = Visibility.Visible;

                for (int i = 0; i < dtFees.Rows.Count; i++)
                {
                    progressBar1.Value = i;
                    System.Windows.Forms.Application.DoEvents();
                    decimal iAmnt = 0, iBF = 3, iInsurance = 7;
                    int iBank_Code = Convert.ToInt32(dtFees.Rows[i]["BANK_CODE"]);

                    string sIcNo = dtFees.Rows[i]["NRIC"].ToString();
                    string sName = dtFees.Rows[i]["MEMBERNAME"].ToString();

                    var MstMember = (from x in AppLib.lstMstMember where x.ICNO_NEW == sIcNo || x.ICNO_OLD == sIcNo || x.NRIC_BYBANK == sIcNo orderby x.DATEOFJOINING descending select x).FirstOrDefault();

                    if (MstMember == null || string.IsNullOrEmpty(sIcNo))
                    {
                        MstMember = (from x in AppLib.lstMstMember where x.MEMBER_NAME.ToUpper().Contains(sName) || x.MEMBERNAME_BYBANK.ToUpper().Contains(sName) orderby x.DATEOFJOINING descending select x).FirstOrDefault();
                        if (MstMember == null)
                        {
                            MstMember = (from x in AppLib.lstMstMember where x.MEMBER_NAME.ToUpper().Contains(sName.ToUpper()) || x.MEMBERNAME_BYBANK.ToUpper().Contains(sName.ToUpper()) orderby x.DATEOFJOINING descending select x).FirstOrDefault();
                            if (MstMember == null)
                            {
                                MstMember = (from x in AppLib.lstMstMember where x.ICNO_NEW.ToUpper() == sIcNo.ToUpper() || x.ICNO_OLD.ToUpper() == sIcNo.ToUpper() || x.NRIC_BYBANK.ToUpper() == sIcNo.ToUpper() orderby x.DATEOFJOINING descending select x).FirstOrDefault();
                            }
                        }
                    }

                    if (MstMember != null)
                    {
                        dtFees.Rows[i]["MEMBERID"] = MstMember.MEMBER_ID;
                        dtFees.Rows[i]["STATUS_CODE"] = MstMember.MEMBERSTATUSCODE;
                        dtFees.Rows[i]["MEMBERCODE"] = MstMember.MEMBER_CODE;
                        iAmnt = Convert.ToDecimal(dtFees.Rows[i]["Amount"]);
                        if (iAmnt == 0 || iAmnt < 0)
                        {
                            iBF = 0;
                            iInsurance = 0;
                            dtFees.Rows[i]["BF"] = "0";
                            dtFees.Rows[i]["INSURANCE"] = "0";
                        }

                        dtFees.Rows[i]["LAST_PAY_DATE"] = string.Format("{0:dd-MMM-yyyy}", MstMember.LASTPAYMENT_DATE);
                        string sStatus = MstMember.MEMBERSTATUSCODE.ToString();
                        string sReason = "";

                        if (MstMember.RESIGNED == true)
                        {
                            sStatus = "3";
                            sReason = "RESIGNED - " + string.Format("{0:dd-MMM-yyyy}", MstMember.LASTPAYMENT_DATE);
                        }

                        if (MstMember.MEMBERNAME_BYBANK != null)
                        {
                            if ((dtFees.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MstMember.MEMBER_NAME.ToUpper().Replace(" ", "")) && dtFees.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MstMember.MEMBERNAME_BYBANK.ToUpper().Replace(" ", ""))
                            {
                                if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason))
                                {
                                    sStatus = "3";
                                    sReason = sReason + "- NAME NOT MATCH-" + MstMember.MEMBER_NAME.ToString();
                                }
                                else
                                {
                                    sStatus = "3";
                                    sReason = "NAME NOT MATCH-" + MstMember.MEMBER_NAME.ToString();
                                }
                            }
                        }
                        else if ((dtFees.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MstMember.MEMBER_NAME.ToUpper().Replace(" ", "")))
                        {
                            if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason))
                            {
                                sStatus = "3";
                                sReason = sReason + "- NAME NOT MATCH-" + MstMember.MEMBER_NAME.ToString();
                            }
                            else
                            {
                                sStatus = "3";
                                sReason = "NAME NOT MATCH-" + MstMember.MEMBER_NAME.ToString();
                            }
                        }

                        if (MstMember.BANK_CODE != iBank_Code)
                        {
                            var bnk = (from x in DB.MASTERBANKs where x.BANK_CODE == MstMember.BANK_CODE select x).FirstOrDefault();
                            if (bnk.HEADER_BANK_CODE != iBank_Code)
                            {
                                if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason))
                                {
                                    sStatus = "3";
                                    sReason = sReason + "- (BANK NOT MATCH -" + bnk.BANK_NAME.ToString() + ")";
                                }
                                else
                                {
                                    sStatus = "3";
                                    sReason = "BANK NOT MATCH -" + bnk.BANK_NAME.ToString();
                                }
                            }
                        }

                        if (Convert.ToInt32(MstMember.MEMBERSTATUSCODE) != 1 && Convert.ToInt32(MstMember.RESIGNED) != 1 && Convert.ToInt32(MstMember.MEMBERSTATUSCODE) != 2)
                        {
                            MASTERSTATU st = (from s in DB.MASTERSTATUS where s.STATUS_CODE == MstMember.MEMBERSTATUSCODE select s).FirstOrDefault();
                            if (st != null)
                            {
                                if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason))
                                {
                                    sStatus = "3";
                                    sReason = sReason + "- (NOT ACTIVE -" + MstMember.MEMBERSTATUS.ToString() + ")";
                                }
                                else
                                {
                                    sStatus = "3";
                                    sReason = "(NOT ACTIVE -" + MstMember.MEMBERSTATUS.ToString() + ")";
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason))
                                {
                                    sStatus = "3";
                                    sReason = sReason + "- (NOT ACTIVE - OTHER)";
                                }
                                else
                                {
                                    sStatus = "3";
                                    sReason = "NOT ACTIVE - OTHER";
                                }
                            }
                        }

                        dtFees.Rows[i]["REASON"] = sReason + " / " + dtFees.Rows[i]["NRIC"].ToString() + " / " + dtFees.Rows[i]["MEMBERNAME"].ToString(); ;
                        dtFees.Rows[i]["STATUS_CODE"] = sStatus;
                    }
                    else
                    {
                        dtFees.Rows[i]["STATUS_CODE"] = "3";
                        //dtFees.Rows[i]["MEMBERCODE"] = "0";
                        //dtFees.Rows[i]["BANK_CODE"] = "0";
                        dtFees.Rows[i]["REASON"] = "NRIC NOT MATCH - " + dtFees.Rows[i]["NRIC"].ToString() + " / " + dtFees.Rows[i]["MEMBERNAME"].ToString();
                        iAmnt = Convert.ToDecimal(dtFees.Rows[i]["Amount"]);
                        if (iAmnt == 0 || iAmnt < 0)
                        {
                            iBF = 0;
                            iInsurance = 0;
                            dtFees.Rows[i]["BF"] = "0";
                            dtFees.Rows[i]["INSURANCE"] = "0";
                        }
                    }

                    iAmnt = (iAmnt - (iBF + iInsurance));
                    if (iAmnt < 0)
                    {
                        iAmnt = 0;
                        dtFees.Rows[i]["STATUS_CODE"] = "4";
                    }
                    dtFees.Rows[i]["SUBSCRIPTION"] = iAmnt.ToString();
                }
                dtFeesEntry = dtFees.Copy();
                MessageBox.Show("Imported Sucessfully", "Sucess");
                cmbBankName.IsEnabled = true;
            }
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
                dtTemp.Columns.Add("MEMBERID", typeof(decimal));
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
                    AppLib.lstMstMember = DB.MemberStatusLogs.OrderByDescending(x => x.DATEOFJOINING).ToList();
                }
                else if (AppLib.lstMstMember.Count == 0)
                {
                    AppLib.lstMstMember = DB.MemberStatusLogs.OrderByDescending(x => x.DATEOFJOINING).ToList();
                }

                if (rbtNRIC.IsChecked == true)
                {
                    for (int i = 0; i < dtTemp.Rows.Count; i++)
                    {
                        progressBar1.Value = i;
                        System.Windows.Forms.Application.DoEvents();
                        decimal iBF = 0;
                        decimal iInsurance = 0;
                        decimal dBank_Code = 0;
                        dBankCode = Convert.ToDecimal(cmbBankName.SelectedValue);

                        string sIcNo = dtTemp.Rows[i]["NRIC"].ToString();
                        //string sIcOld = dtTemp.Rows[i]["ICOLD"].ToString();
                        //decimal sIcNo = Convert.ToDecimal(dtTemp.Rows[i]["NRIC"]);

                        var MstMember = AppLib.lstMstMember.Where(x => x.ICNO_NEW == sIcNo || x.ICNO_OLD == sIcNo || x.NRIC_BYBANK == sIcNo).OrderByDescending(x => x.DATEOFJOINING).FirstOrDefault();
                        //var MstMember = AppLib.lstMstMember.Where(x => x.MEMBER_ID == sIcNo).OrderByDescending(x => x.DATEOFJOINING).FirstOrDefault();

                        //if (MstMember == null)
                        //{
                        //    MstMember = AppLib.lstMstMember.Where(x => x.ICNO_NEW == sIcOld || x.ICNO_OLD == sIcOld || x.NRIC_BYBANK == sIcOld).OrderByDescending(x => x.DATEOFJOINING).FirstOrDefault();
                        //}

                        dAltrBankCode = 0;
                        iBF = 3;
                        iInsurance = 7;
                        if (MstMember != null)
                        {
                            dtTemp.Rows[i]["MEMBERID"] = MstMember.MEMBER_ID;
                            dtTemp.Rows[i]["STATUS_CODE"] = MstMember.MEMBERSTATUSCODE;
                            dtTemp.Rows[i]["MEMBERCODE"] = MstMember.MEMBER_CODE;
                            dtTemp.Rows[i]["BANKCODE"] = MstMember.BANK_CODE;
                            dBank_Code = Convert.ToDecimal(MstMember.BANK_CODE);
                            dAltrBankCode = 0;
                            iAmnt = Convert.ToDecimal(dtTemp.Rows[i]["Amount"]);
                            if (iAmnt > 0)
                            {
                                dtTemp.Rows[i]["BF"] = iBF;
                                dtTemp.Rows[i]["INSURANCE"] = iInsurance;
                            }
                            else
                            {
                                iBF = 0;
                                iInsurance = 0;
                                dtTemp.Rows[i]["BF"] = "0";
                                dtTemp.Rows[i]["INSURANCE"] = "0";
                            }

                            dtTemp.Rows[i]["LAST_PAY_DATE"] = string.Format("{0:dd-MMM-yyyy}", MstMember.LASTPAYMENT_DATE);
                            string sStatus = MstMember.MEMBERSTATUSCODE.ToString();
                            string sReason = "";
                            string sOld = "";

                            if (MstMember.RESIGNED == true)
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

                            //if (dBank_Code != dBankCode && dBankCode != MstMember.BANKCODE_BYBANK)
                            if (dBank_Code != dBankCode)
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
                                    sReason = sReason + "- (BANK NOT MATCH -" + st.BANK_NAME.ToString() + ")";
                                }
                                else
                                {
                                    sStatus = "7";
                                    sReason = "BANK NOT MATCH -" + st.BANK_NAME.ToString();
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
                                        sOld = sOld + "-" + MstMember.MEMBERSTATUSCODE.ToString();
                                    }
                                    else
                                    {
                                        sStatus = "11";
                                        sReason = "NOT ACTIVE-OTHER";
                                        sOld = MstMember.MEMBERSTATUSCODE.ToString();
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
                            var mst = (from x in AppLib.lstMstMember where x.MEMBER_NAME.Contains(sName) select x).FirstOrDefault();

                            if (mst != null)
                            {
                                dtTemp.Rows[i]["REASON"] = "NRIC NOT MATCH BUT NAME MATCH";
                                dtTemp.Rows[i]["STATUS_CODE"] = "13";
                                dtTemp.Rows[i]["MEMBERID"] = mst.MEMBER_ID;

                                if (mst.ICNO_NEW != null)
                                {
                                    dtTemp.Rows[i]["OLD"] = mst.ICNO_NEW.ToString();
                                }
                                if (mst.ICNO_OLD != null)
                                {
                                    dtTemp.Rows[i]["OLD2"] = mst.ICNO_OLD.ToString();
                                }


                                dtTemp.Rows[i]["MEMBERCODE"] = mst.MEMBER_CODE;
                                dtTemp.Rows[i]["BANKCODE"] = mst.BANK_CODE;
                                dBank_Code = Convert.ToDecimal(mst.BANK_CODE);

                                iAmnt = Convert.ToDecimal(dtTemp.Rows[i]["Amount"]);
                                if (iAmnt > 0)
                                {
                                    dtTemp.Rows[i]["BF"] = iBF;
                                    dtTemp.Rows[i]["INSURANCE"] = iInsurance;
                                }
                                else
                                {
                                    iBF = 0;
                                    iInsurance = 0;
                                    dtTemp.Rows[i]["BF"] = "0";
                                    dtTemp.Rows[i]["INSURANCE"] = "0";
                                }
                                dtTemp.Rows[i]["LAST_PAY_DATE"] = string.Format("{0:dd-MMM-yyyy}", mst.LASTPAYMENT_DATE);
                            }
                            else
                            {
                                var MS = (from x in AppLib.lstMstMember where x.MEMBERNAME_BYBANK == sName select x).FirstOrDefault();

                                if (MS != null)
                                {
                                    dtTemp.Rows[i]["REASON"] = "NRIC NOT MATCH BUT NAME MATCH";
                                    dtTemp.Rows[i]["STATUS_CODE"] = "13";
                                    dtTemp.Rows[i]["MEMBERID"] = MS.MEMBER_ID;

                                    if (MS.ICNO_NEW != null)
                                    {
                                        dtTemp.Rows[i]["OLD"] = MS.ICNO_NEW.ToString();
                                    }
                                    if (MS.ICNO_OLD != null)
                                    {
                                        dtTemp.Rows[i]["OLD2"] = MS.ICNO_OLD.ToString();
                                    }

                                    dtTemp.Rows[i]["MEMBERCODE"] = MS.MEMBER_CODE;
                                    dtTemp.Rows[i]["BANKCODE"] = MS.BANK_CODE;
                                    dBank_Code = Convert.ToDecimal(MS.BANK_CODE);
                                    iAmnt = Convert.ToDecimal(dtTemp.Rows[i]["Amount"]);
                                    if (iAmnt > 0)
                                    {
                                        dtTemp.Rows[i]["BF"] = iBF;
                                        dtTemp.Rows[i]["INSURANCE"] = iInsurance;
                                    }
                                    else
                                    {
                                        dtTemp.Rows[i]["BF"] = "0";
                                        dtTemp.Rows[i]["INSURANCE"] = "0";
                                        iBF = 0;
                                        iInsurance = 0;
                                    }
                                    //dtTemp.Rows[i]["LAST_PAY_DATE"] = string.Format("{0:dd-MMM-yyyy}", mst.LASTPAYMENT_DATE);
                                }
                                else
                                {
                                    iBF = 0;

                                    dtTemp.Rows[i]["STATUS_CODE"] = "8";
                                    dtTemp.Rows[i]["MEMBERCODE"] = "0";
                                    dtTemp.Rows[i]["BANKCODE"] = "0";
                                    dtTemp.Rows[i]["REASON"] = "NRIC NOT MATCH - " + dtTemp.Rows[i]["NRIC"].ToString() + " / " + dtTemp.Rows[i]["MEMBERNAME"].ToString();
                                    iAmnt = Convert.ToDecimal(dtTemp.Rows[i]["Amount"]);
                                    if (iAmnt > 0)
                                    {
                                        dtTemp.Rows[i]["BF"] = iBF;
                                        dtTemp.Rows[i]["INSURANCE"] = iInsurance;
                                    }
                                    else
                                    {
                                        dtTemp.Rows[i]["BF"] = "0";
                                        dtTemp.Rows[i]["INSURANCE"] = "0";
                                        iBF = 0;
                                        iInsurance = 0;
                                    }
                                }
                            }
                        }

                        iAmnt = (iAmnt - (iBF + iInsurance));
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
                        decimal iInsurance = 0;
                        decimal dBank_Code = 0;
                        decimal dBankCode = Convert.ToDecimal(cmbBankName.SelectedValue);
                        iAmnt = 0;

                        //decimal dMemberId = Convert.ToDecimal(dtTemp.Rows[i]["Member ID"]);
                        //var MstMember = AppLib.lstMstMember.Where(x => x.MEMBER_ID == dMemberId).OrderByDescending(x => x.DATEOFJOINING).FirstOrDefault();
                        string sName = dtTemp.Rows[i]["MemberName"].ToString();
                        var MstMember = AppLib.lstMstMember.Where(x => x.MEMBER_NAME.Contains(sName)).OrderByDescending(x => x.DATEOFJOINING).FirstOrDefault();
                        iBF = 3;
                        iInsurance = 7;

                        if (MstMember != null)
                        {
                            dtTemp.Rows[i]["NRIC"] = MstMember.ICNO_NEW.ToString();

                            dtTemp.Rows[i]["MEMBERID"] = MstMember.MEMBER_ID;
                            dtTemp.Rows[i]["STATUS_CODE"] = MstMember.MEMBERSTATUSCODE;
                            dtTemp.Rows[i]["MEMBERCODE"] = MstMember.MEMBER_CODE;
                            dtTemp.Rows[i]["BANKCODE"] = MstMember.BANK_CODE;
                            dBank_Code = Convert.ToDecimal(MstMember.BANK_CODE);
                            iAmnt = Convert.ToDecimal(dtTemp.Rows[i]["Amount"]);
                            if (iAmnt > 0)
                            {
                                dtTemp.Rows[i]["BF"] = iBF;
                                dtTemp.Rows[i]["INSURANCE"] = iInsurance;
                            }
                            else
                            {
                                dtTemp.Rows[i]["INSURANCE"] = "0";
                                dtTemp.Rows[i]["BF"] = "0";
                                iBF = 0;
                                iInsurance = 0;
                            }
                            dtTemp.Rows[i]["LAST_PAY_DATE"] = string.Format("{0:dd-MMM-yyyy}", MstMember.LASTPAYMENT_DATE);
                            string sStatus = MstMember.MEMBERSTATUSCODE.ToString();
                            string sReason = "";
                            string sOld = "";

                            if (MstMember.RESIGNED == true)
                            {
                                sStatus = "12";
                                sReason = "RESIGNED";
                                sOld = string.Format("{0:dd-MMM-yyyy}", MstMember.LASTPAYMENT_DATE);
                            }

                            if (MstMember.MEMBERNAME_BYBANK != null)
                            {
                                if (MstMember.MEMBERNAME_BYBANK != null)
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
                            else if ((dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MstMember.MEMBER_NAME.ToUpper().Replace(" ", "")) && (dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MstMember.MEMBERNAME_BYBANK.ToUpper().Replace(" ", "")))
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
                            if (dBank_Code != dBankCode)
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
                                    sReason = sReason + "- (BANK NOT MATCH-" + st.BANK_NAME.ToString() + ")";
                                }
                                else
                                {
                                    sStatus = "7";
                                    sReason = "(BANK NOT MATCH - " + st.BANK_NAME.ToString() + ")";
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
                                        sOld = sOld + "- " + string.Format("{0:dd-MMM-yyyy}", MstMember.LASTPAYMENT_DATE);
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
                                        sOld = sOld + "-" + MstMember.MEMBERSTATUSCODE.ToString();
                                    }
                                    else
                                    {
                                        sStatus = "11";
                                        sReason = "NOT ACTIVE-OTHER";
                                        sOld = MstMember.MEMBERSTATUSCODE.ToString();
                                    }
                                }
                            }
                            dtTemp.Rows[i]["REASON"] = sReason;
                            dtTemp.Rows[i]["STATUS_CODE"] = sStatus;
                            dtTemp.Rows[i]["OLD"] = sOld;
                        }
                        else
                        {
                            var MST = AppLib.lstMstMember.Where(x => x.MEMBERNAME_BYBANK == sName).OrderByDescending(x => x.DATEOFJOINING).FirstOrDefault();
                            if (MST != null)
                            {
                                dtTemp.Rows[i]["NRIC"] = MST.ICNO_NEW.ToString();
                                dtTemp.Rows[i]["MEMBERID"] = MST.MEMBER_ID;
                                dtTemp.Rows[i]["STATUS_CODE"] = MST.MEMBERSTATUSCODE;
                                dtTemp.Rows[i]["MEMBERCODE"] = MST.MEMBER_CODE;
                                dtTemp.Rows[i]["BANKCODE"] = MST.BANK_CODE;
                                dBank_Code = Convert.ToDecimal(MST.BANK_CODE);
                                iAmnt = Convert.ToDecimal(dtTemp.Rows[i]["Amount"]);
                                if (iAmnt > 0)
                                {
                                    dtTemp.Rows[i]["BF"] = iBF;
                                    dtTemp.Rows[i]["INSURANCE"] = iInsurance;
                                }
                                else
                                {
                                    dtTemp.Rows[i]["BF"] = "0";
                                    dtTemp.Rows[i]["INSURANCE"] = "0";
                                    iBF = 0;
                                    iInsurance = 0;
                                }
                                dtTemp.Rows[i]["LAST_PAY_DATE"] = string.Format("{0:dd-MMM-yyyy}", MST.LASTPAYMENT_DATE);
                                string sStatus = MST.MEMBERSTATUSCODE.ToString();
                                string sReason = "";
                                string sOld = "";

                                if (MST.RESIGNED == true)
                                {
                                    sStatus = "12";
                                    sReason = "RESIGNED";
                                    sOld = string.Format("{0:dd-MMM-yyyy}", MST.LASTPAYMENT_DATE);
                                }

                                if (MST.MEMBERNAME_BYBANK != null)
                                {
                                    if (MST.MEMBERNAME_BYBANK != null)
                                        if ((dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MST.MEMBER_NAME.ToUpper().Replace(" ", "")) && dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MST.MEMBERNAME_BYBANK.ToUpper().Replace(" ", ""))
                                        {
                                            if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                                            {
                                                sStatus = "10";
                                                sReason = sReason + "- NAME NOT MATCH";
                                                sOld = sOld + "- " + MST.MEMBER_NAME.ToString();
                                            }
                                            else
                                            {
                                                sStatus = "6";
                                                sReason = "NAME NOT MATCH";
                                                sOld = MST.MEMBER_NAME.ToString();
                                            }
                                        }
                                }
                                else if ((dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MST.MEMBER_NAME.ToUpper().Replace(" ", "")) && (dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MST.MEMBERNAME_BYBANK.ToUpper().Replace(" ", "")))
                                {
                                    if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                                    {
                                        sStatus = "10";
                                        sReason = sReason + "- NAME NOT MATCH";
                                        sOld = sOld + "- " + MST.MEMBER_NAME.ToString();
                                    }
                                    else
                                    {
                                        sStatus = "6";
                                        sReason = "NAME NOT MATCH";
                                        sOld = MST.MEMBER_NAME.ToString();
                                    }
                                }

                                //if (dBank_Code != dBankCode && dBank_Code != MST.BANKCODE_BYBANK)
                                if (dBank_Code != dBankCode)
                                {
                                    var st = (from s in DB.MASTERBANKs where s.BANK_CODE == MST.BANK_CODE select s).FirstOrDefault();
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
                                        sReason = sReason + "- (BANK NOT MATCH- " + st.BANK_NAME.ToString() + ")";
                                    }
                                    else
                                    {
                                        sStatus = "7";
                                        sReason = "(BANK NOT MATCH -" + st.BANK_NAME.ToString() + ")";
                                    }
                                }

                                if (Convert.ToInt32(MST.MEMBERSTATUSCODE) != 1 && Convert.ToInt32(MST.RESIGNED) != 1 && Convert.ToInt32(MST.MEMBERSTATUSCODE) != 2)
                                {
                                    MASTERSTATU st = (from s in DB.MASTERSTATUS where s.STATUS_CODE == MST.MEMBERSTATUSCODE select s).FirstOrDefault();
                                    if (st != null)
                                    {
                                        if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                                        {
                                            sStatus = "10";
                                            sReason = sReason + "- " + st.STATUS_NAME;
                                            sOld = sOld + "- " + string.Format("{0:dd-MMM-yyyy}", MST.LASTPAYMENT_DATE);
                                        }
                                        else
                                        {
                                            sStatus = "9";
                                            sReason = st.STATUS_NAME;
                                            sOld = string.Format("{0:dd-MMM-yyyy}", MST.LASTPAYMENT_DATE);
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                                        {
                                            sStatus = "10";
                                            sReason = sReason + "- NOT ACTIVE-OTHER";
                                            sOld = sOld + "-" + MST.MEMBERSTATUSCODE.ToString();
                                        }
                                        else
                                        {
                                            sStatus = "11";
                                            sReason = "NOT ACTIVE-OTHER";
                                            sOld = MST.MEMBERSTATUSCODE.ToString();
                                        }
                                    }
                                }
                                dtTemp.Rows[i]["REASON"] = sReason;
                                dtTemp.Rows[i]["STATUS_CODE"] = sStatus;
                                dtTemp.Rows[i]["OLD"] = sOld;
                            }
                            else
                            {
                                var MS = AppLib.lstMstMember.Where(x => x.MEMBERNAME_BYBANK == sName).OrderByDescending(x => x.DATEOFJOINING).FirstOrDefault();
                                if (MS != null)
                                {
                                    dtTemp.Rows[i]["NRIC"] = MS.ICNO_NEW.ToString();
                                    dtTemp.Rows[i]["MEMBERID"] = MS.MEMBER_ID;
                                    dtTemp.Rows[i]["STATUS_CODE"] = MS.MEMBERSTATUSCODE;
                                    dtTemp.Rows[i]["MEMBERCODE"] = MS.MEMBER_CODE;
                                    dtTemp.Rows[i]["BANKCODE"] = MS.BANK_CODE;
                                    dBank_Code = Convert.ToDecimal(MS.BANK_CODE);
                                    iAmnt = Convert.ToDecimal(dtTemp.Rows[i]["Amount"]);
                                    if (iAmnt > 0)
                                    {
                                        dtTemp.Rows[i]["BF"] = iBF;
                                        dtTemp.Rows[i]["INSURANCE"] = iInsurance;
                                    }
                                    else
                                    {
                                        dtTemp.Rows[i]["BF"] = "0";
                                        dtTemp.Rows[i]["INSURANCE"] = "0";
                                        iBF = 0;
                                        iInsurance = 0;
                                    }
                                    dtTemp.Rows[i]["LAST_PAY_DATE"] = string.Format("{0:dd-MMM-yyyy}", MS.LASTPAYMENT_DATE);
                                    string sStatus = MS.MEMBERSTATUSCODE.ToString();
                                    string sReason = "";
                                    string sOld = "";

                                    if (MS.RESIGNED == true)
                                    {
                                        sStatus = "12";
                                        sReason = "RESIGNED";
                                        sOld = string.Format("{0:dd-MMM-yyyy}", MS.LASTPAYMENT_DATE);
                                    }

                                    if (MS.MEMBERNAME_BYBANK != null)
                                    {
                                        if (MS.MEMBERNAME_BYBANK != null)
                                            if ((dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MS.MEMBER_NAME.ToUpper().Replace(" ", "")) && dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MS.MEMBERNAME_BYBANK.ToUpper().Replace(" ", ""))
                                            {
                                                if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                                                {
                                                    sStatus = "10";
                                                    sReason = sReason + "- NAME NOT MATCH";
                                                    sOld = sOld + "- " + MS.MEMBER_NAME.ToString();
                                                }
                                                else
                                                {
                                                    sStatus = "6";
                                                    sReason = "NAME NOT MATCH";
                                                    sOld = MS.MEMBER_NAME.ToString();
                                                }
                                            }
                                    }
                                    else if ((dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MS.MEMBER_NAME.ToUpper().Replace(" ", "")) && (dtTemp.Rows[i]["MEMBERNAME"].ToString().ToUpper().Replace(" ", "") != MS.MEMBERNAME_BYBANK.ToUpper().Replace(" ", "")))
                                    {
                                        if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                                        {
                                            sStatus = "10";
                                            sReason = sReason + "- NAME NOT MATCH";
                                            sOld = sOld + "- " + MS.MEMBER_NAME.ToString();
                                        }
                                        else
                                        {
                                            sStatus = "6";
                                            sReason = "NAME NOT MATCH";
                                            sOld = MS.MEMBER_NAME.ToString();
                                        }
                                    }

                                    //if (dBank_Code != dBankCode && dBank_Code != MS.BANKCODE_BYBANK)
                                    if (dBank_Code != dBankCode)
                                    {
                                        var st = (from s in DB.MASTERBANKs where s.BANK_CODE == MS.BANK_CODE select s).FirstOrDefault();
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
                                            sReason = sReason + "-( BANK NOT MATCH" + st.BANK_NAME.ToString() + ")";
                                        }
                                        else
                                        {
                                            sStatus = "7";
                                            sReason = "( BANK NOT MATCH" + st.BANK_NAME.ToString() + ")";
                                        }
                                    }

                                    if (Convert.ToInt32(MS.MEMBERSTATUSCODE) != 1 && Convert.ToInt32(MS.RESIGNED) != 1 && Convert.ToInt32(MS.MEMBERSTATUSCODE) != 2)
                                    {
                                        MASTERSTATU st = (from s in DB.MASTERSTATUS where s.STATUS_CODE == MS.MEMBERSTATUSCODE select s).FirstOrDefault();
                                        if (st != null)
                                        {
                                            if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                                            {
                                                sStatus = "10";
                                                sReason = sReason + "- " + st.STATUS_NAME;
                                                sOld = sOld + "- " + string.Format("{0:dd-MMM-yyyy}", MS.LASTPAYMENT_DATE);
                                            }
                                            else
                                            {
                                                sStatus = "9";
                                                sReason = st.STATUS_NAME;
                                                sOld = string.Format("{0:dd-MMM-yyyy}", MS.LASTPAYMENT_DATE);
                                            }
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(sStatus) && !string.IsNullOrEmpty(sReason) && !string.IsNullOrEmpty(sOld))
                                            {
                                                sStatus = "10";
                                                sReason = sReason + "- NOT ACTIVE-OTHER";
                                                sOld = sOld + "-" + MS.MEMBERSTATUSCODE.ToString();
                                            }
                                            else
                                            {
                                                sStatus = "11";
                                                sReason = "NOT ACTIVE-OTHER";
                                                sOld = MS.MEMBERSTATUSCODE.ToString();
                                            }
                                        }
                                    }
                                    dtTemp.Rows[i]["REASON"] = sReason;
                                    dtTemp.Rows[i]["STATUS_CODE"] = sStatus;
                                    dtTemp.Rows[i]["OLD"] = sOld;
                                }

                                iBF = 0;

                                dtTemp.Rows[i]["STATUS_CODE"] = "6";
                                dtTemp.Rows[i]["MEMBERCODE"] = "0";
                                dtTemp.Rows[i]["BANKCODE"] = "0";
                                dtTemp.Rows[i]["REASON"] = "NAME NOT MATCH";
                                iAmnt = Convert.ToDecimal(dtTemp.Rows[i]["Amount"]);
                                if (iAmnt > 0)
                                {
                                    dtTemp.Rows[i]["BF"] = "3";
                                }
                                else
                                {
                                    dtTemp.Rows[i]["BF"] = "0";
                                }
                            }
                        }
                        iAmnt = (iAmnt - (iBF + iInsurance));
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
            DataView dv = new DataView(dtFeesEntry);
            dv.RowFilter = "(STATUS_CODE=1 OR STATUS_CODE=2) AND AMOUNT>0.0 ";
            dtActive = dv.ToTable();

            dv = new DataView(dtFeesEntry);
            dv.RowFilter = "STATUS_CODE=3 AND AMOUNT>0.0 ";
            dtNotMatch = dv.ToTable();

            dv = new DataView(dtFeesEntry);
            dv.RowFilter = "AMOUNT<=0.0 OR STATUS_CODE=4 ";
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
            //DataTable dtTotal = new DataTable();
            //DataView dv = new DataView(dtFeesEntry);
            //dv.RowFilter = "BANKCODE=" + dBankCode;
            //dtTotal = dv.ToTable();

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
            //if (dtNotMatch.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in dtNotMatch.Rows)
            //    {
            //        if (dr["REASON"].ToString() != "NAME NOT MATCH")
            //        {
            //            MessageBox.Show("Other Member Detials Not Allowed");
            //            dgNotMatch.Focus();
            //            bValidate = true;
            //            return;
            //        }
            //    }
            //}

            //if (dtUnPaid.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in dtUnPaid.Rows)
            //    {
            //        if (string.IsNullOrEmpty(dr["REASON"].ToString()))
            //        {
            //            MessageBox.Show("Un Paid Member Detials Not Allowed");
            //            dgNotMatch.Focus();
            //            bValidate = true;
            //            return;
            //        }
            //    }
            //}

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
            //else if (string.IsNullOrEmpty(cmbBankName.Text))
            //{
            //    MessageBox.Show("Bank Is Empty!");
            //    bValidate = true;
            //    cmbBankName.Focus();
            //}
        }

        #endregion
    }
}
