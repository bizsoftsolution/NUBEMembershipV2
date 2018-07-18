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
        bool bIsClose = false;
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
            if (bIsClose == false)
            {
                if (MessageBox.Show(this, "Are you sure to exit?", "Exit Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    System.Windows.Forms.Application.Exit();
                    this.Close();
                }
            }
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
                        AppLib.iUsertypeId = Convert.ToInt32(User.UserType);
                        var ut = (from x in db.UserTypes where x.Id == AppLib.iUsertypeId && x.IsSuperAdmin == true select x).FirstOrDefault();
                        if (ut != null)
                        {
                            AppLib.bIsSuperAdmin = true;
                        }
                        else
                        {
                            AppLib.bIsSuperAdmin = false;
                        }

                        var lstUserRgt = (from u in db.UserPrevilages where u.UsertypeId == AppLib.iUsertypeId select u).ToList();
                        if (lstUserRgt != null)
                        {
                            AppLib.lstUsreRights = lstUserRgt;
                        }
                        progressBar1.Value = 8;
                        System.Windows.Forms.Application.DoEvents();
                        BindDefaultList();

                        LoginHistory lg = new LoginHistory();
                        lg.UserId = AppLib.iUserCode;
                        lg.LoginOn = DateTime.Now;
                        db.LoginHistories.Add(lg);
                        db.SaveChanges();

                        progressBar1.Value = 10;
                        System.Windows.Forms.Application.DoEvents();

                        frmMain frm = new frmMain();
                        frm.Show();
                        bIsClose = true;
                        this.Close();
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

        void BindDefaultList()
        {
            AppLib.lstMASTERCITY = db.MASTERCITies.OrderBy(x => x.CITY_NAME).ToList();
            AppLib.lstMASTERSTATE = db.MASTERSTATEs.OrderBy(x => x.STATE_NAME).ToList();
            AppLib.lstCountrySetup = db.CountrySetups.OrderBy(x => x.CountryName).ToList();
            AppLib.lstMASTERRELATION = db.MASTERRELATIONs.OrderBy(x => x.RELATION_NAME).ToList();
            AppLib.lstNameTitleSetup = db.NameTitleSetups.OrderBy(x => x.TitleName).ToList();
            AppLib.lstMASTERBANK = db.MASTERBANKs.OrderBy(x => x.BANK_NAME).ToList();
            AppLib.lstMASTERBANKBRANCH = db.MASTERBANKBRANCHes.OrderBy(x => x.BANKBRANCH_NAME).ToList();
            AppLib.lstMASTERMEMBERTYPE = db.MASTERMEMBERTYPEs.OrderBy(x => x.MEMBERTYPE_NAME).ToList();
            AppLib.lstMASTERRACE = db.MASTERRACEs.OrderBy(x => x.RACE_NAME).ToList();
            AppLib.BindEmail();
        }

        #endregion

        #region BACK GROUND WORKER

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string DBPath = "";
                string str = "";
                var dt = (from x in db.Tran_Start where x.EntryDate == DateTime.Today.Date select x).FirstOrDefault();
                if (dt == null)
                {
                    var qry = (from x in db.MASTERNAMESETUPs select x).FirstOrDefault();

                    // ######### VERSION 1 BACKUP ##############

                    if (qry.NEWEXEPATH != null)
                    {
                        str = qry.NEWEXEPATH.ToString();
                        if (str.Contains(".") == true)
                        {
                            string[] path = qry.NEWEXEPATH.Split('.');
                            DBPath = path[0];
                        }
                        else
                        {
                            DBPath = qry.NEWEXEPATH.ToString();
                        }

                        try
                        {
                            using (SqlConnection con = new SqlConnection(AppLib.connStr))
                            {
                                string sBack = " BACKUP DATABASE " + qry.NEWREPORTSPATH + " \r" +
                                " TO DISK='" + DBPath.ToString() + string.Format("{0:ddMMMyyyy}", DateTime.Today) + ".bak'";
                                SqlCommand cmd = new SqlCommand(sBack, con);
                                cmd.Connection.Open();
                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();

                                sBack = " xp_cmdshell 'del " + DBPath.ToString() + string.Format("{0:ddMMMyyyy}", DateTime.Now.AddDays(-3)) + ".bak'";
                                cmd = new SqlCommand(sBack, con);
                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();
                                cmd.Connection.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogging.SendErrorToText(ex);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please Enter the Daily Backup Path Correctly!", "VERSION 1 Daily BackUp Error");
                    }

                    // ######### VERSION 2 BACKUP ##############

                    if (qry.BackUpPath != null)
                    {
                        str = qry.BackUpPath.ToString();
                        if (str.Contains(".") == true)
                        {
                            string[] path = qry.BackUpPath.Split('.');
                            DBPath = path[0];
                        }
                        else
                        {
                            DBPath = qry.BackUpPath.ToString();
                        }
                        try
                        {
                            using (SqlConnection con = new SqlConnection(AppLib.connStr))
                            {
                                string sBack = " BACKUP DATABASE " + con.Database + " \r" +
                                      " TO DISK='" + DBPath.ToString() + string.Format("{0:ddMMMyyyy}", DateTime.Today) + ".bak'";
                                SqlCommand cmd = new SqlCommand(sBack, con);
                                cmd.Connection.Open();
                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();

                                sBack = " xp_cmdshell 'del " + DBPath.ToString() + string.Format("{0:ddMMMyyyy}", DateTime.Now.AddDays(-3)) + ".bak'";
                                cmd = new SqlCommand(sBack, con);
                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();
                                cmd.Connection.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogging.SendErrorToText(ex);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please Enter the Daily Backup Path Correctly!", "VERSION 2 Daily BackUp Error");
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
                MessageBox.Show("Sorry Daily Backup Process Fails ! ,Please Contact to Administrator", "Daily Backup Error");
            }
        }

        #endregion

    }
}
