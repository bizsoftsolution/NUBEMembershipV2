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
using Microsoft.Reporting.WinForms;
using System.Data;
using System.Data.SqlClient;

namespace Nube.Reports
{
    /// <summary>
    /// Interaction logic for frmComparitionReport.xaml
    /// </summary>
    public partial class frmComparitionReport : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        string connStr = AppLib.connStr;
        decimal dMemberCode = 0;

        public frmComparitionReport()
        {
            InitializeComponent();
            try
            {
                fLoadWindow();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        #region"BUTTON EVENTS"

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(dtpFromDate.Text)||string.IsNullOrEmpty(dtpToDate.Text))
                {
                    MessageBox.Show("Date is Empty!");
                    return;
                }
                if (dtpFromDate.SelectedDate > dtpToDate.SelectedDate)
                {
                    MessageBox.Show("FromDate is greater than ToDate!");
                    return;
                }
                else
                {
                    DataTable dt = new DataTable();
                    
                    dt = GetData1();
                    if (dt.Rows.Count > 0)
                    {
                        dgPaidUnPaidReport.ItemsSource = dt.DefaultView;
                    }
                    else
                    {
                        MessageBox.Show(" No Data Found");
                        dgPaidUnPaidReport.ItemsSource = null;
                    }
                    dgHistory.ItemsSource = null;
                    cmbStatus.Text = "";
                    dMemberCode = 0;
                }
            }
            
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
}

private void btnClear_Click(object sender, RoutedEventArgs e)
{
    fClear();
}

private void btnBack_Click(object sender, RoutedEventArgs e)
{
    frmHomeReports frm = new frmHomeReports();
    this.Close();
    frm.ShowDialog();
}

private void btnSave_Click(object sender, RoutedEventArgs e)
{
    if (dgHistory.Items.Count > 0)
    {
        if (Convert.ToInt32(cmbStatus.SelectedValue) != 0)
        {
            if (MessageBox.Show("Do you want to Save this record? ", "SAVE CONFIRMATION", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var vs = (from x in db.MASTERMEMBERs where x.MEMBER_CODE == dMemberCode select x).OrderByDescending(x => x.DATEOFJOINING).FirstOrDefault();
                int iOldStatus = 0;
                if (vs != null)
                {
                    iOldStatus = Convert.ToInt32(vs.STATUS_CODE);
                }

                StatusModify str = new StatusModify
                {
                    MemberCode = Convert.ToInt32(dMemberCode),
                    OldStatusCode = iOldStatus,
                    NewStatusCode = Convert.ToInt32(cmbStatus.SelectedValue),
                    UserCode = AppLib.iUserCode,
                    CreatedOn = DateTime.Now,
                    UpdatedStatus = "Not Updated"
                };
                db.StatusModifies.Add(str);
                db.SaveChanges();
                MessageBox.Show("Status Updated Sucessfully");
                fClear();
            }
        }
        else
        {
            MessageBox.Show("Status is Empty");
            cmbStatus.Focus();
        }
    }
    else
    {
        MessageBox.Show("No Data Found");
    }
}

#endregion

#region"OTHER EVENTS"

private void cmbBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
{
    try
    {
        decimal dBankCode = Convert.ToDecimal(cmbBank.SelectedValue);
        if (dBankCode != 0)
        {
            var mbr = db.MASTERBANKBRANCHes.Where(x => x.BANK_CODE == dBankCode).ToList();
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

private void dgPaidUnPaidReport_SelectionChanged(object sender, SelectionChangedEventArgs e)
{
    try
    {
        if (dgPaidUnPaidReport.SelectedItem != null)
        {
            DataTable dt = new DataTable();
            DataRowView drv = (DataRowView)dgPaidUnPaidReport.SelectedItem;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sQry = string.Format(" SELECT TOP 12 FD.MEMBERCODE,CASE WHEN ISNULL(SM.NEWSTATUSCODE,0)<>0 THEN SM.NEWSTATUSCODE ELSE MM.STATUS_CODE END STATUS_CODE, " +
                                            " CASE WHEN ISNULL(SM.NEWSTATUSCODE,0)<>0 THEN SMS.STATUS_NAME ELSE MS.STATUS_NAME END STATUS_NAME, " +
                                            " FD.AMOUNTBF,FD.AMOUNTINS,FD.AMTSUBS,FD.FEEYEAR,FD.FEEMONTH,ISNULL(MS.STATUS_NAME,'')STATUS_NAME,ISNULL(FD.REASON,'')REASON " +
                                            " FROM FEESDETAILS FD(NOLOCK)" +
                                            " LEFT JOIN TEMPVIEWMASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE=FD.MEMBERCODE" +
                                            " LEFT JOIN (SELECT TOP 1 MEMBERCODE,NEWSTATUSCODE FROM STATUSMODIFY ORDER BY CREATEDON DESC)SM ON SM.MEMBERCODE=FD.MEMBERCODE" +
                                            " LEFT JOIN MASTERSTATUS MS(NOLOCK) ON MS.STATUS_CODE=MM.MEMBERSTATUSCODE" +
                                            " LEFT JOIN MASTERSTATUS SMS(NOLOCK) ON SMS.STATUS_CODE=SM.NEWSTATUSCODE" +
                                            " WHERE FD.ISUNPAID=0 AND FD.MEMBERCODE={0} ORDER BY FD.FEEMONTH DESC", Convert.ToDecimal(drv["MEMBERCODE"]));
                SqlCommand cmd = new SqlCommand(sQry, conn);
                SqlDataAdapter sdp = new SqlDataAdapter(cmd);
                sdp.Fill(dt);
            }
            if (dt.Rows.Count > 0)
            {
                dgHistory.ItemsSource = dt.DefaultView;
                dMemberCode = Convert.ToDecimal(dt.Rows[0]["MEMBERCODE"]);
            }
            else
            {
                dgHistory.ItemsSource = null;
                cmbStatus.Text = "";
                dMemberCode = 0;
            }
        }
    }
    catch (Exception ex)
    {
        ExceptionLogging.SendErrorToText(ex);
    }
}

private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
{

}

#endregion

#region"USER DEFINED FUNCTION"

void fClear()
{
    cmbBank.ItemsSource = null;
    cmbBranch.ItemsSource = null;
    dtpFromDate.Text = "";
    dtpToDate.Text = "";

    dgHistory.ItemsSource = null;
    cmbStatus.ItemsSource = null;
    dgPaidUnPaidReport.ItemsSource = null;
    dMemberCode = 0;

    var bank = db.MASTERBANKs.ToList();
    cmbBank.ItemsSource = bank;
    cmbBank.SelectedValuePath = "BANK_CODE";
    cmbBank.DisplayMemberPath = "BANK_NAME";

    var status = (from x in db.MASTERSTATUS where x.STATUS_CODE == 2 || x.STATUS_CODE == 3 select x).ToList();
    cmbStatus.ItemsSource = status;
    cmbStatus.SelectedValuePath = "STATUS_CODE";
    cmbStatus.DisplayMemberPath = "STATUS_NAME";
}

void fLoadWindow()
{
    var bank = db.MASTERBANKs.ToList();
    cmbBank.ItemsSource = bank;
    cmbBank.SelectedValuePath = "BANK_CODE";
    cmbBank.DisplayMemberPath = "BANK_NAME";

    var status = (from x in db.MASTERSTATUS where x.STATUS_CODE == 2 || x.STATUS_CODE == 3 select x).ToList();
    cmbStatus.ItemsSource = status;
    cmbStatus.SelectedValuePath = "STATUS_CODE";
    cmbStatus.DisplayMemberPath = "STATUS_NAME";

    if (AppLib.bIsSuperAdmin == true)
    {
        cmbStatus.Visibility = Visibility.Visible;
        btnSave.Visibility = Visibility.Visible;
        lblStatus.Visibility = Visibility.Visible;
    }
    else
    {
        cmbStatus.Visibility = Visibility.Hidden;
        btnSave.Visibility = Visibility.Hidden;
        lblStatus.Visibility = Visibility.Hidden;
    }
}

//private DataTable GetData()
//{
//    DataTable dt = new DataTable();
//    using (SqlConnection conn = new SqlConnection(connStr))
//    {
//        string sWhere = "";
//        if (Convert.ToInt64(cmbBank.SelectedValue) != 0)
//        {
//            sWhere = " AND MM.BANK_CODE=" + Convert.ToInt32(cmbBank.SelectedValue);
//        }
//        if (Convert.ToInt64(cmbBranch.SelectedValue) != 0)
//        {
//            sWhere = " AND MM.BRANCH_CODE=" + Convert.ToInt32(cmbBranch.SelectedValue);
//        }
//        if (rbtnPaid.IsChecked == true)
//        {
//            string sQry = string.Format(" SELECT FD.MEMBERCODE,MM.MEMBER_ID,MM.MEMBER_NAME,BK.BANK_USERCODE BANK_NAME,BR.BANKBRANCH_NAME, " +
//                                        " FD.AMOUNTBF BF,FD.AMOUNTINS INS,AMTSUBS SUBS  " +
//                                        " FROM FEESDETAILS FD(NOLOCK) " +
//                                        " LEFT JOIN MASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE = FD.MEMBERCODE " +
//                                        " LEFT JOIN MASTERBANK BK(NOLOCK) ON BK.BANK_CODE = MM.BANK_CODE " +
//                                        " LEFT JOIN MASTERBANKBRANCH BR(NOLOCK) ON BR.BANKBRANCH_CODE = MM.BRANCH_CODE " +
//                                        " WHERE FD.FEEYEAR={0:yyyy} AND FD.FEEMONTH={0:MM} AND MEMBERCODE IN (SELECT MEMBERCODE FROM FEESDETAILS WHERE FEEYEAR ={1:yyyy} AND FEEMONTH={1:MM}) " + sWhere +
//                                        " ORDER BY MM.MEMBER_NAME ASC", dtpDate2.SelectedDate, dtpDate1.SelectedDate);
//            SqlCommand cmd = new SqlCommand(sQry, conn);
//            SqlDataAdapter sdp = new SqlDataAdapter(cmd);
//            sdp.Fill(dt);
//        }
//        else if (rbtnUnPaid.IsChecked == true)
//        {
//            string sQry = string.Format(" SELECT FD.MEMBERCODE,MM.MEMBER_ID,MM.MEMBER_NAME,BK.BANK_USERCODE BANK_NAME,BR.BANKBRANCH_NAME, " +
//                                        " FD.AMOUNTBF BF,FD.AMOUNTINS INS,AMTSUBS SUBS " +
//                                        " FROM FEESDETAILS FD(NOLOCK) " +
//                                        " LEFT JOIN MASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE = FD.MEMBERCODE " +
//                                        " LEFT JOIN MASTERBANK BK(NOLOCK) ON BK.BANK_CODE = MM.BANK_CODE " +
//                                        " LEFT JOIN MASTERBANKBRANCH BR(NOLOCK) ON BR.BANKBRANCH_CODE = MM.BRANCH_CODE " +
//                                        " WHERE FD.FEEYEAR={0:yyyy} AND FD.FEEMONTH={0:MM} AND MEMBERCODE NOT IN (SELECT MEMBERCODE FROM FEESDETAILS WHERE FEEYEAR ={1:yyyy} AND FEEMONTH={1:MM}) " + sWhere +
//                                        " ORDER BY MM.MEMBER_NAME ASC", dtpDate2.SelectedDate, dtpDate1.SelectedDate);
//            SqlCommand cmd = new SqlCommand(sQry, conn);
//            SqlDataAdapter sdp = new SqlDataAdapter(cmd);
//            sdp.Fill(dt);
//        }
//    }
//    return dt;
//}

private DataTable GetData1()
{
    DataTable dt = new DataTable();
    using (SqlConnection conn = new SqlConnection(connStr))
    {
        string sWhere = "";
        string sBank = "";
        //if (Convert.ToInt64(cmbBank.SelectedValue) != 0)
        //{
        //    sWhere = " AND MM.BANK_CODE=" + Convert.ToInt32(cmbBank.SelectedValue);
        //}


        decimal dBankCode = Convert.ToDecimal(cmbBank.SelectedValue);

        if (!string.IsNullOrEmpty(cmbBank.Text) && (dtpFromDate.SelectedDate <= Convert.ToDateTime("31/MAR/2016").Date))
        {
            sWhere = " AND FD.BANK_CODE=" + Convert.ToInt32(cmbBank.SelectedValue);
        }
        else if (!string.IsNullOrEmpty(cmbBank.Text) && rbtSeparate.IsChecked == true && (dtpFromDate.SelectedDate > Convert.ToDateTime("31/MAR/2016").Date))
        {
            var st = (from x in db.MASTERBANKs where x.BANK_CODE == dBankCode && x.HEADER_BANK_CODE == 0 select x).ToList();
            if (st.Count > 0)
            {
                sBank = sBank + " AND FM.BANKID IN (" + cmbBank.SelectedValue + ")";
                var exp = (from x in db.MASTERBANKs where x.HEADER_BANK_CODE == dBankCode select x).FirstOrDefault();
                if (exp != null)
                {
                    sBank = sBank + " AND MM.BANK_CODE NOT IN (" + exp.BANK_CODE.ToString() + ") ";
                }
            }
            else
            {
                sBank = sBank + " AND MM.BANK_CODE IN (" + cmbBank.SelectedValue + ")";
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
                sBank = sBank + " AND FM.BANKID IN (" + str + ") ";
            }
            else
            {
                sBank = sBank + " AND FM.BANKID=" + cmbBank.SelectedValue + " ";
            }
        }

        sWhere = sWhere + sBank;
        if (Convert.ToInt64(cmbBranch.SelectedValue) != 0)
        {
            sWhere = sWhere + " AND MM.BRANCH_CODE=" + Convert.ToInt32(cmbBranch.SelectedValue);
        }

        string sQry = "";

        if (dtpFromDate.SelectedDate <= Convert.ToDateTime("31/MAR/2016").Date)
        {
            if (rbtnPaid.IsChecked == true)
            {
                sWhere = sWhere + string.Format(" AND FD.MEMBER_CODE IN (SELECT MEMBERCODE FROM FEESDETAILS(NOLOCK) WHERE FEEYEAR={0:yyyy} AND FEEMONTH={0:MM}) " + sBank + " AND FD.MEMBER_CODE<>0 \r", dtpToDate.SelectedDate);
            }
            else if (rbtnUnPaid.IsChecked == true)
            {
                sWhere = sWhere + string.Format(" AND FD.MEMBER_CODE NOT IN (SELECT MEMBERCODE FROM FEESDETAILS(NOLOCK) WHERE FEEYEAR={0:yyyy} AND FEEMONTH={0:MM}) " + sBank + " AND FD.MEMBER_CODE<>0 \r", dtpToDate.SelectedDate);
            }
            sQry = string.Format(" SELECT ROW_NUMBER() OVER(ORDER BY MM.MEMBER_NAME ASC) AS RNO,FD.MEMBER_CODE,MM.MEMBER_ID,MM.MEMBER_NAME,BK.BANK_USERCODE BANK_NAME,BR.BANKBRANCH_NAME, \r" +
                                        " FD.TOTALBF_AMOUNT BF,0 INS,TOTALSUBCRP_AMOUNT SUBS  \r" +
                                        " FROM nubestatus..FEE_STATUS FD(NOLOCK) \r" +
                                        " LEFT JOIN MASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE = FD.MEMBER_CODE\r " +
                                        " LEFT JOIN MASTERBANK BK(NOLOCK) ON BK.BANK_CODE = MM.BANK_CODE \r" +
                                        " LEFT JOIN MASTERBANKBRANCH BR(NOLOCK) ON BR.BANKBRANCH_CODE = MM.BRANCH_CODE \r" +
                                        " WHERE FD.FEE_YEAR={0:yyyy} AND MM.ISCANCEL=0 AND FD.FEE_MONTH={0:MM} AND FD.TOTAL_MONTHS>0  \r" + sWhere +
                                        " ORDER BY MM.MEMBER_NAME ASC", dtpFromDate.SelectedDate);
        }
        else
        {
            if (rbtnPaid.IsChecked == true)
            {
                sWhere = sWhere + string.Format(" AND FD.MEMBERCODE IN (SELECT FD.MEMBERCODE FROM FEESDETAILS FD(NOLOCK) \r" +
                                                " LEFT JOIN FEESMASTER FM(NOLOCK) ON FM.FEEID=FD.FEEID \r" +
                                                " LEFT JOIN MASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE=FD.MEMBERCODE \r" +
                                                " WHERE FD.FEEYEAR={0:yyyy} AND FD.FEEMONTH={0:MM} " + sBank + " AND FD.MEMBERCODE<>0) AND FD.MEMBERCODE<>0 \r", dtpToDate.SelectedDate);
            }
            else if (rbtnUnPaid.IsChecked == true)
            {
                sWhere = sWhere + string.Format(" AND FD.MEMBERCODE NOT IN (SELECT FD.MEMBERCODE FROM FEESDETAILS FD(NOLOCK) \r" +
                                                " LEFT JOIN FEESMASTER FM(NOLOCK) ON FM.FEEID=FD.FEEID \r" +
                                                " LEFT JOIN MASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE=FD.MEMBERCODE \r" +
                                                " WHERE FD.FEEYEAR={0:yyyy} AND FD.FEEMONTH={0:MM} " + sBank + " AND FD.MEMBERCODE<>0) AND FD.MEMBERCODE<>0 \r", dtpToDate.SelectedDate);
            }
            sQry = string.Format(" SELECT ROW_NUMBER() OVER(ORDER BY MM.MEMBER_NAME ASC) AS RNO,FD.MEMBERCODE,MM.MEMBER_ID, \r" +
                                 " MM.MEMBER_NAME,BK.BANK_USERCODE BANK_NAME,BR.BANKBRANCH_NAME, \r" +
                                 " FD.AMOUNTBF BF,FD.UNIONCONTRIBUTION INS,AMTSUBS SUBS  \r" +
                                 " FROM FEESDETAILS FD(NOLOCK) \r" +
                                 " LEFT JOIN FEESMASTER FM(NOLOCK) ON FM.FEEID=FD.FEEID \r" +
                                 " LEFT JOIN MASTERMEMBER MM(NOLOCK) ON MM.MEMBER_CODE = FD.MEMBERCODE\r " +
                                 " LEFT JOIN MASTERBANK BK(NOLOCK) ON BK.BANK_CODE = MM.BANK_CODE \r" +
                                 " LEFT JOIN MASTERBANKBRANCH BR(NOLOCK) ON BR.BANKBRANCH_CODE = MM.BRANCH_CODE \r" +
                                 " WHERE FD.FEEYEAR={0:yyyy} AND FD.FEEMONTH={0:MM} AND FD.ISUNPAID=0 \r" + sWhere, dtpFromDate.SelectedDate);
        }

        SqlCommand cmd = new SqlCommand(sQry, conn);
        SqlDataAdapter sdp = new SqlDataAdapter(cmd);
        sdp.SelectCommand.CommandTimeout = 0;
        sdp.Fill(dt);
    }
    return dt;
}

#endregion

private void dgPaidUnPaidReport_MouseDoubleClick(object sender, MouseButtonEventArgs e)
{

}
    }
}
