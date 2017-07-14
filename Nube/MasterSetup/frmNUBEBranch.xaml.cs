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
using MahApps.Metro.Controls;
using Nube;

namespace Nube.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmNUBEBranch.xaml
    /// </summary>
    public partial class frmNUBEBranch : MetroWindow
    {
        public Boolean bIsEdit = false;
        nubebfsEntity db = new nubebfsEntity();
        int ID = 0;
        string sFormName = "";

        public frmNUBEBranch(string sForm_Name = "")
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
            LoadWindow();
            this.KeyDown += new System.Windows.Input.KeyEventHandler(Window_Keydown);
        }

        private void Window_Keydown(object sender, KeyEventArgs e)
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
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtNubeBranch.Text == "")
                {
                    MessageBox.Show("Enter NubeBranch...", "Information");
                }

                else
                {
                    if (MessageBox.Show("Do you wanrt to save this record?", "SAVE CONFIRMATION", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (ID != 0)
                        {
                            MASTERNUBEBRANCH ms = db.MASTERNUBEBRANCHes.Where(x => x.NUBE_BRANCH_CODE == ID).FirstOrDefault();
                            var OldData = new JSonHelper().ConvertObjectToJSon(ms);

                            ms.NUBE_BRANCH_NAME = txtNubeBranch.Text;
                            ms.NUBE_BRANCH_USERCODE = txtNUBECODE.Text;
                            db.SaveChanges();

                            var NewData = new JSonHelper().ConvertObjectToJSon(ms);
                            AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "MASTERNUBEBRANCH");

                            MessageBox.Show("Saved Successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                            FormClear();

                        }
                        else
                        {
                            if (db.MASTERNUBEBRANCHes.Where(x => x.NUBE_BRANCH_NAME == txtNubeBranch.Text).Select(x => x.NUBE_BRANCH_NAME).FirstOrDefault() == txtNubeBranch.Text.ToString())
                            {
                                MessageBox.Show("'" + txtNubeBranch.Text + "' already Name exist! Enter new  Name...!", "Information");
                            }
                            else
                            {
                                MASTERNUBEBRANCH ms = new MASTERNUBEBRANCH();
                                ms.NUBE_BRANCH_NAME = txtNubeBranch.Text;
                                ms.NUBE_BRANCH_USERCODE = txtNUBECODE.Text;
                                db.MASTERNUBEBRANCHes.Add(ms);
                                db.SaveChanges();

                                var NewData = new JSonHelper().ConvertObjectToJSon(ms);
                                AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "MASTERNUBEBRANCH");
                                MessageBox.Show("Saved Successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    if (MessageBox.Show("Do you want to Delete this '" + txtNubeBranch.Text + "'", "DELETE", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        MASTERNUBEBRANCH ms = db.MASTERNUBEBRANCHes.Where(x => x.NUBE_BRANCH_CODE == ID).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(ms);

                        db.MASTERNUBEBRANCHes.Remove(ms);
                        db.SaveChanges();

                        AppLib.EventHistory(this.Tag.ToString(), 2, OldData, "", "MASTERNUBEBRANCH");
                        MessageBox.Show("Deleted Successfully!", "DELETED", MessageBoxButton.OK, MessageBoxImage.Information);
                        FormClear();
                    }
                }
                else
                {
                    MessageBox.Show("Please Select Any NUBE Branch! (Double Click to Select)");
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

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Reports.frmNUBEBranchReport frm = new Reports.frmNUBEBranchReport();
            frm.ShowDialog();
        }

        private void dgvNUBEBranch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (bIsEdit == true)
                {
                    MASTERNUBEBRANCH r = dgvNUBEBranch.SelectedItem as MASTERNUBEBRANCH;
                    txtNubeBranch.Text = r.NUBE_BRANCH_NAME;
                    txtNUBECODE.Text = r.NUBE_BRANCH_USERCODE;
                    ID = Convert.ToInt16(r.NUBE_BRANCH_CODE);
                }
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        //User defined 

        private void LoadWindow()
        {
            try
            {
                if (txtNubeBranch.Text != "")
                {
                    dgvNUBEBranch.ItemsSource = db.MASTERNUBEBRANCHes.Where(x => x.NUBE_BRANCH_NAME.Contains(txtNubeBranch.Text.ToString())).OrderBy(x => x.NUBE_BRANCH_NAME).ToList();
                }
                else
                {
                    dgvNUBEBranch.ItemsSource = db.MASTERNUBEBRANCHes.OrderBy(x => x.NUBE_BRANCH_NAME).ToList();
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
                txtNubeBranch.Clear();
                txtNUBECODE.Clear();
                LoadWindow();
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void dgvNUBEBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ID = 0;
                txtNubeBranch.Clear();
                txtNUBECODE.Clear();
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }
    }
}
