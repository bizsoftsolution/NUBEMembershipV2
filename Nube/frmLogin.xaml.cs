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
using System.ComponentModel;
using System.Threading;
using System.Data.SqlClient;

namespace Nube
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>

    public partial class frmLogin : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        private readonly BackgroundWorker BgWorker = new BackgroundWorker();

        public frmLogin()
        {
            InitializeComponent();

            txtUserId.Focus();
            progressBar1.Visibility = Visibility.Hidden;

            BgWorker.DoWork += BgWorker_DoWork;
            BgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;
        }

        #region BUTTON EVENTS

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                loginIn();
            }
            catch (Exception ex)
            {
                progressBar1.Visibility = Visibility.Hidden;
                MessageBox.Show(ex.Message);
                progressBar1.Visibility = Visibility.Collapsed;
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();           
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtUserId.Text = string.Empty;
            txtPassword.Password = string.Empty;
            txtUserId.Focus();
            progressBar1.Visibility = Visibility.Hidden;

        }

        #endregion

        #region OTHER EVENTS

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    btnLogin.Focus();
                    loginIn();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void txtUserId_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            e.Handled = true;
        }

        #endregion

        #region  USER DEFINED FUNCTION

        void loginIn()
        {
            progressBar1.Minimum = 1;
            progressBar1.Maximum = 10;
            progressBar1.Visibility = Visibility.Visible;
            if (string.IsNullOrEmpty(txtUserId.Text))
            {
                progressBar1.Visibility = Visibility.Hidden;
                MessageBox.Show(Message.frmLogin_EnterUserId);
                txtUserId.Focus();
            }
            else if (string.IsNullOrEmpty(txtPassword.Password))
            {
                progressBar1.Visibility = Visibility.Hidden;
                MessageBox.Show("Please Enter Password");
                txtPassword.Focus();
            }
            else
            {
                progressBar1.Value = 5;
                System.Windows.Forms.Application.DoEvents();
                var User = (from x in db.UserAccounts where x.UserName == txtUserId.Text select x).SingleOrDefault();
                progressBar1.Value = 8;
                System.Windows.Forms.Application.DoEvents();
                string sUserName = "";
                string sPassword = "";
                if (User != null)
                {
                    sUserName = User.UserName;
                    sPassword = User.Password;

                    if (txtUserId.Text != sUserName)
                    {
                        progressBar1.Visibility = Visibility.Hidden;
                        MessageBox.Show("Invalid User Name");
                        txtUserId.Text = "";
                        txtPassword.Password = "";
                        txtUserId.Focus();
                        return;
                    }
                    else if (txtPassword.Password.ToString() != sPassword)
                    {
                        progressBar1.Visibility = Visibility.Hidden;
                        MessageBox.Show("Invalid Password");
                        txtPassword.Password = "";
                        txtPassword.Focus();
                        return;
                    }
                    else if (User != null)
                    {
                        AppLib.iUserCode = Convert.ToInt32(User.UserId);
                        int iUsertypeId = Convert.ToInt32(User.UserType);
                        var ut = (from x in db.UserTypes where x.Id == iUsertypeId && x.IsSuperAdmin == true select x).FirstOrDefault();
                        if (ut != null)
                        {
                            AppLib.bIsSuperAdmin = true;
                        }
                        else
                        {
                            AppLib.bIsSuperAdmin = false;
                        }                        

                        var lstUserRgt = (from u in db.UserPrevilages where u.UsertypeId == iUsertypeId select u).ToList();
                        if (lstUserRgt != null)
                        {
                            AppLib.lstUsreRights = lstUserRgt;
                        }
                        progressBar1.Value = 8;
                        System.Windows.Forms.Application.DoEvents();

                        LoginHistory lg = new LoginHistory();
                        lg.UserId = AppLib.iUserCode;
                        lg.LoginOn = DateTime.Now;
                        db.LoginHistories.Add(lg);
                        db.SaveChanges();

                        progressBar1.Value = 10;
                        System.Windows.Forms.Application.DoEvents();

                        frmMain frm = new frmMain();                        
                        this.Close();
                        frm.ShowDialog();

                        BgWorker.RunWorkerAsync();
                    }
                }
                else
                {
                    progressBar1.Visibility = Visibility.Hidden;
                    MessageBox.Show("Invalid User");
                }
            }
        }

        #endregion

        #region BACK GROUND WORKER

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string DBPath = "";               
                var dt = (from x in db.Tran_Start where x.EntryDate == DateTime.Today.Date select x).FirstOrDefault();
                if (dt == null)
                {
                    var qry = (from x in db.MASTERNAMESETUPs select x).FirstOrDefault();
                    if (qry.BackUpPath != null)
                    {
                        string str = qry.BackUpPath.ToString();
                        if (str.Contains(".") == true)
                        {
                            string[] path = qry.BackUpPath.Split('.');
                            DBPath = path[0];
                        }
                        else
                        {
                            DBPath = qry.BackUpPath.ToString();
                        }
                        using (SqlConnection con = new SqlConnection(AppLib.connStr))
                        {
                            string sBack = " BACKUP DATABASE " + con.Database + " \r" +
                                         " TO DISK='" + DBPath.ToString() + string.Format("{0:ddMMMyyyy}", DateTime.Today) + ".bak'";
                            SqlCommand cmd = new SqlCommand(sBack, con);
                            cmd.Connection.Open();
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                            cmd.Connection.Close();
                        }
                        var TS = (from x in db.Tran_Start where x.Id == 1 select x).FirstOrDefault();
                        if (TS != null)
                        {
                            TS.EntryDate = DateTime.Now.Date;
                            TS.UserId = AppLib.iUserCode;
                            TS.UpdatedTime = DateTime.Now;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please Ensure the Daily Backup Path Correctly!", "Daily BackUp Error");
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Sorry Daily Backup Process Fails! ,Please Contact to Administrator", "Daily Backup Error");
            }
        }

        #endregion

    }
}
