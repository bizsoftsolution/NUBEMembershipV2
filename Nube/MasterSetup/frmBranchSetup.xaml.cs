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
using Nube;

namespace Nube.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmBranchSetup.xaml
    /// </summary>
    public partial class frmBranchSetup : MetroWindow
    {
        public Boolean bIsEdit = false;
        int ID = 0;
        nubebfsEntity db = new nubebfsEntity();
        string connStr = AppLib.connStr;
        DataTable dtBank = new DataTable();
        string sFormName = "";

        public frmBranchSetup(string sForm_Name = "")
        {
            InitializeComponent();
            try
            {
                sFormName = sForm_Name;
                if (sForm_Name == "HOME")
                {
                    btnPrint.Visibility = Visibility.Visible;
                }
                else
                {
                    btnPrint.Visibility = Visibility.Hidden;
                }
                btnClearGd.Visibility = Visibility.Hidden;

                LoadWindow();
                this.KeyDown += new System.Windows.Input.KeyEventHandler(Window_KeyDown);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #region "BUTTON EVENTS"

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                this.Close();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure to exit?", "Exit Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbBankName.Text == "")
                {
                    MessageBox.Show("Enter Bank..", "Information");
                    txtBranchName.Focus();
                }

                else if (txtBranchCode.Text == "")
                {
                    MessageBox.Show("Enter Branchcode..", "Information");
                    txtBranchCode.Focus();
                }
                else if (txtBranchName.Text == "")
                {
                    MessageBox.Show("Enter Branchname..", "Information");
                }
                else
                {
                    if (MessageBox.Show("Do you want to save this record?", "SAVE", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (ID != 0)
                        {
                            decimal id = (decimal)ID;
                            var s = cmbBankName.Text.ToString();
                            MASTERBANKBRANCH mb = db.MASTERBANKBRANCHes.Where(x => x.BANKBRANCH_CODE == id).FirstOrDefault();

                            var OldData = new JSonHelper().ConvertObjectToJSon(mb);

                            mb.BANK_CODE = Convert.ToDecimal(cmbBankName.SelectedValue);
                            mb.BANKBRANCH_NAME = txtBranchName.Text;
                            mb.BANKBRANCH_USERCODE = txtBranchCode.Text;
                            mb.BANKBRANCH_ADDRESS1 = txtAddress1.Text;
                            mb.BANKBRANCH_ADDRESS2 = txtAddress2.Text;
                            mb.BANKBRANCH_ADDRESS3 = txtAddress3.Text;
                            mb.BANKBRANCH_CITY_CODE = Convert.ToDecimal(cmbCity.SelectedValue);
                            mb.BANKBRANCH_COUNTRY = (cmbCountry.Text.ToString() == null ? "" : cmbCountry.Text.ToString());
                            mb.BANKBRANCH_EMAIL = txtEmail.Text;
                            mb.BANKBRANCH_PHONE1 = txtMobile.Text;
                            mb.BANKBRANCH_PHONE2 = txtPhone.Text;
                            mb.BANKBRANCH_STATE_CODE = Convert.ToDecimal(cmbState.SelectedValue);
                            mb.BANKBRANCH_ZIPCODE = txtPostalCode.Text;
                            mb.NUBE_BRANCH_CODE = Convert.ToDecimal(cmbNUBEBranch.SelectedValue);
                            mb.HEAD_QUARTERS = Convert.ToDecimal(ckbHeadOffice.IsChecked);

                            db.SaveChanges();
                            AppLib.lstMASTERBANKBRANCH = db.MASTERBANKBRANCHes.OrderBy(x => x.BANKBRANCH_NAME).ToList();

                            var NewData = new JSonHelper().ConvertObjectToJSon(mb);
                            AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "MASTERBANKBRANCH");

                            MessageBox.Show("Saved Successfully", "Saved");
                            ClearForm();
                        }
                        else
                        {
                            MASTERBANKBRANCH mb = new MASTERBANKBRANCH();
                            mb.BANK_CODE = Convert.ToDecimal(cmbBankName.SelectedValue);
                            mb.BANKBRANCH_NAME = txtBranchName.Text;
                            mb.BANKBRANCH_USERCODE = txtBranchCode.Text;
                            mb.BANKBRANCH_ADDRESS1 = txtAddress1.Text;
                            mb.BANKBRANCH_ADDRESS2 = txtAddress2.Text;
                            mb.BANKBRANCH_ADDRESS3 = txtAddress3.Text;
                            mb.BANKBRANCH_CITY_CODE = Convert.ToDecimal(cmbCity.SelectedValue);
                            mb.BANKBRANCH_COUNTRY = (cmbCountry.Text.ToString() == null ? "" : cmbCountry.Text.ToString());
                            mb.BANKBRANCH_EMAIL = txtEmail.Text;
                            mb.BANKBRANCH_PHONE1 = txtMobile.Text;
                            mb.BANKBRANCH_PHONE2 = txtPhone.Text;
                            mb.BANKBRANCH_STATE_CODE = Convert.ToDecimal(cmbState.SelectedValue);
                            mb.BANKBRANCH_ZIPCODE = txtPostalCode.Text;
                            mb.NUBE_BRANCH_CODE = Convert.ToDecimal(cmbNUBEBranch.SelectedValue);
                            mb.HEAD_QUARTERS = Convert.ToDecimal(ckbHeadOffice.IsChecked);

                            db.MASTERBANKBRANCHes.Add(mb);
                            db.SaveChanges();
                            AppLib.lstMASTERBANKBRANCH = db.MASTERBANKBRANCHes.OrderBy(x => x.BANKBRANCH_NAME).ToList();

                            var NewData = new JSonHelper().ConvertObjectToJSon(mb);
                            AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "MASTERBANKBRANCH");

                            MessageBox.Show("Saved Successfully!", "Saved");
                            ClearForm();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Reports.frmBranchReport frm = new Reports.frmBranchReport();
            frm.ShowDialog();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (sFormName == "HOME")
            {
                frmHomeMaster frm = new frmHomeMaster();
                this.Close();
                frm.ShowDialog();
            }
            else
            {
                this.Close();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ID != 0)
                {
                    if (MessageBox.Show("Do you want to Delete this '" + txtBranchName.Text + "'", "DELETE", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        MASTERBANKBRANCH mb = db.MASTERBANKBRANCHes.Where(x => x.BANKBRANCH_CODE == ID).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(mb);

                        db.MASTERBANKBRANCHes.Remove(mb);
                        db.SaveChanges();

                        AppLib.EventHistory(this.Tag.ToString(), 2, OldData, "", "MASTERBANKBRANCH");
                        MessageBox.Show("Deleted Successfully!", "DELETED");
                        ClearForm();
                    }
                }
                else
                {
                    MessageBox.Show("Please Select Any Bank! (Double Click to Select)");
                }
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void dgvBranch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (bIsEdit == true)
                {
                    DataRowView drv = (DataRowView)dgvBranch.SelectedItem;
                    ID = Convert.ToInt32(drv["BANKBRANCH_CODE"]);
                    txtAddress1.Text = drv["BANKBRANCH_ADDRESS1"].ToString();
                    txtAddress2.Text = drv["BANKBRANCH_ADDRESS2"].ToString();
                    txtAddress3.Text = drv["BANKBRANCH_ADDRESS3"].ToString();
                    txtBranchCode.Text = drv["BANKBRANCH_USERCODE"].ToString();
                    txtBranchName.Text = drv["BANKBRANCH_NAME"].ToString();
                    txtEmail.Text = drv["BANKBRANCH_EMAIL"].ToString();
                    txtMobile.Text = drv["BANKBRANCH_PHONE1"].ToString();
                    txtPhone.Text = drv["BANKBRANCH_PHONE2"].ToString();
                    cmbBankName.SelectedValue = Convert.ToInt32(drv["BANK_CODE"]);
                    cmbCity.SelectedValue = Convert.ToInt32(drv["BANKBRANCH_CITY_CODE"]);
                    cmbCountry.Text = drv["BANKBRANCH_COUNTRY"].ToString();
                    cmbNUBEBranch.SelectedValue = Convert.ToInt32(drv["NUBE_BRANCH_CODE"]);
                    cmbState.SelectedValue = Convert.ToInt32(drv["BANKBRANCH_STATE_CODE"]);
                    ckbHeadOffice.IsChecked = Convert.ToBoolean(drv["HEAD_QUARTERS"]);
                    txtPostalCode.Text = drv["BANKBRANCH_ZIPCODE"].ToString();

                    TCBranchSetup.TabIndex = 1;
                    TCBranchSetup.SelectedIndex = 1;
                    //txtBranchName.Focus();

                }
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void txtMobile_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtMobile.Text.Length == 14)
                {
                    txtMobile.Text = NoFormate.sMobileNo(txtMobile.Text);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void txtPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtPhone.Text.Length == 11)
                {
                    txtPhone.Text = NoFormate.sPhoneNo(txtPhone.Text);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void cmbBrBankName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                decimal dBankCode = Convert.ToDecimal(cmbBrBankName.SelectedValue);
                if (dBankCode != 0)
                {
                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANK_CODE == dBankCode).ToList();
                    cmbBranchSrch.ItemsSource = mbr;
                    cmbBranchSrch.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranchSrch.DisplayMemberPath = "BANKBRANCH_NAME";

                    cmbUserCode.ItemsSource = mbr;
                    cmbUserCode.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbUserCode.DisplayMemberPath = "BANKBRANCH_USERCODE";

                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        dtBank.Rows.Clear();
                        if (Convert.ToInt32(cmbBrBankName.SelectedValue) != 0)
                        {
                            String ST = String.Format(" SELECT ISNULL(BR.BANKBRANCH_NAME,'')BANKBRANCH_NAME,ISNULL(BR.BANKBRANCH_CODE,0)BANKBRANCH_CODE,ISNULL(BR.BANK_CODE,0)BANK_CODE," +
                                 " ISNULL(BR.BANKBRANCH_USERCODE,0)BANKBRANCH_USERCODE,ISNULL(BK.BANK_USERCODE,0)BANK_USERCODE," +
                                 " ISNULL(BR.BANKBRANCH_ADDRESS1, '')BANKBRANCH_ADDRESS1,ISNULL(BR.BANKBRANCH_ADDRESS2, '')BANKBRANCH_ADDRESS2," +
                                 " ISNULL(BR.BANKBRANCH_ADDRESS3, '')BANKBRANCH_ADDRESS3,ISNULL(BR.BANKBRANCH_PHONE1, '')BANKBRANCH_PHONE1," +
                                 " ISNULL(BR.BANKBRANCH_PHONE2, '')BANKBRANCH_PHONE2,ISNULL(BR.BANKBRANCH_EMAIL, '')BANKBRANCH_EMAIL," +
                                 " ISNULL(BR.BANKBRANCH_CITY_CODE, 0)BANKBRANCH_CITY_CODE,ISNULL(BR.BANKBRANCH_STATE_CODE, 0)BANKBRANCH_STATE_CODE," +
                                 " ISNULL(BR.BANKBRANCH_ZIPCODE, '')BANKBRANCH_ZIPCODE,ISNULL(BR.NUBE_BRANCH_CODE, 0)NUBE_BRANCH_CODE," +
                                 " ISNULL(NB.NUBE_BRANCH_NAME,'')NUBE_BRANCH_NAME,ISNULL(BR.BANKBRANCH_COUNTRY,'')BANKBRANCH_COUNTRY,ISNULL(HEAD_QUARTERS,0)HEAD_QUARTERS,CASE WHEN ISNULL(BR.HEAD_QUARTERS,0)<>0 THEN 'YES' ELSE 'NO' END HEAD_QUARTER" +
                                 " FROM MASTERBANKBRANCH BR(NOLOCK)" +
                                 " LEFT JOIN MASTERBANK BK(NOLOCK) ON BK.BANK_CODE=BR.BANK_CODE" +
                                 " LEFT JOIN MASTERNUBEBRANCH NB(NOLOCK) ON NB.NUBE_BRANCH_CODE = BR.NUBE_BRANCH_CODE" +
                                 " WHERE BANKBRANCH_NAME<>'' AND ISNULL(BR.DELETED,0)=0 AND BR.BANK_CODE={0} ORDER BY BR.BANKBRANCH_NAME", cmbBrBankName.SelectedValue);

                            SqlCommand cmd = new SqlCommand(ST, conn);
                            SqlDataAdapter sdp = new SqlDataAdapter(cmd);
                            sdp.Fill(dtBank);
                            dgvBranch.ItemsSource = dtBank.DefaultView;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void cmbUserCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                decimal dBrnkCode = Convert.ToDecimal(cmbUserCode.SelectedValue);
                if (dBrnkCode != 0)
                {
                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANKBRANCH_CODE == dBrnkCode).FirstOrDefault();
                    cmbBranchSrch.SelectedValue = mbr.BANKBRANCH_CODE;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void cmbBranchSrch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                decimal dBrnkCode = Convert.ToDecimal(cmbBranchSrch.SelectedValue);
                if (dBrnkCode != 0)
                {
                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANKBRANCH_CODE == dBrnkCode).FirstOrDefault();
                    cmbUserCode.SelectedValue = mbr.BANKBRANCH_CODE;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnClearGd_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void dgvBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ID = 0;
                cmbBankName.Text = "";
                txtBranchCode.Clear();
                txtBranchName.Clear();
                txtEmail.Clear();
                txtMobile.Clear();
                txtPhone.Clear();
                txtPostalCode.Clear();
                txtAddress1.Clear();
                txtAddress2.Clear();
                txtAddress3.Clear();
                cmbCity.Text = "";
                cmbCountry.Text = "";
                cmbNUBEBranch.Text = "";
                cmbState.Text = "";
                cmbBrBankName.Text = "";
                cmbBranchSrch.Text = "";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion

        #region "USER DEFINED FUNCTION"

        private void LoadWindow()
        {
            try
            {
                //var bank = db.MASTERBANKs.ToList();
                cmbBankName.ItemsSource = AppLib.lstMASTERBANK;
                cmbBankName.SelectedValuePath = "BANK_CODE";
                cmbBankName.DisplayMemberPath = "BANK_NAME";

                cmbBrBankName.ItemsSource = AppLib.lstMASTERBANK;
                cmbBrBankName.SelectedValuePath = "BANK_CODE";
                cmbBrBankName.DisplayMemberPath = "BANK_NAME";

                cmbCity.ItemsSource = AppLib.lstMASTERCITY;
                cmbCity.SelectedValuePath = "CITY_CODE";
                cmbCity.DisplayMemberPath = "CITY_NAME";

                cmbState.ItemsSource = AppLib.lstMASTERSTATE;
                cmbState.SelectedValuePath = "STATE_CODE";
                cmbState.DisplayMemberPath = "STATE_NAME";

                cmbCountry.ItemsSource = AppLib.lstCountrySetup;
                cmbCountry.SelectedValuePath = "CountryName";
                cmbCountry.DisplayMemberPath = "CountryName";

                cmbNUBEBranch.ItemsSource = db.MASTERNUBEBRANCHes.OrderBy(x => x.NUBE_BRANCH_NAME).ToList();
                cmbNUBEBranch.SelectedValuePath = "NUBE_BRANCH_CODE";
                cmbNUBEBranch.DisplayMemberPath = "NUBE_BRANCH_NAME";


                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    dtBank.Rows.Clear();
                    if (!String.IsNullOrEmpty(cmbBranchSrch.Text))
                    {
                        String ST = String.Format(" SELECT ISNULL(BR.BANKBRANCH_NAME,'')BANKBRANCH_NAME,ISNULL(BR.BANKBRANCH_CODE,0)BANKBRANCH_CODE,ISNULL(BR.BANK_CODE,0)BANK_CODE," +
                             " ISNULL(BR.BANKBRANCH_USERCODE,0)BANKBRANCH_USERCODE,ISNULL(BK.BANK_USERCODE,0)BANK_USERCODE," +
                             " ISNULL(BR.BANKBRANCH_ADDRESS1, '')BANKBRANCH_ADDRESS1,ISNULL(BR.BANKBRANCH_ADDRESS2, '')BANKBRANCH_ADDRESS2," +
                             " ISNULL(BR.BANKBRANCH_ADDRESS3, '')BANKBRANCH_ADDRESS3,ISNULL(BR.BANKBRANCH_PHONE1, '')BANKBRANCH_PHONE1," +
                             " ISNULL(BR.BANKBRANCH_PHONE2, '')BANKBRANCH_PHONE2,ISNULL(BR.BANKBRANCH_EMAIL, '')BANKBRANCH_EMAIL," +
                             " ISNULL(BR.BANKBRANCH_CITY_CODE, 0)BANKBRANCH_CITY_CODE,ISNULL(BR.BANKBRANCH_STATE_CODE, 0)BANKBRANCH_STATE_CODE," +
                             " ISNULL(BR.BANKBRANCH_ZIPCODE, '')BANKBRANCH_ZIPCODE,ISNULL(BR.NUBE_BRANCH_CODE, 0)NUBE_BRANCH_CODE," +
                             " ISNULL(NB.NUBE_BRANCH_NAME,'')NUBE_BRANCH_NAME,ISNULL(BR.BANKBRANCH_COUNTRY,'')BANKBRANCH_COUNTRY,ISNULL(HEAD_QUARTERS,0)HEAD_QUARTERS,CASE WHEN ISNULL(BR.HEAD_QUARTERS,0)<>0 THEN 'YES' ELSE 'NO' END HEAD_QUARTER" +
                             " FROM MASTERBANKBRANCH BR(NOLOCK)" +
                             " LEFT JOIN MASTERBANK BK(NOLOCK) ON BK.BANK_CODE=BR.BANK_CODE" +
                             " LEFT JOIN MASTERNUBEBRANCH NB(NOLOCK) ON NB.NUBE_BRANCH_CODE = BR.NUBE_BRANCH_CODE" +
                             " WHERE BANKBRANCH_NAME<>'' AND ISNULL(BR.DELETED,0)=0 AND BR.BANKBRANCH_CODE={0} ORDER BY BR.BANKBRANCH_NAME", cmbBranchSrch.SelectedValue);

                        SqlCommand cmd = new SqlCommand(ST, conn);
                        SqlDataAdapter sdp = new SqlDataAdapter(cmd);
                        sdp.Fill(dtBank);
                        dgvBranch.ItemsSource = dtBank.DefaultView;
                    }
                    else
                    {
                        String ST = " SELECT ISNULL(BR.BANKBRANCH_NAME,'')BANKBRANCH_NAME,ISNULL(BR.BANKBRANCH_CODE,0)BANKBRANCH_CODE,ISNULL(BR.BANK_CODE,0)BANK_CODE," +
                             " ISNULL(BR.BANKBRANCH_USERCODE,0)BANKBRANCH_USERCODE,ISNULL(BK.BANK_USERCODE,0)BANK_USERCODE," +
                             " ISNULL(BR.BANKBRANCH_ADDRESS1, '')BANKBRANCH_ADDRESS1,ISNULL(BR.BANKBRANCH_ADDRESS2, '')BANKBRANCH_ADDRESS2," +
                             " ISNULL(BR.BANKBRANCH_ADDRESS3, '')BANKBRANCH_ADDRESS3,ISNULL(BR.BANKBRANCH_PHONE1, '')BANKBRANCH_PHONE1," +
                             " ISNULL(BR.BANKBRANCH_PHONE2, '')BANKBRANCH_PHONE2,ISNULL(BR.BANKBRANCH_EMAIL, '')BANKBRANCH_EMAIL," +
                             " ISNULL(BR.BANKBRANCH_CITY_CODE, 0)BANKBRANCH_CITY_CODE,ISNULL(BR.BANKBRANCH_STATE_CODE, 0)BANKBRANCH_STATE_CODE," +
                             " ISNULL(BR.BANKBRANCH_ZIPCODE, '')BANKBRANCH_ZIPCODE,ISNULL(BR.NUBE_BRANCH_CODE, 0)NUBE_BRANCH_CODE," +
                             " ISNULL(NB.NUBE_BRANCH_NAME,'')NUBE_BRANCH_NAME,ISNULL(BR.BANKBRANCH_COUNTRY,'')BANKBRANCH_COUNTRY,ISNULL(HEAD_QUARTERS,0)HEAD_QUARTERS,CASE WHEN ISNULL(BR.HEAD_QUARTERS,0)<>0 THEN 'YES' ELSE 'NO' END HEAD_QUARTER" +
                             " FROM MASTERBANKBRANCH BR(NOLOCK)" +
                             " LEFT JOIN MASTERBANK BK(NOLOCK) ON BK.BANK_CODE=BR.BANK_CODE" +
                             " LEFT JOIN MASTERNUBEBRANCH NB(NOLOCK) ON NB.NUBE_BRANCH_CODE = BR.NUBE_BRANCH_CODE" +
                             " WHERE BANKBRANCH_NAME<>'' AND ISNULL(BR.DELETED,0)=0  ORDER BY BR.BANKBRANCH_NAME";

                        SqlCommand cmd = new SqlCommand(ST, conn);
                        SqlDataAdapter sdp = new SqlDataAdapter(cmd);
                        sdp.Fill(dtBank);
                        dgvBranch.ItemsSource = dtBank.DefaultView;
                    }

                }
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void ClearForm()
        {
            ID = 0;
            cmbBankName.Text = "";
            txtBranchCode.Clear();
            txtBranchName.Clear();
            txtEmail.Clear();
            txtMobile.Clear();
            txtPhone.Clear();
            txtPostalCode.Clear();
            txtAddress1.Clear();
            txtAddress2.Clear();
            txtAddress3.Clear();
            cmbCity.Text = "";
            cmbCountry.Text = "";
            cmbNUBEBranch.Text = "";
            cmbState.Text = "";
            cmbBrBankName.Text = "";
            cmbBranchSrch.Text = "";
            cmbUserCode.Text = "";
            LoadWindow();
        }


        #endregion


    }
}
