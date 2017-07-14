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
        string conn = AppLib.connstatus;
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
                if (MessageBox.Show("Do You want to save this Record?", "MonthEnd Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    MessageBox.Show("Already Month End Closed!","Sorry");
                    //DateTime dtime = Convert.ToDateTime(dtpDate.SelectedDate).AddMonths(1);
                    //dtime = new DateTime(dtime.Year, dtime.Month, 1);
                    //dtime = dtime.AddDays(-1);

                    //var fees = (from fd in db.FeesDetails where fd.UpdatedStatus == "Not Updated" & fd.FeeMonth == dtime.Month && fd.FeeYear == dtime.Year select fd).ToList();
                    //DataTable dtFees = LINQResultToDataTable(fees);

                    //if (dtFees.Rows.Count > 0)
                    //{
                    //    var mas = (from mm in db.MASTERMEMBERs where mm.DATEOFJOINING <= dtime select mm).ToList();
                    //    DataTable dtMember = LINQResultToDataTable(mas);

                    //    progressBar1.Minimum = 1;
                    //    progressBar1.Maximum = dtMember.Rows.Count;
                    //    progressBar1.Height = 25;
                    //    progressBar1.Visibility = Visibility.Visible;

                    //    int i = 0;
                    //    using (SqlConnection con = new SqlConnection(conn))
                    //    {
                    //        con.Open();
                    //        string sQry = string.Format("delete from nubestatus..status{0:MMyyyy}", dtime);
                    //        SqlCommand cmd = new SqlCommand(sQry, con);
                    //        cmd.ExecuteNonQuery();
                    //        foreach (DataRow row in dtMember.Rows)
                    //        {
                    //            progressBar1.Value = i + 1;
                    //            System.Windows.Forms.Application.DoEvents();

                    //            DataTable dtUpdate = dtFees.Select("MEMBERCODE=" + row["MEMBER_CODE"]).CopyToDataTable();
                    //            if (dtUpdate.Rows.Count > 0)
                    //            {
                    //                decimal amt = 0;
                    //                decimal BF = 0;
                    //                decimal Subs = 0;
                    //                DateTime doj = (DateTime)row["DATEOFJOINING"];
                    //                DateTime dlp = (DateTime)row["LASTPAYMENT_DATE"];
                    //                int StatusCode = 0;

                    //                int TotalPaidMon = int.Parse(row["TOTALMONTHSPAID"].ToString());
                    //                int MonDue = Math.Abs(((int)(doj.Subtract(dtime).Days / (365.25 / 12)) - TotalPaidMon));

                    //                amt = Convert.ToDecimal(dtUpdate.Rows[0]["TotalAmount"]);
                    //                BF = Convert.ToDecimal(dtUpdate.Rows[0]["AmountPf"]);
                    //                Subs = Convert.ToDecimal(dtUpdate.Rows[0]["AmtSubs"]);

                    //                if (amt != 0)
                    //                {
                    //                    TotalPaidMon += 1;
                    //                    MonDue -= 1;
                    //                    dlp = dtime;
                    //                }

                    //                if (row["RESIGNED"].ToString() == "1")
                    //                {
                    //                    StatusCode = 4;
                    //                }
                    //                else if (MonDue <= 3)
                    //                {
                    //                    StatusCode = 1;
                    //                }
                    //                else
                    //                {
                    //                    if (Math.Abs((int)(dlp.Subtract(DateTime.Now).Days / (365.25 / 12))) <= 12)
                    //                    {
                    //                        StatusCode = 2;
                    //                    }
                    //                    else
                    //                    {
                    //                        StatusCode = 3;
                    //                    }
                    //                }

                    //                string Qry = string.Format("INSERT INTO nubestatus..STATUS{0:MMyyyy}(MEMBER_CODE, MEMBERTYPE_CODE, BANK_CODE, BRANCH_CODE, NUBE_BRANCH_CODE, SUBSCRIPTION_AMOUNT, BF_AMOUNT, LASTPAYMENTDATE, TOTALSUBCRP_AMOUNT, TOTALBF_AMOUNT, TOTAL_MONTHS, ENTRYMODE, DEFAULTINGMONTHS, TOTALMONTHSDUE, TOTALMONTHSPAID, SUBSCRIPTIONDUE, BFDUE, ACCSUBSCRIPTION, ACCBF, ACCBENEFIT, CURRENT_YDTBF, CURRENT_YDTSUBSCRIPTION, STATUS_CODE, RESIGNED, CANCELLED, USER_CODE, ENTRY_DATE, ENTRY_TIME, STRUCKOFF) VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}')",
                    //                 dtpDate.SelectedDate, row["MEMBER_CODE"].ToString(), row["MEMBERTYPE_CODE"].ToString(), row["BANK_CODE"].ToString(), row["BRANCH_CODE"].ToString(), row["NUBE_BRANCH_CODE"].ToString() == "" ? "0" : row["NUBE_BRANCH_CODE"].ToString(), '0', '0', row["LASTPAYMENT_DATE"].ToString(), Subs, BF, 1, "", "", MonDue, TotalPaidMon, 0, 0, Subs, BF, 0, 0, 0, StatusCode, row["RESIGNED"], 0, 0, dtime, "", StatusCode == 3 ? 1 : 0);
                    //                cmd = new SqlCommand(Qry, con);
                    //                cmd.ExecuteNonQuery();

                    //                decimal dMemberCode = Convert.ToDecimal(row["MEMBER_CODE"]);
                    //                MASTERMEMBER mm = (from x in db.MASTERMEMBERs where x.MEMBER_CODE == dMemberCode select x).FirstOrDefault();
                    //                if (mm != null)
                    //                {
                    //                    mm.TOTALMONTHSPAID = mm.TOTALMONTHSPAID + 1;
                    //                    mm.LASTPAYMENT_DATE = Convert.ToDateTime(dtime);
                    //                }
                    //                db.SaveChanges();
                    //            }
                    //        }

                    //    }
                    //}
                    //else
                    //{
                    //MessageBox.Show("Already Month End Closed");
                    //}
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }
              
    }
}
