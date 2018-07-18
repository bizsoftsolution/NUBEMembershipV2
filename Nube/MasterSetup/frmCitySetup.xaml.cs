using System;
using System.Collections.Generic;
using System.Data;
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
using System.Data.SqlClient;

namespace Nube.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmCitySetup.xaml
    /// </summary>
    public partial class frmCitySetup : MetroWindow
    {
        public Boolean bIsEdit = false;
        string conn =AppLib.connStr;
        nubebfsEntity db = new nubebfsEntity();
        DataTable dtState = new DataTable();
        int ID = 0;
        string sFormName = "";

        public frmCitySetup(string sForm_Name = "")
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

                var coun = db.MASTERSTATEs.ToList();
                cmbState.ItemsSource = coun;
                cmbState.DisplayMemberPath = "STATE_NAME";
                cmbState.SelectedValuePath = "STATE_CODE";
                this.KeyDown += new System.Windows.Input.KeyEventHandler(Window_KeyDown);
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

        //Button Events
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtCity.Text == "")
                {
                    MessageBox.Show("Enter City...", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

                else if (cmbState.Text == "")
                {
                    MessageBox.Show("Enter State...", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (MessageBox.Show("Do you wanrt to save this record?", "SAVE CONFIRMATION", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (ID != 0)
                        {
                            MASTERCITY c = db.MASTERCITies.Where(x => x.CITY_CODE == ID).FirstOrDefault();
                            var OldData = new JSonHelper().ConvertObjectToJSon(c);

                            c.CITY_NAME = txtCity.Text;
                            c.STATE_CODE = Convert.ToDecimal(cmbState.SelectedValue);
                            db.SaveChanges();
                            AppLib.lstMASTERCITY = db.MASTERCITies.OrderBy(x => x.CITY_NAME).ToList();

                            var NewData = new JSonHelper().ConvertObjectToJSon(c);
                            AppLib.EventHistory(this.Tag.ToString(), 1, OldData, NewData, "MASTERCITY");

                            MessageBox.Show("Saved Successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                            FormClear();
                        }
                        else
                        {
                            MASTERCITY c = new MASTERCITY();
                            c.CITY_NAME = txtCity.Text;
                            c.STATE_CODE = Convert.ToDecimal(cmbState.SelectedValue);
                            db.MASTERCITies.Add(c);
                            db.SaveChanges();
                            AppLib.lstMASTERCITY = db.MASTERCITies.OrderBy(x => x.CITY_NAME).ToList();

                            var NewData = new JSonHelper().ConvertObjectToJSon(c);
                            AppLib.EventHistory(this.Tag.ToString(), 0, "", NewData, "MASTERCITY");

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
                if (ID != 0)
                {
                    if (MessageBox.Show("Do you want to Delete this '" + txtCity.Text + "'", "DELETE", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        MASTERCITY c = db.MASTERCITies.Where(x => x.CITY_CODE == ID).FirstOrDefault();
                        var OldData = new JSonHelper().ConvertObjectToJSon(c);

                        db.MASTERCITies.Remove(c);
                        db.SaveChanges();
                        AppLib.EventHistory(this.Tag.ToString(), 2, OldData, "", "MASTERCITY");

                        MessageBox.Show("Deleted Successfully", "DELETED", MessageBoxButton.OK, MessageBoxImage.Information);
                        FormClear();
                    }
                }
                else
                {
                    MessageBox.Show("Please Select Any City! (Double Click to Select)");
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

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Reports.frmCityStateReport frm = new Reports.frmCityStateReport();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void dgvState_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (bIsEdit == true)
                {
                    DataRowView drv = (DataRowView)dgvCity.SelectedItem;
                    ID = Convert.ToInt32(drv["CITY_CODE"]);
                    txtCity.Text = drv["CITY_NAME"].ToString();
                    cmbState.SelectedValue = Convert.ToInt32(drv["STATE_CODE"]);
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
                    if (txtCity.Text != "")
                    {
                        DataTable dtCity = new DataTable();
                        string st = string.Format(" SELECT CT.CITY_NAME,CT.CITY_CODE,CT.STATE_CODE,ST.STATE_CODE AS STATECODE,ST.STATE_NAME" +
                                    " FROM MASTERCITY CT(NOLOCK) " +
                                    " LEFT JOIN MASTERSTATE ST ON CT.STATE_CODE = ST.STATE_CODE" +
                                    " WHERE CT.CITY_NAME='{0}'" +
                                    " ORDER BY CT.CITY_NAME", txtCity.Text);
                        SqlCommand cmd = new SqlCommand(st, con);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        adp.Fill(dtCity);
                        dgvCity.ItemsSource = dtCity.DefaultView;
                    }
                    else
                    {
                        DataTable dtCity = new DataTable();
                        string st = " SELECT CT.CITY_NAME,CT.CITY_CODE,CT.STATE_CODE,ST.STATE_CODE AS STATECODE,ST.STATE_NAME" +
                                    " FROM MASTERCITY CT(NOLOCK) " +
                                    " LEFT JOIN MASTERSTATE ST ON CT.STATE_CODE = ST.STATE_CODE" +
                                    " ORDER BY CT.CITY_NAME";
                        SqlCommand cmd = new SqlCommand(st, con);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        adp.Fill(dtCity);
                        dgvCity.ItemsSource = dtCity.DefaultView;
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
                txtCity.Clear();
                cmbState.Text = "";
                LoadWindow();
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void dgvCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ID = 0;
                txtCity.Clear();
                cmbState.Text = "";
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void cmbState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conn))
                {
                    if ((Convert.ToInt32(cmbState.SelectedValue) != 0) && !string.IsNullOrEmpty(txtCity.Text))
                    {
                        DataTable dtCity = new DataTable();
                        string st = string.Format(" SELECT CT.CITY_NAME,CT.CITY_CODE,CT.STATE_CODE,ST.STATE_CODE AS STATECODE,ST.STATE_NAME" +
                                    " FROM MASTERCITY CT(NOLOCK) " +
                                    " LEFT JOIN MASTERSTATE ST ON CT.STATE_CODE = ST.STATE_CODE" +
                                    " WHERE CT.STATE_CODE='{0}' AND CT.CITY_NAME LIKE '%{1}' " +
                                    " ORDER BY CT.CITY_NAME", cmbState.SelectedValue, txtCity.Text);
                        SqlCommand cmd = new SqlCommand(st, con);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        adp.Fill(dtCity);
                        dgvCity.ItemsSource = dtCity.DefaultView;
                    }
                    else
                    {
                        if ((Convert.ToInt32(cmbState.SelectedValue) != 0) && string.IsNullOrEmpty(txtCity.Text))
                        {
                            DataTable dtCity = new DataTable();
                            string st = string.Format(" SELECT CT.CITY_NAME,CT.CITY_CODE,CT.STATE_CODE,ST.STATE_CODE AS STATECODE,ST.STATE_NAME" +
                                        " FROM MASTERCITY CT(NOLOCK) " +
                                        " LEFT JOIN MASTERSTATE ST ON CT.STATE_CODE = ST.STATE_CODE" +
                                        " WHERE CT.STATE_CODE='{0}' " +
                                        " ORDER BY CT.CITY_NAME", cmbState.SelectedValue);
                            SqlCommand cmd = new SqlCommand(st, con);
                            SqlDataAdapter adp = new SqlDataAdapter(cmd);
                            adp.Fill(dtCity);
                            dgvCity.ItemsSource = dtCity.DefaultView;
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


