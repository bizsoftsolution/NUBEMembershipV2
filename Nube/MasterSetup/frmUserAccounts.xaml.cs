using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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

namespace Nube.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmUserAccounts.xaml
    /// </summary>
    public partial class frmUserAccounts : MetroWindow
    {
        public Boolean bIsEdit = false;
        nubebfsEntity db = new nubebfsEntity();
        int iID = 0;
        public frmUserAccounts()
        {
            InitializeComponent();
            try
            {
                var coun = db.UserTypes.ToList();
                cmbUserType.ItemsSource = coun;
                cmbUserType.DisplayMemberPath = "UserType1";
                cmbUserType.SelectedValuePath = "Id";
                btnUserRights.IsEnabled = false;
                LoadWindow();
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnUserRights_Click(object sender, RoutedEventArgs e)
        {
            frmUserRights frm = new frmUserRights(Convert.ToInt32(cmbUserType.SelectedValue));
            this.Hide();
            frm.ShowDialog();
            this.Show();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtUserName.Text))
                {
                    MessageBox.Show("User Name is Empty!");
                    txtUserName.Focus();
                    return;
                }
                else if (string.IsNullOrEmpty(txtPassword.Password.ToString()))
                {
                    MessageBox.Show("Password is Empty!");
                    txtPassword.Focus();
                    return;
                }
                else if (string.IsNullOrEmpty(cmbUserType.Text))
                {
                    MessageBox.Show("User Type is Empty!");
                    cmbUserType.Focus();
                    return;
                }
                else
                {
                    if (MessageBox.Show("Do you want to Save '" + txtUserName.Text + "'?", "SAVE", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (iID == 0)
                        {
                            UserAccount usc = (from x in db.UserAccounts where x.UserName == txtUserName.Text select x).FirstOrDefault();
                            if (usc == null)
                            {
                                UserAccount us = new UserAccount()
                                {
                                    UserName = txtUserName.Text,
                                    UserType = Convert.ToInt32(cmbUserType.SelectedValue),
                                    Password = txtPassword.Password,
                                    CompanyName = txtCompanyName.Text,
                                    UserCode = txtUserCode.Text
                                };
                                db.UserAccounts.Add(us);
                                db.SaveChanges();

                                var NewData = new JSonHelper().ConvertObjectToJSon(us);
                                AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "UserAccount");
                                MessageBox.Show("User Account Added Sucessfully!");
                                fClear();
                            }
                            else
                            {
                                MessageBox.Show("User Name Already Found!");
                                txtUserName.Focus();
                                return;
                            }
                        }
                        else if (iID == 1)
                        {                            
                            MessageBox.Show("You Can't Change This! , Contact Administrator!", "User Restricted!", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            UserAccount usc = (from x in db.UserAccounts where x.UserId == iID select x).FirstOrDefault();
                            var OldData = new JSonHelper().ConvertObjectToJSon(usc);

                            usc.UserName = txtUserName.Text;
                            usc.UserType = Convert.ToInt32(cmbUserType.SelectedValue);
                            usc.Password = txtPassword.Password;
                            usc.CompanyName = txtCompanyName.Text;
                            usc.UserCode = txtUserCode.Text;
                            db.SaveChanges();

                            var NewData = new JSonHelper().ConvertObjectToJSon(usc);
                            AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "UserAccount");
                            MessageBox.Show("User Account Updated Successfully!");
                            fClear();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (iID == 1)
                {
                    MessageBox.Show("You Can't Delete This User! , Contact Administrator!", "User Restricted!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (iID != 0)
                {
                    if (MessageBox.Show("Do you want to delete '" + txtUserName.Text + "'", "DELETE", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        UserAccount usc = (from x in db.UserAccounts where x.UserId == iID select x).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(usc);

                        db.UserAccounts.Remove(usc);
                        db.SaveChanges();

                        AppLib.EventHistory(this.Tag.ToString(), 2, OldData, "", "UserAccount");
                        MessageBox.Show("Deleted Successfully", "DELETED", MessageBoxButton.OK, MessageBoxImage.Information);
                        fClear();
                    }
                }
                else
                {
                    MessageBox.Show("Please Select Any User! (Double Click to Select)", "SELECT", MessageBoxButton.OK, MessageBoxImage.Information);
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
                fClear();
            }
            catch (Exception ex)
            {

                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            frmUserPrevilage frm = new frmUserPrevilage();
            this.Close();
            frm.ShowDialog();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void dgvState_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (bIsEdit == true)
                {
                    if (dgvUserAccounts.SelectedItem != null)
                    {
                        DataRowView drv = (DataRowView)dgvUserAccounts.SelectedItem;
                        iID = Convert.ToInt32(drv["UserId"]);
                        txtUserName.Text = drv["UserName"].ToString();
                        txtPassword.Password = drv["Password"].ToString();
                        txtCompanyName.Text = drv["CompanyName"].ToString();
                        txtUserCode.Text = drv["UserCode"].ToString();
                        cmbUserType.SelectedValue = Convert.ToInt32(drv["Id"]);
                        btnUserRights.IsEnabled = true;
                    }
                }
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
                DataTable dtUserAccounts = new DataTable();
                if (txtUserName.Text != "")
                {
                    var User = (from uc in db.UserAccounts
                                join ut in db.UserTypes on uc.UserType equals ut.Id
                                where uc.UserName == txtUserName.Text.ToString()
                                select new
                                {
                                    uc.UserId,
                                    uc.UserName,
                                    uc.Password,
                                    uc.CompanyName,
                                    uc.UserCode,
                                    ut.Id,
                                    UserType = ut.UserType1
                                }).ToList();
                    dtUserAccounts = AppLib.LINQResultToDataTable(User);
                }
                else
                {
                    var User = (from uc in db.UserAccounts
                                join ut in db.UserTypes on uc.UserType equals ut.Id
                                select new
                                {
                                    uc.UserId,
                                    uc.UserName,
                                    uc.Password,
                                    uc.CompanyName,
                                    uc.UserCode,
                                    ut.Id,
                                    UserType = ut.UserType1
                                }).ToList();
                    dtUserAccounts = AppLib.LINQResultToDataTable(User);
                }
                dgvUserAccounts.ItemsSource = dtUserAccounts.DefaultView;
            }
            catch (Exception ex)
            {

                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        void fClear()
        {
            try
            {
                iID = 0;
                txtUserName.Text = "";
                txtPassword.Password = "";
                txtCompanyName.Text = "";
                txtUserCode.Text = "";
                cmbUserType.Text = "";
                var coun = db.UserTypes.ToList();
                cmbUserType.ItemsSource = coun;
                cmbUserType.DisplayMemberPath = "UserType1";
                cmbUserType.SelectedValuePath = "Id";
                btnUserRights.IsEnabled = false;
                LoadWindow();
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }

        }     

        private void cmbUserType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Convert.ToInt32(cmbUserType.SelectedValue) != 0)
            {
                btnUserRights.IsEnabled = true;
            }
            else
            {
                btnUserRights.IsEnabled = false;
            }
        }

        private void dgvUserAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                iID = 0;
                txtUserName.Text = "";
                txtPassword.Password = "";
                txtCompanyName.Text = "";
                txtUserCode.Text = "";
                cmbUserType.Text = "";
                btnUserRights.IsEnabled = false;
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }
    }
}
