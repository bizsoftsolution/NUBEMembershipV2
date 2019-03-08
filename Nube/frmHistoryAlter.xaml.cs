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

namespace Nube
{
    /// <summary>
    /// Interaction logic for frmHistoryAlter.xaml
    /// </summary>
    public partial class frmHistoryAlter : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        MASTERMEMBER data = new MASTERMEMBER();
        MemberMonthEndStatu mmStatus = new MemberMonthEndStatu();
        public frmHistoryAlter()
        {
            InitializeComponent();
            this.DataContext = data;
            this.gbxStatus.DataContext = mmStatus;            
        }

        public void Search(decimal MEMBER_ID)
        {
            txtMemberId.Text = MEMBER_ID.ToString();
            var mm = db.MASTERMEMBERs.FirstOrDefault(x => x.MEMBER_ID == MEMBER_ID);
            if (mm == null)
            {
                MessageBox.Show("Enter the Valid Member Id");
                txtMemberId.Focusable = true;
                mm = new MASTERMEMBER();
            }
            data = mm;
            this.DataContext = data;            
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var MEMBER_ID = Convert.ToDecimal(txtMemberId.Text);
                Search(MEMBER_ID);
            }
            catch(Exception ex) { }            
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mmStatus.Id == 0)
                {
                    data.MemberMonthEndStatus.Add(mmStatus);
                }
                data.IsHistoryClean = false;
                db.SaveChanges();
            }
            catch(Exception ex) { }
            
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (mmStatus.Id == 0) return;
            if(MessageBox.Show("Do you delete this status?","Delete",MessageBoxButton.YesNo)== MessageBoxResult.Yes)
            {
                data.MemberMonthEndStatus.Remove(mmStatus);
                data.IsHistoryClean = false;
                db.SaveChanges();
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mmStatus = new MemberMonthEndStatu();

                mmStatus.MEMBERTYPE_CODE = data.MEMBERTYPE_CODE;
                mmStatus.BANK_CODE = data.BANK_CODE;
                mmStatus.BRANCH_CODE = data.BRANCH_CODE;
                mmStatus.NUBE_BRANCH_CODE = data.MASTERBANKBRANCH.NUBE_BRANCH_CODE;

                this.gbxStatus.DataContext = mmStatus;
            }catch(Exception ex) { }
        }

       
        private void DgrStatus_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                mmStatus = dgrStatus.SelectedItem as MemberMonthEndStatu;
                this.gbxStatus.DataContext = mmStatus;
            }catch(Exception ex) { }
        }

        private void BtnHistoryReCreate_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you want to Re-Create the History?", "Re-Create", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                MemberHistoryClean();
            }
        }

        void MemberHistoryClean()
        {
            try
            {
                AppLog.WriteLogFileName = $"MemberHistoryClean_MemberId_${data.MEMBER_ID}";

                DateTime dtFrom = data.MemberMonthEndStatus.Min(x => x.StatusMonth.Value);
                DateTime dtTo = data.MemberMonthEndStatus.Max(x => x.StatusMonth.Value);
                DateTime? dtResign = null;

                var resign = data.RESIGNATIONs.FirstOrDefault();                
                if (resign!=null) dtResign= resign.VOUCHER_DATE.Value;

                int CMon = 0;
                int PMon = 0;
                int DMon = 0;


                decimal BF = 0;
                decimal Subs = 0;
                decimal Ins = 0;

                decimal ConBF = 0;
                decimal ConSubs = 0;
                decimal ConIns = 0;

                decimal AccBF = 0;
                decimal AccSubs = 0;
                decimal AccIns = 0;

                decimal DueBF = 0;
                decimal DueSubs = 0;
                decimal DueIns = 0;


                decimal CurBF = 0;
                decimal CurSubs = 0;
                decimal CurIns = 0;

                var st = data.MemberMonthEndStatus.FirstOrDefault(x => x.StatusMonth == dtFrom);
                if (st == null)
                {
                    AppLog.WriteLog($"{dtFrom:MMMM yyyy} is missing in History");
                    return;
                }
                if (st.StatusMonth == new DateTime(2005, 9, 1))
                {
                    bool hasError = false;

                    PMon = Convert.ToInt32(st.TOTALMONTHSPAID.Value);
                    DMon = Convert.ToInt32(st.TOTALMONTHSDUE.Value);
                    CMon = PMon + DMon;

                    var dt = data.DATEOFJOINING.Value;
                    dt = new DateTime(dt.Year, dt.Month, 1);
                    var tmpCMon = dt.MonthDiff(dtFrom);
                    if (CMon != tmpCMon)
                    {
                        AppLog.WriteLog($"Contribute Month : {CMon}, Paid Month: {PMon}, Due Month: {DMon}");
                        AppLog.WriteLog($"Error: Contribute Month is mismatch. Actual Contribute Month is {tmpCMon}");
                        hasError = true;
                    }
                   

                    AccBF = st.ACCBF.Value;
                    AccSubs = st.ACCSUBSCRIPTION.Value;
                    AccIns = st.ACCINSURANCE.Value;

                    DueBF = st.BFDUE.Value;
                    DueSubs = st.SUBSCRIPTIONDUE.Value;
                    DueIns = st.INSURANCEDUE.Value;

                    AppLog.WriteLog($"AccBF: {AccBF}, AccSubs: {AccSubs}, AccIns: {AccIns}, DueBF: {DueBF}, DueSubs: {DueSubs}, DueIns: {DueIns}");

                    if (AccBF != (PMon * st.BF_AMOUNT))
                    {
                        AppLog.WriteLog("AccBF is mismatch");
                        hasError = true;
                    }

                    if (DueBF != (PMon * st.BF_AMOUNT))
                    {
                        AppLog.WriteLog("DueBF is mismatch");
                        hasError = true;
                    }

                    if (hasError) return;
                }
                else
                {
                    CMon = 1;
                    PMon = Convert.ToInt16(st.TOTAL_MONTHS.Value);
                }
                DMon = CMon - PMon;

                BF = st.BF_AMOUNT.Value;
                Subs = st.SUBSCRIPTION_AMOUNT.Value;
                Ins = st.INSURANCE_AMOUNT.Value;

                CurBF = st.CURRENT_YDTBF.Value;
                CurSubs = st.CURRENT_YDTSUBSCRIPTION.Value;
                CurIns = st.CURRENT_YDTINSURANCE.Value;

                AccBF = st.ACCBF.Value;
                AccSubs = st.ACCSUBSCRIPTION.Value;
                AccIns = st.ACCINSURANCE.Value;

                DueBF = st.BFDUE.Value;
                DueSubs = st.SUBSCRIPTIONDUE.Value;
                DueIns = st.INSURANCEDUE.Value;

                ConBF = AccBF + DueBF;
                ConSubs = AccSubs + DueSubs;
                ConIns = AccIns + DueIns;

                AppLog.WriteLog($"Contribute Month : {CMon}, Paid Month: {PMon}, Due Month: {DMon}");
                int LastPaidDue = 0;
                while (dtFrom < dtTo)
                {

                    
                    dtFrom = dtFrom.AddMonths(1);

                    if (dtFrom.Month == 4)
                    {
                        CurBF = 0;
                        CurSubs = 0;
                        CurIns = 0;
                    }

                    st = data.MemberMonthEndStatus.FirstOrDefault(x => x.StatusMonth == dtFrom);
                    if (st == null)
                    {
                        AppLog.WriteLog($"{dtFrom:MMMM yyyy} is missing in History");
                        return;
                    }
                    CMon = CMon + 1;
                    if (st.TOTAL_MONTHS > 0)
                    {
                        PMon = PMon + Convert.ToInt32(st.TOTAL_MONTHS.Value);

                        BF = st.TOTALBF_AMOUNT.Value;
                        Subs = st.TOTALSUBCRP_AMOUNT.Value;
                        Ins = st.TOTALINSURANCE_AMOUNT.Value;

                        AccBF += BF;
                        AccSubs += Subs;
                        AccIns += Ins;

                        CurBF += BF;
                        CurSubs += Subs;
                        CurIns += Ins;

                        st.LASTPAYMENTDATE = dtFrom;
                        LastPaidDue = 0;
                    }
                    else
                    {
                        LastPaidDue += 1;
                    }
                    ConBF += BF;
                    ConSubs += Subs;
                    ConIns += Ins;

                    st.TOTALMONTHSCONTRIBUTION = CMon;
                    st.TOTALMONTHSPAID = PMon;
                    st.TOTALMONTHSDUE = CMon - PMon;

                    st.BF_AMOUNT = BF;
                    st.SUBSCRIPTION_AMOUNT = Subs;
                    st.INSURANCE_AMOUNT = Ins;

                    st.CURRENT_YDTBF = CurBF;
                    st.CURRENT_YDTSUBSCRIPTION = CurSubs;
                    st.CURRENT_YDTINSURANCE = CurIns;

                    st.ACCBF = AccBF;
                    st.ACCSUBSCRIPTION = AccSubs;
                    st.ACCINSURANCE = AccIns;

                    st.BFDUE = ConBF-AccBF;
                    st.SUBSCRIPTIONDUE = ConSubs-AccSubs;
                    st.INSURANCEDUE = ConIns-AccIns;

                    st.STATUS_CODE = 0;
                    if (dtResign != null)
                    {
                        if(dtResign.Value.Year==dtFrom.Year && dtResign.Value.Month == dtFrom.Month)
                        {
                            st.STATUS_CODE = 4;
                        }
                    }
                    if (st.STATUS_CODE == 0)
                    {
                        if (LastPaidDue > 12)
                        {
                            st.STATUS_CODE = 3;
                        }
                        else if (st.TOTALMONTHSDUE > 3)
                        {
                            st.STATUS_CODE = 2;
                        }
                        else
                        {
                            st.STATUS_CODE = 1;
                        }
                    }
                }



                data.TOTALMONTHSPAID = st.TOTALMONTHSPAID;
                data.TOTALMONTHSDUE = st.TOTALMONTHSDUE;
                data.TOTALMONTHCONTRIBUTE = st.TOTALMONTHSCONTRIBUTION;

                data.ACCBF = st.ACCBF;
                data.ACCSUBSCRIPTION = st.ACCSUBSCRIPTION;
                data.ACCINSURANCE = st.ACCINSURANCE;

                data.DUEBF = st.BFDUE;
                data.DUEINSURANCE = st.INSURANCEDUE;
                data.DUESUBSCRIPTION = st.SUBSCRIPTIONDUE;

                data.CURRENT_YTDBF = st.CURRENT_YDTBF;
                data.CURRENT_YTDSUBSCRIPTION = st.CURRENT_YDTSUBSCRIPTION;
                data.CURRENT_YTDINSURANCE = st.CURRENT_YDTINSURANCE;

                data.LASTPAYMENT_DATE = st.LASTPAYMENTDATE;

                data.STATUS_CODE = st.STATUS_CODE;

                db.SaveChanges();
                MessageBox.Show("Done");
            }
            catch(Exception ex)
            {
                AppLog.WriteLog(ex);
                MessageBox.Show(ex.Message);
            }

            
        }
    }
}
