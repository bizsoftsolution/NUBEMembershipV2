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
    /// Interaction logic for frmSalutationSetup.xaml
    /// </summary>
    public partial class frmSalutationSetup : MetroWindow
    {
        public Boolean bIsEdit = false;
        nubebfsEntity db = new nubebfsEntity();
        int ID = 0;
        string sFormName = "";

        public frmSalutationSetup(string sForm_Name = "")
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

        //Closing Events
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

        //Button Events
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtName.Text == "")
                {
                    MessageBox.Show("Enter Name...", "Information");
                }
                else
                {
                    if (MessageBox.Show("Do you want to save this record?", "SAVE CONFIRMATION", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (ID != 0)
                        {
                            SalutationSetup c = db.SalutationSetups.Where(x => x.Id == ID).FirstOrDefault();
                            var OldData = new JSonHelper().ConvertObjectToJSon(c);

                            c.Salutation = txtName.Text;
                            db.SaveChanges();

                            var NewData = new JSonHelper().ConvertObjectToJSon(c);
                            AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "SalutationSetup");
                            MessageBox.Show("Saved Successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                            FormClear();

                        }
                        else
                        {
                            if (db.SalutationSetups.Where(x => x.Salutation == txtName.Text).Select(x => x.Salutation).FirstOrDefault() == txtName.Text.ToString())
                            {
                                MessageBox.Show("'" + txtName.Text + "' already exist! Enter new  Country...", "Information");
                            }
                            else
                            {
                                SalutationSetup c = new SalutationSetup();
                                c.Salutation = txtName.Text;
                                db.SalutationSetups.Add(c);
                                db.SaveChanges();

                                var NewData = new JSonHelper().ConvertObjectToJSon(c);
                                AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "SalutationSetup");
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
                    if (MessageBox.Show("Do you want to delete '" + txtName.Text + "'", "DELETE", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        SalutationSetup c = db.SalutationSetups.Where(x => x.Id == ID).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(c);

                        db.SalutationSetups.Remove(c);
                        db.SaveChanges();

                        AppLib.EventHistory(this.Tag.ToString(), 2, OldData, "", "SalutationSetup");
                        MessageBox.Show("Deleted Successfully", "DELETED", MessageBoxButton.OK, MessageBoxImage.Information);
                        FormClear();
                    }
                }
                else
                {
                    MessageBox.Show("Please Select Any Salutation! (Double Click to Select)", "SELECT", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
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

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Reports.frmSalutationReport frm = new Reports.frmSalutationReport();
                frm.ShowDialog();
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

        private void dgvTitle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (bIsEdit == true)
                {
                    SalutationSetup c = dgvTitle.SelectedItem as SalutationSetup;
                    ID = c.Id;
                    txtName.Text = c.Salutation;
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
                if (!string.IsNullOrEmpty(txtName.Text))
                {
                    dgvTitle.ItemsSource = db.SalutationSetups.Where(x => x.Salutation.Contains(txtName.Text.ToString())).ToList();
                }
                else
                {
                    dgvTitle.ItemsSource = db.SalutationSetups.OrderBy(x => x.Salutation).ToList();
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
                txtName.Clear();
                LoadWindow();
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }

        }

        private void dgvTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ID = 0;
                txtName.Clear();               
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }
    }
}
