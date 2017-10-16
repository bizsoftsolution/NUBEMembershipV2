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

namespace Nube
{
    /// <summary>
    /// Interaction logic for frmMemberHistory.xaml
    /// </summary>
    public partial class frmMemberHistory : MetroWindow
    {
        string connStr = AppLib.connstatus;
        nubebfsEntity dbBFS = new nubebfsEntity();
        string sFormName = "";
        public frmMemberHistory(string FormN = "")
        {
            InitializeComponent();
            sFormName = FormN;
        }
        public void FormLoad(decimal id, DateTime doj, string bank, string branch, string monBF, string monSub, string Type)
        {
            try
            {
                InitializeComponent();
                txtBankName.Text = bank;
                txtBranchkName.Text = branch;
                dtpDOJ.SelectedDate = doj.Date;
                txtType.Text = Type.ToString();
                txtMonthlyBF.Text = monBF.ToString();
                txtMonthlySub.Text = monSub.ToString();
                txtMemberID.Text = id.ToString();

                var vm = (from x in dbBFS.ViewMasterMembers where x.MEMBER_ID == id select x).FirstOrDefault();
                if (vm != null)
                {
                    txtBankName.Text = vm.BANK_USERCODE.ToString();
                    txtMemberName.Text = vm.MEMBER_NAME.ToString();
                    txtStatus.Text = vm.MEMBERSTATUS.ToString();
                    txtLastPaymentDate.Text = string.Format("{0:MMM-yyyy}", vm.LASTPAYMENT_DATE);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (sFormName == "MEMBER QUERY")
            {
                Transaction.frmMemberQuery frm = new Transaction.frmMemberQuery("HomeMember");
                this.Close();
                //frm.ShowDialog();
            }
            else
            {
                Transaction.frmMemberRegistration frm = new Transaction.frmMemberRegistration();
                this.Close();
                //frm.ShowDialog();
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtMemberID.Text))
                {
                    decimal dMemberId = Convert.ToDecimal(txtMemberID.Text);
                    var mm = (from x in dbBFS.MASTERMEMBERs where x.MEMBER_ID == dMemberId select x).FirstOrDefault();
                    if (mm != null)
                    {
                        using (SqlConnection con = new SqlConnection(AppLib.connStr))
                        {
                            progressBar1.Minimum = 1;
                            progressBar1.Maximum = 10;
                            progressBar1.Visibility = Visibility.Visible;
                            con.Open();
                            progressBar1.Value = 5;
                            System.Windows.Forms.Application.DoEvents();
                            SqlCommand cmd = new SqlCommand("SPMEMBERHISTORY", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@MEMBER_CODE", mm.MEMBER_CODE));
                            cmd.Parameters.Add(new SqlParameter("@DOJ", mm.DATEOFJOINING));
                            progressBar1.Value = 7;
                            System.Windows.Forms.Application.DoEvents();
                            SqlDataAdapter adp = new SqlDataAdapter(cmd);
                            adp.SelectCommand.CommandTimeout = 0;
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            progressBar1.Value = 8;
                            System.Windows.Forms.Application.DoEvents();
                            if (dt.Rows.Count > 0)
                            {
                                progressBar1.Value = 9;
                                System.Windows.Forms.Application.DoEvents();
                                dgvDetails.ItemsSource = dt.DefaultView;
                                progressBar1.Visibility = Visibility.Hidden;
                            }
                            else
                            {
                                MessageBox.Show("No Data Found");
                            }
                            progressBar1.Value = 10;
                            System.Windows.Forms.Application.DoEvents();
                            progressBar1.Visibility = Visibility.Hidden;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No Data Found", "Invalid Membership");
                        progressBar1.Visibility = Visibility.Hidden;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                progressBar1.Visibility = Visibility.Hidden;
            }
        }

        private void txtMemberID_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter && !string.IsNullOrEmpty(txtMemberID.Text))
                {
                    decimal dMemberId = Convert.ToDecimal(txtMemberID.Text);
                    var mm = (from x in dbBFS.TVMASTERMEMBERs where x.MEMBER_ID == dMemberId select x).FirstOrDefault();

                    if (mm != null)
                    {
                        dgvDetails.ItemsSource = null;
                        txtBankName.Text = mm.BANK_USERCODE;
                        txtBranchkName.Text = mm.BRANCHNAME;
                        dtpDOJ.SelectedDate = mm.DATEOFJOINING;
                        txtType.Text = mm.MEMBERTYPE_NAME;
                        txtMonthlyBF.Text = mm.MONTHLYBF.ToString();
                        txtMonthlySub.Text = mm.MONTHLYSUBSCRIPTION.ToString();
                        txtMemberName.Text = mm.MEMBER_NAME.ToString();
                        txtStatus.Text = mm.MEMBERSTATUS.ToString();
                        txtLastPaymentDate.Text = string.Format("{0:MMM-yyyy}", mm.LASTPAYMENT_DATE);


                        using (SqlConnection con = new SqlConnection(AppLib.connStr))
                        {
                            progressBar1.Minimum = 1;
                            progressBar1.Maximum = 10;
                            progressBar1.Visibility = Visibility.Visible;
                            con.Open();
                            progressBar1.Value = 2;
                            System.Windows.Forms.Application.DoEvents();
                            SqlCommand cmd = new SqlCommand("SPMEMBERHISTORY", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@MEMBER_CODE", mm.MEMBER_CODE));
                            progressBar1.Value = 5;
                            System.Windows.Forms.Application.DoEvents();
                            SqlDataAdapter adp = new SqlDataAdapter(cmd);
                            adp.SelectCommand.CommandTimeout = 0;
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            progressBar1.Value = 7;
                            System.Windows.Forms.Application.DoEvents();
                            if (dt.Rows.Count > 0)
                            {
                                progressBar1.Value = 9;
                                System.Windows.Forms.Application.DoEvents();
                                dgvDetails.ItemsSource = dt.DefaultView;
                            }
                            else
                            {
                                MessageBox.Show("No Data Found");
                            }
                            progressBar1.Value = 10;
                            System.Windows.Forms.Application.DoEvents();
                            progressBar1.Visibility = Visibility.Hidden;
                        }
                    }
                    else
                    {
                        txtBankName.Text = "";
                        txtBranchkName.Text = "";
                        dtpDOJ.Text = "";
                        txtType.Text = "";
                        txtMonthlyBF.Text = "";
                        txtMonthlySub.Text = "";
                        txtMemberName.Text = "";
                        txtStatus.Text = "";
                        txtLastPaymentDate.Text = "";
                        MessageBox.Show("No Data Found", "Invalid Membership");
                        progressBar1.Visibility = Visibility.Hidden;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                progressBar1.Visibility = Visibility.Hidden;
            }
        }
    }

    public class MemberStatus
    {
        public int RNO { get; set; }
        public DateTime HistoryDate { get; set; }
        public decimal SubscriptionPaid { get; set; }
        public decimal BFPaid { get; set; }
        public decimal TOTAL_MONTHS { get; set; }
        public DateTime LASTPAYMENTDATE { get; set; }
        public decimal TOTALMONTHSPAID { get; set; }
        public decimal TOTALMONTHSDUE { get; set; }
        public string DEFAULTINGMONTHS { get; set; }
        public decimal SUBSCRIPTIONDUE { get; set; }
        public decimal BFDUE { get; set; }
        public decimal ACCSUBSCRIPTION { get; set; }
        public decimal ACCBF { get; set; }
        public decimal ACCBENEFIT { get; set; }
        public decimal CURRENT_YDTSUBSCRIPTION { get; set; }
        public decimal CURRENT_YDTBF { get; set; }
        public decimal Status { get; set; }
    }
}