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

                lblFilterOption.Visibility = Visibility.Collapsed;
                rbtUnion.Visibility = Visibility.Collapsed;
                rbtSeparate.Visibility = Visibility.Collapsed;
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
                txtTotalAmount.Text = "";
                txtTotalBF.Text = "";
                txtTotalIns.Text = "";
                txtTotalSubs.Text = "";
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

                if (Convert.ToInt32(cmbBank.SelectedValue) != 0)
                {
                    lblFilterOption.Visibility = Visibility.Visible;
                    rbtUnion.Visibility = Visibility.Visible;
                    rbtSeparate.Visibility = Visibility.Visible;
                }
                else
                {
                    lblFilterOption.Visibility = Visibility.Collapsed;
                    rbtUnion.Visibility = Visibility.Collapsed;
                    rbtSeparate.Visibility = Visibility.Collapsed;
                }
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
            dgFeeDetail.ItemsSource = null;
            string sWhere = "";
            string sWhere1 = "";
            string sWhere2 = "";
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

                if (!string.IsNullOrEmpty(cmbNubeBranch.Text))
                {
                    if (!string.IsNullOrEmpty(sWhere))
                    {
                        sWhere = sWhere + " AND FD.NUBE_BRANCH_CODE=" + cmbNubeBranch.SelectedValue;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(dtpDate.Text.ToString()))
                {
                    sWhere = string.Format(" WHERE FD.FEEYEAR={0} AND FD.FEEMONTH={1}  ", Convert.ToDateTime(dtpDate.SelectedDate).Year, Convert.ToDateTime(dtpDate.SelectedDate).Month);
                }

                if ((rbtnArrear.IsChecked == false))
                {
                    sWhere = sWhere + " AND FD.STATUS <> 'ARREAR ENTRY' ";
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
                else if (rbtnArrear.IsChecked == true)
                {
                    sWhere = sWhere + " AND FD.STATUS<>'FEES ENTRY' ";
                }
                else
                {
                    sWhere1 = sWhere1 + " AND FD.ISNOTMATCH=1 ";
                    sWhere2 = sWhere2 + " AND FD.ISNOTMATCH=0 ";
                }

                if (!string.IsNullOrEmpty(cmbNubeBranch.Text))
                {
                    if (!string.IsNullOrEmpty(sWhere))
                    {
                        sWhere = sWhere + " AND MB.NUBE_BRANCH_CODE=" + cmbNubeBranch.SelectedValue;
                    }
                }
            }

            decimal dBankCode = 0;
            if (!string.IsNullOrEmpty(cmbBank.Text))
            {
                dBankCode = Convert.ToDecimal(cmbBank.SelectedValue);
            }

            if (!string.IsNullOrEmpty(cmbBank.Text) && (dtpDate.SelectedDate <= Convert.ToDateTime("31/MAR/2016").Date))
            {
                sWhere = sWhere + " AND FD.BANK_CODE=" + dBankCode;
            }
            else if (!string.IsNullOrEmpty(cmbBank.Text) && rbtSeparate.IsChecked == true && (dtpDate.SelectedDate > Convert.ToDateTime("31/MAR/2016").Date))
            {
                var st = (from x in db.MASTERBANKs where x.BANK_CODE == dBankCode && x.HEADER_BANK_CODE == 0 select x).ToList();
                if (st.Count > 0)
                {
                    sWhere = sWhere + " AND FM.BANKID IN (" + cmbBank.SelectedValue + ")";
                    var exp = (from x in db.MASTERBANKs where x.HEADER_BANK_CODE == dBankCode select x).FirstOrDefault();
                    if (exp != null)
                    {
                        sWhere = sWhere + " AND MM.BANK_CODE NOT IN (" + exp.BANK_CODE.ToString() + ") ";
                    }
                }
                else
                {
                    sWhere = sWhere + " AND MM.BANK_CODE IN (" + cmbBank.SelectedValue + ")";
                }
            }
            else if (!string.IsNullOrEmpty(cmbBank.Text) && rbtUnion.IsChecked == true)
            {
                var st = (from x in db.MASTERBANKs where x.BANK_CODE == dBankCode || x.HEADER_BANK_CODE == dBankCode select x).ToList();
                if (st != null)
                {
                    DataTable dtBank = AppLib.LINQResultToDataTable(st);
                    string str = "";
                    foreach (DataRow dr in dtBank.Rows)
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            str = dr["BANK_CODE"].ToString();
                        }
                        else
                        {
                            str = str + "," + dr["BANK_CODE"].ToString();
                        }
                        if (Convert.ToInt32(dr["HEADER_BANK_CODE"]) > 0)
                        {
                            str = str + "," + dr["HEADER_BANK_CODE"].ToString();
                        }
                    }
                    sWhere = sWhere + " AND FM.BANKID IN (" + str + ") ";
                }
                else
                {
                    sWhere = sWhere + " AND FM.BANKID=" + cmbBank.SelectedValue + " ";
                }
            }

            if (!string.IsNullOrEmpty(cmbBranch.Text))
            {
                sWhere = sWhere + " AND MM.BRANCH_CODE=" + cmbBranch.SelectedValue;
            }

            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppLib.connStr))
            {
                string str = "";
                if (dtpDate.SelectedDate <= Convert.ToDateTime("31/MAR/2016").Date)
                {
                    str = " SELECT '' NO,0 DETAILID,0 FEEID,FD.MEMBER_CODE,ISNULL(MM.MEMBER_ID,0)MEMBERID,ISNULL(MM.MEMBER_NAME,'')MEMBER_NAME, \r" +
                           " CASE WHEN ISNULL(MM.ICNO_NEW,'')<>'' THEN MM.ICNO_NEW ELSE MM.ICNO_OLD END NRIC,\r" +
                           " (FD.TOTALBF_AMOUNT+FD.TOTALSUBCRP_AMOUNT) TOTALAMOUNT,'' DEPT,FD.TOTALBF_AMOUNT AMOUNTBF,0 AMOUNTINS,TOTALSUBCRP_AMOUNT AMTSUBS,'' REASON \r" +
                           " FROM nubestatus..FEE_STATUS FD(NOLOCK) \r" +
                           " LEFT JOIN MASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE = FD.MEMBER_CODE \r" + sWhere +
                           " ORDER BY MM.MEMBER_NAME ";
                }
                else
                {
                    if (rbtnNotMatch.IsChecked == true || rbtnUnPaid.IsChecked == true)
                    {
                        str = " SELECT '' NO,FD.DETAILID,FD.FEEID,FD.MEMBERCODE,ISNULL(MM.MEMBER_ID,0)MEMBERID, \r" +
                        " ISNULL(MM.MEMBER_NAME,FD.MEMBERNAME_BANK)MEMBER_NAME,CASE WHEN ISNULL(MM.ICNO_NEW,'')<>'' THEN MM.ICNO_NEW ELSE ISNULL(FD.NRIC_Bank,'') END NRIC, \r" +
                        " FD.TOTALAMOUNT,ISNULL(FD.DEPT, '')DEPT,FD.AMOUNTBF,FD.UNIONCONTRIBUTION AMOUNTINS,AMTSUBS,ISNULL(REASON, '')REASON \r" +
                        " FROM FEESDETAILSNOTMATCH FD(NOLOCK) \r" +
                        " LEFT JOIN FEESMASTER FM(NOLOCK) ON FM.FEEID=FD.FEEID \r" +
                        " LEFT JOIN MASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE = FD.MEMBERCODE \r" +
                        " LEFT JOIN MASTERBANKBRANCH MB(NOLOCK) ON MB.BANKBRANCH_CODE=MM.BRANCH_CODE \r" + sWhere +
                        " ORDER BY MM.MEMBER_NAME ";
                    }
                    else if ((rbtnNotMatch.IsChecked == false && rbtnUnPaid.IsChecked == false) && (rbtnArrear.IsChecked == true || rbtnActive.IsChecked == true))
                    {
                        str = " SELECT '' NO,FD.DETAILID,FD.FEEID,FD.MEMBERCODE,ISNULL(MM.MEMBER_ID,0)MEMBERID, \r" +
                       " ISNULL(MM.MEMBER_NAME,'')MEMBER_NAME,CASE WHEN ISNULL(MM.ICNO_NEW,'')<>'' THEN MM.ICNO_NEW ELSE MM.ICNO_OLD END NRIC, \r" +
                       " FD.TOTALAMOUNT,ISNULL(FD.DEPT, '')DEPT,FD.AMOUNTBF,FD.UNIONCONTRIBUTION AMOUNTINS,AMTSUBS,ISNULL(REASON, '')REASON \r" +
                       " FROM FEESDETAILS FD(NOLOCK) \r" +
                       " LEFT JOIN FEESMASTER FM(NOLOCK) ON FM.FEEID=FD.FEEID \r" +
                       " LEFT JOIN MASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE = FD.MEMBERCODE \r" +
                       " LEFT JOIN MASTERBANKBRANCH MB(NOLOCK) ON MB.BANKBRANCH_CODE=MM.BRANCH_CODE \r" + sWhere +
                       " ORDER BY MM.MEMBER_NAME ";
                    }
                    else
                    {
                        str = " SELECT NO,DETAILID,FEEID,MEMBERCODE,MEMBERID,MEMBER_NAME,NRIC,TOTALAMOUNT,DEPT,AMOUNTBF,AMOUNTINS,AMTSUBS,REASON \r" +
                         " FROM(\r" +
                         " SELECT '' NO, FD.DETAILID, FD.FEEID, FD.MEMBERCODE, ISNULL(MM.MEMBER_ID, 0)MEMBERID,\r" +
                         " ISNULL(MM.MEMBER_NAME, '')MEMBER_NAME, CASE WHEN ISNULL(MM.ICNO_NEW, '') <> '' THEN MM.ICNO_NEW ELSE MM.ICNO_OLD END NRIC,\r" +
                         " FD.TOTALAMOUNT, ISNULL(FD.DEPT, '')DEPT, FD.AMOUNTBF,FD.UNIONCONTRIBUTION AMOUNTINS, AMTSUBS, ISNULL(REASON, '')REASON\r" +
                         " FROM FEESDETAILS FD(NOLOCK)\r" +
                         " LEFT JOIN FEESMASTER FM(NOLOCK) ON FM.FEEID = FD.FEEID\r" +
                         " LEFT JOIN MASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE = FD.MEMBERCODE\r" +
                         " LEFT JOIN MASTERBANKBRANCH MB(NOLOCK) ON MB.BANKBRANCH_CODE = MM.BRANCH_CODE\r" +
                         sWhere + sWhere2 + "\r" +
                         " UNION ALL\r" +
                         " SELECT '' NO, FD.DETAILID, FD.FEEID, FD.MEMBERCODE, ISNULL(MM.MEMBER_ID, 0)MEMBERID,\r" +
                         " ISNULL(MM.MEMBER_NAME,FD.MEMBERNAME_BANK)MEMBER_NAME, CASE WHEN ISNULL(MM.ICNO_NEW, '') <> '' THEN MM.ICNO_NEW ELSE ISNULL(FD.NRIC_Bank,'') END NRIC,\r" +
                         " FD.TOTALAMOUNT, ISNULL(FD.DEPT, '')DEPT, FD.AMOUNTBF,FD.UNIONCONTRIBUTION AMOUNTINS, AMTSUBS, ISNULL(REASON, '')REASON\r" +
                         " FROM FEESDETAILSNOTMATCH FD(NOLOCK)\r" +
                         " LEFT JOIN FEESMASTER FM(NOLOCK) ON FM.FEEID = FD.FEEID\r" +
                         " LEFT JOIN MASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE = FD.MEMBERCODE\r" +
                         " LEFT JOIN MASTERBANKBRANCH MB(NOLOCK) ON MB.BANKBRANCH_CODE = MM.BRANCH_CODE\r" +
                         sWhere + sWhere1 + " )TEMP\r" +
                         " ORDER BY MEMBER_NAME";
                    }
                }

                SqlCommand cmd = new SqlCommand(str, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(dt);
                decimal dTotalAmount = 0; decimal dTotalBF = 0;
                decimal dTotalIns = 0; decimal dTotalSubs = 0;
                if (dt.Rows.Count > 0)
                {
                    int i = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        i++;
                        dr["NO"] = i;
                        dTotalAmount = dTotalAmount + Convert.ToDecimal(dr["TOTALAMOUNT"]);
                        dTotalBF = dTotalBF + Convert.ToDecimal(dr["AMOUNTBF"]);
                        dTotalIns = dTotalIns + Convert.ToDecimal(dr["AMOUNTINS"]);
                        dTotalSubs = dTotalSubs + Convert.ToDecimal(dr["AMTSUBS"]);

                    }
                    dgFeeDetail.ItemsSource = dt.DefaultView;
                    txtTotalAmount.Text = dTotalAmount.ToString();
                    txtTotalBF.Text = dTotalBF.ToString();
                    txtTotalIns.Text = dTotalIns.ToString();
                    txtTotalSubs.Text = dTotalSubs.ToString();
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
