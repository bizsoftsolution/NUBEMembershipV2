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

namespace Nube.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmDataAuditing.xaml
    /// </summary>
    public partial class frmDataAuditing : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        public frmDataAuditing()
        {
            InitializeComponent();
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
                progressBar1.Minimum = 1;
                progressBar1.Maximum = 10;
                progressBar1.Visibility = Visibility.Visible;
                DateTime dt = dtpDate.SelectedDate.Value;
                dt = new DateTime(dt.Year, dt.Month, 1);
                DateTime dtFromDate = dt.AddMonths(-1);
                DateTime dtToDate = dt.AddDays(-1);
                DateTime dtNextmonth = dt.AddMonths(1);
                DataTable dtOldStatus = new DataTable();
                DataTable dtNewStatus = new DataTable();

                progressBar1.Value = 4;
                System.Windows.Forms.Application.DoEvents();
                using (SqlConnection con = new SqlConnection(AppLib.connStr))
                {
                    SqlCommand cmd;
                    string str = string.Format("SELECT MEMBER_CODE,LASTPAYMENTDATE,TOTALMONTHSDUE,TOTALMONTHSPAID,STATUS_CODE,STRUCKOFF,RESIGNED FROM \r " +
                                               " NUBESTATUS..STATUS{0:MMyyyy} ST(NOLOCK)", dtFromDate);
                    cmd = new SqlCommand(str, con);
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.SelectCommand.CommandTimeout = 0;
                    adp.Fill(dtOldStatus);

                    cmd = new SqlCommand();
                    str = string.Format("SELECT MEMBER_CODE,LASTPAYMENTDATE,TOTALMONTHSDUE,TOTALMONTHSPAID,STATUS_CODE,STRUCKOFF,RESIGNED FROM \r " +
                                               " NUBESTATUS..STATUS{0:MMyyyy} ST(NOLOCK)", dt);
                    cmd = new SqlCommand(str, con);
                    adp = new SqlDataAdapter(cmd);
                    adp.SelectCommand.CommandTimeout = 0;
                    adp.Fill(dtNewStatus);
                }
                progressBar1.Value = 7;
                System.Windows.Forms.Application.DoEvents();
                DataView dv = new DataView(dtOldStatus);
                dv.RowFilter = " STATUS_CODE=1 ";

                DataTable dtOldActiveMember = new DataTable();
                dtOldActiveMember = dv.ToTable();
                txtPreviousMonthActiveMember.Text = dtOldActiveMember.Rows.Count.ToString();

                dv = new DataView(dtNewStatus);
                dv.RowFilter = " STATUS_CODE=1 ";

                DataTable dtNewActiveMember = new DataTable();
                dtNewActiveMember = dv.ToTable();
                txtCurrentMonthActiveMember.Text = dtNewActiveMember.Rows.Count.ToString();

                DateTime dtOldFDate = dtFromDate.AddMonths(-1);
                dtOldFDate = dtOldFDate.AddDays(-1);
                DateTime dtOldTDate = dtFromDate.AddDays(-1);
                dtOldTDate = dtOldTDate.AddDays(1);
                dtFromDate = dtFromDate.AddDays(-1);
                dtToDate = dtToDate.AddDays(-1);

                var lstOldMember = db.MASTERMEMBERs.Where(x => x.DATEOFJOINING > dtOldFDate && x.DATEOFJOINING < dtOldTDate).ToList();
                txtPreviousMonthNewMember.Text = lstOldMember.Count.ToString();
                var lstNewMember = db.MASTERMEMBERs.Where(x => x.DATEOFJOINING > dtFromDate && x.DATEOFJOINING < dtToDate).ToList();
                txtCurrentMonthNewMember.Text = lstNewMember.Count.ToString();


                var lstOldResignation = db.RESIGNATIONs.Where(x => x.VOUCHER_DATE > dtFromDate && x.VOUCHER_DATE < dtToDate).ToList();
                txtPreviousMonthResignedMember.Text = lstOldResignation.Count.ToString();
                var lstNewResignation = db.RESIGNATIONs.Where(x => x.VOUCHER_DATE > dtToDate && x.VOUCHER_DATE < dtNextmonth).ToList();
                txtCurrentMonthResignedMember.Text = lstNewResignation.Count.ToString();

                dv = new DataView(dtOldStatus);
                dv.RowFilter = " STATUS_CODE=2 ";

                DataTable dtOldDefaulterMember = new DataTable();
                dtOldDefaulterMember = dv.ToTable();
                txtPreviousMonthDefaulterMember.Text = dtOldDefaulterMember.Rows.Count.ToString();

                dv = new DataView(dtNewStatus);
                dv.RowFilter = " STATUS_CODE=2 ";

                DataTable dtNewDefaulterMember = new DataTable();
                dtNewDefaulterMember = dv.ToTable();
                txtCurrentMonthDefaulterMember.Text = dtNewDefaulterMember.Rows.Count.ToString();
                progressBar1.Value = 9;
                System.Windows.Forms.Application.DoEvents();
                if (dtOldActiveMember.Rows.Count == dtNewActiveMember.Rows.Count)
                {
                    if (lstNewMember.Count == 0 && lstNewResignation.Count == 0)
                    {

                    }
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(AppLib.connStr))
                    {
                        SqlCommand cmd;
                        string str = string.Format(" SELECT MEMBER_CODE,LASTPAYMENTDATE,TOTALMONTHSDUE,TOTALMONTHSPAID,STATUS_CODE,STRUCKOFF,RESIGNED \r " +
                                                   " FROM NUBESTATUS..STATUS{0:MMyyyy} (NOLOCK) WHERE STATUS_CODE=1 AND TOTALMONTH=1 \r " +
                                                   " AND MEMBER_CODE NOT IN (SELECT MEMBER_CODE FROM NUBESTATUS..STATUS{1:MMyyyy}(NOLOCK))", dtFromDate);
                        cmd = new SqlCommand(str, con);
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        adp.SelectCommand.CommandTimeout = 0;
                        adp.Fill(dtOldStatus);

                        cmd = new SqlCommand();
                        str = string.Format("SELECT MEMBER_CODE,LASTPAYMENTDATE,TOTALMONTHSDUE,TOTALMONTHSPAID,STATUS_CODE,STRUCKOFF,RESIGNED FROM \r " +
                                                   " NUBESTATUS..STATUS{0:MMyyyy} ST(NOLOCK)", dt);
                        cmd = new SqlCommand(str, con);
                        adp = new SqlDataAdapter(cmd);
                        adp.SelectCommand.CommandTimeout = 0;
                        adp.Fill(dtNewStatus);
                    }
                }

                dv = new DataView(dtOldStatus);
                dv.RowFilter = " STATUS_CODE=3 ";

                DataTable dtOldStruckOffMember = new DataTable();
                dtOldStruckOffMember = dv.ToTable();
                txtPreviousMonthStruckOffMember.Text = dtOldStruckOffMember.Rows.Count.ToString();

                dv = new DataView(dtNewStatus);
                dv.RowFilter = " STATUS_CODE=3 ";

                DataTable dtNewStruckOffMember = new DataTable();
                dtNewStruckOffMember = dv.ToTable();
                txtCurrentMonthStruckOffMember.Text = dtNewStruckOffMember.Rows.Count.ToString();

                //var lstStatus = (from x in db.MasterMemberStatus where x.FeeYear == dt.Year && x.FeeMonth == dt.Month select x).ToList();
                //var lstPreStatus = (from x in db.MasterMemberStatus where x.FeeYear == dtFromDate.Year && x.FeeMonth == dtFromDate.Month select x).ToList();

                //var Resign = (from x in lstStatus where x.RESIGNED == 1 select x).ToList();
                //var PreResign = (from x in lstPreStatus where x.RESIGNED == 1 select x).ToList();

                //if (Resign.Count != PreResign.Count)
                //{
                //    var rg = (from x in db.RESIGNATIONs where x.RESIGNATION_DATE >= dtFromDate && x.RESIGNATION_DATE <= dtToDate select x).ToList();
                //    if ((rg.Count + PreResign.Count) != Resign.Count)
                //    {

                //    }
                //}

                //var Active = (from x in lstStatus where x.STATUS_CODE == 1 select x).ToList();
                //var PreActive = (from x in lstPreStatus where x.STATUS_CODE == 1 select x).ToList();

                //if (Active.Count != PreActive.Count)
                //{
                //    var lstMember = (from x in db.MASTERMEMBERs where x.DATEOFJOINING >= dtFromDate && x.DATEOFJOINING <= dtToDate select x).ToList();

                //    int iTotalActive = PreActive.Count + lstMember.Count;
                //    if (iTotalActive == Active.Count)
                //    {

                //    }
                //}

                ////var lstmm = from mm in lstMember
                ////            join st in lstStatus on mm.MEMBER_CODE equals st.MEMBER_CODE into Stat
                ////            from StatusData in Stat.DefaultIfEmpty()
                ////            where new { lstStatus.}
                ////            select new { code = mm.MEMBER_CODE, Member = mm, Status = StatusData };

                ////foreach (var mm in lstMember)
                ////{

                ////}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
