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
using System.Data;
using System.Reflection;
using System.Data.SqlClient;

namespace Nube.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmMonthEndClosing.xaml
    /// </summary>
    public partial class frmMonthEndClosing : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        nubebfsstatusEntities dbStatus = new nubebfsstatusEntities();

        public frmMonthEndClosing()
        {
            InitializeComponent();
            progressBar1.Visibility = Visibility.Visible;
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            frmHomeMaster frm = new frmHomeMaster();
            this.Close();
            frm.ShowDialog();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtpDate.SelectedDate == null)
                {
                    MessageBox.Show("Select Month End");
                    dtpDate.Focus();
                }
                else
                {
                    DateTime dt = dtpDate.SelectedDate.Value;
                    DateTime dtFrom = new DateTime(dt.Year, dt.Month, 1);
                    DateTime dtTo = dtFrom.AddMonths(1);


                    var lstMember = db.MASTERMEMBERNEWs.Where(x => x.DATEOFJOINING < dtTo).ToList();
                    var lstFees = db.FeesDetails.Where(x => x.FeeYear == dtFrom.Year && x.FeeMonth == dtFrom.Month).ToList();

                    var lstMemberAndFees = from mm in lstMember
                                           from fee in lstFees
                                           where mm.MEMBER_CODE == fee.MemberCode
                                           select new { code = mm.MEMBER_CODE, Member = mm, Fee = fee };

                    var lstMemberAndFeesGroupByCode = lstMemberAndFees.GroupBy(x => x.code).ToList();

                    progressBar1.Minimum = 5;
                    progressBar1.Maximum = lstMemberAndFeesGroupByCode.Count;
                    progressBar1.Visibility = Visibility.Visible;
                    int i = 0;
                    foreach (var d in lstMemberAndFeesGroupByCode)
                    {
                        progressBar1.Value += 1;
                        System.Windows.Forms.Application.DoEvents();

                        var MM = d.FirstOrDefault().Member;
                        int MonDue = 0;
                        if (MM.RESIGNED && MM.ResignationDate < dtFrom)
                        {
                            continue;
                        }
                        else
                        {
                            var feeAmt = d.Sum(x => x.Fee.TotalAmount);
                            if (feeAmt > 0)
                            {
                                MM.LASTPAYMENT_DATE = dtTo.AddDays(-1);
                                MM.TOTALMONTHSPAID += d.Sum(x => x.Fee.TotalMonthsPaid);
                                MM.ACCSUBSCRIPTION += d.Sum(x => x.Fee.AmtSubs);
                                MM.ACCBF += d.Sum(x => x.Fee.AmountBF);
                            }

                            if (MM.REJOINED && MM.ResignationDate <= dtTo)
                            {
                                MM.STATUS_CODE = (int)AppLib.MemberStatus.Resigned;
                            }
                            else
                            {
                                MonDue = Math.Abs( (int)Math.Abs(MM.DATEOFJOINING.Value.Subtract(dtFrom).Days / (365.25 / 12)) - MM.TOTALMONTHSPAID.Value);
                                if (MonDue <= 3)
                                {
                                    MM.STATUS_CODE = (int)AppLib.MemberStatus.Active;
                                }
                                else if (Math.Abs((int)(MM.LASTPAYMENT_DATE.Value.Subtract(dtFrom).Days / (365.25 / 12))) <= 12)
                                {
                                    MM.STATUS_CODE = (int)AppLib.MemberStatus.Defaulter;
                                }
                                else
                                {
                                    MM.STATUS_CODE = (int)AppLib.MemberStatus.StruckOff;
                                    MM.STRUCKOFF = true;
                                }
                            }
                        }

                        MasterMemberStatu ss = new MasterMemberStatu()
                        {
                            MEMBER_CODE = MM.MEMBER_CODE,
                            MEMBERTYPE_CODE = MM.MEMBERTYPE_CODE,
                            BANK_CODE = MM.BANK_CODE,
                            BRANCH_CODE = MM.BRANCH_CODE,
                            NUBE_BRANCH_CODE = 0,
                            SUBSCRIPTION_AMOUNT = MM.MONTHLYSUBSCRIPTION,
                            BF_AMOUNT = MM.MONTHLYBF,
                            LASTPAYMENTDATE = dtTo.AddDays(-1),
                            TOTALSUBCRP_AMOUNT = d.Sum(x => x.Fee.AmtSubs),
                            TOTALBF_AMOUNT = d.Sum(x => x.Fee.AmountBF),
                            TOTAL_MONTHS = d.Sum(x => x.Fee.TotalMonthsPaid),
                            ENTRYMODE = "",
                            DEFAULTINGMONTHS = "",
                            TOTALMONTHSDUE = MonDue,
                            TOTALMONTHSPAID = MM.TOTALMONTHSPAID + d.Sum(x => x.Fee.TotalMonthsPaid),
                            SUBSCRIPTIONDUE = 0,
                            BFDUE = 0,
                            ACCSUBSCRIPTION = MM.ACCSUBSCRIPTION + d.Sum(x => x.Fee.AmtSubs),
                            ACCBF = MM.ACCBF + d.Sum(x => x.Fee.AmountBF),
                            ACCBENEFIT = MM.ACCBENEFIT,
                            CURRENT_YDTBF = MM.CURRENT_YTDBF + d.Sum(x => x.Fee.AmountBF),
                            CURRENT_YDTSUBSCRIPTION = MM.CURRENT_YTDSUBSCRIPTION + d.Sum(x => x.Fee.AmtSubs),
                            STATUS_CODE = MM.STATUS_CODE,
                            RESIGNED = Convert.ToDecimal(MM.RESIGNED),
                            CANCELLED = 0,
                            USER_CODE = 1,
                            ENTRY_DATE = DateTime.Now,
                            ENTRY_TIME = DateTime.Now.TimeOfDay.ToString(),
                            STRUCKOFF = MM.STATUS_CODE == 3 ? 1 : 0
                        };
                        db.MasterMemberStatus.Add(ss);
                    }
                    db.SaveChanges();
                    MessageBox.Show("Done");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLoad_Click1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Do You want to save this Record?", "MonthEnd Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (SqlConnection con = new SqlConnection(AppLib.connstatus))
                        {
                            DateTime dt = Convert.ToDateTime(dtpDate.SelectedDate).AddMonths(1);
                            dt = new DateTime(dt.Year, dt.Month, 1);
                            DateTime dtNxMonth = new DateTime(Convert.ToDateTime(dtpDate.SelectedDate).Year, Convert.ToDateTime(dtpDate.SelectedDate).Month, 1);
                            DateTime dtPreviousMonth = Convert.ToDateTime(dtpDate.SelectedDate).AddMonths(-2);
                            dtPreviousMonth = new DateTime(dtPreviousMonth.Year, dtPreviousMonth.Month, 1);
                            dtPreviousMonth = dtPreviousMonth.AddMonths(1).AddDays(-1);
                            dt = dt.AddDays(-1);

                            con.Open();
                            DataTable dtMember = new DataTable();
                            DataTable dtFeeEntry = new DataTable();
                            DataTable dtStatus = GetStatusTable();
                            int nCol = dtStatus.Columns.Count;

                            DataView dvFeeEntry = dtFeeEntry.AsDataView();

                            string Qry = string.Format(" INSERT INTO NUBEBFS..MASTERMEMBERNEW(MEMBER_CODE,MEMBER_NAME,MEMBER_ID,MEMBER_TITLE,BF_NO,ICNO_OLD,ICNO_NEW,BANK_CODE,BRANCH_CODE,ADDRESS1,ADDRESS2,ADDRESS3,CITY_CODE,STATE_CODE,COUNTRY,ZIPCODE,PHONE, \r " +
                                                       " MOBILE, EMAIL, DATEOFJOINING, DATEOFBIRTH, AGE_IN_YEARS, DATEOFEMPLOYMENT, MEMBERTYPE_CODE, SEX, RACE_CODE, REJOINED, STATUS_CODE, PREVIOUS_STATUSCODE, LEVY, LEVY_AMOUNT, LEVYPAYMENTDATE, \r " +
                                                       " TDF, TDF_AMOUNT, TDF_PAYMENTDATE, TOTALMONTHSPAID, ENTRANCEFEE, BATCHAMT, HQFEE, MONTHLYBF, MONTHLYSUBSCRIPTION, ACCBF, ACCSUBSCRIPTION, ACCBENEFIT, CURRENT_YTDBF, CURRENT_YTDSUBSCRIPTION, \r " +
                                                       " RESIGNED, REGISTRATION_FEE, LASTPAYMENT_DATE, BLACKLISTED, BLACKLISTREASON, STRUCKOFF, STRUCKOFF_REMARKS, STRUCKOFF_DATE, SALARY, MEMBERNAME_BYBANK, NRIC_BYBANK, BANKCODE_BYBANK, CREATEDON, \r " +
                                                       " CREATEDBY, UPDATEDON, UPDATEDBY, CANCELON, CANCELBY, AI_INSURANCE, AI_MEMBERNO, GE_INSURANCE, GE_CONTRACTNO, BRANCHMEMBERCODE) \r " +
                                                       " SELECT MEMBER_CODE,MEMBER_NAME,MEMBER_ID,MEMBER_TITLE,BF_NO,ICNO_OLD,ICNO_NEW,BANK_CODE,BRANCH_CODE,ADDRESS1,ADDRESS2,ADDRESS3,CITY_CODE,STATE_CODE,COUNTRY,ZIPCODE,PHONE, \r " +
                                                       " MOBILE, EMAIL, DATEOFJOINING, DATEOFBIRTH, AGE_IN_YEARS, DATEOFEMPLOYMENT, MEMBERTYPE_CODE,CASE WHEN SEX='MALE' THEN 0 ELSE 1 END, RACE_CODE, REJOINED,1, 1, LEVY, LEVY_AMOUNT, LEVYPAYMENTDATE, \r " +
                                                       " TDF, TDF_AMOUNT, TDF_PAYMENTDATE, 1, ENTRANCEFEE, BATCHAMT, HQFEE, MONTHLYBF, MONTHLYSUBSCRIPTION, ACCBF, ACCSUBSCRIPTION, ACCBENEFIT, CURRENT_YTDBF, CURRENT_YTDSUBSCRIPTION, \r " +
                                                       " RESIGNED, REGISTRATION_FEE,'{0:dd/MMM/yyyy}', BLACKLISTED, BLACKLISTREASON, 0, STRUCKOFF_REMARKS, STRUCKOFF_DATE, SALARY, MEMBERNAME_BYBANK, NRIC_BYBANK, BANKCODE_BYBANK, CREATEDON, \r " +
                                                       " 1, UPDATEDON, 0, '' CANCELON, 0 CANCELBY, AI_INSURANCE, AI_MEMBERNO, GE_INSURANCE, GE_CONTRACTNO,ISNULL(BRANCHMEMBERCODE,0) \r " +
                                                       " FROM NUBEBFS..MASTERMEMBER(NOLOCK) WHERE DATEOFJOINING < '{1:dd/MMM/yyyy}' AND DATEOFJOINING >='{2:dd/MMM/yyyy}' \r " +
                                                       " ORDER BY DATEOFJOINING ASC ", dt, dtNxMonth, dtPreviousMonth);
                            SqlCommand cmd = new SqlCommand(Qry, con);
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("New Member Insert Sucessfully!", "Sucess");

                            string str = string.Format(" SELECT NUBE_BRANCH_CODE,RESIGNED,* FROM NUBEBFS..MASTERMEMBERNEW MM(NOLOCK) \r " +
                                                       " LEFT JOIN NUBEBFS..MASTERBANKBRANCH BB(NOLOCK) ON BB.BANKBRANCH_CODE=MM.BRANCH_CODE \r " +
                                                       " WHERE MM.MEMBER_CODE NOT IN( \r " +
                                                       " SELECT MEMBER_CODE FROM NUBEBFS..RESIGNATION(NOLOCK) WHERE RESIGNATION_DATE<'{0:dd/MMM/yyyy}') \r " +
                                                       " ORDER BY MEMBER_CODE DESC ", dtNxMonth);
                            cmd = new SqlCommand(str, con);
                            SqlDataAdapter adp = new SqlDataAdapter(cmd);
                            adp.Fill(dtMember);

                            cmd = new SqlCommand(String.Format("SELECT * FROM NUBEBFS..FEESDETAILS(NOLOCK) WHERE FEEYEAR={0} and FEEMONTH={1}", Convert.ToDateTime(dtpDate.SelectedDate).Year, Convert.ToDateTime(dtpDate.SelectedDate).Month), con);
                            adp = new SqlDataAdapter(cmd);
                            adp.SelectCommand.CommandTimeout = 0;
                            adp.Fill(dtFeeEntry);
                            Qry = "";
                            var fees = dtFeeEntry.AsEnumerable().ToList();
                            progressBar1.Minimum = 5;
                            progressBar1.Maximum = dtMember.Rows.Count;
                            progressBar1.Visibility = Visibility.Visible;

                            int i = 0;
                            Qry = string.Format("DELETE FROM NUBEBFSSTATUS..STATUS{0:MMyyyy}", dt);
                            cmd = new SqlCommand(Qry, con);
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();

                            MessageBox.Show("All Process Complted, Now For Loop Loding!", "Sucess");
                            List<STATUS042016> lstStatus042016 = new List<STATUS042016>();
                            foreach (DataRow row in dtMember.Rows)
                            {
                                i = i + 1;
                                progressBar1.Value = i;
                                System.Windows.Forms.Application.DoEvents();

                                decimal amt = 0;
                                decimal BF = 0;
                                decimal Subs = 0;
                                DateTime doj = (DateTime)row["DATEOFJOINING"];
                                DateTime dlp = (DateTime)row["LASTPAYMENT_DATE"];

                                int TotalPaidMon = int.Parse(row["TOTALMONTHSPAID"].ToString());
                                int MonDue = Math.Abs(((int)(doj.Subtract(dt).Days / (365.25 / 12)) - TotalPaidMon));

                                var fee = fees.Where(x => x.Field<decimal>("MEMBERCODE") == Convert.ToDecimal(row["MEMBER_CODE"])).FirstOrDefault();
                                if (fee != null)
                                {
                                    amt = fee.Field<decimal>("TotalAmount");
                                    BF = fee.Field<decimal>("AmountBF");
                                    Subs = fee.Field<decimal>("AmtSubs");
                                }

                                if (amt != 0)
                                {
                                    TotalPaidMon += 1;
                                    MonDue -= 1;
                                    dlp = dt;
                                }

                                int StatusCode = 0;

                                if (MonDue <= 3)
                                {
                                    StatusCode = 1;
                                }
                                else
                                {
                                    if (Math.Abs((int)(dlp.Subtract(DateTime.Now).Days / (365.25 / 12))) <= 12)
                                    {
                                        StatusCode = 2;
                                    }
                                    else
                                    {
                                        StatusCode = 3;
                                    }
                                }

                                if (row["RESIGNED"].ToString() == "1")
                                {
                                    StatusCode = 4;
                                }

                                DateTime dtDate = Convert.ToDateTime(dtpDate.SelectedDate);
                                dtDate = new DateTime(dtDate.Year, dtDate.Month, 1);

                                cmd = new SqlCommand("SPMONTHEND", con);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add(new SqlParameter("@TABLENAME", string.Format("NUBEBFSSTATUS..STATUS{0:MMyyyy}", dt)));
                                cmd.Parameters.Add(new SqlParameter("@MEMBER_CODE", Convert.ToDecimal(row["MEMBER_CODE"])));
                                cmd.Parameters.Add(new SqlParameter("@MEMBERTYPE_CODE", Convert.ToDecimal(row["MEMBERTYPE_CODE"])));
                                cmd.Parameters.Add(new SqlParameter("@BANK_CODE", Convert.ToDecimal(row["BANK_CODE"])));
                                cmd.Parameters.Add(new SqlParameter("@BRANCH_CODE", Convert.ToDecimal(row["BRANCH_CODE"])));
                                cmd.Parameters.Add(new SqlParameter("@NUBE_BRANCH_CODE", Convert.ToDecimal(row["NUBE_BRANCH_CODE"])));
                                cmd.Parameters.Add(new SqlParameter("@MONTHLYSUBSCRIPTION", Convert.ToDecimal(row["MONTHLYSUBSCRIPTION"])));
                                cmd.Parameters.Add(new SqlParameter("@MONTHLYBF", Convert.ToDecimal(row["MONTHLYBF"])));
                                cmd.Parameters.Add(new SqlParameter("@LASTPAYMENTDATE", Convert.ToDecimal(row["LASTPAYMENT_DATE"])));
                                cmd.Parameters.Add(new SqlParameter("@SUBS", Subs));
                                cmd.Parameters.Add(new SqlParameter("@BF", BF));
                                cmd.Parameters.Add(new SqlParameter("@MONDUE", MonDue));
                                cmd.Parameters.Add(new SqlParameter("@TOTALPAIDMON", TotalPaidMon));
                                cmd.Parameters.Add(new SqlParameter("@ACCSUBSCRIPTION", Convert.ToDecimal(row["ACCSUBSCRIPTION"])));
                                cmd.Parameters.Add(new SqlParameter("@ACCBF", Convert.ToDecimal(row["ACCBF"])));
                                cmd.Parameters.Add(new SqlParameter("@ACCBENEFIT", Convert.ToDecimal(row["ACCBENEFIT"])));
                                cmd.Parameters.Add(new SqlParameter("@CURRENT_YTDBF", Convert.ToDecimal(row["CURRENT_YTDBF"])));
                                cmd.Parameters.Add(new SqlParameter("@CURRENT_YTDSUBSCRIPTION", Convert.ToDecimal(row["CURRENT_YTDSUBSCRIPTION"])));
                                cmd.Parameters.Add(new SqlParameter("@STATUSCODE", StatusCode));
                                cmd.Parameters.Add(new SqlParameter("@RESIGNED", row["RESIGNED"].ToString().ToUpper() == "FALSE" ? 0 : 1));
                                cmd.Parameters.Add(new SqlParameter("@STRUCKOFF", StatusCode == 3 ? 1 : 0));
                                cmd.Parameters.Add(new SqlParameter("@DLP_Date", dlp.Date));
                                progressBar1.Value = 7;
                                System.Windows.Forms.Application.DoEvents();

                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();

                                //STATUS042016 st = new STATUS042016();
                                //st.MEMBER_CODE = Convert.ToDecimal(row["MEMBER_CODE"]);
                                //st.MEMBERTYPE_CODE = Convert.ToDecimal(row["MEMBERTYPE_CODE"]);
                                //st.BANK_CODE = Convert.ToDecimal(row["BANK_CODE"]);
                                //st.BRANCH_CODE = Convert.ToDecimal(row["BRANCH_CODE"]);
                                //st.BRANCH_CODE = Convert.ToDecimal(row["NUBE_BRANCH_CODE"]);
                                //st.SUBSCRIPTION_AMOUNT = Convert.ToDecimal(row["MONTHLYSUBSCRIPTION"]);
                                //st.BF_AMOUNT = Convert.ToDecimal(row["MONTHLYBF"]);
                                //st.LASTPAYMENTDATE = Convert.ToDateTime(row["LASTPAYMENT_DATE"]).Date;
                                //st.TOTALSUBCRP_AMOUNT = Subs;
                                //st.TOTALBF_AMOUNT = BF;
                                //st.TOTAL_MONTHS = 1;
                                //st.ENTRYMODE = "";
                                //st.DEFAULTINGMONTHS = "";
                                //st.TOTALMONTHSDUE = MonDue;
                                //st.TOTALMONTHSPAID = TotalPaidMon;
                                //st.SUBSCRIPTIONDUE = 0;
                                //st.BFDUE = 0;
                                //st.ACCSUBSCRIPTION = Convert.ToDecimal(row["ACCSUBSCRIPTION"]) + Subs;
                                //st.ACCBF = Convert.ToDecimal(row["ACCBF"]) + BF;
                                //st.ACCBENEFIT = Convert.ToDecimal(row["ACCBENEFIT"]);
                                //st.CURRENT_YDTBF = Convert.ToDecimal(row["CURRENT_YTDBF"]) + BF;
                                //st.CURRENT_YDTSUBSCRIPTION = Convert.ToDecimal(row["CURRENT_YTDSUBSCRIPTION"]) + Subs;
                                //st.STATUS_CODE = StatusCode;
                                //st.RESIGNED = row["RESIGNED"].ToString().ToUpper() == "FALSE" ? 0 : 1;
                                //st.CANCELLED = 0;
                                //st.USER_CODE = 1;
                                //st.ENTRY_DATE = dt.Date;
                                //st.ENTRY_TIME = dt.Date.TimeOfDay.ToString();
                                //st.STRUCKOFF = StatusCode == 3 ? 1 : 0;
                                //lstStatus042016.Add(st);
                                //int iMemberCode = Convert.ToInt32(row["MEMBER_CODE"]);

                                //var mm = (from x in db.MASTERMEMBERNEWs where x.MEMBER_CODE == iMemberCode select x).FirstOrDefault();
                                //if (mm != null)
                                //{
                                //    mm.LASTPAYMENT_DATE = dlp.Date;
                                //    mm.TOTALMONTHSPAID = TotalPaidMon;                                    
                                //}
                            }
                            //if (lstStatus042016 != null)
                            //{
                            //    dbStatus.STATUS042016.AddRange(lstStatus042016);
                            //}
                            //db.SaveChanges();
                            con.Close();
                            MessageBox.Show("Months End Suceesfully Completed", "Sucess");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        enum enStatus
        {
            MEMBER_CODE, MEMBERTYPE_CODE, BANK_CODE, BRANCH_CODE, NUBE_BRANCH_CODE, SUBSCRIPTION_AMOUNT, BF_AMOUNT, LASTPAYMENTDATE,
            TOTALSUBCRP_AMOUNT, TOTALBF_AMOUNT, TOTAL_MONTHS, ENTRYMODE, DEFAULTINGMONTHS, TOTALMONTHSDUE, TOTALMONTHSPAID,
            SUBSCRIPTIONDUE, BFDUE, ACCSUBSCRIPTION, ACCBF, ACCBENEFIT, CURRENT_YDTBF, CURRENT_YDTSUBSCRIPTION, STATUS_CODE, RESIGNED,
            CANCELLED, USER_CODE, ENTRY_DATE, ENTRY_TIME, STRUCKOFF


        }
        DataTable GetStatusTable()
        {
            DataTable dtTemp = new DataTable();
            dtTemp.Columns.Add(enStatus.MEMBER_CODE.ToString(), typeof(int));
            dtTemp.Columns.Add(enStatus.MEMBERTYPE_CODE.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.BANK_CODE.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.BRANCH_CODE.ToString(), typeof(float));
            dtTemp.Columns.Add(enStatus.NUBE_BRANCH_CODE.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.SUBSCRIPTION_AMOUNT.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.BF_AMOUNT.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.LASTPAYMENTDATE.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.TOTALSUBCRP_AMOUNT.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.TOTALBF_AMOUNT.ToString(), typeof(decimal));
            dtTemp.Columns.Add(enStatus.TOTAL_MONTHS.ToString(), typeof(decimal));
            dtTemp.Columns.Add(enStatus.ENTRYMODE.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.DEFAULTINGMONTHS.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.TOTALMONTHSDUE.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.TOTALMONTHSPAID.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.SUBSCRIPTIONDUE.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.BFDUE.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.ACCSUBSCRIPTION.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.ACCBF.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.ACCBENEFIT.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.CURRENT_YDTBF.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.CURRENT_YDTSUBSCRIPTION.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.STATUS_CODE.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.RESIGNED.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.CANCELLED.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.USER_CODE.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.ENTRY_DATE.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.ENTRY_TIME.ToString(), typeof(string));
            dtTemp.Columns.Add(enStatus.STRUCKOFF.ToString(), typeof(string));
            return dtTemp;
        }

    }
}
