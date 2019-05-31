using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmMonthEndClosed.xaml
    /// </summary>
    public partial class frmMonthEndClosed : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        Model.MonthlySubs data = new Model.MonthlySubs();
        ObservableCollection<Model.MonthEndClosed> lstMonthEndClosed = new ObservableCollection<Model.MonthEndClosed>();
        List<MASTERMEMBER> lstMasterMember = new List<MASTERMEMBER>();
        List<MonthlySubscriptionMember> lstPaidMember = new List<MonthlySubscriptionMember>();
        DateTime dtMonthEnd, dtMonthEndPrevious, dtMonthEndNext;
        public frmMonthEndClosed(Model.MonthlySubs d)
        {
            InitializeComponent();
            data = d;
            dtMonthEnd = d.SelecctedDate;
            dtMonthEndPrevious = d.SelecctedDate.AddMonths(-1);
            dtMonthEndNext = d.SelecctedDate.AddMonths(1);
            dgvBank.ItemsSource = lstMonthEndClosed;
        }

        private void BtnMonthEndClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUserId.Text))
                {
                    MessageBox.Show("Please enter the User Id");
                    txtUserId.Focus();
                }
                else if (string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    MessageBox.Show("Please enter the password");
                    txtPassword.Focus();
                }
                else if (db.UserAccounts.FirstOrDefault(x => x.UserName == txtUserId.Text && x.Password == txtPassword.Password.ToString()) == null)
                {
                    MessageBox.Show("Please enter the valid User Id and Password");
                }
                else if (MessageBox.Show($"Do you want to Close the Month End of {data.SelecctedDate:MMMM yyyy}?", "Month End Closed", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    MonthEndClosed();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Month End not Closed");
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new ThreadStart(loadData));
        }

        void MonthEndClosed()
        {
            var lstMemberMonthEndPrevious = db.MemberMonthEndStatus.Where(x => x.StatusMonth == dtMonthEndPrevious).ToList();
            var lstMemberMonthEnd = db.MemberMonthEndStatus.Where(x => x.StatusMonth == dtMonthEnd).ToList();
            foreach (var mce in lstMonthEndClosed)
            {
                var lstMember = lstMasterMember.Where(x => x.BANK_CODE == mce.BankCode).ToList();
                foreach(var mm in lstMember)
                {
                    var mmME_Pre = lstMemberMonthEndPrevious.FirstOrDefault(x => x.MEMBER_CODE == mm.MEMBER_CODE);
                    var mmME = lstMemberMonthEnd.FirstOrDefault(x => x.MEMBER_CODE == mm.MEMBER_CODE);
                    var mmPaid = lstPaidMember.FirstOrDefault(x => x.MemberCode == mm.MEMBER_CODE);
                    

                    if (mmME_Pre == null)
                    {
                        if (mm.DATEOFJOINING?.Year == dtMonthEnd.Year && mm.DATEOFJOINING?.Month == dtMonthEnd.Month)
                        {
                            mmME_Pre = new MemberMonthEndStatu() {

                            };
                        }
                        else
                        {
                            MessageBox.Show($"Member Id[{mm.MEMBER_ID}] is not found on Previous month history.");
                            return;
                        }
                    }
                    if (mmME == null)
                    {
                        mmME = new MemberMonthEndStatu();
                        mmME.StatusMonth = dtMonthEnd;
                        mmME.MEMBER_CODE = mm.MEMBER_CODE;

                        db.MemberMonthEndStatus.Add(mmME);
                    }

                    decimal BF = 3;
                    decimal Ins = 7;
                    decimal Subs = mmPaid == null ? mmME_Pre.SUBSCRIPTION_AMOUNT??0 : mmPaid.Amount - (BF + Ins);

                    mmME.MEMBERTYPE_CODE = mm.MEMBERTYPE_CODE;
                    mmME.BANK_CODE = mm.BANK_CODE;
                    mmME.BRANCH_CODE = mm.BRANCH_CODE;
                    mmME.NUBE_BRANCH_CODE = mm.MASTERBANKBRANCH.NUBE_BRANCH_CODE;

                    mmME.SUBSCRIPTION_AMOUNT = Subs;
                    mmME.BF_AMOUNT = BF;
                    mmME.INSURANCE_AMOUNT = Ins;
                    mmME.ENTRYMODE = "S";
                    mmME.TOTALMONTHSCONTRIBUTION = mmME_Pre.TOTALMONTHSCONTRIBUTION + 1;

                    if (mmPaid == null)
                    {
                        mmME.ACCSUBSCRIPTION = mmME_Pre.ACCSUBSCRIPTION;
                        mmME.ACCBF = mmME_Pre.ACCBF;
                        mmME.ACCINSURANCE = mmME_Pre.ACCINSURANCE;

                        mmME.SUBSCRIPTIONDUE = mmME_Pre.SUBSCRIPTIONDUE+Subs;
                        mmME.BFDUE = mmME_Pre.BFDUE+BF;
                        mmME.INSURANCEDUE = mmME_Pre.INSURANCEDUE+Ins;

                        if (dtMonthEnd.Month == 4)
                        {
                            mmME.CURRENT_YDTSUBSCRIPTION = 0;
                            mmME.CURRENT_YDTBF = 0;
                            mmME.CURRENT_YDTINSURANCE = 0;
                        }
                        else
                        {
                            mmME.CURRENT_YDTSUBSCRIPTION = mmME_Pre.CURRENT_YDTSUBSCRIPTION;
                            mmME.CURRENT_YDTBF = mmME_Pre.CURRENT_YDTBF;
                            mmME.CURRENT_YDTINSURANCE = mmME_Pre.CURRENT_YDTINSURANCE;
                        }


                        mmME.TOTALSUBCRP_AMOUNT = 0;
                        mmME.TOTALBF_AMOUNT = 0;
                        mmME.TOTALINSURANCE_AMOUNT = 0;

                        mmME.TOTAL_MONTHS = 0;

                        mmME.TOTALMONTHSPAID = mmME_Pre.TOTALMONTHSPAID;
                        mmME.TOTALMONTHSDUE = mmME_Pre.TOTALMONTHSDUE + 1;
                        
                        mmME.LASTPAYMENTDATE = mmME_Pre.LASTPAYMENTDATE;
                    }
                    else
                    {
                        mmME.ACCSUBSCRIPTION = mmME_Pre.ACCSUBSCRIPTION + Subs;
                        mmME.ACCBF = mmME_Pre.ACCBF+BF;
                        mmME.ACCINSURANCE = mmME_Pre.ACCINSURANCE + Ins;

                        mmME.SUBSCRIPTIONDUE = mmME_Pre.SUBSCRIPTIONDUE;
                        mmME.BFDUE = mmME_Pre.BFDUE;
                        mmME.INSURANCEDUE = mmME_Pre.INSURANCEDUE;

                        if (dtMonthEnd.Month == 4)
                        {
                            mmME.CURRENT_YDTSUBSCRIPTION =  Subs;
                            mmME.CURRENT_YDTBF = BF;
                            mmME.CURRENT_YDTINSURANCE = Ins;
                        }
                        else
                        {
                            mmME.CURRENT_YDTSUBSCRIPTION = mmME_Pre.CURRENT_YDTSUBSCRIPTION + Subs;
                            mmME.CURRENT_YDTBF = mmME_Pre.CURRENT_YDTBF + BF;
                            mmME.CURRENT_YDTINSURANCE = mmME_Pre.CURRENT_YDTINSURANCE + Ins;
                        }

                        

                        mmME.TOTALSUBCRP_AMOUNT = Subs;
                        mmME.TOTALBF_AMOUNT = BF;
                        mmME.TOTALINSURANCE_AMOUNT = Ins;

                        mmME.TOTAL_MONTHS = 1;

                        mmME.TOTALMONTHSPAID = mmME_Pre.TOTALMONTHSPAID+1;
                        mmME.TOTALMONTHSDUE = mmME_Pre.TOTALMONTHSDUE;


                        mmME.LASTPAYMENTDATE = dtMonthEnd;
                    }


                    var mmResign = db.RESIGNATIONs.FirstOrDefault(x => x.MEMBER_CODE == mm.MEMBER_CODE);

                    if (mmResign != null)
                    {
                        mmME.RESIGNED = 1;                        
                        mmME.STATUS_CODE =(decimal) AppLib.MemberStatus.Resigned;
                    }
                    else if(mmME.LASTPAYMENTDATE.Value.MonthDiff(dtMonthEnd)>=12)
                    {
                        mmME.STRUCKOFF = 1;
                        mmME.STATUS_CODE = (decimal)AppLib.MemberStatus.StruckOff;
                    }
                    else if (mmME.TOTALMONTHSDUE > 3)
                    {
                        mmME.STATUS_CODE = (decimal)AppLib.MemberStatus.Defaulter;
                    }
                    else
                    {
                        mmME.STATUS_CODE = (decimal)AppLib.MemberStatus.Active;
                    }

                    mmME.USER_CODE = AppLib.iUserCode;

                    mmME.ENTRY_DATE = DateTime.Now;

                    mce.Closed += 1;
                    updateBankTotal();
                    System.Windows.Forms.Application.DoEvents();
                }
                db.SaveChanges();
            }
            MessageBox.Show("Month End Closed");
        }
        void loadData()
        {
            lstMasterMember = db.MASTERMEMBERs.Where(x => (x.STATUS_CODE == (decimal)AppLib.MemberStatus.Active || x.STATUS_CODE == (decimal)AppLib.MemberStatus.Defaulter) && x.DATEOFJOINING<dtMonthEndNext).ToList();
            lstPaidMember = db.MonthlySubscriptionMembers.Where(x => (x.MonthlySubcriptionMemberStatusId == (int)AppLib.MemberStatus.Active || x.MonthlySubcriptionMemberStatusId == (int)AppLib.MemberStatus.Defaulter) && x.MonthlySubscriptionBank.MonthlySubscription.date==dtMonthEnd).ToList();


            var lstBank = db.MASTERBANKs.OrderBy(x=> x.BANK_USERCODE).ToList();
            
            foreach(var bank in lstBank)
            {
                loadBank(bank);
            }
            updateBankTotal();
        }

        void loadBank(MASTERBANK bank)
        {            
            Model.MonthEndClosed mec = new Model.MonthEndClosed();
            mec.BankCode = bank.BANK_CODE;
            mec.BankName = $"{bank.BANK_USERCODE} - {bank.BANK_NAME}";
            mec.PaidA = lstPaidMember.Count(x => x.MASTERMEMBER.BANK_CODE == bank.BANK_CODE && x.MonthlySubcriptionMemberStatusId == (int)AppLib.MemberStatus.Active);
            mec.PaidD = lstPaidMember.Count(x => x.MASTERMEMBER.BANK_CODE == bank.BANK_CODE && x.MonthlySubcriptionMemberStatusId == (int)AppLib.MemberStatus.Defaulter);
            mec.Paid = mec.PaidA + mec.PaidD;

            mec.UnpaidA = lstMasterMember.Count(x => x.BANK_CODE == bank.BANK_CODE && x.STATUS_CODE==(decimal)AppLib.MemberStatus.Active) - mec.PaidA;
            mec.UnpaidD = lstMasterMember.Count(x => x.BANK_CODE == bank.BANK_CODE && x.STATUS_CODE == (decimal)AppLib.MemberStatus.Defaulter) - mec.PaidD;
            mec.Unpaid = mec.UnpaidA + mec.UnpaidD;

            mec.Total = lstMasterMember.Count(x=> x.BANK_CODE == bank.BANK_CODE);            

            if (mec.Total != 0)
            {
                lstMonthEndClosed.Add(mec);
                System.Windows.Forms.Application.DoEvents();
            }
            
        }

        void updateBankTotal()
        {
            var mec = lstMonthEndClosed.FirstOrDefault(x => x.BankName == "Total");
            if (mec == null)
            {
                mec = new Model.MonthEndClosed();
                mec.BankName = "Total";
                mec.PaidA = lstMonthEndClosed.Where(x => x.BankName != "Total").Sum(x => x.PaidA);
                mec.PaidD = lstMonthEndClosed.Where(x => x.BankName != "Total").Sum(x => x.PaidD);
                mec.Paid = lstMonthEndClosed.Where(x => x.BankName != "Total").Sum(x => x.Paid);

                mec.UnpaidA = lstMonthEndClosed.Where(x => x.BankName != "Total").Sum(x => x.UnpaidA);
                mec.UnpaidD = lstMonthEndClosed.Where(x => x.BankName != "Total").Sum(x => x.UnpaidD);
                mec.Unpaid = lstMonthEndClosed.Where(x => x.BankName != "Total").Sum(x => x.Unpaid);

                mec.Total = lstMonthEndClosed.Where(x => x.BankName != "Total").Sum(x => x.Total);

                lstMonthEndClosed.Add(mec);
            }
            mec.Closed= lstMonthEndClosed.Where(x => x.BankName != "Total").Sum(x => x.Closed);

        }

        private void DgvBank_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            }
            catch (Exception ex) { }
        }

        private void DgvBank_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Model.MonthlySubsBank d = dgvBank.SelectedItem as Model.MonthlySubsBank;
                if (data != null)
                {
                    if (data.MonthlySubscription.Id != 0)
                    {
                        frmMonthlySubscriptionMembers f = new frmMonthlySubscriptionMembers(data.MonthlySubscription.Id);
                        f.cbxBank.Text = d.BankName;
                        f.Search();
                        f.ShowDialog();
                    }
                }
            }
            catch (Exception ex) { }
        }
    }
}
