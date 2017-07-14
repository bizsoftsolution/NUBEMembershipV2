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
using System.Data.SqlClient;
using System.Data;

namespace Nube.Reports
{
    /// <summary>
    /// Interaction logic for frmFeeReport.xaml
    /// </summary>
    public partial class frmFeeReport : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        public frmFeeReport()
        {
            InitializeComponent();
            try
            {
                var NUBE = db.MASTERNUBEBRANCHes.OrderBy(x => x.NUBE_BRANCH_NAME).ToList();
                cmbNubeBranch.ItemsSource = NUBE;
                cmbNubeBranch.SelectedValuePath = "NUBE_BRANCH_CODE";
                cmbNubeBranch.DisplayMemberPath = "NUBE_BRANCH_NAME";

                var bank = db.MASTERBANKs.OrderBy(x => x.BANK_NAME).ToList();
                cmbBank.ItemsSource = bank;
                cmbBank.SelectedValuePath = "BANK_CODE";
                cmbBank.DisplayMemberPath = "BANK_NAME";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #region "BUTTON EVENTS"

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(dtpDate.Text.ToString()))
                {
                    FormLoad();
                }
                else
                {
                    MessageBox.Show("Please Select the Date!");
                    dtpDate.Focus();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dtpDate.Text = "";
                rbtnAll.IsChecked = true;
                dgFeeDetail.ItemsSource = null;
                cmbBank.Text = "";
                cmbBranch.Text = "";
                cmbNubeBranch.Text = "";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmHomeReports frm = new frmHomeReports();
                this.Close();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void cmbBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Decimal d = Convert.ToDecimal(cmbBank.SelectedValue);
                var branch = db.MASTERBANKBRANCHes.Where(x => x.BANK_CODE == d).OrderBy(x => x.BANKBRANCH_NAME).ToList();
                cmbBranch.ItemsSource = branch;
                cmbBranch.SelectedValuePath = "BANKBRANCH_CODE";
                cmbBranch.DisplayMemberPath = "BANKBRANCH_NAME";
            }
            catch (Exception ex)
            {
                Nube.ExceptionLogging.SendErrorToText(ex);
            }
        }

        #endregion

        #region "USER DEFINED FUNCTION"

        void FormLoad()
        {
            string sWhere = "";
            if (dtpDate.SelectedDate <= Convert.ToDateTime("31/MAR/2016").Date)
            {
                if (!string.IsNullOrEmpty(dtpDate.Text.ToString()))
                {
                    sWhere = string.Format(" WHERE FD.FEE_YEAR={0} AND FD.FEE_MONTH={1} ", Convert.ToDateTime(dtpDate.SelectedDate).Year, Convert.ToDateTime(dtpDate.SelectedDate).Month);
                }

                if (rbtnActive.IsChecked == true)
                {
                    sWhere = sWhere + " AND FD.TOTAL_MONTHS>0  ";
                }
                else if (rbtnNotMatch.IsChecked == true)
                {
                    sWhere = sWhere + " AND FD.TOTAL_MONTHS<=0 ";
                }
                else if (rbtnUnPaid.IsChecked == true)
                {
                    sWhere = sWhere + " AND FD.TOTAL_MONTHS<=0 ";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(dtpDate.Text.ToString()))
                {
                    sWhere = string.Format(" WHERE FD.FEEYEAR={0} AND FD.FEEMONTH={1} ", Convert.ToDateTime(dtpDate.SelectedDate).Year, Convert.ToDateTime(dtpDate.SelectedDate).Month);
                }

                if (rbtnActive.IsChecked == true)
                {
                    sWhere = sWhere + " AND ISNOTMATCH=0 AND ISUNPAID=0 ";
                }
                else if (rbtnNotMatch.IsChecked == true)
                {
                    sWhere = sWhere + " AND ISNOTMATCH=1 ";
                }
                else if (rbtnUnPaid.IsChecked == true)
                {
                    sWhere = sWhere + " AND ISUNPAID=1 ";
                }
            }


            if (!string.IsNullOrEmpty(cmbNubeBranch.Text))
            {
                if (!string.IsNullOrEmpty(sWhere))
                {
                    sWhere = sWhere + " AND MM.NUBEBANCHNAME='" + cmbNubeBranch.Text + "' ";
                }
                else
                {
                    sWhere = sWhere + " MM.NUBEBANCHNAME='" + cmbNubeBranch.Text + "' ";
                }
            }

            if (!string.IsNullOrEmpty(cmbBank.Text))
            {
                if (!string.IsNullOrEmpty(sWhere))
                {
                    sWhere = sWhere + " AND MM.BANK_NAME='" + cmbBank.Text + "' ";
                }
                else
                {
                    sWhere = sWhere + " MM.BANK_NAME='" + cmbBank.Text + "' ";
                }
            }

            if (!string.IsNullOrEmpty(cmbBranch.Text))
            {
                if (!string.IsNullOrEmpty(sWhere))
                {
                    sWhere = sWhere + " AND MM.BRANCHNAME='" + cmbBranch.Text + "' ";
                }
                else
                {
                    sWhere = sWhere + " MM.BRANCHNAME='" + cmbBranch.Text + "' ";
                }
            }

            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppLib.connStr))
            {
                string str = "";
                if (dtpDate.SelectedDate <= Convert.ToDateTime("31/MAR/2016").Date)
                {
                    str = " SELECT '' NO,0 DETAILID,0 FEEID,FD.MEMBER_CODE,ISNULL(MM.MEMBER_ID,0)MEMBERID,ISNULL(MM.MEMBER_NAME,'')MEMBER_NAME, " +
                           " (FD.TOTALBF_AMOUNT+FD.TOTALSUBCRP_AMOUNT) TOTALAMOUNT,'' DEPT,FD.TOTALBF_AMOUNT AMOUNTBF,0 AMOUNTINS,TOTALSUBCRP_AMOUNT AMTSUBS,'' REASON " +
                           " FROM FEE_STATUS FD(NOLOCK) " +
                           " LEFT JOIN TEMPVIEWMASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE = FD.MEMBER_CODE " + sWhere +
                           " ORDER BY MM.MEMBER_NAME ";
                }
                else
                {
                    str = " SELECT '' NO,FD.DETAILID,FD.FEEID,FD.MEMBERCODE,ISNULL(MM.MEMBER_ID,0)MEMBERID,ISNULL(MM.MEMBER_NAME,'')MEMBER_NAME, " +
                           " FD.TOTALAMOUNT,ISNULL(FD.DEPT, '')DEPT,FD.AMOUNTBF,AMOUNTINS,AMTSUBS,ISNULL(REASON, '')REASON " +
                           " FROM FEESDETAILS FD(NOLOCK) " +
                           " LEFT JOIN TEMPVIEWMASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE = FD.MEMBERCODE " + sWhere +
                           " ORDER BY MM.MEMBER_NAME ";
                }

                SqlCommand cmd = new SqlCommand(str, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    int i = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        i++;
                        dr["NO"] = i;
                    }
                    dgFeeDetail.ItemsSource = dt.DefaultView;
                }
                else
                {
                    MessageBox.Show("No Data Foud!");
                    dgFeeDetail.ItemsSource = null;
                }
            }
        }

        #endregion


    }
}
