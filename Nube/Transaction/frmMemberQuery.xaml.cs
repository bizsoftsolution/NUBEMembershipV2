using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using Microsoft.Win32;
using System.Data.SqlClient;
using Nube.MasterSetup;

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmMemberQuery.xaml
    /// </summary>
    public partial class frmMemberQuery : MetroWindow
    {
        Boolean bIsClear = false;
        DataTable dtMember = new DataTable();
        String sFormName = "";
        decimal dMember_Code = 0;
        string conn = AppLib.connStr;
        nubebfsEntity db = new nubebfsEntity();
        public frmMemberQuery(string sForm_Name = "")
        {
            InitializeComponent();
            sFormName = sForm_Name;
            btnSearch.Focus();

            if (AppLib.dtMemberQuery.Rows.Count > 0)
            {
                dtMember = AppLib.dtMemberQuery.Copy();
                dgvDetails.ItemsSource = dtMember.DefaultView;
            }
            //else
            //{
            //    FormLoad();
            //}
        }

        #region"BUTTON EVENTS"

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FormLoad();
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
                txtBankUserCode.Text = "";
                txtICNo.Text = "";
                txtMemberName.Text = "";
                txtNubeBranch.Text = "";
                txtMemberType.Text = "";
                txtMemberNo.Text = "";
                txtNoofMembers.Text = "";

                bIsClear = true;
                ckbActive.IsChecked = false;
                ckbDefaulter.IsChecked = false;
                ckbResigned.IsChecked = false;
                ckbstrukoff.IsChecked = false;
                bIsClear = false;
                dtpFromDate.Text = "";
                dtpTodate.Text = "";

                AppLib.dtMemberQuery.Rows.Clear();
                FormLoad();

                //dgvDetails.ItemsSource = dtMember.DefaultView;
                //txtNoofMembers.Text = dtMember.Rows.Count.ToString();

                //AppLib.lstMstMember.Clear();
                //var lstMM = (from x in db.ViewMasterMembers select x).ToList();
                //AppLib.lstMstMember = lstMM;
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sFormName == "HomeMember")
                {
                    frmHomeMembership frm = new frmHomeMembership();
                    this.Close();
                    frm.ShowDialog();
                }
                else if (sFormName == "Registration")
                {
                    frmMemberRegistration frm = new frmMemberRegistration(0);
                    this.Close();
                    frm.ShowDialog();
                }
                else if (sFormName == "PostArrear")
                {
                    frmArrearPost16 frm = new frmArrearPost16(0);
                    this.Close();
                    frm.ShowDialog();
                }
                else if (sFormName == "PreArrear")
                {
                    frmArrearPre16 frm = new frmArrearPre16(0);
                    this.Close();
                    frm.ShowDialog();
                }
                else if (sFormName == "Resingation")
                {
                    frmResingation frm = new frmResingation(0);
                    this.Close();
                    frm.ShowDialog();
                }
                else if (sFormName == "Transfer")
                {
                    frmTransfer frm = new frmTransfer(0);
                    this.Close();
                    frm.ShowDialog();
                }
                else if (sFormName == "LEVY")
                {
                    frmLevy frm = new frmLevy(0);
                    this.Close();
                    frm.ShowDialog();
                }

                else if (sFormName == "TDF")
                {
                    frmTDF frm = new frmTDF(0);
                    this.Close();
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.OriginalSource as Button;
                decimal dMemberId = Convert.ToDecimal(btn.Tag);
                var mm = (from x in db.ViewMasterMembers where x.MEMBER_ID == dMemberId select x).FirstOrDefault();

                if (mm != null)
                {
                    frmMemberHistory history = new frmMemberHistory("MEMBER QUERY");
                    history.FormLoad(Convert.ToDecimal(btn.Tag), Convert.ToDateTime(mm.DATEOFJOINING), mm.BANK_USERCODE, mm.BRANCHNAME, mm.MONTHLYBF.ToString(), mm.MONTHLYSUBSCRIPTION.ToString(), mm.MEMBERTYPE_NAME);
                    //this.Close();
                    history.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion

        #region"OTHER EVENTS"                   

        private void txtMemberNo_TextInput(object sender, TextCompositionEventArgs e)
        {
            AppLib.CheckIsNumeric(e);
        }

        private void ckbActive_Click(object sender, RoutedEventArgs e)
        {
            if (bIsClear == false)
            {
                filteration();
            }
        }

        private void ckbDefaulter_Click(object sender, RoutedEventArgs e)
        {
            if (bIsClear == false)
            {
                filteration();
            }
        }

        private void ckbstrukoff_Click(object sender, RoutedEventArgs e)
        {
            if (bIsClear == false)
            {
                filteration();
            }
        }

        private void ckbResigned_Click(object sender, RoutedEventArgs e)
        {
            if (bIsClear == false)
            {
                filteration();
            }
        }

        private void txtBankName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bIsClear == false)
            {
                filteration();
            }
        }

        private void txtBranchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bIsClear == false)
            {
                filteration();
            }
        }

        private void txtBankUserCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bIsClear == false)
            {
                filteration();
            }
        }

        private void txtICNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bIsClear == false)
            {
                filteration();
            }
        }

        private void txtMemberName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bIsClear == false)
            {
                filteration();
            }
        }

        private void txtNubeBranch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bIsClear == false)
            {
                filteration();
            }
        }

        private void txtMemberType_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bIsClear == false)
            {
                filteration();
            }
        }

        private void txtMemberNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bIsClear == false)
            {
                filteration();
            }
        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //if ((sFormName == "Registration" || sFormName == "LEVY" || sFormName == "TDF" || sFormName == "Resingation" ||
                //    sFormName == "Transfer" || sFormName == "PostArrear" || sFormName == "PreArrear" || sFormName == "HomeMember" ||
                //    sFormName== "Annual Statement") && (dgvDetails.SelectedItem != null))

                if ((dgvDetails.SelectedItem != null))
                {

                    DataRowView drv = (DataRowView)dgvDetails.SelectedItem;
                    dMember_Code = Convert.ToDecimal(drv["MEMBER_CODE"]);
                    if (dMember_Code != 0)
                    {
                        if (sFormName == "Transfer")
                        {
                            frmTransfer frm = new frmTransfer(dMember_Code);
                            frm.Show();
                            this.Hide();
                        }
                        else if (sFormName == "PostArrear")
                        {
                            frmArrearPost16 frm = new frmArrearPost16(dMember_Code, 0);
                            frm.Show();
                            this.Hide();
                        }
                        else if (sFormName == "PreArrear")
                        {
                            frmArrearPre16 frm = new frmArrearPre16(dMember_Code, 0);
                            frm.Show();
                            this.Hide();
                        }
                        else if (sFormName == "Resingation")
                        {
                            frmResingation frm = new frmResingation(dMember_Code);
                            frm.Show();
                            this.Hide();
                        }
                        else if (sFormName == "LEVY")
                        {
                            frmLevy frm = new frmLevy(dMember_Code);
                            frm.Show();
                            this.Close();
                        }

                        else if (sFormName == "TDF")
                        {
                            frmTDF frm = new frmTDF(dMember_Code);
                            frm.Show();
                            this.Close();
                        }
                        else if (sFormName == "Annual Statement")
                        {
                            Reports.frmAnnualStatement frm = new Reports.frmAnnualStatement(dMember_Code);
                            frm.Show();
                            this.Close();
                        }
                        else
                        {
                            frmMemberRegistration frm = new frmMemberRegistration(dMember_Code, false);
                            frm.Show();
                            this.Hide();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void dtpFromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bIsClear == false)
            {
                filteration();
            }
        }

        private void dtpTodate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bIsClear == false)
            {
                filteration();
            }
        }

        #endregion

        #region "USER DEFINED FUNCTIONS"   

        void FormLoad()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conn))
                {
                    progressBar1.Minimum = 1;
                    progressBar1.Maximum = 10;
                    progressBar1.Visibility = Visibility.Visible;

                    if (AppLib.dtMemberQuery.Rows.Count > 0)
                    {
                        dtMember = AppLib.dtMemberQuery.Copy();
                    }
                    else
                    {
                        //SqlCommand cmd;
                        //cmd = new SqlCommand("SPMEMBERSHIP", con);
                        //cmd.CommandType = CommandType.StoredProcedure;
                        //SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        progressBar1.Value = 5;
                        //System.Windows.Forms.Application.DoEvents();
                        //dtMember.Rows.Clear();
                        //adp.SelectCommand.CommandTimeout = 0;
                        //adp.Fill(dtMember);
                        var ms = db.SPMEMBERSHIP().ToList();                        
                        dtMember = AppLib.LINQResultToDataTable(ms);
                        dgvDetails.ItemsSource = dtMember.DefaultView;
                        AppLib.dtMemberQuery = dtMember.Copy();
                    }

                    progressBar1.Value = 7;
                    System.Windows.Forms.Application.DoEvents();
                    if (dtMember.Rows.Count > 0)
                    {
                        progressBar1.Value = 9;
                        System.Windows.Forms.Application.DoEvents();
                        dgvDetails.ItemsSource = dtMember.DefaultView;
                        txtNoofMembers.Text = dtMember.Rows.Count.ToString();
                        filteration();
                    }
                    progressBar1.Value = 10;
                    System.Windows.Forms.Application.DoEvents();                    
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
                string sWhere = "";
                string sWhere1 = "";
                string sWhere2 = "";
                string sWhere3 = "";

                if (!string.IsNullOrEmpty(txtBankName.Text))
                {
                    if (!string.IsNullOrEmpty(sWhere1))
                    {
                        sWhere1 = sWhere1 + " AND BANKUSER_CODE like '" + txtBankName.Text + "%'";
                    }
                    else
                    {
                        sWhere1 = sWhere1 + "BANKUSER_CODE like '" + txtBankName.Text + "%'";
                    }
                }

                if (!string.IsNullOrEmpty(txtBranchName.Text))
                {
                    if (!string.IsNullOrEmpty(sWhere1))
                    {
                        sWhere1 = sWhere1 + " AND BRANCH_NAME like '" + txtBranchName.Text + "%'";
                    }
                    else
                    {
                        sWhere1 = sWhere1 + "BRANCH_NAME like '" + txtBranchName.Text + "%'";
                    }
                }

                if (!string.IsNullOrEmpty(txtBankUserCode.Text))
                {
                    if (!string.IsNullOrEmpty(sWhere1))
                    {
                        sWhere1 = sWhere1 + " AND SEX_MF like'" + txtBankUserCode.Text + "%'";
                    }
                    else
                    {
                        sWhere1 = sWhere1 + "SEX_MF like'" + txtBankUserCode.Text + "%'";
                    }
                }

                if (!string.IsNullOrEmpty(txtICNo.Text))
                {
                    if (!string.IsNullOrEmpty(sWhere1))
                    {
                        sWhere1 = sWhere1 + " AND (ICNO_NEW like '%" + txtICNo.Text + "%' OR ICNO_OLD like '%" + txtICNo.Text + "%' OR NRIC_BYBANK like '%" + txtICNo.Text + "%') ";
                    }
                    else
                    {
                        sWhere1 = sWhere1 + " (ICNO_NEW like '%" + txtICNo.Text + "%' OR ICNO_OLD like '%" + txtICNo.Text + "%' OR NRIC_BYBANK like '%" + txtICNo.Text + "%') ";
                    }
                }

                if (!string.IsNullOrEmpty(txtMemberName.Text))
                {
                    if (!string.IsNullOrEmpty(sWhere1))
                    {
                        sWhere1 = sWhere1 + " AND (MEMBER_NAME like '%" + txtMemberName.Text + "%' OR MEMBERNAME_BYBANK like '%" + txtMemberName.Text + "%') ";
                    }
                    else
                    {
                        sWhere1 = sWhere1 + " (MEMBER_NAME like '%" + txtMemberName.Text + "%' OR MEMBERNAME_BYBANK like '%" + txtMemberName.Text + "%') ";
                    }
                }
                if (!string.IsNullOrEmpty(txtNubeBranch.Text))
                {
                    if (!string.IsNullOrEmpty(sWhere1))
                    {
                        sWhere1 = sWhere1 + " AND NUBEBRANCH_NAME like '" + txtNubeBranch.Text + "%'";
                    }
                    else
                    {
                        sWhere1 = sWhere1 + "NUBEBRANCH_NAME like '" + txtNubeBranch.Text + "%'";
                    }
                }

                if (!string.IsNullOrEmpty(txtMemberType.Text))
                {
                    if (!string.IsNullOrEmpty(sWhere1))
                    {
                        sWhere1 = sWhere1 + " AND MEMBERTYPE_NAME like '" + txtMemberType.Text + "%'";
                    }
                    else
                    {
                        sWhere1 = sWhere1 + "MEMBERTYPE_NAME like '" + txtMemberType.Text + "%'";
                    }
                }

                if (!string.IsNullOrEmpty(txtMemberNo.Text))
                {
                    if (!string.IsNullOrEmpty(sWhere1))
                    {
                        sWhere1 = sWhere1 + " AND MEMBER_ID=" + txtMemberNo.Text;
                    }
                    else
                    {
                        sWhere1 = sWhere1 + "MEMBER_ID=" + txtMemberNo.Text;
                    }
                }


                if (ckbActive.IsChecked == true)
                {
                    if (!string.IsNullOrEmpty(sWhere2))
                    {
                        sWhere2 = sWhere2 + " OR MEMBERSTATUSCODE=1";
                    }
                    else
                    {
                        sWhere2 = sWhere2 + "MEMBERSTATUSCODE=1";
                    }
                }

                if (ckbDefaulter.IsChecked == true)
                {
                    if (!string.IsNullOrEmpty(sWhere2))
                    {
                        sWhere2 = sWhere2 + " OR MEMBERSTATUSCODE=2";
                    }
                    else
                    {
                        sWhere2 = sWhere2 + "MEMBERSTATUSCODE=2";
                    }
                }

                if (ckbstrukoff.IsChecked == true)
                {
                    if (!string.IsNullOrEmpty(sWhere2))
                    {
                        sWhere2 = sWhere2 + " OR MEMBERSTATUSCODE=3";
                    }
                    else
                    {
                        sWhere2 = sWhere2 + "MEMBERSTATUSCODE=3";
                    }
                }

                if (ckbResigned.IsChecked == true)
                {
                    if (!string.IsNullOrEmpty(sWhere2))
                    {
                        sWhere2 = sWhere2 + " OR MEMBERSTATUSCODE=6";
                    }
                    else
                    {
                        sWhere2 = sWhere2 + "MEMBERSTATUSCODE=6";
                    }
                }

                if (!string.IsNullOrEmpty(dtpFromDate.Text) && !string.IsNullOrEmpty(dtpTodate.Text))
                {
                    if (!string.IsNullOrEmpty(sWhere1))
                    {
                        sWhere1 = sWhere1 + string.Format(" AND DATEOFJOINING >='{0:dd/MMM/yyyy}' AND DATEOFJOINING <='{1:dd/MMM/yyyy}'", dtpFromDate.SelectedDate, dtpTodate.SelectedDate);
                    }
                    else
                    {
                        sWhere1 = sWhere1 + string.Format(" DATEOFJOINING >='{0:dd/MMM/yyyy}' AND DATEOFJOINING <='{1:dd/MMM/yyyy}'", dtpFromDate.SelectedDate, dtpTodate.SelectedDate);
                    }

                }

                else if (!string.IsNullOrEmpty(dtpFromDate.Text))
                {
                    if (!string.IsNullOrEmpty(sWhere1))
                    {
                        sWhere1 = sWhere1 + string.Format(" AND DATEOFJOINING='{0:dd/MMM/yyyy}' ", dtpFromDate.SelectedDate);
                    }
                    else
                    {
                        sWhere1 = sWhere1 + string.Format(" DATEOFJOINING='{0:dd/MMM/yyyy}'", dtpFromDate.SelectedDate);
                    }
                }

                else if (!string.IsNullOrEmpty(dtpTodate.Text))
                {
                    if (!string.IsNullOrEmpty(sWhere1))
                    {
                        sWhere1 = sWhere1 + string.Format(" AND DATEOFJOINING='{0:dd/MMM/yyyy}' ", dtpTodate.SelectedDate);
                    }
                    else
                    {
                        sWhere1 = sWhere1 + string.Format(" DATEOFJOINING='{0:dd/MMM/yyyy}'", dtpTodate.SelectedDate);
                    }
                }


                if (!string.IsNullOrEmpty(sWhere2))
                {
                    if (!string.IsNullOrEmpty(sWhere1))
                    {
                        sWhere = "(" + sWhere1 + ")" + " AND (" + sWhere2 + ")";
                    }
                    else
                    {
                        sWhere = sWhere2;
                    }
                }
                else
                {
                    sWhere = sWhere1;
                }

                if (!string.IsNullOrEmpty(sWhere))
                {
                    DataView dv = new DataView(dtMember);
                    dv.RowFilter = sWhere;

                    DataTable dtTemp = new DataTable();
                    dtTemp = dv.ToTable();
                    int i = 0;
                    foreach (DataRow row in dtTemp.Rows)
                    {
                        row["RNO"] = i + 1;
                        i++;
                    }
                    dgvDetails.ItemsSource = null;
                    dgvDetails.ItemsSource = dtTemp.DefaultView;
                    txtNoofMembers.Text = dtTemp.Rows.Count.ToString();
                }
                else
                {
                    dgvDetails.ItemsSource = null;
                    dgvDetails.ItemsSource = dtMember.DefaultView;
                    txtNoofMembers.Text = dtMember.Rows.Count.ToString();
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
