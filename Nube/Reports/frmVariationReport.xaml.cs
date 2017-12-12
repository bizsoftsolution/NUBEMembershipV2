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

namespace Nube.Reports
{
    /// <summary>
    /// Interaction logic for frmVariationReport.xaml
    /// </summary>
    public partial class frmVariationReport : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        public frmVariationReport()
        {
            InitializeComponent();
            fFormLoad();
        }

        #region Events

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(dtpFromDate.Text))
                {
                    MessageBox.Show("From Date is Empty!");
                    dtpFromDate.Focus();
                    return;
                }
                else if (string.IsNullOrEmpty(dtpToDate.Text))
                {
                    MessageBox.Show("To Date is Empty!");
                    dtpToDate.Focus();
                    return;
                }
                else if (Convert.ToDateTime(dtpFromDate.SelectedDate).Month == Convert.ToDateTime(dtpToDate.SelectedDate).Month)
                {
                    MessageBox.Show("From Date and To Date Must be Different");
                    dtpToDate.Focus();
                    return;
                }
                else if (Convert.ToDateTime(dtpFromDate.SelectedDate).Month > Convert.ToDateTime(dtpToDate.SelectedDate).Month)
                {
                    MessageBox.Show("To Date is Grater than From Date!");
                    dtpToDate.Focus();
                    return;
                }
                else
                {
                    fGetData();
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
                fClear();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            frmHomeReports frm = new frmHomeReports();
            this.Close();
            frm.ShowDialog();
        }

        private void cmbBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                decimal dBankCode = Convert.ToDecimal(cmbBank.SelectedValue);
                if (dBankCode != 0)
                {
                    var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANK_CODE == dBankCode).OrderBy(x => x.BANKBRANCH_NAME).ToList();
                    cmbBranch.ItemsSource = mbr;
                    cmbBranch.SelectedValuePath = "BANKBRANCH_CODE";
                    cmbBranch.DisplayMemberPath = "BANKBRANCH_NAME";
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        private void dgVariationReport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion

        #region Functiond

        void fFormLoad()
        {
            cmbBank.ItemsSource = db.MASTERBANKs.ToList();
            cmbBank.SelectedValuePath = "BANK_CODE";
            cmbBank.DisplayMemberPath = "BANK_NAME";

            cmbNUBEBranch.ItemsSource = db.MASTERNUBEBRANCHes.ToList();
            cmbNUBEBranch.SelectedValuePath = "NUBE_BRANCH_CODE";
            cmbNUBEBranch.DisplayMemberPath = "NUBE_BRANCH_NAME";
        }

        void fClear()
        {
            cmbBank.ItemsSource = null;
            cmbBranch.ItemsSource = null;
            cmbNUBEBranch.ItemsSource = null;
            dtpFromDate.Text = "";
            dtpToDate.Text = "";
            dgVariationReport.ItemsSource = null;
            fFormLoad();
        }

        void fGetData()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppLib.connStr))
            {
                string sJOIN = "";
                string sSelectQry = "";
                string sWhere = "";

                if (!string.IsNullOrEmpty(cmbNUBEBranch.Text) || !string.IsNullOrEmpty(cmbBank.Text) || !string.IsNullOrEmpty(cmbBranch.Text))
                {
                    sJOIN = sJOIN + " LEFT JOIN MEMBERSTATUSLOG MM(NOLOCK) ON MM.MEMBER_CODE=FD.MEMBERCODE \r" +
                                    " LEFT JOIN FEESMASTER FM(NOLOCK) ON FM.FEEID=FD.FEEID ";
                }

                if (!string.IsNullOrEmpty(cmbNUBEBranch.Text))
                {
                    sWhere = sWhere + " AND MM.NUBEBRANCH_CODE=" + cmbNUBEBranch.SelectedValue;
                }

                if (!string.IsNullOrEmpty(cmbBank.Text))
                {
                    sWhere = sWhere + " AND FM.BANKID=" + cmbBank.SelectedValue;
                }

                if (!string.IsNullOrEmpty(cmbBranch.Text))
                {
                    sWhere = sWhere + " AND MM.BRANCH_CODE=" + cmbBranch.SelectedValue;
                }

                if (chkIncludeArrear.IsChecked == false)
                {
                    sWhere = sWhere + " AND STATUS='FEES ENTRY' ";
                }

                if (rbtnActiveMember.IsChecked == true)
                {
                    sSelectQry = sSelectQry + ",SUM(FA_AMT)+SUM(SA_AMT)TOTALAMOUNT,SUM(FA_MEM)+SUM(SA_MEM)TOTALMEMBERS \r" +
                                              ",SUM(FA_AMT)-SUM(SA_AMT)VARIATIONAMOUNT,SUM(FA_MEM)-SUM(SA_MEM)VARIATIONMEMBER \r";
                }
                else
                {
                    sSelectQry = sSelectQry + ",SUM(FT_AMT)+SUM(ST_AMT)TOTALAMOUNT,SUM(FT_MEM)+SUM(ST_MEM)TOTALMEMBERS \r" +
                                              ",SUM(FT_AMT)-SUM(ST_AMT)VARIATIONAMOUNT,SUM(FT_MEM)-SUM(ST_MEM)VARIATIONMEMBER \r";
                }

                string sQry = string.Format("SELECT BANKID,CASE WHEN ISNULL(MB.BANK_USERCODE,'')<>'' THEN MB.BANK_USERCODE ELSE 'Arrear Entry' END BANK_USERCODE, \r" +
                    " SUM(FA_AMT)FA_AMT,SUM(FA_MEM)FA_MEM,SUM(FS_AMT)FS_AMT,SUM(FS_MEM)FS_MEM,SUM(FT_AMT)FT_AMT,SUM(FT_MEM)FT_MEM, \r" +
                    " SUM(SA_AMT)SA_AMT,SUM(SA_MEM)SA_MEM,SUM(SS_AMT)SS_AMT,SUM(SS_MEM)SS_MEM,SUM(ST_AMT)ST_AMT,SUM(ST_MEM)ST_MEM \r" + sSelectQry +
                    " FROM (SELECT FEEYEAR FY,FEEMONTH FM,FM.BANKID,TMP.FEEID FI,SUM(AC_AMOUNT) FA_AMT,SUM(AC_MEMBERS)FA_MEM,SUM(SD_AMOUNT)FS_AMT,SUM(SD_MEMBERS)FS_MEM, \r" +
                    " SUM(AC_AMOUNT)+SUM(SD_AMOUNT)FT_AMT,SUM(AC_MEMBERS)+SUM(SD_MEMBERS)FT_MEM, \r" +
                    " 0 SY,0 SM,0 SI,0 SA_AMT,0 SA_MEM,0 SS_AMT,0 SS_MEM,0 ST_AMT,0 ST_MEM \r" +
                    " FROM(SELECT FD.FEEYEAR,FD.FEEMONTH,FD.FEEID,SUM(TOTALAMOUNT)AC_AMOUNT,COUNT(*)AC_MEMBERS,0 SD_AMOUNT,0 SD_MEMBERS \r" +
                    " FROM FEESDETAILS FD(NOLOCK) \r" + sJOIN +
                    " WHERE FD.FEEYEAR=YEAR('{0:dd/MMM/yyyy}') AND FEEMONTH=MONTH('{0:dd/MMM/yyyy}') AND ISNOTMATCH=0 " + sWhere + " \r" +
                    " GROUP BY FD.FEEYEAR,FD.FEEMONTH,FD.FEEID \r" +
                    " UNION ALL \r" +
                    " SELECT FD.FEEYEAR,FD.FEEMONTH,FD.FEEID,0 AC_AMOUNT,0 AC_MEMBERS,SUM(TOTALAMOUNT) SD_AMOUNT,COUNT(*) SD_MEMBERS \r" +
                    " FROM FEESDETAILS FD(NOLOCK) \r" + sJOIN +
                    " WHERE FD.FEEYEAR=YEAR('{0:dd/MMM/yyyy}') AND FD.FEEMONTH=MONTH('{0:dd/MMM/yyyy}') AND ISNOTMATCH=1 " + sWhere + " \r" +
                    " GROUP BY FD.FEEYEAR,FD.FEEMONTH,FD.FEEID)TMP \r" +
                    " LEFT JOIN FEESMASTER FM(NOLOCK) ON FM.FEEID = TMP.FEEID \r" +
                    " GROUP BY FEEYEAR,FEEMONTH,FM.BANKID,TMP.FEEID \r" +
                    " UNION ALL \r" +
                    " SELECT 0 FY,0 FM,FM.BANKID,0 FI,0 FA_AMT,0 FA_MEM,0 FS_AMT,0 FS_MEM,0 FT_AMT,0 FT_MEM, \r" +
                    " FEEYEAR SY,FEEMONTH SM,TMP.FEEID SI,SUM(AC_AMOUNT) SA_AMT,SUM(AC_MEMBERS)SA_MEM,SUM(SD_AMOUNT)SS_AMT,SUM(SD_MEMBERS)SS_MEM, \r" +
                    " SUM(AC_AMOUNT)+SUM(SD_AMOUNT)ST_AMT,SUM(AC_MEMBERS)+SUM(SD_MEMBERS)ST_MEM \r" +
                    " FROM(SELECT FD.FEEYEAR,FD.FEEMONTH,FD.FEEID,SUM(TOTALAMOUNT)AC_AMOUNT,COUNT(*)AC_MEMBERS,0 SD_AMOUNT,0 SD_MEMBERS \r" +
                    " FROM FEESDETAILS FD(NOLOCK) \r" + sJOIN +
                    " WHERE FD.FEEYEAR=YEAR('{1:dd/MMM/yyyy}') AND FD.FEEMONTH=MONTH('{1:dd/MMM/yyyy}') AND ISNOTMATCH=0 " + sWhere + " \r" +
                    " GROUP BY FD.FEEYEAR, FD.FEEMONTH, FD.FEEID \r" +
                    " UNION ALL \r" +
                    " SELECT FD.FEEYEAR, FD.FEEMONTH, FD.FEEID,0 AC_AMOUNT,0 AC_MEMBERS,SUM(TOTALAMOUNT) SD_AMOUNT,COUNT(*) SD_MEMBERS \r" +
                    " FROM FEESDETAILS FD(NOLOCK) \r" + sJOIN +
                    " WHERE FD.FEEYEAR=YEAR('{1:dd/MMM/yyyy}') AND FD.FEEMONTH=MONTH('{1:dd/MMM/yyyy}') AND ISNOTMATCH=1 " + sWhere + " \r" +
                    " GROUP BY FD.FEEYEAR,FD.FEEMONTH,FD.FEEID)TMP \r" +
                    " LEFT JOIN FEESMASTER FM(NOLOCK) ON FM.FEEID = TMP.FEEID \r" +
                    " GROUP BY FEEYEAR, FEEMONTH, FM.BANKID, TMP.FEEID)TMP \r" +
                    " LEFT JOIN MASTERBANK MB(NOLOCK) ON MB.BANK_CODE = TMP.BANKID \r" +
                    " GROUP BY BANKID, MB.BANK_USERCODE \r" +
                    " ORDER BY MB.BANK_USERCODE ", dtpFromDate.SelectedDate, dtpToDate.SelectedDate);

                SqlCommand cmd = new SqlCommand(sQry, conn);
                SqlDataAdapter sdp = new SqlDataAdapter(cmd);
                sdp.SelectCommand.CommandTimeout = 0;
                sdp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dgVariationReport.ItemsSource = dt.DefaultView;
                    dgVariationReport.Columns[1].Header = string.Format("{0:MMM} A Member", dtpFromDate.SelectedDate);
                    dgVariationReport.Columns[2].Header = string.Format("{0:MMM} A Amount", dtpFromDate.SelectedDate);
                    dgVariationReport.Columns[3].Header = string.Format("{0:MMM} S Member", dtpFromDate.SelectedDate);
                    dgVariationReport.Columns[4].Header = string.Format("{0:MMM} S Amount", dtpFromDate.SelectedDate);
                    dgVariationReport.Columns[5].Header = string.Format("{0:MMM} Tot Member", dtpFromDate.SelectedDate);
                    dgVariationReport.Columns[6].Header = string.Format("{0:MMM} Tot Amount", dtpFromDate.SelectedDate);
                    dgVariationReport.Columns[7].Header = string.Format("{0:MMM} A Member", dtpToDate.SelectedDate);
                    dgVariationReport.Columns[8].Header = string.Format("{0:MMM} A Amount", dtpToDate.SelectedDate);
                    dgVariationReport.Columns[9].Header = string.Format("{0:MMM} S Member", dtpToDate.SelectedDate);
                    dgVariationReport.Columns[10].Header = string.Format("{0:MMM} S Amount", dtpToDate.SelectedDate);
                    dgVariationReport.Columns[11].Header = string.Format("{0:MMM} Tot Member", dtpToDate.SelectedDate);
                    dgVariationReport.Columns[12].Header = string.Format("{0:MMM} Tot Amount", dtpToDate.SelectedDate);
                }
                else
                {
                    dgVariationReport.ItemsSource = null;
                    MessageBox.Show("No Records Found!", "Empty");
                    dtpFromDate.Focus();
                }
            }
        }

        #endregion
    }
}
