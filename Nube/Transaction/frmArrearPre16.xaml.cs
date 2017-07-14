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
    /// Interaction logic for frmArrearPre16.xaml
    /// </summary>
    public partial class frmArrearPre16 : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        decimal dMember_Code = 0;
        int iArrearMst_ID = 0;
        Boolean bValidation = false;
        int iTotalMonthsPaid = 0;
        DataTable dtArrearPre = new DataTable();
        Boolean bNewRow = false;
        Boolean bUpdate = false;
       
        string connectionstring =AppLib.connstatus;

        public frmArrearPre16(decimal dMembercode = 0, int iID = 0)
        {
            InitializeComponent();
            dMember_Code = dMembercode;
            iArrearMst_ID = iID;
            newDataTable();
            txtMemberNo.Focus();
            btnEditOldDue.Visibility = Visibility.Collapsed;
            if (dMember_Code != 0)
            {
                FormFill();
                bUpdate = false;
            }
            if (iArrearMst_ID != 0)
            {
                bUpdate = true;
                GridFill();
            }
        }

        #region "CLICK EVENTS"

        private void txtMemberNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    dgArrearPre.ItemsSource = null;
                    txtAmount.Text = "";
                    txtBF.Text = "";
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

        private void btnEditOldDue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dMember_Code != 0)
                {
                    ArrearPreMaster mst = (from mas in db.ArrearPreMasters where mas.MemberCode == dMember_Code select mas).FirstOrDefault();
                    if (mst != null)
                    {
                        frmArrearDueList frm = new frmArrearDueList(dMember_Code, "PreArrear", txtMemberType.Text, txtMemberName.Text, txtMemberNo.Text);
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
            frmMemberQuery frm = new frmMemberQuery("PreArrear");
            this.Close();
            frm.ShowDialog();
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
                        DataView dv = new DataView(dtArrearPre);
                        dv.RowFilter = "AMOUNT>0";
                        int iTotalDuePay = dv.Count;

                        ArrearPreMaster arrearMaster = new ArrearPreMaster
                        {
                            MemberCode = dMember_Code,
                            EntryDate = DateTime.Now,
                            BeforeLastPaymentDate = mmMst.LASTPAYMENT_DATE,
                            BeforeTotalBF = mmMst.ACCBF,
                            BeforeTotalSubscription = mmMst.ACCSUBSCRIPTION,
                            BeforeTotalMonthsPaid = Convert.ToInt32(mmMst.TOTALMONTHSPAID),
                            TotalDuePay = iTotalDuePay,
                            UpdatedStatus="Not Updated"
                        };
                        db.ArrearPreMasters.Add(arrearMaster);
                        db.SaveChanges();

                        var NewData = new JSonHelper().ConvertObjectToJSon(arrearMaster);
                        AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "ArrearPreMaster");

                        DataTable dtTemp = new DataTable();
                        dtTemp = ((DataView)dgArrearPre.ItemsSource).ToTable();
                        var iMasterId = Convert.ToInt32(db.ArrearPreMasters.Max(x => x.ID));

                        List<ArrearPreDetail> lstArrearDtl = new List<ArrearPreDetail>();                                         

                        foreach (DataRow row in dtTemp.Rows)
                        {
                            if (Convert.ToDecimal(row["AMOUNT"]) != 0)
                            {
                                ArrearPreDetail arrearDtl = new ArrearPreDetail
                                {
                                    MasterId = iMasterId,
                                    Year = Convert.ToInt32(row["YEAR"]),
                                    Month = Convert.ToDateTime("1/" + row["MONTH"] + row["YEAR"]).Month,
                                    TotalAmount = Convert.ToDecimal(row["AMOUNT"]),
                                    AmounBF = Convert.ToDecimal(row["BF"]),
                                    AmountSubscription = Convert.ToDecimal(row["SUBSCRIPTION"]),
                                    UpdatedStatus = "Not Updated",                                    
                                    MemberCode = Convert.ToInt32(dMember_Code)
                                };

                                lstArrearDtl.Add(arrearDtl);                                                           
                            }
                        }

                        if (lstArrearDtl.Count > 0)
                        {
                            db.ArrearPreDetails.AddRange(lstArrearDtl);
                            db.SaveChanges();

                            var NewData1 = new JSonHelper().ConvertObjectToJSon(lstArrearDtl);
                            AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData1, "ArrearPreDetail");
                        }
                       

                        MessageBox.Show("Details are Saved Sucessfully");
                        fClear();
                    }
                    else
                    {
                        ArrearPreMaster mst = (from mas in db.ArrearPreMasters where mas.ID == iArrearMst_ID select mas).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(mst);
                        if (mst != null)
                        {
                            mst.UpdatedEntryDate = DateTime.Now;
                            mst.IsEdited = true;
                        }
                        db.SaveChanges();

                        var NewData = new JSonHelper().ConvertObjectToJSon(mst);
                        AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "ArrearPreMaster");

                        ArrearPreDetail dtl = (from mas in db.ArrearPreDetails where mas.MasterId == iArrearMst_ID select mas).FirstOrDefault();
                        var OldData1 = new JSonHelper().ConvertObjectToJSon(dtl);
                        if (dtl != null)
                        {
                            db.ArrearPreDetails.RemoveRange(db.ArrearPreDetails.Where(x => x.MasterId == iArrearMst_ID));
                            db.SaveChanges();
                        }                      

                        DataTable dtTemp = new DataTable();
                        dtTemp = ((DataView)dgArrearPre.ItemsSource).ToTable();

                        List<ArrearPreDetail> lstArrearDtl = new List<ArrearPreDetail>();                       

                        foreach (DataRow row in dtTemp.Rows)
                        {
                            if (Convert.ToDecimal(row["AMOUNT"]) != 0)
                            {
                                ArrearPreDetail arrearDtl = new ArrearPreDetail
                                {
                                    MasterId = iArrearMst_ID,
                                    Year = Convert.ToInt32(row["YEAR"]),
                                    Month = Convert.ToDateTime("1/" + row["MONTH"] + row["YEAR"]).Month,
                                    TotalAmount = Convert.ToDecimal(row["AMOUNT"]),
                                    AmounBF = Convert.ToDecimal(row["BF"]),
                                    AmountSubscription = Convert.ToDecimal(row["SUBSCRIPTION"]),
                                    UpdatedStatus = "Not Updated",                                   
                                    MemberCode = Convert.ToInt32(dMember_Code)
                                };
                                lstArrearDtl.Add(arrearDtl);                               
                            }
                        }

                        if (lstArrearDtl.Count > 0)
                        {
                            db.ArrearPreDetails.AddRange(lstArrearDtl);
                            db.SaveChanges();

                            var NewData1 = new JSonHelper().ConvertObjectToJSon(lstArrearDtl);
                            AppLib.EventHistory(this.Tag.ToString(), 1, OldData1, NewData1, "ArrearPreDetail");
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

        private void dgArrearPre_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
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
                                int iRow = dgArrearPre.SelectedIndex;
                                if (!string.IsNullOrEmpty(el.Text) && Convert.ToInt32(el.Text) != 0)
                                {
                                    dtArrearPre.Rows[iRow]["YEAR"] = el.Text;
                                }
                                dgArrearPre.CurrentCell = new DataGridCellInfo(dgArrearPre.Items[iRow], dgArrearPre.Columns[1]);
                                dgArrearPre.BeginEdit();
                            }
                        }
                        else if (sColumnName == "MONTH")
                        {
                            var el = e.EditingElement as TextBox;
                            if (!string.IsNullOrEmpty(el.Text))
                            {
                                int iRow = dgArrearPre.SelectedIndex;
                                if (!string.IsNullOrEmpty(el.Text))
                                {
                                    dtArrearPre.Rows[iRow]["MONTH"] = el.Text;
                                }
                                dgArrearPre.CurrentCell = new DataGridCellInfo(dgArrearPre.Items[iRow], dgArrearPre.Columns[2]);
                                dgArrearPre.BeginEdit();
                            }
                        }
                        else if (sColumnName == "AMOUNT")
                        {
                            var el = e.EditingElement as TextBox;
                            if (!string.IsNullOrEmpty(el.Text))
                            {
                                int iRow = dgArrearPre.SelectedIndex;
                                if (!string.IsNullOrEmpty(el.Text))
                                {
                                    if (Convert.ToInt32(el.Text) != 0)
                                    {
                                        MASTERMEMBER mm = (from mas in db.MASTERMEMBERs where mas.MEMBER_CODE == dMember_Code select mas).FirstOrDefault();
                                        dtArrearPre.Rows[iRow]["AMOUNT"] = el.Text;
                                        dtArrearPre.Rows[iRow]["BF"] =Convert.ToInt32(mm.MONTHLYBF);
                                        dtArrearPre.Rows[iRow]["SUBSCRIPTION"] = Convert.ToInt32(el.Text)- Convert.ToInt32(mm.MONTHLYBF);

                                        fTotalAmount();
                                        if (dgArrearPre.Items.Count <= (iRow + 1))
                                        {
                                            fNewRow();
                                            if (bNewRow == true)
                                            {
                                                dgArrearPre.CurrentCell = new DataGridCellInfo(dgArrearPre.Items[iRow + 1], dgArrearPre.Columns[0]);
                                                dgArrearPre.BeginEdit();
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
                var selectedItem = this.dgArrearPre.CurrentItem;

                if (!string.IsNullOrEmpty(sMonth))
                {
                    int iRow = dgArrearPre.SelectedIndex;
                    if (!string.IsNullOrEmpty(sMonth))
                    {
                        dtArrearPre.Rows[iRow]["MONTH"] = sMonth;
                    }
                    dgArrearPre.CurrentCell = new DataGridCellInfo(dgArrearPre.Items[iRow], dgArrearPre.Columns[2]);
                    //dgArrearPre.BeginEdit();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion

        #region "USER DEFINED FUNCTIONS"

        private void GridFill()
        {
            try
            {
                var qry = (from ad in db.ArrearPreDetails
                           where ad.MasterId == iArrearMst_ID
                           select new
                           {
                               YEAR = ad.Year,
                               MONTH = ad.Month,
                               AMOUNT = ad.TotalAmount,
                               BF = ad.AmounBF,
                               SUBSCRIPTION = ad.AmountSubscription
                           }).ToList();
                dtArrearPre = AppLib.LINQResultToDataTable(qry);
                dgArrearPre.ItemsSource = dtArrearPre.DefaultView;
                fTotalAmount();
                bUpdate = true;
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }      

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
                                   mt.MEMBERTYPE_NAME,
                                   mm.MEMBER_ID,
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
                                   mm.TOTALMONTHSPAID,
                                   mm.MEMBER_ID
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
            txtSubscription.Text = "";
            dgArrearPre.ItemsSource = null;
            dMember_Code = 0;
            iArrearMst_ID = 0;
            txtRace.Text = "";
            bUpdate = false;
            dtArrearPre.Rows.Clear();

        }

        void fNewRow()
        {
            if (dgArrearPre.Items.Count > 0)
            {
                dtArrearPre.Rows.Add(0, "", 0, 0, 0);
                dgArrearPre.ItemsSource = dtArrearPre.DefaultView;
                bNewRow = true;
            }
            else
            {
                DataRow row;
                row = dtArrearPre.NewRow();
                dtArrearPre.Rows.Add(row);
                dgArrearPre.ItemsSource = dtArrearPre.DefaultView;

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

                dgArrearPre.CurrentCell = new DataGridCellInfo(dgArrearPre.Items[0], dgArrearPre.Columns[0]);
                dgArrearPre.BeginEdit();

            }
        }

        private void newDataTable()
        {
            dtArrearPre.Columns.Add("YEAR");
            dtArrearPre.Columns.Add("MONTH");
            dtArrearPre.Columns.Add("AMOUNT");
            dtArrearPre.Columns.Add("BF");
            dtArrearPre.Columns.Add("SUBSCRIPTION");

            dtArrearPre.Columns[0].DataType = typeof(Int32);
            dtArrearPre.Columns[1].DataType = typeof(string);
            dtArrearPre.Columns[2].DataType = typeof(Int32);
            dtArrearPre.Columns[3].DataType = typeof(Int32);
            dtArrearPre.Columns[4].DataType = typeof(Int32);

            dtArrearPre.Columns[0].DefaultValue = 0;
            dtArrearPre.Columns[1].DefaultValue = "";
            dtArrearPre.Columns[2].DefaultValue = 0;
            dtArrearPre.Columns[3].DefaultValue = 0;
            dtArrearPre.Columns[4].DefaultValue = 0;
        }

        void fTotalAmount()
        {
            int iTotalAmount = 0;
            int iTotalBF = 0;
            int iTotalSubs = 0;

            foreach (DataRow dr in dtArrearPre.Rows)
            {
                iTotalAmount = iTotalAmount + Convert.ToInt32(dr["AMOUNT"]);
                iTotalBF = iTotalBF + Convert.ToInt32(dr["BF"]);
                iTotalSubs = iTotalSubs + Convert.ToInt32(dr["SUBSCRIPTION"]);
            }
            txtAmount.Text = iTotalAmount.ToString();
            txtBF.Text = iTotalBF.ToString();
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
            else if (dgArrearPre.Items.Count == 0)
            {
                MessageBox.Show("Arrear Details are Empty!");
                dgArrearPre.Focus();
                bValidation = true;
            }
        }

        #endregion
    }
}
