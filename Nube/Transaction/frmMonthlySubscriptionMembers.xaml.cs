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
using System.Windows.Threading;

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmMonthlySubscriptionMembers.xaml
    /// </summary>
    public partial class frmMonthlySubscriptionMembers : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        public int MonthlySubscriptionId=0;
        List<MonthlySubscriptionMember> lstMSMembers = new List<MonthlySubscriptionMember>();
        public frmMonthlySubscriptionMembers()
        {
            InitializeComponent();

            cbxBank.ItemsSource = db.MASTERBANKs.OrderBy(x => x.BANK_NAME).ToList();
            cbxBank.DisplayMemberPath = "BANK_NAME";
            cbxBank.SelectedValuePath = "BANK_CODE";

            cbxMemberStatus.ItemsSource = db.MonthlySubscriptionMemberStatus.OrderBy(x => x.Status).ToList();
            cbxBank.DisplayMemberPath = "Status";
            cbxBank.SelectedValuePath = "Id";

            cbxApprovalStatus.ItemsSource = db.MonthlySubscriptionMatchingTypes.OrderBy(x => x.Name).ToList();
            cbxApprovalStatus.DisplayMemberPath = "Name";
            cbxApprovalStatus.SelectedValuePath = "Id";

            cbxMemberStatus.ItemsSource = db.MonthlySubscriptionMembers.Select(x => x.MemberName).Distinct().OrderBy(x=> x).ToList();
            cbxNRIC.ItemsSource = db.MonthlySubscriptionMembers.Select(x => x.NRIC).Distinct().OrderBy(x => x).ToList();           
        }

        void Search()
        {
            lstMSMembers = new List<MonthlySubscriptionMember>();

            try
            {
                var lst = db.MonthlySubscriptionMembers.Where(x => x.MonthlySubscriptionBank.MonthlySubscriptionId == MonthlySubscriptionId);
                if (!string.IsNullOrWhiteSpace(cbxBank.Text))
                {
                    try
                    {
                        decimal BankCode = (decimal)cbxBank.SelectedValue;
                        if(ckbFromMonthlySubscription.IsChecked==true)
                        {
                            lst = lst.Where(x => x.MonthlySubscriptionBank.BankCode == BankCode);
                        }
                        else
                        {
                            lst = lst.Where(x => x.MASTERMEMBER.BANK_CODE== BankCode);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (!string.IsNullOrWhiteSpace(cbxMemberStatus.Text))
                {
                    try
                    {
                        int Id = (int)cbxMemberStatus.SelectedValue;
                        lst = lst.Where(x => x.MonthlySubcriptionMemberStatusId == Id);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (!string.IsNullOrWhiteSpace(cbxApprovalStatus.Text))
                {
                    try
                    {
                        int Id = (int)cbxApprovalStatus.SelectedValue;
                        lst = lst.Where(x => x.MonthlySubscriptionMemberMatchingResults.Count(y => y.MonthlySubscriptionMatchingTypeId == Id) > 0);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                if (!string.IsNullOrWhiteSpace(cbxMemberName.Text))
                {
                    lst = lst.Where(x => x.MemberName.ToLower().Contains(cbxMemberName.Text.ToLower()));
                }
                if (!string.IsNullOrWhiteSpace(cbxNRIC.Text))
                {
                    lst = lst.Where(x => x.NRIC.ToLower().Contains(cbxNRIC.Text.ToLower()));
                }
                if (!string.IsNullOrWhiteSpace(txtMinAmount.Text))
                {
                    lst = lst.Where(x => x.Amount > Convert.ToDecimal(txtMinAmount.Text));
                }

                if (!string.IsNullOrWhiteSpace(txtMaxAmount.Text))
                {
                    lst = lst.Where(x => x.Amount < Convert.ToDecimal(txtMaxAmount.Text));
                }
                lst = lst.OrderBy(x => x.MonthlySubcriptionMemberStatusId).OrderBy(x => x.MemberName);
                lstMSMembers = lst.ToList();
            }
            catch(Exception ex)
            {

            }

            dgvMember.ItemsSource = lstMSMembers;
        }
        public void LoadDataByBank(int  MonthlySubscriptionBankId )
        {
            var data = db.MonthlySubscriptionBanks.FirstOrDefault(x => x.Id == MonthlySubscriptionBankId);
            
            if (data != null)
            {
                lstMSMembers = data.MonthlySubscriptionMembers.ToList();
                var lst = lstMSMembers.Select(x => new { x.MemberName, x.NRIC, x.Amount,BF =3, Ins=7,Subs=x.Amount-10,x.MonthlySubscriptionMemberStatu.Status,x.MonthlySubcriptionMemberStatusId }).ToList().OrderBy(x=> x.MonthlySubcriptionMemberStatusId).OrderBy(x=> x.MemberName).ToList();
                dgvMember.ItemsSource = lst;                
            }
            this.Title = $"Members : {data.MASTERBANK.BANK_NAME} - {data.MonthlySubscription.date:MMMM yyyy} ";
        }


        void MemberScan(MonthlySubscriptionMember msMember)
        {
            try
            {
                var mm = db.MASTERMEMBERs.Select(x => new { x.MEMBER_CODE, x.STATUS_CODE, x.BANK_CODE, x.MASTERBANK.BANK_NAME, x.MEMBER_NAME, x.ICNO_NEW, x.ICNO_OLD }).FirstOrDefault(x => x.ICNO_OLD == msMember.NRIC || x.ICNO_NEW == msMember.NRIC);
                if (mm == null)
                {
                    msMember.MonthlySubcriptionMemberStatusId = (int)AppLib.MonthlySubscriptionMemberStatus.SundryCreditor;
                    try
                    {                        
                        var msmmr = msMember.MonthlySubscriptionMemberMatchingResults.FirstOrDefault(x => x.MonthlySubscriptionMatchingTypeId == (int)AppLib.MonthlySubscriptionMatchingType.NRICNotMatched);
                        if (msmmr == null)
                        {
                            msmmr = new MonthlySubscriptionMemberMatchingResult()
                            {
                                MonthlySubscriptionMatchingTypeId = (int)AppLib.MonthlySubscriptionMatchingType.NRICNotMatched,
                                Description = $""
                            };
                            msMember.MonthlySubscriptionMemberMatchingResults.Add(msmmr);
                        }
                        db.MonthlySubscriptionMemberMatchingResults.RemoveRange(db.MonthlySubscriptionMemberMatchingResults.Where(x => x.MonthlySubscriptionMemberId == msMember.Id && x.MonthlySubscriptionMatchingTypeId != (int)AppLib.MonthlySubscriptionMatchingType.NRICNotMatched));
                    }
                    catch(Exception ex)
                    {

                    }                    
                }
                else
                {
                    msMember.MonthlySubcriptionMemberStatusId = (int)mm.STATUS_CODE.Value;
                    msMember.MemberCode = mm.MEMBER_CODE;
                    try
                    {
                        var msmmr = msMember.MonthlySubscriptionMemberMatchingResults.FirstOrDefault(x => x.MonthlySubscriptionMatchingTypeId == (int)AppLib.MonthlySubscriptionMatchingType.NRICNotMatched);
                        if (msmmr != null) msMember.MonthlySubscriptionMemberMatchingResults.Remove(msmmr);

                        msmmr = msMember.MonthlySubscriptionMemberMatchingResults.FirstOrDefault(x => x.MonthlySubscriptionMatchingTypeId == (int)AppLib.MonthlySubscriptionMatchingType.NRICMatched);
                        if (msmmr == null)
                        {
                            msmmr = new MonthlySubscriptionMemberMatchingResult()
                            {
                                MonthlySubscriptionMatchingTypeId = (int)AppLib.MonthlySubscriptionMatchingType.NRICMatched,
                                Description = $"",
                                ApprovedBy = AppLib.iUserCode
                            };
                            msMember.MonthlySubscriptionMemberMatchingResults.Add(msmmr);
                        }

                        msmmr = msMember.MonthlySubscriptionMemberMatchingResults.FirstOrDefault(x => x.MonthlySubscriptionMatchingTypeId == (int)AppLib.MonthlySubscriptionMatchingType.MismatchedMemberName);
                        if (msMember.MemberName.ToLower() != mm.MEMBER_NAME.ToLower())
                        {
                            if (msmmr == null)
                            {
                                msmmr = new MonthlySubscriptionMemberMatchingResult()
                                {
                                    MonthlySubscriptionMatchingTypeId = (int)AppLib.MonthlySubscriptionMatchingType.MismatchedMemberName,
                                };
                                msMember.MonthlySubscriptionMemberMatchingResults.Add(msmmr);
                            }
                            msmmr.Description = $"From Bank : {msMember.MemberName}\r\nFrom NUBE : {mm.MEMBER_NAME}";
                        }
                        else
                        {
                            if (msmmr != null) msMember.MonthlySubscriptionMemberMatchingResults.Remove(msmmr);
                        }


                        msmmr = msMember.MonthlySubscriptionMemberMatchingResults.FirstOrDefault(x => x.MonthlySubscriptionMatchingTypeId == (int)AppLib.MonthlySubscriptionMatchingType.MismatchedBank);
                        if (msMember.MonthlySubscriptionBank.BankCode != mm.BANK_CODE)
                        {
                            if (msmmr == null)
                            {
                                msmmr = new MonthlySubscriptionMemberMatchingResult()
                                {
                                    MonthlySubscriptionMatchingTypeId = (int)AppLib.MonthlySubscriptionMatchingType.MismatchedBank,
                                };
                                msMember.MonthlySubscriptionMemberMatchingResults.Add(msmmr);
                            }                            
                            msmmr.Description = $"From Bank : {msMember.MonthlySubscriptionBank.MASTERBANK.BANK_NAME}\r\nFrom NUBE : {mm.BANK_NAME}";
                        }
                        else
                        {
                            if (msMember != null) msMember.MonthlySubscriptionMemberMatchingResults.Remove(msmmr);
                        }


                        msmmr = msMember.MonthlySubscriptionMemberMatchingResults.FirstOrDefault(x => x.MonthlySubscriptionMatchingTypeId == (int)AppLib.MonthlySubscriptionMatchingType.StruckOffMembers);
                        if (msMember.MonthlySubcriptionMemberStatusId == (int) AppLib.MonthlySubscriptionMemberStatus.StruckOff)
                        {
                            if (msmmr == null)
                            {
                                msmmr = new MonthlySubscriptionMemberMatchingResult()
                                {
                                    MonthlySubscriptionMatchingTypeId = (int)AppLib.MonthlySubscriptionMatchingType.StruckOffMembers,
                                };
                                msMember.MonthlySubscriptionMemberMatchingResults.Add(msmmr);
                            }
                            
                            msmmr.Description = $"";
                        }
                        else
                        {
                            if (msMember != null) msMember.MonthlySubscriptionMemberMatchingResults.Remove(msmmr);
                        }

                        msmmr = msMember.MonthlySubscriptionMemberMatchingResults.FirstOrDefault(x => x.MonthlySubscriptionMatchingTypeId == (int)AppLib.MonthlySubscriptionMatchingType.ResignedMembers);
                        if (msMember.MonthlySubcriptionMemberStatusId == (int)AppLib.MonthlySubscriptionMemberStatus.StruckOff)
                        {
                            if (msmmr == null)
                            {
                                msmmr = new MonthlySubscriptionMemberMatchingResult()
                                {
                                    MonthlySubscriptionMatchingTypeId = (int)AppLib.MonthlySubscriptionMatchingType.ResignedMembers,
                                };
                                msMember.MonthlySubscriptionMemberMatchingResults.Add(msmmr);
                            }
                            msmmr.Description = $"";
                        }
                        else
                        {
                            if (msMember != null) msMember.MonthlySubscriptionMemberMatchingResults.Remove(msmmr);
                        }

                    }
                    catch(Exception ex)
                    {

                    }                    
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void DgvMember_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            }
            catch (Exception ex) { }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            cbxBank.Text = "";
            ckbFromMonthlySubscription.IsChecked = true;
            cbxMemberStatus.Text = "";
            cbxApprovalStatus.Text = "";
            cbxMemberName.Text = "";
            cbxNRIC.Text = "";
            txtMinAmount.Text = "";
            txtMaxAmount.Text = "";
        }

        private void BtnScan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pbrStatus.Minimum = 0;
                pbrStatus.Maximum = lstMSMembers.Count();
                pbrStatus.Value = 0;
                foreach (var msMember in lstMSMembers)
                {
                    MemberScan(msMember);
                    pbrStatus.Value++;
                    DoEvents();
                }
                MessageBox.Show("Scaning Done");
                Search();
            }
            catch(Exception ex) { }
            
        }
        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }
    }
}
