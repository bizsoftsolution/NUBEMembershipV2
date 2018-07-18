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

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmMemberApprovalEntry.xaml
    /// </summary>
    public partial class frmMemberApprovalEntry : MetroWindow
    {
        Boolean bIsClear = false;
        DataTable dtMemberApproval = new DataTable();
        nubebfsEntity db = new nubebfsEntity();
        public frmMemberApprovalEntry()
        {

            InitializeComponent();
            dgvMemberApproval.Columns[9].Header = "Approve";
            load_Form();
        }

        #region Button Events

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                load_Form();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtBankName.Text = "";
                txtBranchName.Text = "";
                txtMF.Text = "";
                txtICNo.Text = "";
                txtMemberName.Text = "";
                txtNubeBranch.Text = "";
                txtMemberType.Text = "";
                txtMemberNo.Text = "";
                txtNoofMembers.Text = "";

                bIsClear = true;
                rbtPending.IsChecked = true;
                bIsClear = false;
                dtpFromDate.Text = "";
                dtpTodate.Text = "";

                AppLib.dtMemberQuery.Rows.Clear();
                load_Form();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnApproveAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            frmHomeMembership frm = new frmHomeMembership();
            this.Close();
            frm.ShowDialog();
        }

        #endregion

        #region Other Events

        private void dgvMemberApproval_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView drv = (DataRowView)dgvMemberApproval.SelectedItem;
                if (drv != null)
                {
                    var btn = e.OriginalSource as Button;
                    decimal dMemberCode = 0;
                    dMemberCode = Convert.ToDecimal(btn.Tag);
                    if (dMemberCode != 0)
                    {
                        if (rbtPending.IsChecked == true || rbtDeclain.IsChecked == true)
                        {
                            var mm = (from x in db.MemberInsertBranches where x.MEMBER_CODE == dMemberCode select x).FirstOrDefault();
                            if (mm != null)
                            {
                                frmMemberRegistration frm = new frmMemberRegistration(mm.MEMBER_CODE, true);
                                frm.Height = 600;
                                frm.Width = 1000;
                                frm.btnApprove.Visibility = Visibility.Visible;
                                frm.btnDenied.Visibility = Visibility.Collapsed;
                                frm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                                frm.btnNew.Visibility = Visibility.Collapsed;
                                frm.btnSave.Visibility = Visibility.Collapsed;
                                frm.btnDelete.Visibility = Visibility.Collapsed;
                                frm.btnHistory.Visibility = Visibility.Collapsed;
                                frm.btnSearch.Visibility = Visibility.Collapsed;
                                frm.btnHome.Visibility = Visibility.Collapsed;
                                frm.btnMembercard.Visibility = Visibility.Collapsed;
                                frm.btnNomAdd.Visibility = Visibility.Collapsed;
                                frm.btnNomClear.Visibility = Visibility.Collapsed;
                                frm.tiPhoto.Visibility = Visibility.Visible;
                                frm.ShowDialog();
                                filteration();
                            }
                            else
                            {
                                MessageBox.Show("No Data Found!");
                            }
                        }
                        else
                        {
                            var mm = (from x in db.MASTERMEMBERs where x.BranchMemberCode == dMemberCode select x).FirstOrDefault();
                            if (mm != null)
                            {
                                frmMemberRegistration frm = new frmMemberRegistration(mm.MEMBER_CODE, false);
                                frm.Height = 600;
                                frm.Width = 1000;
                                frm.btnApprove.Visibility = Visibility.Collapsed;
                                frm.btnDenied.Visibility = Visibility.Visible;
                                frm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                                frm.btnNew.Visibility = Visibility.Collapsed;
                                frm.btnSave.Visibility = Visibility.Collapsed;
                                frm.btnDelete.Visibility = Visibility.Collapsed;
                                frm.btnHistory.Visibility = Visibility.Collapsed;
                                frm.btnSearch.Visibility = Visibility.Collapsed;
                                frm.btnHome.Visibility = Visibility.Collapsed;
                                frm.btnMembercard.Visibility = Visibility.Collapsed;
                                frm.btnNomAdd.Visibility = Visibility.Collapsed;
                                frm.btnNomClear.Visibility = Visibility.Collapsed;
                                frm.tiPhoto.Visibility = Visibility.Visible;
                                frm.ShowDialog();
                                filteration();
                            }
                            else
                            {
                                MessageBox.Show("No Data Found!");
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvMemberApproval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvMemberApproval.CurrentCell.Column.Header.ToString() == "View")
            {
                DataRowView drv = (DataRowView)dgvMemberApproval.SelectedItem;
                if (drv != null)
                {
                    decimal dMemberCode = Convert.ToDecimal(drv["MEMBER_CODE"]);
                    if (rbtPending.IsChecked == true || rbtDeclain.IsChecked == true)
                    {
                        var mm = (from x in db.MemberInsertBranches where x.MEMBER_CODE == dMemberCode select x).FirstOrDefault();
                        if (mm != null)
                        {
                            frmMemberRegistration frm = new frmMemberRegistration(mm.MEMBER_CODE, true);
                            frm.Height = 600;
                            frm.Width = 1000;
                            frm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            frm.btnApprove.Visibility = Visibility.Visible;
                            frm.btnDenied.Visibility = Visibility.Collapsed;
                            frm.tiPhoto.Visibility = Visibility.Visible;
                            frm.btnNew.Visibility = Visibility.Collapsed;
                            frm.btnSave.Visibility = Visibility.Collapsed;
                            frm.btnDelete.Visibility = Visibility.Collapsed;
                            frm.btnHistory.Visibility = Visibility.Collapsed;
                            frm.btnSearch.Visibility = Visibility.Collapsed;
                            frm.btnHome.Visibility = Visibility.Collapsed;
                            frm.btnMembercard.Visibility = Visibility.Collapsed;
                            frm.btnNomAdd.Visibility = Visibility.Collapsed;
                            frm.btnNomClear.Visibility = Visibility.Collapsed;
                            frm.ShowDialog();
                            filteration();
                        }
                        else
                        {
                            MessageBox.Show("No Data Found!");
                        }
                    }
                    else
                    {
                        var mm = (from x in db.MASTERMEMBERs where x.BranchMemberCode == dMemberCode select x).FirstOrDefault();
                        if (mm != null)
                        {
                            frmMemberRegistration frm = new frmMemberRegistration(mm.MEMBER_CODE, false);
                            frm.Height = 600;
                            frm.Width = 1000;
                            frm.btnApprove.Visibility = Visibility.Collapsed;
                            frm.btnDenied.Visibility = Visibility.Visible;
                            frm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            frm.btnNew.Visibility = Visibility.Collapsed;
                            frm.btnSave.Visibility = Visibility.Collapsed;
                            frm.btnDelete.Visibility = Visibility.Collapsed;
                            frm.btnHistory.Visibility = Visibility.Collapsed;
                            frm.btnSearch.Visibility = Visibility.Collapsed;
                            frm.btnHome.Visibility = Visibility.Collapsed;
                            frm.btnMembercard.Visibility = Visibility.Collapsed;
                            frm.btnNomAdd.Visibility = Visibility.Collapsed;
                            frm.btnNomClear.Visibility = Visibility.Collapsed;
                            frm.ShowDialog();
                            filteration();
                        }
                        else
                        {
                            MessageBox.Show("No Data Found!");
                        }
                    }

                }
            }
            else if ((dgvMemberApproval.CurrentCell.Column.Header.ToString() == "Approve" || dgvMemberApproval.CurrentCell.Column.Header.ToString() == "Declain") && rbtApproved.IsChecked == false)
            {
                DataRowView drv = (DataRowView)dgvMemberApproval.SelectedItem;
                if (drv != null)
                {
                    if (MessageBox.Show(this, "Are you Sure to Approve this Member? It will Affect Current Data!", "Save Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        decimal dMemberCode = Convert.ToDecimal(drv["MEMBER_CODE"]);
                        var wm = (from x in db.MemberInsertBranches where x.MEMBER_CODE == dMemberCode select x).FirstOrDefault();
                        if (wm != null)
                        {
                            decimal dMemberId = Convert.ToDecimal(db.MASTERMEMBERs.Max(x => x.MEMBER_ID)) + 1;

                            MASTERMEMBER mm = new MASTERMEMBER();

                            mm.MEMBERTYPE_CODE = wm.MEMBERTYPE_CODE;
                            mm.MEMBER_ID = dMemberId;
                            mm.MEMBER_TITLE = wm.MEMBER_TITLE;
                            mm.MEMBER_NAME = wm.MEMBER_NAME;
                            mm.DATEOFBIRTH = wm.DATEOFBIRTH;
                            mm.AGE_IN_YEARS = wm.AGE_IN_YEARS;
                            mm.SEX = wm.SEX;
                            mm.REJOINED = wm.REJOINED;
                            mm.RACE_CODE = wm.RACE_CODE;
                            mm.ICNO_NEW = wm.ICNO_NEW;
                            mm.ICNO_OLD = wm.ICNO_OLD;
                            mm.DATEOFJOINING = wm.DATEOFJOINING;

                            mm.BANK_CODE = wm.BANK_CODE;
                            mm.BRANCH_CODE = wm.BANKBRANCH_CODE;
                            mm.DATEOFEMPLOYMENT = wm.DATEOFEMPLOYMENT;
                            mm.Salary = wm.Salary;
                            mm.LEVY = wm.LEVY;
                            mm.TDF = wm.TDF;
                            mm.LEVY_AMOUNT = wm.LEVY_AMOUNT;
                            mm.TDF_AMOUNT = wm.TDF_AMOUNT;
                            //mm.LevyPaymentDate = DateTime.Now;
                            //mm.Tdf_PaymentDate = DateTime.Now;

                            mm.ENTRANCEFEE = wm.ENTRANCEFEE;
                            mm.MONTHLYBF = wm.MONTHLYBF;
                            mm.ACCBF = wm.ACCBF;
                            mm.CURRENT_YTDBF = wm.CURRENT_YTDBF;
                            mm.MONTHLYSUBSCRIPTION = wm.MONTHLYSUBSCRIPTION;
                            mm.ACCSUBSCRIPTION = wm.ACCSUBSCRIPTION;
                            mm.CURRENT_YTDSUBSCRIPTION = wm.CURRENT_YTDSUBSCRIPTION;
                            mm.ACCBENEFIT = wm.ACCBENEFIT;
                            mm.TOTALMONTHSPAID = wm.TOTALMONTHSPAID;
                            mm.ADDRESS1 = wm.ADDRESS1;
                            mm.ADDRESS2 = wm.ADDRESS2;
                            mm.ADDRESS3 = wm.ADDRESS3;
                            mm.PHONE = wm.PHONE;
                            mm.MOBILE = wm.MOBILE;
                            mm.EMAIL = wm.EMAIL;
                            mm.CITY_CODE = wm.CITY_CODE;
                            mm.ZIPCODE = wm.ZIPCODE;
                            mm.STATE_CODE = wm.STATE_CODE;
                            mm.COUNTRY = wm.COUNTRY;
                            mm.UpdatedBy = AppLib.iUserCode;
                            mm.UpdatedOn = DateTime.Now;
                            mm.IsBranchRegister = true;
                            mm.BranchMemberCode = Convert.ToInt32(dMemberCode);

                            db.MASTERMEMBERs.Add(mm);

                            wm.IsApproved = 1;
                            db.SaveChanges();

                            var bk = (from x in db.MASTERBANKs where x.BANK_CODE == wm.BANK_CODE select x).FirstOrDefault();
                            var bb = (from x in db.MASTERBANKBRANCHes where x.BANKBRANCH_CODE == wm.BANKBRANCH_CODE select x).FirstOrDefault();

                            MemberStatusLog ms = new MemberStatusLog();
                            ms.MEMBER_CODE = Convert.ToInt32(mm.MEMBER_CODE);
                            ms.MEMBER_NAME = wm.MEMBER_NAME;
                            ms.MEMBER_ID = Convert.ToInt32(mm.MEMBER_ID);
                            ms.MEMBERTYPE_CODE = Convert.ToInt32(wm.MEMBERTYPE_CODE);
                            if (Convert.ToInt32(wm.MEMBERTYPE_CODE) == 1)
                            {
                                ms.MEMBERTYPE_NAME = "C";
                            }
                            else
                            {
                                ms.MEMBERTYPE_NAME = "N";
                            }
                            ms.SEX = wm.SEX;

                            if (wm.SEX == "Male")
                            {
                                ms.SEX_MF = "M";
                            }
                            else
                            {
                                ms.SEX_MF = "F";
                            }

                            if (wm.RACE_CODE == 1)
                            {
                                ms.RACE = "M";
                            }
                            else if (wm.RACE_CODE == 2)
                            {
                                ms.RACE = "I";
                            }
                            else if (wm.RACE_CODE == 3)
                            {
                                ms.RACE = "C";
                            }
                            else
                            {
                                ms.RACE = "O";
                            }

                            ms.ICNO_NEW = string.IsNullOrEmpty(wm.ICNO_NEW) ? "" : wm.ICNO_NEW;
                            ms.ICNO_OLD = string.IsNullOrEmpty(wm.ICNO_OLD) ? "" : wm.ICNO_OLD;
                            ms.Levy = wm.LEVY;
                            ms.TDF = wm.TDF;
                            ms.CITY_CODE = Convert.ToInt32(wm.CITY_CODE);
                            ms.STATE_CODE = Convert.ToInt32(wm.STATE_CODE);
                            ms.MOBILE_NO = wm.MOBILE;
                            ms.DATEOFBIRTH = wm.DATEOFBIRTH;
                            ms.BANK_CODE = Convert.ToInt32(wm.BANK_CODE);
                            ms.BANKUSER_CODE = bk.BANK_USERCODE;

                            ms.BRANCH_CODE = Convert.ToInt32(wm.BANKBRANCH_CODE);
                            ms.BRANCH_USER_CODE = bb.BANKBRANCH_USERCODE;
                            ms.BRANCH_NAME = bb.BANKBRANCH_NAME;
                            ms.DATEOFJOINING = wm.DATEOFJOINING;
                            ms.REJOINED = Convert.ToBoolean(wm.REJOINED);
                            ms.TOTALMONTHSPAID = 1;
                            ms.TOTALMOTHSDUE = 0;
                            ms.LASTPAYMENT_DATE = DateTime.Today;
                            ms.MEMBERSTATUS = "ACTIVE";
                            ms.MEMBERSTATUSCODE = 1;
                            db.MemberStatusLogs.Add(ms);
                            db.SaveChanges();


                            int iMemberCode = Convert.ToInt32(db.MASTERMEMBERs.Max(x => x.MEMBER_CODE));

                            var ni = (from x in db.NomineeInsertBranches where x.MEMBER_CODE == dMemberCode select x).FirstOrDefault();
                            if (ni != null)
                            {
                                MASTERNOMINEE mn = new MASTERNOMINEE();
                                mn.MEMBER_CODE = iMemberCode;
                                mn.NAME = ni.NAME;
                                mn.ICNO_NEW = ni.ICNO_NEW;
                                mn.SEX = ni.SEX;
                                mn.AGE = ni.AGE;
                                mn.RELATION_CODE = ni.RELATION_CODE;
                                mn.ADDRESS1 = ni.ADDRESS1;
                                mn.ADDRESS2 = ni.ADDRESS2;
                                mn.ADDRESS3 = ni.ADDRESS3;
                                mn.CITY_CODE = ni.CITY_CODE;
                                mn.STATE_CODE = ni.STATE_CODE;
                                mn.COUNTRY = ni.COUNTRY;
                                mn.ZIPCODE = ni.ZIPCODE;
                                mn.PHONE = ni.PHONE;
                                mn.MOBILE = ni.MOBILE;
                                db.MASTERNOMINEEs.Add(mn);
                                db.SaveChanges();
                            }

                            var gi = (from x in db.GuardianInsertBranches where x.MEMBER_CODE == dMemberCode select x).FirstOrDefault();
                            if (gi != null)
                            {
                                MASTERGUARDIAN mg = new MASTERGUARDIAN();
                                mg.MEMBER_CODE = iMemberCode;
                                mg.NAME = gi.NAME;
                                mg.ICNO_NEW = gi.ICNO_NEW;
                                mg.SEX = gi.SEX;
                                mg.AGE = gi.AGE;
                                mg.RELATION_CODE = gi.RELATION_CODE;
                                mg.ADDRESS1 = gi.ADDRESS1;
                                mg.ADDRESS2 = gi.ADDRESS2;
                                mg.ADDRESS3 = gi.ADDRESS3;
                                mg.CITY_CODE = gi.CITY_CODE;
                                mg.STATE_CODE = gi.STATE_CODE;
                                mg.COUNTRY = gi.COUNTRY;
                                mg.ZIPCODE = gi.ZIPCODE;
                                mg.PHONE = gi.PHONE;
                                mg.MOBILE = gi.MOBILE;
                                db.MASTERGUARDIANs.Add(mg);
                                db.SaveChanges();
                            }
                            MessageBox.Show("Member Added Sucessfully!", "Sucessfully");
                            txtBankName.Text = "";
                            txtBranchName.Text = "";
                            txtMF.Text = "";
                            txtICNo.Text = "";
                            txtMemberName.Text = "";
                            txtNubeBranch.Text = "";
                            txtMemberType.Text = "";
                            txtMemberNo.Text = "";
                            txtNoofMembers.Text = "";
                            load_Form();
                        }
                    }
                }
            }
            else if (rbtApproved.IsChecked == true)
            {
                DataRowView drv = (DataRowView)dgvMemberApproval.SelectedItem;
                if (drv != null)
                {
                    if (MessageBox.Show(this, "Are you Sure to Decline this Member? It will Affect Current Data!", "Save Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        decimal dMemberCode = Convert.ToDecimal(drv["MEMBER_CODE"]);
                        var mm = (from x in db.MASTERMEMBERs where x.BranchMemberCode == dMemberCode select x).FirstOrDefault();
                        if (mm != null)
                        {
                            var sl = (from x in db.MemberStatusLogs where x.MEMBER_CODE == mm.MEMBER_CODE select x).FirstOrDefault();
                            if (sl != null)
                            {
                                db.MemberStatusLogs.Remove(sl);
                            }

                            var nm = (from x in db.MASTERNOMINEEs where x.MEMBER_CODE == mm.MEMBER_CODE select x).FirstOrDefault();
                            if (nm != null)
                            {
                                db.MASTERNOMINEEs.Remove(nm);
                            }

                            var gr = (from x in db.MASTERGUARDIANs where x.MEMBER_CODE == mm.MEMBER_CODE select x).FirstOrDefault();
                            if (gr != null)
                            {
                                db.MASTERGUARDIANs.Remove(gr);
                            }

                            db.MASTERMEMBERs.Remove(mm);
                            db.SaveChanges();
                        }

                        var wm = (from x in db.MemberInsertBranches where x.MEMBER_CODE == dMemberCode select x).FirstOrDefault();
                        if (wm != null)
                        {
                            wm.IsApproved = 2;
                            db.SaveChanges();
                        }
                        MessageBox.Show("Member Decline Sucessfully!", "Sucessfully");
                        txtBankName.Text = "";
                        txtBranchName.Text = "";
                        txtMF.Text = "";
                        txtICNo.Text = "";
                        txtMemberName.Text = "";
                        txtNubeBranch.Text = "";
                        txtMemberType.Text = "";
                        txtMemberNo.Text = "";
                        txtNoofMembers.Text = "";
                        load_Form();
                    }
                }
            }
        }

        private void txtBankName_TextChanged(object sender, TextChangedEventArgs e)
        {
            filteration();
        }

        private void txtBranchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            filteration();
        }

        private void txtICNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            filteration();
        }

        private void txtMF_TextChanged(object sender, TextChangedEventArgs e)
        {
            filteration();
        }

        private void txtMemberName_TextChanged(object sender, TextChangedEventArgs e)
        {
            filteration();
        }

        private void txtNubeBranch_TextChanged(object sender, TextChangedEventArgs e)
        {
            filteration();
        }

        private void txtMemberType_TextChanged(object sender, TextChangedEventArgs e)
        {
            filteration();
        }

        private void txtMemberNo_TextInput(object sender, TextCompositionEventArgs e)
        {
            filteration();
        }

        private void dtpFromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            filteration();
        }

        private void dtpTodate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            filteration();
        }

        private void txtMemberNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            filteration();
        }

        private void rbtPending_Click(object sender, RoutedEventArgs e)
        {
            dgvMemberApproval.Columns[1].Header = "Temp Membership No";
            dgvMemberApproval.Columns[9].Header = "Approve";

            filteration();
        }

        private void rbtApproved_Click(object sender, RoutedEventArgs e)
        {
            filteration();
            dgvMemberApproval.Columns[1].Header = "Membership No";
            dgvMemberApproval.Columns[9].Header = "Decline";
        }

        private void rbtDeclain_Click(object sender, RoutedEventArgs e)
        {
            filteration();
            dgvMemberApproval.Columns[1].Header = "Temp Membership No";
            dgvMemberApproval.Columns[9].Header = "Approve";
        }

        #endregion

        #region User Defined Functions

        void load_Form()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppLib.connStr))
                {
                    progressBar1.Minimum = 1;
                    progressBar1.Maximum = 10;
                    progressBar1.Visibility = Visibility.Visible;

                    SqlCommand cmd;
                    cmd = new SqlCommand("SPMEMBERAPPROVAL", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Add(new SqlParameter("@STATUS", "0"));
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    progressBar1.Value = 5;
                    System.Windows.Forms.Application.DoEvents();
                    dtMemberApproval.Rows.Clear();
                    dgvMemberApproval.ItemsSource = null;
                    adp.SelectCommand.CommandTimeout = 0;
                    adp.Fill(dtMemberApproval);
                    progressBar1.Value = 7;
                    System.Windows.Forms.Application.DoEvents();
                    if (dtMemberApproval.Rows.Count > 0)
                    {
                        progressBar1.Value = 9;
                        System.Windows.Forms.Application.DoEvents();
                        dgvMemberApproval.ItemsSource = dtMemberApproval.DefaultView;
                        txtNoofMembers.Text = dtMemberApproval.Rows.Count.ToString();
                        filteration();
                        progressBar1.Value = 10;
                        System.Windows.Forms.Application.DoEvents();
                    }
                    else
                    {
                        progressBar1.Value = 10;
                        System.Windows.Forms.Application.DoEvents();
                        MessageBox.Show("No Records Found");
                    }
                }
            }
            catch (Exception ex)
            {
                progressBar1.Visibility = Visibility.Hidden;
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        void filteration()
        {
            try
            {
                string sWhere1 = "";

                if (rbtPending.IsChecked == true)
                {
                    sWhere1 = " ISAPPROVED=0 ";
                }
                else if (rbtApproved.IsChecked == true)
                {
                    sWhere1 = " ISAPPROVED=1 ";
                }
                else if (rbtDeclain.IsChecked == true)
                {
                    sWhere1 = " ISAPPROVED=2 ";
                }


                if (!string.IsNullOrEmpty(txtBankName.Text))
                {

                    sWhere1 = sWhere1 + " AND BANK_USERCODE like '" + txtBankName.Text + "%'";
                }

                if (!string.IsNullOrEmpty(txtBranchName.Text))
                {
                    sWhere1 = sWhere1 + " AND BANKBRANCH_NAME like '" + txtBranchName.Text + "%'";

                }

                if (!string.IsNullOrEmpty(txtMF.Text))
                {

                    sWhere1 = sWhere1 + " AND SEX like'" + txtMF.Text + "%'";

                }

                if (!string.IsNullOrEmpty(txtICNo.Text))
                {

                    sWhere1 = sWhere1 + " AND ICNO_NEW like '%" + txtICNo.Text + "%' ";

                }

                if (!string.IsNullOrEmpty(txtMemberName.Text))
                {

                    sWhere1 = sWhere1 + " AND MEMBER_NAME like '%" + txtMemberName.Text + "%' ";

                }
                if (!string.IsNullOrEmpty(txtNubeBranch.Text))
                {
                    sWhere1 = sWhere1 + " AND NUBE_BRANCH_NAME like '" + txtNubeBranch.Text + "%'";

                }

                if (!string.IsNullOrEmpty(txtMemberType.Text))
                {

                    sWhere1 = sWhere1 + " AND MEMBERTYPE_NAME like '" + txtMemberType.Text + "%'";

                }

                if (!string.IsNullOrEmpty(txtMemberNo.Text))
                {

                    sWhere1 = sWhere1 + " AND MEMBER_ID=" + txtMemberNo.Text;

                }


                if (!string.IsNullOrEmpty(dtpFromDate.Text) && !string.IsNullOrEmpty(dtpTodate.Text))
                {

                    sWhere1 = sWhere1 + string.Format(" AND DATEOFJOINING >='{0:dd/MMM/yyyy}' AND DATEOFJOINING <='{1:dd/MMM/yyyy}'", dtpFromDate.SelectedDate, dtpTodate.SelectedDate);


                }

                else if (!string.IsNullOrEmpty(dtpFromDate.Text))
                {

                    sWhere1 = sWhere1 + string.Format(" AND DATEOFJOINING='{0:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate);

                }

                else if (!string.IsNullOrEmpty(dtpTodate.Text))
                {

                    sWhere1 = sWhere1 + string.Format(" AND DATEOFJOINING='{0:dd/MMM/yyyy}' ", dtpTodate.SelectedDate);

                }

                if (!string.IsNullOrEmpty(sWhere1))
                {
                    DataView dv = new DataView(dtMemberApproval);
                    dv.RowFilter = sWhere1;

                    DataTable dtTemp = new DataTable();
                    dtTemp = dv.ToTable();
                    int i = 0;
                    foreach (DataRow row in dtTemp.Rows)
                    {
                        row["RNO"] = i + 1;
                        i++;
                    }
                    dgvMemberApproval.ItemsSource = null;
                    dgvMemberApproval.ItemsSource = dtTemp.DefaultView;
                    txtNoofMembers.Text = dtTemp.Rows.Count.ToString();
                }
                else
                {
                    dgvMemberApproval.ItemsSource = null;
                    dgvMemberApproval.ItemsSource = dtMemberApproval.DefaultView;
                    txtNoofMembers.Text = dtMemberApproval.Rows.Count.ToString();
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
