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
    /// Interaction logic for frmStateSetup.xaml
    /// </summary>
    public partial class frmStateSetup : MetroWindow
    {
        public Boolean bIsEdit = false;
        string conn = AppLib.connStr;
        nubebfsEntity db = new nubebfsEntity();
        DataTable dtState = new DataTable();
        int ID = 0;
        string sFormName = "";

        public frmStateSetup(string sForm_Name = "")
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
                LoadWindow();

                var coun = db.CountrySetups.ToList();
                cmbCountry.ItemsSource = coun;
                cmbCountry.DisplayMemberPath = "CountryName";
                cmbCountry.SelectedValuePath = "ID";
                this.KeyDown += new System.Windows.Input.KeyEventHandler(Window_KeyDown);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
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
                if (cmbCountry.Text == "")
                {
                    MessageBox.Show("Enter Bank..", "Information");

                }

                else if (txtStateName.Text == "")
                {
                    MessageBox.Show("Enter Branchcode..", "Information");
                }

                else
                {
                    if (MessageBox.Show("Do you want to save this record?", "SAVE", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (ID != 0)
                        {
                            decimal id = (decimal)ID;
                            var s = cmbCountry.Text.ToString();
                            MASTERSTATE mb = db.MASTERSTATEs.Where(x => x.STATE_CODE == id).FirstOrDefault();
                            var OldData = new JSonHelper().ConvertObjectToJSon(mb);

                            mb.Country = Convert.ToDecimal(cmbCountry.SelectedValue);
                            mb.STATE_NAME = txtStateName.Text;

                            db.SaveChanges();
                            AppLib.lstMASTERSTATE = db.MASTERSTATEs.OrderBy(x => x.STATE_NAME).ToList();

                            var NewData = new JSonHelper().ConvertObjectToJSon(mb);
                            AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "MASTERSTATE");
                            MessageBox.Show("Saved Successfully", "Saved");
                            FormClear();
                        }
                        else
                        {
                            MASTERSTATE mb = new MASTERSTATE();
                            mb.Country = Convert.ToDecimal(cmbCountry.SelectedValue);
                            mb.STATE_NAME = txtStateName.Text;

                            db.MASTERSTATEs.Add(mb);
                            db.SaveChanges();
                            AppLib.lstMASTERSTATE = db.MASTERSTATEs.OrderBy(x => x.STATE_NAME).ToList();

                            var NewData = new JSonHelper().ConvertObjectToJSon(mb);
                            AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "MASTERSTATE");
                            MessageBox.Show("Saved Successfully", "Saved");
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
                if (ID != 0)
                {
                    if (MessageBox.Show("Do you want to delete '" + txtStateName.Text + "'", "DELETE", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        MASTERSTATE c = db.MASTERSTATEs.Where(x => x.STATE_CODE == ID).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(c);

                        db.MASTERSTATEs.Remove(c);
                        db.SaveChanges();

                        AppLib.EventHistory(this.Tag.ToString(), 2, OldData, "", "MASTERSTATE");
                        MessageBox.Show("Deleted Successfully", "DELETED", MessageBoxButton.OK, MessageBoxImage.Information);
                        FormClear();
                    }
                }
                else
                {
                    MessageBox.Show("Please Select Any State! (Double Click to Select)", "SELECT", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            FormClear();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Reports.frmStateReport frm = new Reports.frmStateReport();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void dgvState_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (bIsEdit == true)
                {
                    DataRowView drv = (DataRowView)dgvState.SelectedItem;
                    ID = Convert.ToInt32(drv["STATE_CODE"]);
                    txtStateName.Text = drv["STATE_NAME"].ToString();
                    cmbCountry.SelectedValue = Convert.ToInt32(drv["ID"]);
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
                using (SqlConnection con = new SqlConnection(conn))
                {
                    if (txtStateName.Text != "")
                    {
                        DataTable dtCity = new DataTable();
                        string st = string.Format(" SELECT ST.STATE_CODE,ST.STATE_NAME,ST.COUNTRY,CT.ID, CT.COUNTRYNAME" +
                                    " FROM MASTERSTATE ST(NOLOCK)  " +
                                    " LEFT JOIN COUNTRYSETUP CT ON ST.COUNTRY =CT.ID" +
                                    " WHERE ST.STATE_CODE='{0}'" +
                                    " ORDER BY ST.STATE_NAME", txtStateName.Text);
                        SqlCommand cmd = new SqlCommand(st, con);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        adp.Fill(dtCity);
                        dgvState.ItemsSource = dtCity.DefaultView;
                    }
                    else
                    {
                        DataTable dtCity = new DataTable();
                        string st = " SELECT ST.STATE_CODE,ST.STATE_NAME,ST.COUNTRY,CT.ID, CT.COUNTRYNAME" +
                                    " FROM MASTERSTATE ST(NOLOCK)  " +
                                    " LEFT JOIN COUNTRYSETUP CT ON ST.COUNTRY =CT.ID" +
                                    " ORDER BY ST.STATE_NAME";
                        SqlCommand cmd = new SqlCommand(st, con);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        adp.Fill(dtCity);
                        dgvState.ItemsSource = dtCity.DefaultView;
                    }
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
                txtStateName.Clear();
                cmbCountry.Text = "";
                LoadWindow();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
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

        private void dgvState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ID = 0;
                txtStateName.Clear();
                cmbCountry.Text = "";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void cmbCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conn))
                {
                    if (!string.IsNullOrEmpty(txtStateName.Text) && Convert.ToInt32(cmbCountry.SelectedValue) != 0)
                    {
                        DataTable dtCity = new DataTable();
                        string st = string.Format(" SELECT ST.STATE_CODE,ST.STATE_NAME,ST.COUNTRY,CT.ID, CT.COUNTRYNAME" +
                                    " FROM MASTERSTATE ST(NOLOCK)  " +
                                    " LEFT JOIN COUNTRYSETUP CT ON ST.COUNTRY =CT.ID" +
                                    " WHERE ST.STATE_NAME LIKE'%{0}' AND ST.COUNTRY={1} " +
                                    " ORDER BY ST.STATE_NAME", txtStateName.Text, cmbCountry.SelectedValue);
                        SqlCommand cmd = new SqlCommand(st, con);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        adp.Fill(dtCity);
                        dgvState.ItemsSource = dtCity.DefaultView;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(txtStateName.Text) && Convert.ToInt32(cmbCountry.SelectedValue) != 0)
                        {
                            DataTable dtCity = new DataTable();
                            string st = string.Format(" SELECT ST.STATE_CODE,ST.STATE_NAME,ST.COUNTRY,CT.ID, CT.COUNTRYNAME" +
                                        " FROM MASTERSTATE ST(NOLOCK) " +
                                        " LEFT JOIN COUNTRYSETUP CT ON ST.COUNTRY =CT.ID" +
                                        " WHERE ST.COUNTRY={0} " +
                                        " ORDER BY ST.STATE_NAME", cmbCountry.SelectedValue);
                            SqlCommand cmd = new SqlCommand(st, con);
                            SqlDataAdapter adp = new SqlDataAdapter(cmd);
                            adp.Fill(dtCity);
                            dgvState.ItemsSource = dtCity.DefaultView;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }
    }


}

