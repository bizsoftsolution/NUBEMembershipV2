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
    /// Interaction logic for frmCountrySetup.xaml
    /// </summary>
    public partial class frmCountrySetup : MetroWindow
    {
        public Boolean bIsEdit = false;
        nubebfsEntity db = new nubebfsEntity();
        int ID = 0;
        string sFormName = "";
        public frmCountrySetup(string sForm_Name = "")
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
            this.KeyDown += new System.Windows.Input.KeyEventHandler(Window_KeyDown);
        }

        //Closing Events
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

        //Button Events
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtCountry.Text == "")
                {
                    MessageBox.Show("Enter Country...", "Information");
                    txtCountry.Focus();
                }

                else
                {
                    if (MessageBox.Show("Do you wanrt to save this record?", "SAVE CONFIRMATION", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (db.CountrySetups.Where(x => x.CountryName == txtCountry.Text).Select(x => x.CountryName).FirstOrDefault() == txtCountry.Text.ToString())
                        {
                            MessageBox.Show("'" + txtCountry.Text + "' already exist! Enter new  Country...", "Information");
                        }
                        else if (ID != 0)
                        {
                            CountrySetup c = db.CountrySetups.Where(x => x.ID == ID).FirstOrDefault();
                            var OldData = new JSonHelper().ConvertObjectToJSon(c);

                            c.CountryName = txtCountry.Text;
                            db.SaveChanges();
                            AppLib.lstCountrySetup = db.CountrySetups.OrderBy(x => x.CountryName).ToList();

                            var NewData = new JSonHelper().ConvertObjectToJSon(c);
                            AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "CountrySetup");

                            MessageBox.Show("Saved Successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                            FormClear();

                        }
                        else
                        {
                            CountrySetup c = new CountrySetup();
                            c.CountryName = txtCountry.Text;
                            db.CountrySetups.Add(c);
                            db.SaveChanges();
                            AppLib.lstCountrySetup = db.CountrySetups.OrderBy(x => x.CountryName).ToList();

                            var NewData = new JSonHelper().ConvertObjectToJSon(c);

                            AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "CountrySetup");
                            MessageBox.Show("Saved Successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                            FormClear();
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
                if (txtCountry.Text == "")
                {
                    MessageBox.Show("No record to delete", "DELETE", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (MessageBox.Show("Do you want to delete '" + txtCountry.Text + "'?", "DELETE CONFIRMATION", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        CountrySetup c = db.CountrySetups.Where(x => x.ID == ID).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(c);

                        db.CountrySetups.Remove(c);
                        db.SaveChanges();

                        AppLib.EventHistory(this.Tag.ToString(), 2, OldData, "", "CountrySetup");
                        MessageBox.Show("Deleted Successfully", "DELETED", MessageBoxButton.OK, MessageBoxImage.Information);
                        FormClear();
                    }
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
            Reports.frmCountryReports frm = new Reports.frmCountryReports();
            frm.ShowDialog();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            FormClear();
        }

        private void dgvCountry_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (bIsEdit == true)
                {
                    CountrySetup c = dgvCountry.SelectedItem as CountrySetup;
                    ID = c.ID;
                    txtCountry.Text = c.CountryName;
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
            if (txtCountry.Text != "")
            {
                dgvCountry.ItemsSource = db.CountrySetups.Where(x => x.CountryName.Contains(txtCountry.Text.ToString())).ToList();
            }
            else
            {
                dgvCountry.ItemsSource = db.CountrySetups.ToList();
            }
        }

        private void FormClear()
        {
            ID = 0;
            txtCountry.Clear();
            LoadWindow();
        }

        private void dgvCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ID = 0;
            txtCountry.Clear();
        }
    }
}
