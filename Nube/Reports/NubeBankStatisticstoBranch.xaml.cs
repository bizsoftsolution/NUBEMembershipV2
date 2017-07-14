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
using System.Collections.ObjectModel;
using System.Data.SqlClient;

namespace Nube
{
    /// <summary>
    /// Interaction logic for NubeBankStatisticstoBranch.xaml
    /// </summary>
    public partial class NubeBankStatisticstoBranch : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();        
        string Qry = "";
        string connStr =AppLib.connStr;
        DataTable dtBankStatistic = new DataTable();

        ObservableCollection<branchStatistics> lstBranchStatistic = new ObservableCollection<branchStatistics>();

        public NubeBankStatisticstoBranch()
        {
            InitializeComponent();
            dgvBankStatistics.ItemsSource = lstBranchStatistic;

        }

        private void LoadReport()
        {
            dgvBankStatistics.ItemsSource = dtBankStatistic.DefaultView;
            var nubeBranch = db.MASTERNUBEBRANCHes.Select(x => x.NUBE_BRANCH_CODE).ToList();
        }

        private int GetData(string Gender, string Race, string BranchCode, Boolean IsBenefit, Boolean IsNubeBranch)
        {
            string dateMonth = string.Format("{0:MMyyyy}", dtpDOB.SelectedDate.Value);
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                Qry = String.Format("select isnull(Count(*),0) from {0}.dbo.Status{1:MMyyyy} AS ST Left Join MasterMember As MM On ST.Member_Code=MM.Member_Code where MM.Sex='{2}' And MM.Race_Code={3} And ST.Status_Code{4}{5} and ST.{6}={7}", AppLib.DBStatus, dtpDOB.SelectedDate.Value, Gender, Race, "=", (IsBenefit == true) ? 1 : 2, "Branch_Code", BranchCode);
                SqlCommand cmd = new SqlCommand(Qry, con);
                Int32 count = (Int32)cmd.ExecuteScalar();
                return count;
            }
        }

        private void GetDetails()
        {
            DataTable dt = new DataTable();

            string dateMonth = string.Format("{0:MMyyyy}", dtpDOB.SelectedDate.Value);
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                Qry = String.Format("select isnull(nb.NUBE_BRANCH_NAME,'') AS NUBE_BRANCH_NAME,isnull(bk.BANK_USERCODE,'')+'_'+isnull(br.BANKBRANCH_USERCODE,'') as BankBranch,isnull(mm.SEX,'') as SEX,isnull(mm.RACE_CODE,0) as RACE_CODE,isnull(mm.MEMBERTYPE_CODE,0) as MEMBERTYPE_CODE from {1}.dbo.STATUS{2} as st left join {0}.dbo.MASTERMEMBER as mm on mm.MEMBER_CODE=st.MEMBER_CODE left join {0}.dbo.MASTERBANK as bk on st.BANK_CODE = bk.BANK_CODE left join {0}.dbo.MASTERNUBEBRANCH as nb on nb.NUBE_BRANCH_CODE=st.NUBE_BRANCH_CODE left join {0}.dbo.MASTERBANKBRANCH as br on br.BANKBRANCH_CODE=st.BRANCH_CODE  where st.RESIGNED=0", AppLib.DBBFS, AppLib.DBStatus,dateMonth);
                SqlCommand cmd = new SqlCommand(Qry, con);
                SqlDataAdapter sdp = new SqlDataAdapter(cmd);
                sdp.Fill(dt);
            }
            var datas = (from DataRow row in dt.Rows
                         select new
                         {
                             Nube_branch_code= (string)(row["NUBE_BRANCH_NAME"]??""),
                             Branch_Code = (string)(row["BankBranch"]??""),
                             SEX = (string)(row["SEX"]??""),
                             RACE_CODE = (decimal)(row["RACE_CODE"]),
                             MEMBERTYPE_CODE = (decimal)(row["MEMBERTYPE_CODE"])
                         }).ToList();

            var BankBranchDatas1 = datas.GroupBy(x => x.Branch_Code).ToList();
            var BankBranchdatas2 = BankBranchDatas1.Select(x => new branchStatistics
            {

                BranchCode = x.Key.ToString(),

                CMM = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 1 && y.MEMBERTYPE_CODE == 1).Count(),
                CMI = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 2 && y.MEMBERTYPE_CODE == 1).Count(),
                CMC = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 3 && y.MEMBERTYPE_CODE == 1).Count(),
                CMO = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 4 && y.MEMBERTYPE_CODE == 1).Count(),

                CFM = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 1 && y.MEMBERTYPE_CODE == 1).Count(),
                CFI = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 2 && y.MEMBERTYPE_CODE == 1).Count(),
                CFC = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 3 && y.MEMBERTYPE_CODE == 1).Count(),
                CFO = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 4 && y.MEMBERTYPE_CODE == 1).Count(),

                NFM = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 1 && y.MEMBERTYPE_CODE == 2).Count(),
                NFI = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 2 && y.MEMBERTYPE_CODE == 2).Count(),
                NFC = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 3 && y.MEMBERTYPE_CODE == 2).Count(),
                NFO = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 4 && y.MEMBERTYPE_CODE == 2).Count(),

                NMM = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 1 && y.MEMBERTYPE_CODE == 2).Count(),
                NMI = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 2 && y.MEMBERTYPE_CODE == 2).Count(),
                NMC = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 3 && y.MEMBERTYPE_CODE == 2).Count(),
                NMO = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 4 && y.MEMBERTYPE_CODE == 2).Count()

            });
            var BankBranchDatas3 = BankBranchdatas2.Select(x => new branchStatistics
            {
                BranchCode=x.BranchCode,
                CMM = x.CMM,
                CMI = x.CMI,
                CMC = x.CMC,
                CMO = x.CMO,
                CMSTOT = x.CMM + x.CMI + x.CMC + x.CMO,
                CFC = x.CFC,
                CFI = x.CFI,
                CFM = x.CFM,
                CFO = x.CFO,
                CFSTOT = x.CFO + x.CFM + x.CFI + x.CFC,
                CTOTAL = x.CMM + x.CMI + x.CMC + x.CMO + x.CFO + x.CFM + x.CFI + x.CFC,

                NMM = x.NMM,
                NMI = x.NMI,
                NMO = x.NMO,
                NMC = x.NMC,
                NMSTOT = x.NMM + x.NMI + x.NMO + x.NMC,
                NFC = x.NFC,
                NFM = x.NFM,
                NFI = x.NFI,
                NFO = x.NFO,
                NFSTOT = x.NFC + x.NFM + x.NFI + x.NFO,

                NTOTAL = x.NMM + x.NMI + x.NMO + x.NMC + x.NFC + x.NFM + x.NFI + x.NFO,
                GTOTAL = x.CMM + x.CMI + x.CMC + x.CMO + x.CFO + x.CFM + x.CFI + x.CFC + x.NMM + x.NMI + x.NMO + x.NMC + x.NFC + x.NFM + x.NFI + x.NFO

            }).ToList();
            dgvBankStatistics.ItemsSource = BankBranchDatas3;


            var NubeBranchDatas1 = datas.GroupBy(x => x.Nube_branch_code).ToList();
            var NubeBranchdatas2 = NubeBranchDatas1.Select(x => new branchStatistics
            {

                BranchCode = x.Key.ToString(),

                CMM = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 1 && y.MEMBERTYPE_CODE == 1).Count(),
                CMI = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 2 && y.MEMBERTYPE_CODE == 1).Count(),
                CMC = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 3 && y.MEMBERTYPE_CODE == 1).Count(),
                CMO = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 4 && y.MEMBERTYPE_CODE == 1).Count(),

                CFM = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 1 && y.MEMBERTYPE_CODE == 1).Count(),
                CFI = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 2 && y.MEMBERTYPE_CODE == 1).Count(),
                CFC = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 3 && y.MEMBERTYPE_CODE == 1).Count(),
                CFO = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 4 && y.MEMBERTYPE_CODE == 1).Count(),

                NFM = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 1 && y.MEMBERTYPE_CODE == 2).Count(),
                NFI = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 2 && y.MEMBERTYPE_CODE == 2).Count(),
                NFC = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 3 && y.MEMBERTYPE_CODE == 2).Count(),
                NFO = x.Where(y => y.SEX == "Female" && y.RACE_CODE == 4 && y.MEMBERTYPE_CODE == 2).Count(),

                NMM = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 1 && y.MEMBERTYPE_CODE == 2).Count(),
                NMI = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 2 && y.MEMBERTYPE_CODE == 2).Count(),
                NMC = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 3 && y.MEMBERTYPE_CODE == 2).Count(),
                NMO = x.Where(y => y.SEX == "Male" && y.RACE_CODE == 4 && y.MEMBERTYPE_CODE == 2).Count()

            });
            var NubeBranchDatas3 = NubeBranchdatas2.Select(x => new branchStatistics
            {
                BranchCode = x.BranchCode,
                CMM = x.CMM,
                CMI = x.CMI,
                CMC = x.CMC,
                CMO = x.CMO,
                CMSTOT = x.CMM + x.CMI + x.CMC + x.CMO,
                CFC = x.CFC,
                CFI = x.CFI,
                CFM = x.CFM,
                CFO = x.CFO,
                CFSTOT = x.CFO + x.CFM + x.CFI + x.CFC,
                CTOTAL = x.CMM + x.CMI + x.CMC + x.CMO + x.CFO + x.CFM + x.CFI + x.CFC,

                NMM = x.NMM,
                NMI = x.NMI,
                NMO = x.NMO,
                NMC = x.NMC,
                NMSTOT = x.NMM + x.NMI + x.NMO + x.NMC,
                NFC = x.NFC,
                NFM = x.NFM,
                NFI = x.NFI,
                NFO = x.NFO,
                NFSTOT = x.NFC + x.NFM + x.NFI + x.NFO,

                NTOTAL = x.NMM + x.NMI + x.NMO + x.NMC + x.NFC + x.NFM + x.NFI + x.NFO,
                GTOTAL = x.CMM + x.CMI + x.CMC + x.CMO + x.CFO + x.CFM + x.CFI + x.CFC + x.NMM + x.NMI + x.NMO + x.NMC + x.NFC + x.NFM + x.NFI + x.NFO

            }).ToList();
            dgvNubeStatistics.ItemsSource = NubeBranchDatas3;



        }

        public class MemberStatistic
        {
            public decimal Member_code { get; set; }
            public decimal Nube_branch_code { get; set; }
            public decimal Branch_Code { get; set; }
            public string SEX { get; set; }
            public decimal RACE_CODE { get; set; }
            public decimal STATUS_CODE { get; set; }
            public decimal MEMBERTYPE_CODE { get; set; }

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            frmHomeReports frm = new frmHomeReports();
            this.Close();
            frm.ShowDialog();
        }

        private void dgBranchReport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //LoadReport();
            GetDetails();

        }

        class branchStatistics
        {
            public string BranchCode { get; set; }            

            public double CMM { get; set; }
            public double CMC { get; set; }
            public double CMI { get; set; }
            public double CMO { get; set; }
            public double CMSTOT { get; set; }

            public double CFM { get; set; }
            public double CFC { get; set; }
            public double CFI { get; set; }
            public double CFO { get; set; }
            public double CFSTOT { get; set; }
            public double CTOTAL { get; set; }

            public double NMM { get; set; }
            public double NMC { get; set; }
            public double NMI { get; set; }
            public double NMO { get; set; }
            public double NMSTOT { get; set; }

            public double NFM { get; set; }
            public double NFI { get; set; }
            public double NFC { get; set; }
            public double NFO { get; set; }
            public double NFSTOT { get; set; }
            public double NTOTAL { get; set; }

            public double GTOTAL { get; set; }
        }

        private void newDataTable()
        {
            dtBankStatistic.Columns.Add("BANKCODE");
            dtBankStatistic.Columns.Add("CMM");
            dtBankStatistic.Columns.Add("CMC");
            dtBankStatistic.Columns.Add("CMI");
            dtBankStatistic.Columns.Add("CMO");
            dtBankStatistic.Columns.Add("CFM ");

            dtBankStatistic.Columns.Add("CMSTOT ");
            dtBankStatistic.Columns.Add("CFM ");
            dtBankStatistic.Columns.Add("CFC ");
            dtBankStatistic.Columns.Add("CFI");
            dtBankStatistic.Columns.Add("CFSTOT");
            dtBankStatistic.Columns.Add("CTOTAL");

            dtBankStatistic.Columns.Add("NMM");
            dtBankStatistic.Columns.Add("NMC");
            dtBankStatistic.Columns.Add("NMI");
            dtBankStatistic.Columns.Add("NMO");
            dtBankStatistic.Columns.Add("NMSTOT");
            dtBankStatistic.Columns.Add("NFM ");

            dtBankStatistic.Columns.Add("NFC");
            dtBankStatistic.Columns.Add("NFI");
            dtBankStatistic.Columns.Add("NFO");
            dtBankStatistic.Columns.Add("NFSTOT");
            dtBankStatistic.Columns.Add("NTOTAL");
            dtBankStatistic.Columns.Add("GTOTAL");



            dtBankStatistic.Columns[0].DataType = typeof(string);
            dtBankStatistic.Columns[1].DataType = typeof(Int32);
            dtBankStatistic.Columns[2].DataType = typeof(Int32);
            dtBankStatistic.Columns[4].DataType = typeof(Int32);
            dtBankStatistic.Columns[5].DataType = typeof(Int32);
            dtBankStatistic.Columns[6].DataType = typeof(Int32);
            dtBankStatistic.Columns[7].DataType = typeof(Int32);
            dtBankStatistic.Columns[8].DataType = typeof(Int32);
            dtBankStatistic.Columns[9].DataType = typeof(Int32);
            dtBankStatistic.Columns[10].DataType = typeof(Int32);
            dtBankStatistic.Columns[11].DataType = typeof(Int32);
            dtBankStatistic.Columns[12].DataType = typeof(Int32);
            dtBankStatistic.Columns[13].DataType = typeof(Int32);
            dtBankStatistic.Columns[14].DataType = typeof(Int32);
            dtBankStatistic.Columns[15].DataType = typeof(Int32);
            dtBankStatistic.Columns[16].DataType = typeof(Int32);
            dtBankStatistic.Columns[17].DataType = typeof(Int32);
            dtBankStatistic.Columns[18].DataType = typeof(Int32);
            dtBankStatistic.Columns[19].DataType = typeof(Int32);
            dtBankStatistic.Columns[20].DataType = typeof(Int32);
            dtBankStatistic.Columns[21].DataType = typeof(Int32);
            dtBankStatistic.Columns[22].DataType = typeof(Int32);
            dtBankStatistic.Columns[23].DataType = typeof(Int32);


        }
    }
}
