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
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using Nube.MasterSetup;

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmArrearPost16.xaml
    /// </summary>
    public partial class frmArrearPost16 : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        decimal dMember_Code = 0;
        int iArrearMst_ID = 0;
        Boolean bValidation = false;
        int iTotalMonthsPaid = 0;
        DataTable dtArrearPost = new DataTable();
        Boolean bNewRow = false;
        Boolean bUpdate = false;

        string qry = "";
        string connectionstring = AppLib.connstatus;

        public frmArrearPost16(decimal dMembercode = 0, int iID = 0)
        {
            InitializeComponent();
            dMember_Code = dMembercode;
            iArrearMst_ID = iID;
            newDataTable();
            txtMemberNo.Focus();
            btnEditOldDue.Visibility = Visibility.Hidden;
            if (dMember_Code != 0)
            {
                bUpdate = false;
                FormFill();
            }
            if (iArrearMst_ID != 0)
            {
                bUpdate = true;
                GridFill();
            }
        }

        #region "BUTTON EVENTS"

        private void btnEditOldDue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dMember_Code != 0)
                {
                    ArrearPostMaster mst = (from mas in db.ArrearPostMasters where mas.MemberCode == dMember_Code select mas).FirstOrDefault();
                    if (mst != null)
                    {
                        frmArrearDueList frm = new frmArrearDueList(dMember_Code, "PostArrear", txtMemberType.Text, txtMemberName.Text, txtMemberNo.Text);
                        this.Close();
                        frm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("No Data Found!");
                        txtMemberNo.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter the Membership No!");
                    txtMemberNo.Focus();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            txtMemberNo.Text = "";
            frmMemberQuery frm = new frmMemberQuery("PostArrear");
            frm.Show();
            this.Close();
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bValidation = false;
                BeforeUpdate();
                if (bValidation == false)
                {
                    MASTERMEMBER mmMst = (from mas in db.MASTERMEMBERs where mas.MEMBER_CODE == dMember_Code select mas).FirstOrDefault();

                    if (bUpdate == false)
                    {
                        DataView dv = new DataView(dtArrearPost);
                        dv.RowFilter = "AMOUNT>0";
                        int iTotalDuePay = dv.Count;

                        ArrearPostMaster arrearMaster = new ArrearPostMaster
                        {
                            MemberCode = dMember_Code,
                            EntryDate = DateTime.Now,
                            BeforeLastPaymentDate = mmMst.LASTPAYMENT_DATE,
                            BeforeTotalBF = mmMst.ACCBF,
                            BeforeTotalSubscription = mmMst.ACCSUBSCRIPTION,
                            BeforeTotalMonthsPaid = Convert.ToInt32(mmMst.TOTALMONTHSPAID),
                            TotalDuePay = iTotalDuePay,
                            UpdatedStatus = "Not Updated"
                        };
                        db.ArrearPostMasters.Add(arrearMaster);
                        db.SaveChanges();

                        var NewData = new JSonHelper().ConvertObjectToJSon(arrearMaster);
                        AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "ArrearPostMaster");

                        DataTable dtTemp = new DataTable();
                        dtTemp = ((DataView)dgArrearPost.ItemsSource).ToTable();
                        var iMasterId = Convert.ToInt32(db.ArrearPostMasters.Max(x => x.ID));

                        List<ArrearPostDetail> lstArrearDtl = new List<ArrearPostDetail>();

                        foreach (DataRow row in dtTemp.Rows)
                        {
                            if (Convert.ToDecimal(row["AMOUNT"]) != 0)
                            {
                                ArrearPostDetail arrearDtl = new ArrearPostDetail
                                {
                                    MasterId = iMasterId,
                                    Year = Convert.ToInt32(row["YEAR"]),                                    
                                    Month = Convert.ToDateTime("1/" + row["MONTH"] + row["YEAR"]).Month,
                                    TotalAmount = Convert.ToDecimal(row["AMOUNT"]),
                                    AmounBF = Convert.ToDecimal(row["BF"]),
                                    AmountInsurance = Convert.ToDecimal(row["INSURANCE"]),
                                    AmountSubscription = Convert.ToDecimal(row["SUBSCRIPTION"]),
                                    UpdatedStatus = "Not Updated",
                                    MemberCode = Convert.ToInt32(dMember_Code)
                                };
                                lstArrearDtl.Add(arrearDtl);
                            }
                        }

                        if (lstArrearDtl.Count > 0)
                        {
                            db.ArrearPostDetails.AddRange(lstArrearDtl);
                            db.SaveChanges();

                            var NewData1 = new JSonHelper().ConvertObjectToJSon(lstArrearDtl);
                            AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData1, "ArrearPostDetail");
                        }

                        MessageBox.Show("Details are Saved Sucessfully");
                        fClear();
                    }
                    else
                    {
                        ArrearPostMaster mst = (from mas in db.ArrearPostMasters where mas.ID == iArrearMst_ID select mas).FirstOrDefault();

                        var OldData = new JSonHelper().ConvertObjectToJSon(mst);
                        DataView dv = new DataView(dtArrearPost);
                        dv.RowFilter = "AMOUNT>0";
                        int iTotalDuePay = dv.Count;

                        if (mst != null)
                        {
                            mst.UpdatedEntryDate = DateTime.Now;
                            mst.IsEdited = true;
                            mst.TotalDuePay = iTotalDuePay;
                        }
                        db.SaveChanges();

                        var NewData = new JSonHelper().ConvertObjectToJSon(mst);
                        AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "ArrearPostMaster");

                        ArrearPostDetail dtl = (from mas in db.ArrearPostDetails where mas.MasterId == iArrearMst_ID select mas).FirstOrDefault();
                        var OldData1 = new JSonHelper().ConvertObjectToJSon(dtl);
                        if (dtl != null)
                        {
                            db.ArrearPostDetails.RemoveRange(db.ArrearPostDetails.Where(x => x.MasterId == iArrearMst_ID));
                            db.SaveChanges();
                        }

                        DataTable dtTemp = new DataTable();
                        dtTemp = ((DataView)dgArrearPost.ItemsSource).ToTable();

                        List<ArrearPostDetail> lstArrearDtl = new List<ArrearPostDetail>();

                        foreach (DataRow row in dtTemp.Rows)
                        {
                            if (Convert.ToDecimal(row["AMOUNT"]) != 0)
                            {
                                ArrearPostDetail arrearDtl = new ArrearPostDetail
                                {
                                    MasterId = iArrearMst_ID,
                                    Year = Convert.ToInt32(row["YEAR"]),
                                    Month = Convert.ToDateTime("1/" + row["MONTH"] + row["YEAR"]).Month,
                                    TotalAmount = Convert.ToDecimal(row["AMOUNT"]),
                                    AmounBF = Convert.ToDecimal(row["BF"]),
                                    AmountInsurance = Convert.ToDecimal(row["INSURANCE"]),
                                    AmountSubscription = Convert.ToDecimal(row["SUBSCRIPTION"]),
                                    UpdatedStatus = "Not Updated",
                                    MemberCode = Convert.ToInt32(dMember_Code)
                                };
                                lstArrearDtl.Add(arrearDtl);
                            }
                        }

                        if (lstArrearDtl.Count > 0)
                        {
                            db.ArrearPostDetails.AddRange(lstArrearDtl);
                            db.SaveChanges();

                            var NewData1 = new JSonHelper().ConvertObjectToJSon(lstArrearDtl);
                            AppLib.EventHistory(this.Tag.ToString(), 1, OldData1, NewData1, "ArrearPostDetail");
                        }

                        MessageBox.Show("Details are Updated Sucessfully");
                        fClear();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            frmHomeMembership frm = new frmHomeMembership();
            this.Close();
            frm.ShowDialog();
        }

        #endregion

        #region "OTHER EVENTS"

        private void txtMemberNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    dgArrearPost.ItemsSource = null;
                    txtAmount.Text = "";
                    txtBF.Text = "";
                    txtInsurance.Text = "";
                    txtSubscription.Text = "";
                    dMember_Code = 0;
                    iArrearMst_ID = 0;
                    FormFill();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void dgArrearPost_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (e.EditAction == DataGridEditAction.Commit)
                {
                    var column = e.Column as DataGridBoundColumn;
                    if (column != null)
                    {
                        var sColumnName = (column.Binding as Binding).Path.Path;
                        if (sColumnName == "YEAR")
                        {
                            var el = e.EditingElement as TextBox;
                            if (!string.IsNullOrEmpty(el.Text))
                            {
                                int iRow = dgArrearPost.SelectedIndex;
                                if (!string.IsNullOrEmpty(el.Text) && Convert.ToInt32(el.Text) != 0)
                                {
                                    dtArrearPost.Rows[iRow]["YEAR"] = el.Text;
                                }
                                dgArrearPost.CurrentCell = new DataGridCellInfo(dgArrearPost.Items[iRow], dgArrearPost.Columns[1]);
                                dgArrearPost.BeginEdit();
                            }
                        }
                        else if (sColumnName == "MONTH")
                        {
                            var el = e.EditingElement as TextBox;
                            if (!string.IsNullOrEmpty(el.Text))
                            {
                                int iRow = dgArrearPost.SelectedIndex;
                                if (!string.IsNullOrEmpty(el.Text))
                                {
                                    dtArrearPost.Rows[iRow]["MONTH"] = el.Text;
                                }
                                dgArrearPost.CurrentCell = new DataGridCellInfo(dgArrearPost.Items[iRow], dgArrearPost.Columns[2]);
                                dgArrearPost.BeginEdit();
                            }
                        }
                        else if (sColumnName == "AMOUNT")
                        {
                            var el = e.EditingElement as TextBox;
                            if (!string.IsNullOrEmpty(el.Text))
                            {
                                int iRow = dgArrearPost.SelectedIndex;
                                if (!string.IsNullOrEmpty(el.Text))
                                {
                                    if (Convert.ToInt32(el.Text) != 0)
                                    {
                                        MASTERMEMBER mm = (from mas in db.MASTERMEMBERs where mas.MEMBER_CODE == dMember_Code select mas).FirstOrDefault();
                                        dtArrearPost.Rows[iRow]["AMOUNT"] = el.Text;
                                        dtArrearPost.Rows[iRow]["BF"] = Convert.ToInt32(mm.MONTHLYBF);
                                        dtArrearPost.Rows[iRow]["INSURANCE"] = 4;
                                        dtArrearPost.Rows[iRow]["SUBSCRIPTION"] = Convert.ToInt32(el.Text) - (Convert.ToInt32(mm.MONTHLYBF) + 4);

                                        fTotalAmount();
                                        if (dgArrearPost.Items.Count <= (iRow + 1))
                                        {
                                            fNewRow();
                                            if (bNewRow == true)
                                            {
                                                dgArrearPost.CurrentCell = new DataGridCellInfo(dgArrearPost.Items[iRow + 1], dgArrearPost.Columns[0]);
                                                dgArrearPost.BeginEdit();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void GridComboBoxChangeEvent(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var comboBox = sender as ComboBox;
                string sMonth = comboBox.Text.ToString();
                var selectedItem = this.dgArrearPost.CurrentItem;

                if (!string.IsNullOrEmpty(sMonth))
                {
                    int iRow = dgArrearPost.SelectedIndex;
                    if (!string.IsNullOrEmpty(sMonth))
                    {
                        dtArrearPost.Rows[iRow]["MONTH"] = sMonth;
                    }
                    dgArrearPost.CurrentCell = new DataGridCellInfo(dgArrearPost.Items[iRow], dgArrearPost.Columns[2]);
                    //dgArrearPost.BeginEdit();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion

        #region "USER DEFINED FUNCTIONS"

        private void FormFill()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtMemberNo.Text))
                {
                    decimal dtxtMember_ID = Convert.ToDecimal(txtMemberNo.Text);
                    var qry = (from mm in db.MASTERMEMBERs
                               join mt in db.MASTERMEMBERTYPEs on mm.MEMBERTYPE_CODE equals mt.MEMBERTYPE_CODE
                               join bk in db.MASTERBANKs on mm.BANK_CODE equals bk.BANK_CODE
                               join bc in db.MASTERBANKBRANCHes on mm.BRANCH_CODE equals bc.BANKBRANCH_CODE
                               join rc in db.MASTERRACEs on mm.RACE_CODE equals rc.RACE_CODE
                               where mm.MEMBER_ID == dtxtMember_ID
                               select new
                               {
                                   mm.MEMBER_ID,
                                   mt.MEMBERTYPE_NAME,
                                   mm.MEMBER_CODE,
                                   mm.MEMBER_NAME,
                                   mm.DATEOFBIRTH,
                                   mm.AGE_IN_YEARS,
                                   rc.RACE_NAME,
                                   mm.SEX,
                                   mm.ICNO_OLD,
                                   mm.ICNO_NEW,
                                   mm.DATEOFJOINING,
                                   bk.BANK_CODE,
                                   bk.BANK_NAME,
                                   mm.BRANCH_CODE,
                                   bc.BANKBRANCH_NAME,
                                   mm.LASTPAYMENT_DATE,
                                   mm.TOTALMONTHSPAID
                               }
                         ).FirstOrDefault();
                    if (qry != null)
                    {
                        dMember_Code = qry.MEMBER_CODE;
                        txtMemberType.Text = qry.MEMBERTYPE_NAME;
                        txtMemberNo.Text = qry.MEMBER_ID.ToString();
                        txtMemberName.Text = qry.MEMBER_NAME;
                        dtpDOB.SelectedDate = qry.DATEOFBIRTH;
                        txtAge.Text = qry.AGE_IN_YEARS.ToString();
                        txtRace.Text = qry.RACE_NAME.ToString();

                        txtGender.Text = qry.SEX;
                        txtBankCode.Text = qry.BANK_CODE.ToString();
                        txtBankName.Text = qry.BANK_NAME.ToString();
                        txtBranchCode.Text = qry.BRANCH_CODE.ToString();
                        txtBranchName.Text = qry.BANKBRANCH_NAME;
                        txtOldIC.Text = qry.ICNO_OLD;
                        txtNewIC.Text = qry.ICNO_NEW;
                        dtpDOJ.SelectedDate = qry.DATEOFJOINING;
                        DateTime dt = Convert.ToDateTime(qry.LASTPAYMENT_DATE);
                        txtLastPaymentDate.Text = dt.Day.ToString() + "-" + dt.Month.ToString() + "-" + dt.Year.ToString();
                        DateTime dJoiningDate = Convert.ToDateTime(qry.DATEOFJOINING);
                        DateTime dToday = DateTime.Now;
                        int iTotalMonth = Convert.ToInt32(((dToday.Year - dJoiningDate.Year) * 12) + dToday.Month - dJoiningDate.Month);
                        int iBalanceDue = (iTotalMonth - Convert.ToInt32(qry.TOTALMONTHSPAID));
                        iTotalMonthsPaid = Convert.ToInt32(qry.TOTALMONTHSPAID);
                        txtBalanceDue.Text = iBalanceDue.ToString();
                        fNewRow();
                        btnEditOldDue.Visibility = Visibility.Visible;
                    }
                }
                else if (dMember_Code != 0)
                {
                    var qry = (from mm in db.MASTERMEMBERs
                               join mt in db.MASTERMEMBERTYPEs on mm.MEMBERTYPE_CODE equals mt.MEMBERTYPE_CODE
                               join bk in db.MASTERBANKs on mm.BANK_CODE equals bk.BANK_CODE
                               join bc in db.MASTERBANKBRANCHes on mm.BRANCH_CODE equals bc.BANKBRANCH_CODE
                               join rc in db.MASTERRACEs on mm.RACE_CODE equals rc.RACE_CODE
                               where mm.MEMBER_CODE == dMember_Code
                               select new
                               {
                                   mt.MEMBERTYPE_NAME,
                                   mm.MEMBER_ID,
                                   mm.MEMBER_NAME,
                                   mm.DATEOFBIRTH,
                                   mm.AGE_IN_YEARS,
                                   rc.RACE_NAME,
                                   mm.SEX,
                                   mm.ICNO_OLD,
                                   mm.ICNO_NEW,
                                   mm.DATEOFJOINING,
                                   bk.BANK_CODE,
                                   bk.BANK_NAME,
                                   mm.BRANCH_CODE,
                                   bc.BANKBRANCH_NAME,
                                   mm.LASTPAYMENT_DATE,
                                   mm.TOTALMONTHSPAID
                               }
                         ).FirstOrDefault();
                    if (qry != null)
                    {
                        txtMemberType.Text = qry.MEMBERTYPE_NAME;
                        txtMemberNo.Text = qry.MEMBER_ID.ToString();
                        txtMemberName.Text = qry.MEMBER_NAME;
                        dtpDOB.SelectedDate = qry.DATEOFBIRTH;
                        txtAge.Text = qry.AGE_IN_YEARS.ToString();
                        txtRace.Text = qry.RACE_NAME.ToString();

                        txtGender.Text = qry.SEX;
                        txtBankCode.Text = qry.BANK_CODE.ToString();
                        txtBankName.Text = qry.BANK_NAME.ToString();
                        txtBranchCode.Text = qry.BRANCH_CODE.ToString();
                        txtBranchName.Text = qry.BANKBRANCH_NAME;
                        txtOldIC.Text = qry.ICNO_OLD;
                        txtNewIC.Text = qry.ICNO_NEW;
                        dtpDOJ.SelectedDate = qry.DATEOFJOINING;
                        DateTime dt = Convert.ToDateTime(qry.LASTPAYMENT_DATE);
                        txtLastPaymentDate.Text = dt.Day.ToString() + "-" + dt.Month.ToString() + "-" + dt.Year.ToString();
                        DateTime dJoiningDate = Convert.ToDateTime(qry.DATEOFJOINING);
                        DateTime dToday = DateTime.Now;
                        int iTotalMonth = Convert.ToInt32(((dToday.Year - dJoiningDate.Year) * 12) + dToday.Month - dJoiningDate.Month);
                        int iBalanceDue = (iTotalMonth - Convert.ToInt32(qry.TOTALMONTHSPAID));
                        iTotalMonthsPaid = Convert.ToInt32(qry.TOTALMONTHSPAID);
                        txtBalanceDue.Text = iBalanceDue.ToString();
                        fNewRow();
                        btnEditOldDue.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void GridFill()
        {
            try
            {
                var qry = (from ad in db.ArrearPostDetails
                           where ad.MasterId == iArrearMst_ID
                           select new
                           {
                               YEAR = ad.Year,
                               MONTH = ad.Month,
                               AMOUNT = ad.TotalAmount,
                               BF = ad.AmounBF,
                               INSURANCE = ad.AmountInsurance,
                               SUBSCRIPTION = ad.AmountSubscription
                           }).ToList();
                dtArrearPost = AppLib.LINQResultToDataTable(qry);
                dgArrearPost.ItemsSource = dtArrearPost.DefaultView;
                fTotalAmount();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }    

        void fClear()
        {
            txtMemberType.Text = "";
            txtMemberNo.Text = "";
            txtMemberName.Text = "";
            dtpDOB.Text = "";
            txtAge.Text = "";

            txtGender.Text = "";
            txtBankCode.Text = "";
            txtBankName.Text = "";
            txtBranchCode.Text = "";
            txtBranchName.Text = "";
            txtOldIC.Text = "";
            txtNewIC.Text = "";
            dtpDOJ.Text = "";
            txtBalanceDue.Text = "";
            txtLastPaymentDate.Text = "";

            txtAmount.Text = "";
            txtBF.Text = "";
            txtInsurance.Text = "";
            txtSubscription.Text = "";
            dgArrearPost.ItemsSource = null;
            dMember_Code = 0;
            iArrearMst_ID = 0;
            txtRace.Text = "";
            bUpdate = false;
            dtArrearPost.Rows.Clear();
        }

        void fNewRow()
        {
            if (dgArrearPost.Items.Count > 0)
            {
                dtArrearPost.Rows.Add(0, "", 0, 0, 0, 0);
                dgArrearPost.ItemsSource = dtArrearPost.DefaultView;
                bNewRow = true;
            }
            else
            {
                DataRow row;
                row = dtArrearPost.NewRow();
                dtArrearPost.Rows.Add(row);
                dgArrearPost.ItemsSource = dtArrearPost.DefaultView;

                List<string> lstMonths = new List<string>();
                lstMonths.Add("JAN");
                lstMonths.Add("FEB");
                lstMonths.Add("MAR");
                lstMonths.Add("APR");
                lstMonths.Add("MAY");
                lstMonths.Add("JUN");
                lstMonths.Add("JUL");
                lstMonths.Add("AUQ");
                lstMonths.Add("SEP");
                lstMonths.Add("OCT");
                lstMonths.Add("NOV");
                lstMonths.Add("DEC");
                MONTH.ItemsSource = lstMonths;

                dgArrearPost.CurrentCell = new DataGridCellInfo(dgArrearPost.Items[0], dgArrearPost.Columns[0]);
                dgArrearPost.BeginEdit();

            }
        }

        private void newDataTable()
        {
            dtArrearPost.Columns.Add("YEAR");
            dtArrearPost.Columns.Add("MONTH");
            dtArrearPost.Columns.Add("AMOUNT");
            dtArrearPost.Columns.Add("BF");
            dtArrearPost.Columns.Add("INSURANCE");
            dtArrearPost.Columns.Add("SUBSCRIPTION");

            dtArrearPost.Columns[0].DataType = typeof(Int32);
            dtArrearPost.Columns[1].DataType = typeof(string);
            dtArrearPost.Columns[2].DataType = typeof(Int32);
            dtArrearPost.Columns[3].DataType = typeof(Int32);
            dtArrearPost.Columns[4].DataType = typeof(Int32);
            dtArrearPost.Columns[5].DataType = typeof(Int32);

            dtArrearPost.Columns[0].DefaultValue = 0;
            dtArrearPost.Columns[1].DefaultValue = "";
            dtArrearPost.Columns[2].DefaultValue = 0;
            dtArrearPost.Columns[3].DefaultValue = 0;
            dtArrearPost.Columns[4].DefaultValue = 0;
            dtArrearPost.Columns[5].DefaultValue = 0;
        }

        void fTotalAmount()
        {
            int iTotalAmount = 0;
            int iTotalBF = 0;
            int iTotalInsur = 0;
            int iTotalSubs = 0;

            foreach (DataRow dr in dtArrearPost.Rows)
            {
                iTotalAmount = iTotalAmount + Convert.ToInt32(dr["AMOUNT"]);
                iTotalBF = iTotalBF + Convert.ToInt32(dr["BF"]);
                iTotalInsur = iTotalInsur + Convert.ToInt32(dr["INSURANCE"]);
                iTotalSubs = iTotalSubs + Convert.ToInt32(dr["SUBSCRIPTION"]);
            }
            txtAmount.Text = iTotalAmount.ToString();
            txtBF.Text = iTotalBF.ToString();
            txtInsurance.Text = iTotalInsur.ToString();
            txtSubscription.Text = iTotalSubs.ToString();
        }

        void BeforeUpdate()
        {
            if (string.IsNullOrEmpty(txtMemberNo.Text))
            {
                MessageBox.Show("Please Enter the Membership No!");
                txtMemberNo.Focus();
                bValidation = true;
            }
            else if (dgArrearPost.Items.Count == 0)
            {
                MessageBox.Show("Arrear Details are Empty!");
                dgArrearPost.Focus();
                bValidation = true;
            }
            else if (dMember_Code == 0)
            {
                MessageBox.Show("Please Select Any Member!");
                btnSelect.Focus();
                bValidation = true;
            }
        }

        #endregion

    }
}
