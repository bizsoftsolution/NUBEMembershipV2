using MahApps.Metro.Controls;
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

namespace Nube
{
    /// <summary>
    /// Interaction logic for frmUserType.xaml
    /// </summary>
    /// 
    public partial class frmUserType : MetroWindow
    {
        int ID = 0;
        nubebfsEntity db = new nubebfsEntity();
        public Boolean bIsEdit = false;

        public frmUserType()
        {
            InitializeComponent();
            try
            {
                LoadWindow();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #region "BUTTON EVENTS"

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadWindow();
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
                if (txtUserTypeName.Text == "")
                {
                    MessageBox.Show("User Type Name is Empty !");
                    txtUserTypeName.Focus();
                }
                else if (MessageBox.Show("Do You want to save this Record?", "Save Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {

                    if (ID != 0)
                    {
                        decimal id = (decimal)ID;
                        UserType mb = db.UserTypes.Where(x => x.Id == id).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(mb);
                        mb.UserType1 = txtUserTypeName.Text;
                        if (chkIsAdmin.IsChecked == true)
                        {
                            mb.IsSuperAdmin = true;
                        }
                        else
                        {
                            mb.IsSuperAdmin = false;
                        }
                        db.SaveChanges();

                        var NewData = new JSonHelper().ConvertObjectToJSon(mb);
                        AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "UserType");

                        MessageBox.Show("Saved Successfully");
                        ClearForm();
                    }
                    else
                    {
                        var ut = (from x in db.UserTypes where x.UserType1.Contains(txtUserTypeName.Text.ToString()) select x).FirstOrDefault();
                        if (ut != null)
                        {
                            MessageBox.Show("This User Type Name : " + ut.UserType1 + " Already Found!");
                        }
                        else
                        {
                            UserType mb = new UserType();
                            mb.UserType1 = txtUserTypeName.Text;
                            if (chkIsAdmin.IsChecked == true)
                            {
                                mb.IsSuperAdmin = true;
                            }
                            else
                            {
                                mb.IsSuperAdmin = false;
                            }
                            db.UserTypes.Add(mb);
                            db.SaveChanges();

                            var NewData = new JSonHelper().ConvertObjectToJSon(mb);
                            AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "UserType");
                            MessageBox.Show("Saved Successfully");
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ID == 3)
                {
                    MessageBox.Show("You Can't Change This User Type! , Contact Administrator!", "User Restricted!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (ID != 0)
                {
                    if (MessageBox.Show("Do you want to Delete '" + txtUserTypeName.Text.ToString() + "'?", "Delete Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        decimal i = Convert.ToDecimal(ID);
                        UserType mb = db.UserTypes.Where(x => x.Id == ID).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(mb);

                        db.UserTypes.Remove(mb);
                        db.SaveChanges();

                        AppLib.EventHistory(this.Tag.ToString(), 2, OldData, "", "UserType");
                        MessageBox.Show("Deleted Successfully");
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
            try
            {
                ClearForm();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            frmUserPrevilage frm = new frmUserPrevilage();
            this.Close();
            frm.ShowDialog();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //if (MessageBox.Show(this, "Are you sure to exit?", "Exit Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //{
            //    e.Cancel = false;
            //}
            //else
            //{
            //    e.Cancel = true;
            //}
        }

        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                this.Close();
        }

        #endregion

        #region "OTHER EVENTS"

        private void dgvUserType_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (bIsEdit == true)
                {
                    DataRowView drv = (DataRowView)dgvUserType.SelectedItem;
                    ID = Convert.ToInt32(drv["ID"]);
                    txtUserTypeName.Text = drv["USERTYPE"].ToString();
                    chkIsAdmin.IsChecked = Convert.ToBoolean(drv["ISSUPERADMIN"]);
                }
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void dgvUserType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ID = 0;
                txtUserTypeName.Clear();
                chkIsAdmin.IsChecked = false;
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion

        #region "USER DEFINED FUNCTION"

        private void ClearForm()
        {
            try
            {
                ID = 0;
                txtUserTypeName.Clear();
                chkIsAdmin.IsChecked = false;
                LoadWindow();
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void LoadWindow()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppLib.connStr))
                {
                    string sWhere = "";
                    if (!string.IsNullOrEmpty(txtUserTypeName.Text))
                    {
                        sWhere = " WHERE USERTYPE LIKE '%" + txtUserTypeName.Text.ToString() + "%'";
                    }

                    DataTable dt = new DataTable();
                    string st = " SELECT ID,USERTYPE,CASE WHEN ISSUPERADMIN<>0 THEN 'TRUE' ELSE 'FALSE' END ISSUPERADMIN  FROM USERTYPE(NOLOCK) " + sWhere +
                                " ORDER BY USERTYPE ";
                    SqlCommand cmd = new SqlCommand(st, con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                    dgvUserType.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion

    }
}
