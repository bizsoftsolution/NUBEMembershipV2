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

namespace Nube.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmRelationSetup.xaml
    /// </summary>
    public partial class frmRelationSetup : MetroWindow
    {
        public Boolean bIsEdit = false;
        nubebfsEntity db = new nubebfsEntity();
        int ID = 0;
        string sFormName = "";

        public frmRelationSetup(string sForm_Name = "")
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
                if (txtRelationName.Text == "")
                {
                    MessageBox.Show("Enter Relation...", "Information");
                }
                else
                {
                    if (MessageBox.Show("Do you wanrt to save this record?", "SAVE CONFIRMATION", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (ID != 0)
                        {
                            MASTERRELATION ms = db.MASTERRELATIONs.Where(x => x.RELATION_CODE == ID).FirstOrDefault();
                            var OldData = new JSonHelper().ConvertObjectToJSon(ms);

                            ms.RELATION_NAME = txtRelationName.Text;
                            db.SaveChanges();
                            AppLib.lstMASTERRELATION = db.MASTERRELATIONs.OrderBy(x => x.RELATION_NAME).ToList();

                            var NewData = new JSonHelper().ConvertObjectToJSon(ms);
                            AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "MASTERRELATION");
                            MessageBox.Show("Saved Successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                            FormClear();
                        }
                        else
                        {
                            if (db.MASTERRELATIONs.Where(x => x.RELATION_NAME == txtRelationName.Text).Select(x => x.RELATION_NAME).FirstOrDefault() == txtRelationName.Text.ToString())
                            {
                                MessageBox.Show("'" + txtRelationName.Text + "' already exist! Enter new  Country...", "Information");
                            }
                            else
                            {
                                MASTERRELATION ms = new MASTERRELATION();
                                ms.RELATION_NAME = txtRelationName.Text;

                                db.MASTERRELATIONs.Add(ms);
                                db.SaveChanges();
                                AppLib.lstMASTERRELATION = db.MASTERRELATIONs.OrderBy(x => x.RELATION_NAME).ToList();

                                var NewData = new JSonHelper().ConvertObjectToJSon(ms);
                                AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "MASTERRELATION");
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

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            FormClear();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Reports.frmRelationReport frm = new Reports.frmRelationReport();
                frm.ShowDialog();
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
                    MASTERRELATION r = dgvReason.SelectedItem as MASTERRELATION;
                    txtRelationName.Text = r.RELATION_NAME;
                    ID = Convert.ToInt16(r.RELATION_CODE);
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
                if (txtRelationName.Text != "")
                {
                    dgvReason.ItemsSource = db.MASTERRELATIONs.Where(x => x.RELATION_NAME.Contains(txtRelationName.Text.ToString())).OrderBy(x => x.RELATION_NAME).ToList();
                }
                else
                {
                    dgvReason.ItemsSource = db.MASTERRELATIONs.OrderBy(x => x.RELATION_NAME).ToList();
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
                txtRelationName.Clear();
                LoadWindow();
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
                    if (MessageBox.Show("Do you want to delete '" + txtRelationName.Text + "'", "DELETE", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        MASTERRELATION ms = db.MASTERRELATIONs.Where(x => x.RELATION_CODE == ID).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(ms);

                        db.MASTERRELATIONs.Remove(ms);
                        db.SaveChanges();

                        AppLib.EventHistory(this.Tag.ToString(), 2, OldData, "", "MASTERRELATION");

                        MessageBox.Show("Deleted Successfully!", "DELETED", MessageBoxButton.OK, MessageBoxImage.Information);
                        FormClear();
                    }
                }
                else
                {
                    MessageBox.Show("Please Select any Relation! (Double Click to Select)", "SELECT", MessageBoxButton.OK, MessageBoxImage.Information);
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
                txtRelationName.Clear();
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

    }
}
