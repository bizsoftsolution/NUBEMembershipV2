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
using Nube;
using System.Data;
using System.Reflection;

namespace Nube.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmReasonSetup.xaml
    /// </summary>
    public partial class frmReasonSetup : MetroWindow
    {
        public Boolean bIsEdit = false;
        nubebfsEntity db = new nubebfsEntity();
        int ID = 0;
        string sFormName = "";
        public frmReasonSetup(string sForm_Name = "")
        {
            InitializeComponent();
            sFormName = sForm_Name;
            if (sForm_Name == "HOME")
            {
                btnPrint.Visibility = Visibility.Visible;
            }
            else
            {
                btnPrint.Visibility = Visibility.Hidden;
            }
            txtMinimumYear.Visibility = Visibility.Hidden;
            txtMinimumRefund.Visibility = Visibility.Hidden;
            txtMAximumRefund.Visibility = Visibility.Hidden;
            txtAmnt1.Visibility = Visibility.Hidden;
            txtAmnt2.Visibility = Visibility.Hidden;

            LoadWindow();

            this.KeyDown += new System.Windows.Input.KeyEventHandler(Window_KeyDown);
        }

        //closing events
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
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

        //Button events
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Reports.frmReasonReport frm = new Reports.frmReasonReport();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
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
                if (txtReasonName.Text == "")
                {
                    MessageBox.Show("Enter Reason...", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtReasonName.Focus();
                }
                else if (chkIsBefitValid.IsChecked == true && string.IsNullOrEmpty(txtMinimumYear.Text))
                {
                    MessageBox.Show("Enter Minimum Year...", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtMinimumYear.Focus();
                }
                else if (chkIsBefitValid.IsChecked == true && string.IsNullOrEmpty(txtMinimumRefund.Text))
                {
                    MessageBox.Show("Enter Minimum Refund...", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtMinimumRefund.Focus();
                }
                else if (chkIsBefitValid.IsChecked == true && string.IsNullOrEmpty(txtMAximumRefund.Text))
                {
                    MessageBox.Show("Enter Maximum Refund...", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtMAximumRefund.Focus();
                }
                else if (chkIsBefitValid.IsChecked == true && string.IsNullOrEmpty(txtAmnt1.Text))
                {
                    MessageBox.Show("Enter Amount1...", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtAmnt1.Focus();
                }
                else if (chkIsBefitValid.IsChecked == true && string.IsNullOrEmpty(txtAmnt2.Text))
                {
                    MessageBox.Show("Enter Amount2...", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtAmnt2.Focus();
                }
                else
                {
                    if (MessageBox.Show("Do you want to save this record?", "SAVE CONFIRMATION", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (ID != 0)
                        {
                            MASTERRESIGNSTATU ms = db.MASTERRESIGNSTATUS.Where(x => x.RESIGNSTATUS_CODE == ID).FirstOrDefault();
                            var OldData = new JSonHelper().ConvertObjectToJSon(ms);

                            ms.RESIGNSTATUS_NAME = txtReasonName.Text;
                            if (chkIsBefitValid.IsChecked == true)
                            {
                                ms.IsBenefitValid = Convert.ToBoolean(chkIsBefitValid.IsChecked);
                                ms.MinimumYear = Convert.ToInt32(txtMinimumYear.Text);
                                ms.MinimumRefund = Convert.ToInt32(txtMinimumRefund.Text);
                                ms.MaximumRefund = Convert.ToInt32(txtMAximumRefund.Text);
                                ms.AmtPerYear1 = Convert.ToInt32(txtAmnt1.Text);
                                ms.AmtPerYear2 = Convert.ToInt32(txtAmnt2.Text);
                            }

                            db.SaveChanges();

                            var NewData = new JSonHelper().ConvertObjectToJSon(ms);
                            AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "MASTERRESIGNSTATUS");
                            MessageBox.Show("Saved Successfully!", "SAVED", MessageBoxButton.OK, MessageBoxImage.Information);
                            FormClear();
                        }
                        else
                        {
                            if (db.MASTERRESIGNSTATUS.Where(x => x.RESIGNSTATUS_NAME == txtReasonName.Text).ToString() == txtReasonName.Text.ToString())
                            {
                                MessageBox.Show("'" + txtReasonName.Text + "' Already exist.Enter new relation.", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                                txtReasonName.Focus();
                            }
                            else
                            {
                                MASTERRESIGNSTATU ms = new MASTERRESIGNSTATU();
                                ms.RESIGNSTATUS_NAME = txtReasonName.Text;
                                if (chkIsBefitValid.IsChecked == true)
                                {
                                    ms.IsBenefitValid = Convert.ToBoolean(chkIsBefitValid.IsChecked);
                                    ms.MinimumYear = Convert.ToInt32(txtMinimumYear.Text);
                                    ms.MinimumRefund = Convert.ToInt32(txtMinimumRefund.Text);
                                    ms.MaximumRefund = Convert.ToInt32(txtMAximumRefund.Text);
                                    ms.AmtPerYear1 = Convert.ToInt32(txtAmnt1.Text);
                                    ms.AmtPerYear2 = Convert.ToInt32(txtAmnt2.Text);
                                }

                                db.MASTERRESIGNSTATUS.Add(ms);
                                db.SaveChanges();

                                var NewData = new JSonHelper().ConvertObjectToJSon(ms);
                                AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "MASTERRESIGNSTATUS");
                                MessageBox.Show("Saved Successfully!", "SAVED", MessageBoxButton.OK, MessageBoxImage.Information);
                                FormClear();
                            }
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
                if (ID != 0)
                {
                    if (MessageBox.Show("Do you want to delete '" + txtReasonName.Text + "'", "DELETE", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        MASTERRESIGNSTATU ms = db.MASTERRESIGNSTATUS.Where(x => x.RESIGNSTATUS_CODE == ID).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(ms);

                        db.MASTERRESIGNSTATUS.Remove(ms);
                        db.SaveChanges();

                        AppLib.EventHistory(this.Tag.ToString(), 2, OldData, "", "MASTERRESIGNSTATUS");
                        MessageBox.Show("Deleted Successfully!", "DELETED", MessageBoxButton.OK, MessageBoxImage.Information);
                        FormClear();
                    }
                }
                else
                {
                    MessageBox.Show("Please Select any Reason! (Double Click to Select)", "SELECT", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            FormClear();
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

        private void txtMinimumYear_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtMinimumRefund_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void txtMAximumRefund_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        //Userdefined 

        private void LoadWindow()
        {
            try
            {
                if (txtReasonName.Text != "")
                {
                    var rg = (from x in db.MASTERRESIGNSTATUS
                              where x.RESIGNSTATUS_NAME.Contains(txtReasonName.Text.ToString())
                              orderby x.RESIGNSTATUS_NAME
                              select new
                              {
                                  x.RESIGNSTATUS_CODE,
                                  x.RESIGNSTATUS_NAME,
                                  IsBenefitValid = (x.IsBenefitValid == true ? "YES" : "NO"),
                                  x.MinimumYear,
                                  x.MinimumRefund,
                                  x.MaximumRefund,
                                  x.AmtPerYear1,
                                  x.AmtPerYear2
                              }).ToList();
                    DataTable dtReason = AppLib.LINQResultToDataTable(rg);
                    dgvReason.ItemsSource = dtReason.DefaultView;
                    txtReasonName.Text = dtReason.Rows[0]["RESIGNSTATUS_NAME"].ToString();
                    ID = Convert.ToInt32(dtReason.Rows[0]["RESIGNSTATUS_CODE"]);

                    chkIsBefitValid.IsChecked = (dtReason.Rows[0]["IsBenefitValid"].ToString() == "YES" ? true : false);
                    if (chkIsBefitValid.IsChecked == true)
                    {
                        txtMinimumYear.Text = dtReason.Rows[0]["MinimumYear"].ToString();
                        txtMinimumRefund.Text = dtReason.Rows[0]["MinimumRefund"].ToString();
                        txtMAximumRefund.Text = dtReason.Rows[0]["MaximumRefund"].ToString();
                        txtAmnt1.Text = dtReason.Rows[0]["AmtPerYear1"].ToString();
                        txtAmnt2.Text = dtReason.Rows[0]["AmtPerYear2"].ToString();
                        txtMinimumYear.Visibility = Visibility.Visible;
                        txtMinimumRefund.Visibility = Visibility.Visible;
                        txtMAximumRefund.Visibility = Visibility.Visible;
                        txtAmnt1.Visibility = Visibility.Visible;
                        txtAmnt2.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        txtMinimumYear.Clear();
                        txtMinimumRefund.Clear();
                        txtMAximumRefund.Clear();
                        txtMinimumYear.Visibility = Visibility.Hidden;
                        txtMinimumRefund.Visibility = Visibility.Hidden;
                        txtMAximumRefund.Visibility = Visibility.Hidden;
                        txtAmnt1.Visibility = Visibility.Hidden;
                        txtAmnt2.Visibility = Visibility.Hidden;
                    }

                }
                else
                {
                    var rg = (from x in db.MASTERRESIGNSTATUS
                              orderby x.RESIGNSTATUS_NAME
                              select new
                              {
                                  x.RESIGNSTATUS_CODE,
                                  x.RESIGNSTATUS_NAME,
                                  IsBenefitValid = (x.IsBenefitValid == true ? "YES" : "NO"),
                                  x.MinimumYear,
                                  x.MinimumRefund,
                                  x.MaximumRefund,
                                  x.AmtPerYear1,
                                  x.AmtPerYear2
                              }).ToList();
                    dgvReason.ItemsSource = AppLib.LINQResultToDataTable(rg).DefaultView;
                    txtMinimumYear.Clear();
                    txtMinimumRefund.Clear();
                    txtMAximumRefund.Clear();
                    chkIsBefitValid.IsChecked = false;
                    txtMinimumYear.Visibility = Visibility.Hidden;
                    txtMinimumRefund.Visibility = Visibility.Hidden;
                    txtMAximumRefund.Visibility = Visibility.Hidden;
                    txtAmnt1.Visibility = Visibility.Hidden;
                    txtAmnt2.Visibility = Visibility.Hidden;
                }

            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }

        }

        private void FormClear()
        {
            try
            {
                ID = 0;
                txtReasonName.Clear();
                LoadWindow();
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void dgvReason_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (bIsEdit == true)
                {
                    DataRowView drv = (DataRowView)dgvReason.SelectedItem;
                    txtReasonName.Text = drv["RESIGNSTATUS_NAME"].ToString();
                    ID = Convert.ToInt32(drv["RESIGNSTATUS_CODE"]);

                    chkIsBefitValid.IsChecked = (drv["IsBenefitValid"].ToString() == "YES" ? true : false);
                    if (chkIsBefitValid.IsChecked == true)
                    {
                        txtMinimumYear.Text = drv["MinimumYear"].ToString();
                        txtMinimumRefund.Text = drv["MinimumRefund"].ToString();
                        txtMAximumRefund.Text = drv["MaximumRefund"].ToString();
                        txtAmnt1.Text = drv["AmtPerYear1"].ToString();
                        txtAmnt2.Text = drv["AmtPerYear2"].ToString();
                        txtMinimumYear.Visibility = Visibility.Visible;
                        txtMinimumRefund.Visibility = Visibility.Visible;
                        txtMAximumRefund.Visibility = Visibility.Visible;
                        txtAmnt1.Visibility = Visibility.Visible;
                        txtAmnt2.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        txtMinimumYear.Clear();
                        txtMinimumRefund.Clear();
                        txtMAximumRefund.Clear();
                        txtMinimumYear.Visibility = Visibility.Hidden;
                        txtMinimumRefund.Visibility = Visibility.Hidden;
                        txtMAximumRefund.Visibility = Visibility.Hidden;
                        txtAmnt1.Visibility = Visibility.Hidden;
                        txtAmnt2.Visibility = Visibility.Hidden;
                    }

                }
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void dgvReason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ID = 0;
                txtReasonName.Clear();
                txtMinimumYear.Clear();
                txtMinimumRefund.Clear();
                txtMAximumRefund.Clear();
                txtAmnt1.Clear();
                txtAmnt2.Clear();
                chkIsBefitValid.IsChecked = false;
                txtMinimumYear.Visibility = Visibility.Hidden;
                txtMinimumRefund.Visibility = Visibility.Hidden;
                txtMAximumRefund.Visibility = Visibility.Hidden;
                txtAmnt1.Visibility = Visibility.Hidden;
                txtAmnt2.Visibility = Visibility.Hidden;

            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void chkIsBefitValid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chkIsBefitValid.IsChecked == true)
                {
                    txtMinimumYear.Visibility = Visibility.Visible;
                    txtMinimumRefund.Visibility = Visibility.Visible;
                    txtMAximumRefund.Visibility = Visibility.Visible;
                    txtAmnt1.Visibility = Visibility.Visible;
                    txtAmnt2.Visibility = Visibility.Visible;
                }
                else
                {
                    txtMinimumYear.Visibility = Visibility.Hidden;
                    txtMinimumRefund.Visibility = Visibility.Hidden;
                    txtMAximumRefund.Visibility = Visibility.Hidden;
                    txtAmnt1.Visibility = Visibility.Hidden;
                    txtAmnt2.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }
      
    }
}
