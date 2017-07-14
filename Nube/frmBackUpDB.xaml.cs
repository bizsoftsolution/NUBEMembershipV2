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
using System.IO;

namespace Nube
{
    /// <summary>
    /// Interaction logic for frmBackUpDB.xaml
    /// </summary>
    public partial class frmBackUpDB : MetroWindow
    {
        public frmBackUpDB()
        {
            InitializeComponent();
            LoadWindow();
            //progressBar1.Visibility = Visibility.Hidden;
        }

        #region BUTTON EVENTS

        private void btnBackUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(cmbDBName.Text))
                {
                    MessageBox.Show("Please select Database !","DB Name is Empty");
                    cmbDBName.Focus();
                    return;
                }
                else if (string.IsNullOrEmpty(txtPath.Text))
                {
                    MessageBox.Show("Please select Database Destination Folder !","Save Path is Empty");
                    txtPath.Focus();
                    return;
                }
                else
                {
                    progressBar1.Minimum = 1;
                    progressBar1.Maximum = 10;
                    System.Windows.Forms.Application.DoEvents();
                    progressBar1.Visibility = Visibility.Visible;
                    using (SqlConnection con = new SqlConnection(AppLib.connStr))
                    {
                        string str = " BACKUP DATABASE " + cmbDBName.Text + " \r" +
                                     " TO DISK = '" + txtPath.Text + "'";
                        progressBar1.Value = 6;
                        System.Windows.Forms.Application.DoEvents();
                        SqlCommand cmd = new SqlCommand(str, con);
                        cmd.Connection.Open();
                        cmd.CommandTimeout = 0;

                        int i = cmd.ExecuteNonQuery();
                        progressBar1.Value = 10;
                        System.Windows.Forms.Application.DoEvents();
                        if (i == 0)
                        {
                            MessageBox.Show("BackUp Not Execute!", "Error");
                        }
                        else
                        {
                            MessageBox.Show("BackUp Completed Successfully!", "Executed");
                        }
                        cmd.Connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            frmMain frm = new frmMain();
            this.Close();
            frm.ShowDialog();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cmbDBName.Text = "";
                txtPath.Text = "";
                progressBar1.Value = 0;
                System.Windows.Forms.Application.DoEvents();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnOpenDialogBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(cmbDBName.Text))
                {
                    MessageBox.Show("Please Select Database!", "DB Name is Empty");
                    cmbDBName.Focus();
                    return;
                }
                else
                {
                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.InitialDirectory = Environment.CurrentDirectory;
                    dlg.Title = "NUBE Back Up";
                    dlg.Filter = "Backup Files|*.bak;*.BAK|All files|*.*";
                    dlg.FileName = cmbDBName.Text.ToUpper() + string.Format("{0:ddMMyyy}", DateTime.Today) + ".bak";                   
                    if (dlg.ShowDialog() == true)
                    {
                        txtPath.Text = dlg.FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion

        #region FUNCTION

        void LoadWindow()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppLib.connStr))
                {
                    DataTable dt = new DataTable();
                    SqlCommand cmd;
                    cmd = new SqlCommand("SELECT db.[name] as DBNAME FROM [master].[sys].[databases] db", con);
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.SelectCommand.CommandTimeout = 0;
                    adp.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        cmbDBName.ItemsSource = dt.DefaultView;
                        cmbDBName.SelectedValuePath = "DBNAME";
                        cmbDBName.DisplayMemberPath = "DBNAME";
                    }
                    else
                    {
                        MessageBox.Show("Please Verify ConnectionProperty!,Connection Error");
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion


    }
}
