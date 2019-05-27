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
        int MonthlySubscriptionId=0;
        DateTime MonthlySubsDate;
        List<Model.MonthlySubsMember> lstMSMembers = new List<Model.MonthlySubsMember>();
        List<Model.MonthSubsUnPaidMember> lstUnPaidMembers = new List<Model.MonthSubsUnPaidMember>();

        public frmMonthlySubscriptionMembers(int monthlySubscriptionId)
        {
            InitializeComponent();
            MonthlySubscriptionId = monthlySubscriptionId;
            MonthlySubsDate = db.MonthlySubscriptions.FirstOrDefault(x=> x.Id==MonthlySubscriptionId).date;

            cbxBank.ItemsSource = db.MASTERBANKs.OrderBy(x => x.BANK_NAME).ToList();
            cbxBank.DisplayMemberPath = "BANK_NAME";
            cbxBank.SelectedValuePath = "BANK_CODE";

            cbxMemberStatus.ItemsSource = db.MonthlySubscriptionMemberStatus.OrderBy(x => x.Status).ToList();
            cbxMemberStatus.DisplayMemberPath = "Status";
            cbxMemberStatus.SelectedValuePath = "Id";

            cbxApprovalStatus.ItemsSource = db.MonthlySubscriptionMatchingTypes.OrderBy(x => x.Name).ToList();
            cbxApprovalStatus.DisplayMemberPath = "Name";
            cbxApprovalStatus.SelectedValuePath = "Id";

            //cbxMemberName.ItemsSource = db.MonthlySubscriptionMembers.Select(x => x.MemberName).Distinct().OrderBy(x => x).ToList();
            //cbxNRIC.ItemsSource = db.MonthlySubscriptionMembers.Select(x => x.NRIC).Distinct().OrderBy(x => x).ToList();
        }

        public void Search()
        {
            lstMSMembers = new List<Model.MonthlySubsMember>();
            lstUnPaidMembers = new List<Model.MonthSubsUnPaidMember>();
            dgvMember.ItemsSource = lstMSMembers;
            dgvUnPaidMember.ItemsSource = lstUnPaidMembers;

            try
            {
                var lstPaidMember = db.MonthlySubscriptionMembers.Where(x => x.MonthlySubscriptionBank.MonthlySubscriptionId == MonthlySubscriptionId);
                var dt = MonthlySubsDate.AddMonths(-1);
                var lstUnpaidember = db.MemberMonthEndStatus.Where(x => x.StatusMonth == dt);

                if (!string.IsNullOrWhiteSpace(cbxBank.Text))
                {
                    try
                    {
                        decimal BankCode = (decimal)cbxBank.SelectedValue;
                        if(ckbFromMonthlySubscription.IsChecked==true)
                        {
                            lstPaidMember = lstPaidMember.Where(x => x.MonthlySubscriptionBank.BankCode == BankCode);
                            lstUnpaidember = lstUnpaidember.Where(x => x.BANK_CODE == BankCode);
                        }
                        else
                        {
                            lstPaidMember = lstPaidMember.Where(x => x.MASTERMEMBER.BANK_CODE== BankCode);
                            lstUnpaidember = lstUnpaidember.Where(x => x.MASTERMEMBER.BANK_CODE == BankCode);
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
                        lstPaidMember = lstPaidMember.Where(x => x.MonthlySubcriptionMemberStatusId == Id);
                        lstUnpaidember = lstUnpaidember.Where(x => x.STATUS_CODE == Id);
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
                        lstPaidMember = lstPaidMember.Where(x => x.MonthlySubscriptionMemberMatchingResults.Count(y => y.MonthlySubscriptionMatchingTypeId == Id) > 0);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                if (!string.IsNullOrWhiteSpace(txtMemberName.Text))
                {
                    lstPaidMember = lstPaidMember.Where(x => x.MemberName.ToLower().Contains(txtMemberName.Text.ToLower()));
                    lstUnpaidember = lstUnpaidember.Where(x => x.MASTERMEMBER.MEMBER_NAME.ToLower().Contains(txtMemberName.Text.ToLower()));
                }
                if (!string.IsNullOrWhiteSpace(txtNRIC.Text))
                {
                    lstPaidMember = lstPaidMember.Where(x => x.NRIC.ToLower().Contains(txtNRIC.Text.ToLower()));
                    lstUnpaidember = lstUnpaidember.Where(x => x.MASTERMEMBER.ICNO_NEW.Contains(txtNRIC.Text));
                }
                if (!string.IsNullOrWhiteSpace(txtMinAmount.Text))
                {
                    lstPaidMember = lstPaidMember.Where(x => x.Amount > Convert.ToDecimal(txtMinAmount.Text));
                }

                if (!string.IsNullOrWhiteSpace(txtMaxAmount.Text))
                {
                    lstPaidMember = lstPaidMember.Where(x => x.Amount < Convert.ToDecimal(txtMaxAmount.Text));
                }

                lstPaidMember = lstPaidMember.OrderBy(x => x.MonthlySubcriptionMemberStatusId).OrderBy(x => x.MemberName);
                
                
                lstMSMembers = lstPaidMember.Select(x=> new Model.MonthlySubsMember() {
                    Id=x.Id,
                    MemberName=x.MemberName,
                    NRIC=x.NRIC,
                    Amount=x.Amount,
                    BankUserCode = x.MonthlySubscriptionBank.MASTERBANK.BANK_USERCODE,
                    MemberStatusId = x.MonthlySubcriptionMemberStatusId,
                    MemberStatus = x.MonthlySubscriptionMemberStatu.Status,
                    MemberId = x.MASTERMEMBER==null?null:x.MASTERMEMBER.MEMBER_ID,                    
                    Membercode = x.MASTERMEMBER == null ? null :(decimal?) x.MASTERMEMBER.MEMBER_CODE,
                    IsApproved = x.MonthlySubscriptionMemberMatchingResults.Count(y=> y.UserAccount==null)==0?0:1,
                    DueMonth = x.MASTERMEMBER==null?null:x.MASTERMEMBER.TOTALMONTHSDUE
                }).ToList();
                dgvMember.ItemsSource = lstMSMembers;

                var lstMemberCode = lstMSMembers.Select(x => x.Membercode).ToList();
                var lst = lstUnpaidember.Select(x=> new { x.MASTERMEMBER, x.TOTAL_MONTHS,x.LASTPAYMENTDATE}).ToList();
                lstUnPaidMembers = lst.Where(x=> x.MASTERMEMBER!=null).Select(x => new Model.MonthSubsUnPaidMember() {
                    MemberCode = x.MASTERMEMBER.MEMBER_CODE,
                    MemberId = x.MASTERMEMBER.MEMBER_ID.Value,
                    MemberName = x.MASTERMEMBER.MEMBER_NAME,
                    BankUserCode = x.MASTERMEMBER.MASTERBANK.BANK_USERCODE,
                    NRIC = x.MASTERMEMBER.ICNO_NEW,
                    IsPaid = x.TOTAL_MONTHS != 0 ? true : false,
                    LastPaid = x.LASTPAYMENTDATE.Value,
                    Status = x.MASTERMEMBER.MASTERSTATU.STATUS_NAME,
                    MemberStatusId = x.MASTERMEMBER.STATUS_CODE.Value
                }).ToList();
                lstUnPaidMembers = lstUnPaidMembers.Where(x => !lstMemberCode.Contains(x.MemberCode)).OrderByDescending(x => x.LastPaid).ToList();                
                dgvUnPaidMember.ItemsSource = lstUnPaidMembers;
            }
            catch(Exception ex)
            {

            }

            
        }
       
        void MemberStatusUpdate(MonthlySubscriptionMember msMember)
        {
            try
            {
                var mm = db.MASTERMEMBERs.Where(x=> x.ICNO_OLD == msMember.NRIC || x.ICNO_NEW == msMember.NRIC || x.NRIC_ByBank == msMember.NRIC).OrderByDescending(x=> x.MEMBER_CODE).Select(x => new { x.MEMBER_CODE, x.STATUS_CODE}).FirstOrDefault();
                if (mm == null)
                {
                    msMember.MonthlySubcriptionMemberStatusId = (int)AppLib.MonthlySubscriptionMemberStatus.SundryCreditor;
                    msMember.MemberCode = null;
                }
                else
                {
                    msMember.MonthlySubcriptionMemberStatusId = (int)mm.STATUS_CODE.Value;
                    msMember.MemberCode = mm.MEMBER_CODE;
                                                           
                }                
            }
            catch (Exception ex)
            {

            }
        }

        void ApprovalStatusUpdate(MonthlySubscriptionMember msMember)
        {
            try
            {
                var mm = db.MASTERMEMBERs.Select(x => new { x.MEMBER_CODE, x.STATUS_CODE, x.BANK_CODE, x.MASTERBANK.BANK_NAME, x.MEMBER_NAME, x.ICNO_NEW, x.ICNO_OLD, x.NRIC_ByBank }).OrderByDescending(x => x.MEMBER_CODE).FirstOrDefault(x => x.ICNO_OLD == msMember.NRIC || x.ICNO_NEW == msMember.NRIC || x.NRIC_ByBank == msMember.NRIC);

                if (mm == null)
                {
                    try
                    {
                        var msmmr = getMonthlySubscriptionMemberMatchingResult(msMember,AppLib.MonthlySubscriptionMatchingType.NRICNotMatched);
                        if (msmmr == null)
                        {
                            msmmr = new MonthlySubscriptionMemberMatchingResult()
                            {
                                MonthlySubscriptionMemberId = msMember.Id,
                                MonthlySubscriptionMatchingTypeId = (int)AppLib.MonthlySubscriptionMatchingType.NRICNotMatched,
                                
                            };
                            db.MonthlySubscriptionMemberMatchingResults.Add(msmmr);
                        }
                        msmmr.Description = $"";
                        db.MonthlySubscriptionMemberMatchingResults.RemoveRange(db.MonthlySubscriptionMemberMatchingResults.Where(x => x.MonthlySubscriptionMemberId == msMember.Id && x.MonthlySubscriptionMatchingTypeId != (int)AppLib.MonthlySubscriptionMatchingType.NRICNotMatched));
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
                else
                {
                    try
                    {
                        var msmmr = getMonthlySubscriptionMemberMatchingResult(msMember, AppLib.MonthlySubscriptionMatchingType.NRICNotMatched);
                        if (msmmr != null) db.MonthlySubscriptionMemberMatchingResults.Remove(msmmr);
                        db.SaveChanges();


                        msmmr = getMonthlySubscriptionMemberMatchingResult(msMember, AppLib.MonthlySubscriptionMatchingType.NRICMatched);
                        if (msMember.NRIC == mm.ICNO_NEW)
                        {
                            if (msmmr == null)
                            {
                                msmmr = new MonthlySubscriptionMemberMatchingResult()
                                {
                                    MonthlySubscriptionMemberId = msMember.Id,
                                    MonthlySubscriptionMatchingTypeId = (int)AppLib.MonthlySubscriptionMatchingType.NRICMatched,
                                };
                                db.MonthlySubscriptionMemberMatchingResults.Add(msmmr);
                            }
                            msmmr.Description = $"";
                            msmmr.ApprovedBy = AppLib.iUserCode;
                        }
                        else if (msmmr != null)
                        {
                            db.MonthlySubscriptionMemberMatchingResults.Remove(msmmr);
                        }
                        db.SaveChanges();


                        msmmr = getMonthlySubscriptionMemberMatchingResult(msMember, AppLib.MonthlySubscriptionMatchingType.NRICOldMatched);
                        if (msMember.NRIC != mm.ICNO_NEW && msMember.NRIC == mm.ICNO_OLD)
                        {
                            if (msmmr == null)
                            {
                                msmmr = new MonthlySubscriptionMemberMatchingResult()
                                {
                                    MonthlySubscriptionMemberId = msMember.Id,
                                    MonthlySubscriptionMatchingTypeId = (int)AppLib.MonthlySubscriptionMatchingType.NRICOldMatched,
                                };
                                db.MonthlySubscriptionMemberMatchingResults.Add(msmmr);
                            }
                            msmmr.Description = $"";
                            msmmr.ApprovedBy = AppLib.iUserCode;
                        }
                        else if (msmmr != null)
                        {
                            db.MonthlySubscriptionMemberMatchingResults.Remove(msmmr);
                        }
                        db.SaveChanges();

                        msmmr = getMonthlySubscriptionMemberMatchingResult(msMember, AppLib.MonthlySubscriptionMatchingType.NRICByBankMatched);
                        if (msMember.NRIC != mm.ICNO_NEW && msMember.NRIC != mm.ICNO_OLD && msMember.NRIC == mm.NRIC_ByBank)
                        {
                            if (msmmr == null)
                            {
                                msmmr = new MonthlySubscriptionMemberMatchingResult()
                                {
                                    MonthlySubscriptionMemberId = msMember.Id,
                                    MonthlySubscriptionMatchingTypeId = (int)AppLib.MonthlySubscriptionMatchingType.NRICByBankMatched,
                                };
                                db.MonthlySubscriptionMemberMatchingResults.Add(msmmr);
                            }
                            msmmr.Description = $"";
                            msmmr.ApprovedBy = AppLib.iUserCode;
                        }
                        else if (msmmr != null)
                        {
                            db.MonthlySubscriptionMemberMatchingResults.Remove(msmmr);
                        }
                        db.SaveChanges();


                        msmmr = getMonthlySubscriptionMemberMatchingResult(msMember, AppLib.MonthlySubscriptionMatchingType.StruckOffMembers);
                        if (msMember.MonthlySubcriptionMemberStatusId == (int)AppLib.MonthlySubscriptionMemberStatus.StruckOff)
                        {
                            if (msmmr == null)
                            {
                                msmmr = new MonthlySubscriptionMemberMatchingResult()
                                {
                                    MonthlySubscriptionMemberId = msMember.Id,
                                    MonthlySubscriptionMatchingTypeId = (int)AppLib.MonthlySubscriptionMatchingType.StruckOffMembers,
                                };
                                db.MonthlySubscriptionMemberMatchingResults.Add(msmmr);
                            }

                            msmmr.Description = $"";
                        }
                        else if (msmmr != null)
                        {
                            db.MonthlySubscriptionMemberMatchingResults.Remove(msmmr);
                        }
                        db.SaveChanges();

                        msmmr = getMonthlySubscriptionMemberMatchingResult(msMember, AppLib.MonthlySubscriptionMatchingType.ResignedMembers);
                        if (msMember.MonthlySubcriptionMemberStatusId == (int)AppLib.MonthlySubscriptionMemberStatus.Resigned)
                        {
                            if (msmmr == null)
                            {
                                msmmr = new MonthlySubscriptionMemberMatchingResult()
                                {
                                    MonthlySubscriptionMemberId = msMember.Id,
                                    MonthlySubscriptionMatchingTypeId = (int)AppLib.MonthlySubscriptionMatchingType.ResignedMembers,
                                };
                                db.MonthlySubscriptionMemberMatchingResults.Add(msmmr);
                            }
                            msmmr.Description = $"";
                        }
                        else if (msmmr != null)
                        {
                            db.MonthlySubscriptionMemberMatchingResults.Remove(msmmr);
                        }
                        db.SaveChanges();

                        msmmr = getMonthlySubscriptionMemberMatchingResult(msMember, AppLib.MonthlySubscriptionMatchingType.MismatchedMemberName);
                        if (msMember.MemberName.ToLower() != mm.MEMBER_NAME.ToLower())
                        {
                            if (msmmr == null)
                            {
                                msmmr = new MonthlySubscriptionMemberMatchingResult()
                                {
                                    MonthlySubscriptionMemberId = msMember.Id,
                                    MonthlySubscriptionMatchingTypeId = (int)AppLib.MonthlySubscriptionMatchingType.MismatchedMemberName,
                                };
                                db.MonthlySubscriptionMemberMatchingResults.Add(msmmr);
                            }
                            msmmr.Description = $"From Bank : {msMember.MemberName}\r\nFrom NUBE : {mm.MEMBER_NAME}";
                        }
                        else if (msmmr != null)
                        {
                            db.MonthlySubscriptionMemberMatchingResults.Remove(msmmr);
                        }
                        db.SaveChanges();

                        msmmr = getMonthlySubscriptionMemberMatchingResult(msMember, AppLib.MonthlySubscriptionMatchingType.MismatchedBank);
                        if (msMember.MonthlySubscriptionBank.BankCode != mm.BANK_CODE)
                        {
                            if (msmmr == null)
                            {
                                msmmr = new MonthlySubscriptionMemberMatchingResult()
                                {
                                    MonthlySubscriptionMemberId = msMember.Id,
                                    MonthlySubscriptionMatchingTypeId = (int)AppLib.MonthlySubscriptionMatchingType.MismatchedBank,
                                };
                                db.MonthlySubscriptionMemberMatchingResults.Add(msmmr);
                            }
                            msmmr.Description = $"From Bank : {msMember.MonthlySubscriptionBank.MASTERBANK.BANK_NAME}\r\nFrom NUBE : {mm.BANK_NAME}";
                        }
                        else if (msmmr != null)
                        {
                            db.MonthlySubscriptionMemberMatchingResults.Remove(msmmr);
                        }

                        db.SaveChanges();

                    }
                    catch (Exception ex)
                    {

                    }
                }

            }
            catch (Exception ex)
            {

            }

        }


        MonthlySubscriptionMemberMatchingResult getMonthlySubscriptionMemberMatchingResult(MonthlySubscriptionMember msMember, AppLib.MonthlySubscriptionMatchingType type)
        {
            return db.MonthlySubscriptionMemberMatchingResults.FirstOrDefault(x => x.MonthlySubscriptionMemberId == msMember.Id && x.MonthlySubscriptionMatchingTypeId == (int)type);
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
            txtMemberName.Text = "";
            txtNRIC.Text = "";
            txtMinAmount.Text = "";
            txtMaxAmount.Text = "";
        }

        private void btnMemberStatusScan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pbrStatus.Minimum = 0;
                pbrStatus.Maximum = lstMSMembers.Count();
                pbrStatus.Value = 0;
                foreach (var mm in lstMSMembers)
                {
                    var msMember = db.MonthlySubscriptionMembers.FirstOrDefault(x => x.Id == mm.Id);
                    if(msMember!=null) MemberStatusUpdate(msMember);
                    pbrStatus.Value++;
                    DoEvents();
                }
                db.SaveChanges();
                MessageBox.Show("Scaning Done");
                //Search();
            }
            catch(Exception ex) { }
            
        }
        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        private void btnApprovalStatusScan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pbrStatus.Minimum = 0;
                pbrStatus.Maximum = lstMSMembers.Count();
                pbrStatus.Value = 0;
                foreach (var mm in lstMSMembers)
                {
                    var msMember = db.MonthlySubscriptionMembers.FirstOrDefault(x => x.Id == mm.Id);
                    if (msMember != null) ApprovalStatusUpdate(msMember);
                    pbrStatus.Value++;
                    DoEvents();
                }
                db.SaveChanges();
                MessageBox.Show("Scaning Done");
                Search();
            }
            catch (Exception ex)
            {

            }
        }

        private void DgvMember_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var mm = dgvMember.SelectedItem as Model.MonthlySubsMember;
            if (mm != null && mm?.Membercode != null)
            {
                frmMemberRegistration f = new frmMemberRegistration(mm.Membercode.Value);
                f.ShowDialog();
            }
        }

        private void btnDetail_Click(object sender, RoutedEventArgs e)
        {
            var mm = dgvMember.SelectedItem as Model.MonthlySubsMember;
            if (mm != null && mm?.Membercode != null)
            {
                frmMemberRegistration f = new frmMemberRegistration(mm.Membercode.Value);
                f.ShowDialog();
            }
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            var mm = dgvMember.SelectedItem as Model.MonthlySubsMember;
            if (mm != null && mm?.Membercode != null)
            {
                try
                {
                    frmHistoryAlter frm = new frmHistoryAlter();
                    frm.Search(Convert.ToDecimal(mm.MemberId));
                    frm.ShowDialog();
                }
                catch (Exception ex) { }
            }
            
        }

        private void btnApproval_Click(object sender, RoutedEventArgs e)
        {
            var mm = dgvMember.SelectedItem as Model.MonthlySubsMember;
            if (mm != null && mm?.Membercode != null)
            {
                frmMonthlySubscriptionMemberApproval f = new frmMonthlySubscriptionMemberApproval(mm.Id);
                f.ShowDialog();
                Search();
            }
            
        }

        private void dgvUnPaidMember_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var mm = dgvUnPaidMember.SelectedItem as Model.MonthSubsUnPaidMember;
            if (mm != null)
            {
                frmMemberRegistration f = new frmMemberRegistration(mm.MemberCode);
                f.ShowDialog();
            }
        }

        private void dgvUnPaidMember_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            }
            catch (Exception ex) { }
        }

        private void btnDetailUnPaid_Click(object sender, RoutedEventArgs e)
        {
            var mm = dgvUnPaidMember.SelectedItem as Model.MonthSubsUnPaidMember;
            if (mm != null)
            {
                frmMemberRegistration f = new frmMemberRegistration(mm.MemberCode);
                f.ShowDialog();
            }
        }

        private void btnHistoryUnPaid_Click(object sender, RoutedEventArgs e)
        {
            var mm = dgvUnPaidMember.SelectedItem as Model.MonthSubsUnPaidMember;
            if (mm != null)
            {
                try
                {
                    frmHistoryAlter frm = new frmHistoryAlter();
                    frm.Search(Convert.ToDecimal(mm.MemberId));
                    frm.ShowDialog();
                }
                catch (Exception ex) { }
            }
        }
    }
}
