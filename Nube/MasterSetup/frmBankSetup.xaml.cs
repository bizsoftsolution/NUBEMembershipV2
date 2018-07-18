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
using System.Collections.ObjectModel;

namespace Nube.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmBankSetup.xaml
    /// </summary>
    public partial class frmBankSetup : MetroWindow
    {
        public Boolean bIsEdit = false;
        int ID = 0;
        nubebfsEntity db = new nubebfsEntity();
        DataTable dtBank = new DataTable();
        string sFormName = "";

        public frmBankSetup(string sForm_Name = "")
        {
            InitializeComponent();
            sFormName = sForm_Name;
            try
            {
                if (sForm_Name == "HOME")
                {
                    btnPrint.Visibility = Visibility.Visible;
                }
                else
                {
                    btnPrint.Visibility = Visibility.Hidden;
                }
                var nube = db.MASTERNUBEBRANCHes.ToList();
                cmbNubeBranch.Visibility = Visibility.Hidden;
                //cmbNubeBranch.ItemsSource = nube;
                //cmbNubeBranch.DisplayMemberPath = "NUBE_BRANCH_NAME";
                //cmbNubeBranch.SelectedValuePath = "NUBE_BRANCH_CODE";


                LoadWindow();
                this.KeyDown += new System.Windows.Input.KeyEventHandler(Window_KeyDown);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #region "BUTTON EVENTS"

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtBankName.Text == "")
                {
                    MessageBox.Show("Enter Bank Name!");
                    txtBankName.Focus();
                }
                else if (txtBankUserCode.Text == "")
                {
                    MessageBox.Show("Enter User Code!");
                    txtBankUserCode.Focus();
                }
                //else if (cmbNubeBranch.Text == "")
                //{
                //    MessageBox.Show("Enter Branch Name!");
                //    cmbNubeBranch.Focus();
                //}
                else if (MessageBox.Show("Do You want to save this Record?", "Save Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (ID != 0)
                    {
                        decimal id = (decimal)ID;
                        MASTERBANK mb = db.MASTERBANKs.Where(x => x.BANK_CODE == id).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(mb);

                        mb.BANK_NAME = txtBankName.Text;
                        mb.BANK_USERCODE = txtBankUserCode.Text;
                        //mb.NUBE_BRANCH = 2;

                        db.SaveChanges();
                        AppLib.lstMASTERBANK = db.MASTERBANKs.OrderBy(x => x.BANK_NAME).ToList();
                        var NewData = new JSonHelper().ConvertObjectToJSon(mb);

                        AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "MASTERBANK");
                        MessageBox.Show("Saved Successfully");                        
                        ClearForm();
                    }
                    else
                    {
                        MASTERBANK mb = new MASTERBANK();
                        mb.BANK_NAME = txtBankName.Text;
                        mb.BANK_USERCODE = txtBankUserCode.Text;
                        //mb.NUBE_BRANCH = 2;
                        db.MASTERBANKs.Add(mb);
                        db.SaveChanges();
                        AppLib.lstMASTERBANK = db.MASTERBANKs.OrderBy(x => x.BANK_NAME).ToList();

                        var NewData = new JSonHelper().ConvertObjectToJSon(mb);
                        AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "MASTERBANK");
                        MessageBox.Show("Saved Successfully");                        
                        ClearForm();
                    }
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
            LoadWindow();
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtBankName.Text != "")
                {
                    LoadWindow();
                }
                else
                {
                    MessageBox.Show("Enter Bank Name!..", "Value Missing");
                }
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ID != 0)
                {
                    var s = txtBankName.Text;
                    if (MessageBox.Show("Do you want to Delete '" + s + "'?", "Delete Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        decimal i = Convert.ToDecimal(ID);
                        MASTERBANK mb = db.MASTERBANKs.Where(x => x.BANK_CODE == ID).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(mb);
                        db.MASTERBANKs.Remove(mb);
                        db.SaveChanges();

                        AppLib.EventHistory(this.Tag.ToString(), 2, OldData, "", "MASTERBANK");
                        MessageBox.Show("Deleted Successfully");
                        ClearForm();
                        LoadWindow();
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

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Reports.frmBankReport frm = new Reports.frmBankReport();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void dgvBank_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (bIsEdit == true)
                {
                    DataRowView drv = (DataRowView)dgvBank.SelectedItem;
                    ID = Convert.ToInt32(drv["BANK_CODE"]);
                    txtBankName.Text = drv["BANK_NAME"].ToString();
                    txtBankUserCode.Text = drv["BANK_USERCODE"].ToString();
                    //cmbNubeBranch.Text = drv["NUBE_BRANCH_NAME"].ToString();
                }
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }

        }

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

        private void dgvBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ID = 0;
                txtBankName.Clear();
                txtBankUserCode.Clear();
                cmbNubeBranch.Text = "";
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }
        #endregion

        #region "USER DEFINED FUNCTION"

        private void LoadWindow()
        {
            try
            {
                string conn = AppLib.connStr;
                using (SqlConnection con = new SqlConnection(conn))
                {
                    if (txtBankName.Text != "")
                    {
                        DataTable dtBank = new DataTable();
                        string st = string.Format("SELECT BN.BANK_CODE,BN.BANK_NAME,BN.BANK_USERCODE" +
                                    " FROM MASTERBANK BN(NOLOCK)" +
                                    " WHERE ISNULL(BN.BANK_NAME,'')<>'' AND BN.BANK_NAME LIKE '%{0}%'" +
                                    " ORDER BY BN.BANK_NAME", txtBankName.Text);
                        SqlCommand cmd = new SqlCommand(st, con);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        adp.Fill(dtBank);
                        dgvBank.ItemsSource = dtBank.DefaultView;                        
                    }
                    else
                    {
                        DataTable dtBank = new DataTable();
                        string st = " SELECT BN.BANK_CODE,BN.BANK_NAME,BN.BANK_USERCODE" +
                                    " FROM MASTERBANK BN(NOLOCK)" +
                                    " WHERE ISNULL(BN.BANK_NAME,'')<>'' ORDER BY BN.BANK_NAME";
                        SqlCommand cmd = new SqlCommand(st, con);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        adp.Fill(dtBank);
                        dgvBank.ItemsSource = dtBank.DefaultView;
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
            try
            {
                ID = 0;
                txtBankName.Clear();
                txtBankUserCode.Clear();
                cmbNubeBranch.Text = "";
                LoadWindow();
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }
        #endregion

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
