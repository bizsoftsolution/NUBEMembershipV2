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

namespace Nube.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmPersonTitleSetup.xaml
    /// </summary>
    public partial class frmPersonTitleSetup : MetroWindow
    {
        public Boolean bIsEdit = false;
        nubebfsEntity db = new nubebfsEntity();
        int ID = 0;

        public frmPersonTitleSetup()
        {
            InitializeComponent();
            LoadWindow();
            this.KeyDown += new System.Windows.Input.KeyEventHandler(Window_KeyDown);

        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                this.Close();
        }

        //Button events
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            FormClear();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Reports.frmTitleReport frm = new Reports.frmTitleReport();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void dgvTitle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (bIsEdit == true)
                {
                    NameTitleSetup r = dgvTitle.SelectedItem as NameTitleSetup;
                    txtPersonTitle.Text = r.TitleName;
                    ID = Convert.ToInt16(r.ID);
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
                if (txtPersonTitle.Text != "")
                {
                    dgvTitle.ItemsSource = db.NameTitleSetups.Where(x => x.TitleName.Contains(txtPersonTitle.Text.ToString())).OrderBy(x => x.TitleName).ToList();
                }
                else
                {
                    dgvTitle.ItemsSource = db.NameTitleSetups.OrderBy(x => x.TitleName).ToList();
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
                txtPersonTitle.Clear();
                LoadWindow();

            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtPersonTitle.Text == "")
                {
                    MessageBox.Show("Enter Title...", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtPersonTitle.Focus();
                }
                else
                {
                    if (MessageBox.Show("Do you want to save this record?", "SAVE CONFIRMATION", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (ID != 0)
                        {
                            NameTitleSetup ms = db.NameTitleSetups.Where(x => x.ID == ID).FirstOrDefault();
                            var OldData = new JSonHelper().ConvertObjectToJSon(ms);

                            ms.TitleName = txtPersonTitle.Text;
                            db.SaveChanges();
                            AppLib.lstNameTitleSetup = db.NameTitleSetups.ToList();

                            var NewData = new JSonHelper().ConvertObjectToJSon(ms);
                            AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "NameTitleSetup");
                            MessageBox.Show("Saved Successfully!", "SAVED", MessageBoxButton.OK, MessageBoxImage.Information);
                            FormClear();
                        }
                        else
                        {
                            var pt = (from p in db.NameTitleSetups where p.TitleName == txtPersonTitle.Text select p).SingleOrDefault();
                            if (pt != null)
                            {
                                MessageBox.Show("'" + txtPersonTitle.Text + "' Already exist.Enter new Title.", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                                txtPersonTitle.Focus();
                            }
                            else
                            {
                                NameTitleSetup ms = new NameTitleSetup();
                                ms.TitleName = txtPersonTitle.Text;
                                db.NameTitleSetups.Add(ms);
                                db.SaveChanges();
                                AppLib.lstNameTitleSetup = db.NameTitleSetups.ToList();

                                var NewData = new JSonHelper().ConvertObjectToJSon(ms);
                                AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "NameTitleSetup");
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

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            frmHomeMaster frm = new frmHomeMaster();
            this.Close();
            frm.ShowDialog();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ID != 0)
                {
                    if (MessageBox.Show("Do you want to delete '" + txtPersonTitle.Text + "'", "DELETE", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        NameTitleSetup ms = db.NameTitleSetups.Where(x => x.ID == ID).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(ms);


                        db.NameTitleSetups.Remove(ms);
                        db.SaveChanges();

                        AppLib.EventHistory(this.Tag.ToString(), 2, OldData, "", "NameTitleSetup");
                        MessageBox.Show("Deleted Successfully!", "DELETED", MessageBoxButton.OK, MessageBoxImage.Information);
                        FormClear();
                    }
                }
                else
                {
                    MessageBox.Show("Please Select Any Title! (Double Click to Select)", "SELECT", MessageBoxButton.OK, MessageBoxImage.Information);
                }

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
                txtPersonTitle.Clear();
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }
    }
}
