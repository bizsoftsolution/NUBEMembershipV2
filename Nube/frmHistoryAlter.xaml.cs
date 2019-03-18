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
			data = new MASTERMEMBER();
			this.DataContext = data;

			var mm = db.MASTERMEMBERs.FirstOrDefault(x => x.MEMBER_ID == MEMBER_ID);
			if (mm == null)
			{
				MessageBox.Show("Enter the Valid Member Id");
				txtMemberId.Focusable = true;
				mm = new MASTERMEMBER();
			}
			else
			{
				mm.MemberMonthEndStatus = mm.MemberMonthEndStatus.OrderBy(x => x.StatusMonth).ToList();
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

				var dtFrom = mmStatus.StatusMonth.Value;
				var dtTo = mmStatus.LASTPAYMENTDATE.Value;
				while (dtFrom <= dtTo)
				{
					var st = data.MemberMonthEndStatus.FirstOrDefault(x => x.StatusMonth == dtFrom);
					if (st == null)
					{
						st = new MemberMonthEndStatu();

						st.MEMBERTYPE_CODE = data.MEMBERTYPE_CODE;
						st.BANK_CODE = data.BANK_CODE;
						st.BRANCH_CODE = data.BRANCH_CODE;
						st.NUBE_BRANCH_CODE = data.MASTERBANKBRANCH.NUBE_BRANCH_CODE;
						st.StatusMonth = dtFrom;
						data.MemberMonthEndStatus.Add(st);
					}

					st.TOTAL_MONTHS = mmStatus.TOTAL_MONTHS;
					st.TOTALSUBCRP_AMOUNT = mmStatus.TOTALSUBCRP_AMOUNT;
					st.TOTALBF_AMOUNT = mmStatus.TOTALBF_AMOUNT;
					st.TOTALINSURANCE_AMOUNT = mmStatus.TOTALINSURANCE_AMOUNT;
					if ((st.StatusMonth.Value.Year == 2005 && st.StatusMonth.Value.Month == 9) || (st.StatusMonth.Value.Year == data.DATEOFJOINING.Value.Year && st.StatusMonth.Value.Month == data.DATEOFJOINING.Value.Month))
					{
						st.LASTPAYMENTDATE = data.DATEOFJOINING.Value;

						var cmon = data.DATEOFJOINING.Value.MonthDiff(st.StatusMonth.Value) + 1;
						var pmon = st.TOTAL_MONTHS;
						var dmon = cmon - pmon;


						st.TOTALMONTHSCONTRIBUTION = cmon;
						st.TOTALMONTHSPAID = pmon;
						st.TOTALMONTHSDUE = dmon;

						st.ACCSUBSCRIPTION = st.TOTALSUBCRP_AMOUNT;
						st.ACCBF = st.TOTALBF_AMOUNT;
						st.ACCINSURANCE = st.TOTALINSURANCE_AMOUNT;

						try
						{
							st.SUBSCRIPTION_AMOUNT = st.ACCSUBSCRIPTION / st.TOTALMONTHSPAID;
							st.BF_AMOUNT = st.ACCBF / st.TOTALMONTHSPAID;
							st.INSURANCE_AMOUNT = st.ACCINSURANCE / st.TOTALMONTHSPAID;

							st.SUBSCRIPTIONDUE = st.SUBSCRIPTION_AMOUNT * st.TOTALMONTHSDUE;
							st.BFDUE = st.BF_AMOUNT * st.TOTALMONTHSDUE;
							st.INSURANCEDUE= st.INSURANCE_AMOUNT * st.TOTALMONTHSDUE;							
						}
						catch (Exception ex) { }
						

					}
					dtFrom = dtFrom.AddMonths(1);
				}

				data.IsHistoryClean = false;
				db.SaveChanges();

				MessageBox.Show("Saved");
				try
				{
					var MEMBER_ID = Convert.ToDecimal(txtMemberId.Text);
					Search(MEMBER_ID);
				}
				catch (Exception ex) { }
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
				MessageBox.Show("Deleted");
				try
				{
					var MEMBER_ID = Convert.ToDecimal(txtMemberId.Text);
					Search(MEMBER_ID);
				}
				catch (Exception ex) { }

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
				mmStatus.LASTPAYMENTDATE = mmStatus.StatusMonth;
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
				DateTime dtLastPaid = data.DATEOFJOINING.Value;
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
					var str = $"{dtFrom:MMMM yyyy} is missing in History";

					AppLog.WriteLog(str);
					MessageBox.Show(str);
                    return;
                }
                if (st.StatusMonth == new DateTime(2005, 9, 1))
                {
                    bool hasError = false;

                    PMon = Convert.ToInt32(st.TOTALMONTHSPAID??0);
                    DMon = Convert.ToInt32(st.TOTALMONTHSDUE??0);
                    CMon = PMon + DMon;

                    var dt = data.DATEOFJOINING.Value;
                    dt = new DateTime(dt.Year, dt.Month, 1);
                    var tmpCMon = dt.MonthDiff(dtFrom)+1;
                    if (CMon != tmpCMon)
                    {
                        AppLog.WriteLog($"Contribute Month : {CMon}, Paid Month: {PMon}, Due Month: {DMon}");
                        AppLog.WriteLog($"Error: Contribute Month is mismatch. Actual Contribute Month is {tmpCMon}");
                        //hasError = true;
                    }
                   

                    AccBF = st.ACCBF??0;
                    AccSubs = st.ACCSUBSCRIPTION??0;
                    AccIns = st.ACCINSURANCE??0;

                    DueBF = st.BFDUE??0;
                    DueSubs = st.SUBSCRIPTIONDUE??0;
                    DueIns = st.INSURANCEDUE??0;
					
                    AppLog.WriteLog($"AccBF: {AccBF}, AccSubs: {AccSubs}, AccIns: {AccIns}, DueBF: {DueBF}, DueSubs: {DueSubs}, DueIns: {DueIns}");

                    if (AccBF != (PMon * st.BF_AMOUNT))
                    {
                        AppLog.WriteLog("AccBF is mismatch");
                        //hasError = true;
                    }

                    if (DueBF != (DMon * st.BF_AMOUNT))
                    {
                        AppLog.WriteLog("DueBF is mismatch");
                        //hasError = true;
                    }

					if (hasError)
					{
						MessageBox.Show("Error. for more details see log");
						return;
					}
                }
                else
                {
                    CMon = 1;
                    PMon = Convert.ToInt16(st.TOTAL_MONTHS??0);
                }
                DMon = CMon - PMon;

                BF = st.BF_AMOUNT??0;
                Subs = st.SUBSCRIPTION_AMOUNT??0;
                Ins = st.INSURANCE_AMOUNT??0;

                CurBF = st.CURRENT_YDTBF??0;
                CurSubs = st.CURRENT_YDTSUBSCRIPTION??0;
                CurIns = st.CURRENT_YDTINSURANCE??0;

                AccBF = st.ACCBF??0;
                AccSubs = st.ACCSUBSCRIPTION??0;
                AccIns = st.ACCINSURANCE??0;

                DueBF = st.BFDUE??0;
                DueSubs = st.SUBSCRIPTIONDUE??0;
                DueIns = st.INSURANCEDUE??0;

                ConBF = AccBF + DueBF;
                ConSubs = AccSubs + DueSubs;
                ConIns = AccIns + DueIns;
				dtLastPaid = st.LASTPAYMENTDATE.Value;
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
						var str = $"{dtFrom:MMMM yyyy} is missing in History";

						AppLog.WriteLog(str);
						MessageBox.Show(str);
                        return;
                    }
                    CMon = CMon + 1;
                    if (st.TOTAL_MONTHS > 0)
                    {
                        PMon = PMon + Convert.ToInt32(st.TOTAL_MONTHS??0);

                        BF = st.TOTALBF_AMOUNT??0;
                        Subs = st.TOTALSUBCRP_AMOUNT??0;
                        Ins = st.TOTALINSURANCE_AMOUNT??0;

                        AccBF += BF;
                        AccSubs += Subs;
                        AccIns += Ins;

                        CurBF += BF;
                        CurSubs += Subs;
                        CurIns += Ins;

						dtLastPaid = dtFrom;
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
					st.LASTPAYMENTDATE = dtLastPaid;
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
				try
				{
					var MEMBER_ID = Convert.ToDecimal(txtMemberId.Text);
					Search(MEMBER_ID);
				}
				catch (Exception ex) { }
			}
            catch(Exception ex)
            {
                AppLog.WriteLog(ex);
                MessageBox.Show(ex.Message);
            }

            
        }
    }
}
