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
using Microsoft.Reporting.WinForms;

namespace Nube.Reports
{
    /// <summary>
    /// Interaction logic for frmMonthlyStatement.xaml
    /// </summary>
    public partial class frmMonthlyStatement : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();                
        string mon = "";
        int month = 0;
        int year = 0;
        string connStr =AppLib.connStr;
        public frmMonthlyStatement()
        {
            InitializeComponent();
            var nubebranch = db.MASTERNUBEBRANCHes.ToList();
            cmbNubeBranch.ItemsSource = nubebranch;
            cmbNubeBranch.SelectedValuePath = "NUBE_BRANCH_NAME";
            cmbNubeBranch.DisplayMemberPath = "NUBE_BRANCH_NAME";
            var bank = db.MASTERBANKs.ToList();
            cmbBank.ItemsSource = bank;
            cmbBank.SelectedValuePath = "BANK_CODE";
            cmbBank.DisplayMemberPath = "BANK_NAME";
            dtpDate.SelectedDate = Convert.ToDateTime(DateTime.Now);
        }

        //Button Events
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            mon = dtpDate.SelectedDate.Value.Month.ToString();
            if (cmbNubeBranch.Text == "")
            {
                MessageBox.Show("Enter Nube Branch");
            }
            else
            {
                try
                {
                    NewMemberReport.Reset();
                    DataTable dt = getData();
                    ReportDataSource masterData = new ReportDataSource("AnnualStatement", dt);

                    NewMemberReport.LocalReport.DataSources.Add(masterData);
                    NewMemberReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.MonthlyStatement.rdlc";
                   // NewMemberReport.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(Details);

                    ReportParameter[] rp = new ReportParameter[3];
                    rp[0] = new ReportParameter("BranchName", cmbNubeBranch.SelectedValue.ToString());
                    rp[1] = new ReportParameter("Month", String.Format("{0:MMM-yyyy}", dtpDate.SelectedDate.Value));
                    rp[2] = new ReportParameter("Title", "New Member");

                    NewMemberReport.LocalReport.SetParameters(rp);
                    NewMemberReport.RefreshReport();
                    LoadResignMember();
                }
                catch (Exception ex)
                {
                    ExceptionLogging.SendErrorToText(ex);
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cmbBank.Text = "";
            cmbBranch.Text = "";
            cmbBranch.Text = "";
            cmbNubeBranch.Text = "";
            dtpDate.SelectedDate = Convert.ToDateTime(DateTime.Now);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            frmHomeReports frm = new frmHomeReports();
            this.Close();
            frm.ShowDialog();
        }

        //Load Report
        private void LoadResignMember()
        {
            ResignMemberReport.Reset();
            DataTable dt = getResignData();
            ReportDataSource masterData = new ReportDataSource("AnnualStatement", dt);

            ResignMemberReport.LocalReport.DataSources.Add(masterData);
            ResignMemberReport.LocalReport.ReportEmbeddedResource = "Nube.Reports.MonthlyStatement.rdlc";
         //   ResignMemberReport.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(Details);

            ReportParameter[] rp = new ReportParameter[3];
            rp[0] = new ReportParameter("BranchName", cmbNubeBranch.SelectedValue.ToString());
            rp[1] = new ReportParameter("Month", String.Format("{0:MMM-yyyy}", dtpDate.SelectedDate.Value));
            rp[2] = new ReportParameter("Title", "Resigned Member");

            ResignMemberReport.LocalReport.SetParameters(rp);

            ResignMemberReport.RefreshReport();

        }

        private DataTable getData()
        {
            DataTable dt = new DataTable();
            try
            {
                string qry = "";
                if (cmbNubeBranch.Text != "")
                {                   
                    if (qry != "")
                    {
                        qry = qry + "and MM.NubeBanchName='" + cmbNubeBranch.Text + "'";
                    }
                    else
                    {
                        qry = "MM.NubeBanchName='" + cmbNubeBranch.Text + "'";
                    }
                }


                if (cmbBank.Text != "")
                {                    
                    if (qry != "")
                    {
                        qry = qry + "and MM.BANK_NAME='" + cmbBank.Text + "'";
                    }
                    else
                    {
                        qry = " MM.BANK_NAME='" + cmbBank.Text + "'";
                    }
                }

                if (cmbBranch.Text != "")
                {                  
                    if (qry != "")
                    {
                        qry = qry + "and MM.BranchName='" + cmbBranch.Text + "'";
                    }
                    else
                    {
                        qry = "MM.BranchName='" + cmbBranch.Text + "'";
                    }
                }


                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd;
                    month = dtpDate.SelectedDate.Value.Month;
                    year = dtpDate.SelectedDate.Value.Year;

                    string dateMonth = string.Format("{0:MMyyyy}", dtpDate.SelectedDate.Value);

                    cmd = new SqlCommand("select MM.Member_ID,MM.Member_Name,MM.BranchAdr1, MM.BranchAdr2, MM.BranchAdr3, MM.BranchName, MM.BRANCH_CODE,MM.LASTPAYMENT_DATE, MM.CURRENT_YTDBF, MM.CURRENT_YTDSUBSCRIPTION,MM.DATEOFJOINING,MM.ENTRANCEFEE, MM.BANK_NAME, MM.NubeBranch,MM.MonDue,isNull(MM.ICNo_New,isnull(mm.icno_old,'')) as ICNO,MM.Bank_UserCode + '/' + MM.BranchUserCode as BankCode,ST.TOTALMONTHSPAID,ST.ACCBF,LASTPAYMENTDATE,MT.MEMBERTYPE_NAME,MM.MemberStatus,MM.NubeBanchName from nubestatus.dbo.STATUS" + dateMonth + " As ST Left Join TEMPVIEWMASTERMEMBER As MM On MM.Member_Code = ST.Member_Code Left Join MasterMemberType As MT On MT.MemberType_Code = ST.MemberType_Code Left Join MasterStatus As MS On MS.Status_Code = ST.Status_Code WHERE DatePart(M,DateOfJoining)='" + month + "' and datepart(yy,dateofjoining)='" + year + "' and " + qry + "", con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                    qry = "";
                }

            }

            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            return dt;
        }

        private DataTable getResignData()
        {
            DataTable dt = new DataTable();
            try
            {
                //Wqry();
                string qry = "";
                if (cmbNubeBranch.Text != "")
                {                   
                    if (qry != "")
                    {

                        qry = qry + "and MM.NubeBanchName='" + cmbNubeBranch.Text + "'";
                    }
                    else
                    {
                        qry = "MM.NubeBanchName='" + cmbNubeBranch.Text + "'";
                    }
                }


                if (cmbBank.Text != "")
                {                 
                    if (qry != "")
                    {
                        qry = qry + "and MM.BANK_NAME='" + cmbBank.Text + "'";
                    }
                    else
                    {
                        qry = " MM.BANK_NAME='" + cmbBank.Text + "'";
                    }

                }
                if (cmbBranch.Text != "")
                {                    
                    if (qry != "")
                    {
                        qry = qry + "and MM.BranchName='" + cmbBranch.Text + "'";
                    }
                    else
                    {
                        qry = "MM.BranchName='" + cmbBranch.Text + "'";
                    }
                }

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd;
                    month = dtpDate.SelectedDate.Value.Month;
                    year = dtpDate.SelectedDate.Value.Year;

                    string dateMonth = string.Format("{0:MMyyyy}", dtpDate.SelectedDate.Value);

                    cmd = new SqlCommand("select MM.Member_ID,MM.Member_Name,MM.BranchAdr1, MM.BranchAdr2, MM.BranchAdr3, MM.BranchName, MM.BRANCH_CODE,MM.LASTPAYMENT_DATE, MM.CURRENT_YTDBF, MM.CURRENT_YTDSUBSCRIPTION,MM.DATEOFJOINING,MM.ENTRANCEFEE, MM.BANK_NAME, MM.NubeBranch,MM.MonDue,isNull(MM.ICNo_New,isnull(mm.icno_old,'')) as ICNO,MM.Bank_UserCode + '/' + MM.BranchUserCode as BankCode,ST.TOTALMONTHSPAID,ST.ACCBF,LASTPAYMENTDATE,MT.MEMBERTYPE_NAME,MM.MemberStatus,MM.NubeBanchName from nubestatus.dbo.STATUS" + dateMonth + " As ST Left Join TEMPVIEWMASTERMEMBER As MM On MM.Member_Code = ST.Member_Code Left Join MasterMemberType As MT On MT.MemberType_Code = ST.MemberType_Code Left Join MasterStatus As MS On MS.Status_Code = ST.Status_Code where MM.RESIGNED=1 and " + qry + "", con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);
                   // qry = "";
                }

            }

            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            return dt;
        }
       
        //private void Details(object sender, SubreportProcessingEventArgs e)
        //{
        //    try
        //    {
        //        string Branch = e.Parameters["BranchName"].Values[0];
        //        DataTable dt = GetDetails(Branch);
        //        ReportDataSource rs = new ReportDataSource("AnnualStatementDetails", dt);
        //        e.DataSources.Add(rs);
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogging.SendErrorToText(ex);
        //    }
        //}

        //private void RDetails(object sender, SubreportProcessingEventArgs e)
        //{
        //    try
        //    {
        //        string Branch = e.Parameters["BranchName"].Values[0];
        //        DataTable dt = GetRDetails(Branch);
        //        ReportDataSource rs = new ReportDataSource("AnnualStatementDetails", dt);
        //        e.DataSources.Add(rs);
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogging.SendErrorToText(ex);
        //    }
        //}

        //private DataTable GetDetails(string Branch)
        //{
        //    DataTable dt = new DataTable();
        //    try
        //    {

        //        using (SqlConnection con = new SqlConnection(connStr))
        //        {
        //            SqlCommand cmd;
        //            month = dtpDate.SelectedDate.Value.Month;
        //            year = dtpDate.SelectedDate.Value.Year;

        //            string dateMonth = string.Format("{0:MMyyyy}", dtpDate.SelectedDate.Value);

        //            cmd = new SqlCommand("select MM.Member_ID,MM.Member_Name,MM.BranchAdr1, MM.BranchAdr2, MM.BranchAdr3, MM.BranchName, MM.BRANCH_CODE,MM.LASTPAYMENT_DATE, MM.CURRENT_YTDBF, MM.CURRENT_YTDSUBSCRIPTION,MM.DATEOFJOINING,MM.ENTRANCEFEE, MM.BANK_NAME, MM.NubeBranch,MM.MonDue,isNull(MM.ICNo_New,isnull(mm.icno_old,'')) as ICNO,MM.Bank_UserCode + '/' + MM.BranchUserCode as BankCode,ST.TOTALMONTHSPAID,ST.ACCBF,LASTPAYMENTDATE,MT.MEMBERTYPE_NAME,MM.MemberStatus,MM.NubeBanchName from nubestatus.dbo.STATUS" + dateMonth + " As ST Left Join ViewMasterMember As MM On MM.Member_Code = ST.Member_Code Left Join MasterMemberType As MT On MT.MemberType_Code = ST.MemberType_Code Left Join MasterStatus As MS On MS.Status_Code = ST.Status_Code where BranchName='" + Branch + "'", con);
        //            SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //            adp.Fill(dt);
        //            qry = "";


        //        }

        //    }


        //    catch (Exception ex)
        //    {
        //        ExceptionLogging.SendErrorToText(ex);
        //    }
        //    return dt;
        //}

        //private DataTable GetRDetails(string Branch)
        //{
        //    DataTable dt = new DataTable();
        //    try
        //    {

        //        using (SqlConnection con = new SqlConnection(connStr))
        //        {
        //            SqlCommand cmd;
        //            month = dtpDate.SelectedDate.Value.Month;
        //            year = dtpDate.SelectedDate.Value.Year;

        //            string dateMonth = string.Format("{0:MMyyyy}", dtpDate.SelectedDate.Value);

        //            cmd = new SqlCommand("select MM.Member_ID,MM.Member_Name,MM.BranchAdr1, MM.BranchAdr2, MM.BranchAdr3, MM.BranchName, MM.BRANCH_CODE,MM.LASTPAYMENT_DATE, MM.CURRENT_YTDBF, MM.CURRENT_YTDSUBSCRIPTION,MM.DATEOFJOINING,MM.ENTRANCEFEE, MM.BANK_NAME, MM.NubeBranch,MM.MonDue,isNull(MM.ICNo_New,isnull(mm.icno_old,'')) as ICNO,MM.Bank_UserCode + '/' + MM.BranchUserCode as BankCode,ST.TOTALMONTHSPAID,ST.ACCBF,LASTPAYMENTDATE,MT.MEMBERTYPE_NAME,MM.MemberStatus,MM.NubeBanchName from nubestatus.dbo.STATUS" + dateMonth + " As ST Left Join ViewMasterMember As MM On MM.Member_Code = ST.Member_Code Left Join MasterMemberType As MT On MT.MemberType_Code = ST.MemberType_Code Left Join MasterStatus As MS On MS.Status_Code = ST.Status_Code where BranchName='" + Branch + "' and MM.RESIGNED=1", con);
        //            SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //            adp.Fill(dt);
        //            qry = "";


        //        }

        //    }


        //    catch (Exception ex)
        //    {
        //        ExceptionLogging.SendErrorToText(ex);
        //    }
        //    return dt;
        //}

      

        //public string Wqry()
        //{
        //    if (cmbNubeBranch.Text != "")
        //    {
        //        NubeBranch = cmbNubeBranch.Text;
        //        if (qry != "")
        //        {

        //            qry = qry + "and MM.NubeBanchName='" + NubeBranch + "'";
        //        }
        //        else
        //        {
        //            qry = "MM.NubeBanchName='" + NubeBranch + "'";
        //        }
        //    }


        //    if (cmbBank.Text != "")
        //    {
        //        BankName = cmbBank.Text;
        //        if (qry != "")
        //        {
        //            qry = qry + "and MM.BANK_NAME='" + BankName + "'";
        //        }
        //        else
        //        {
        //            qry = " MM.BANK_NAME='" + BankName + "'";
        //        }

        //    }
        //    if (cmbBranch.Text != "")
        //    {
        //        BranchName = cmbBranch.Text;
        //        if (qry != "")
        //        {
        //            qry = qry + "and MM.BranchName='" + BranchName + "'";
        //        }
        //        else
        //        {
        //            qry = "MM.BranchName='" + BranchName + "'";
        //        }
        //    }

        //    return qry;
        //}
    }
}




       
    

