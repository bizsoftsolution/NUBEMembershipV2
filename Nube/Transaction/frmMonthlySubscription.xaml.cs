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
using System.Data.OleDb;
using Microsoft.Win32;
using System.IO;
using System.Data;

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmMonthlySubscription.xaml
    /// </summary>
    public partial class frmMonthlySubscription : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        MonthlySubscription dataMS = new MonthlySubscription();

        Model.MonthlySubs data = new Model.MonthlySubs();

        public frmMonthlySubscription()
        {
            InitializeComponent();

            data = new Model.MonthlySubs();
            this.DataContext = data;

            cbxBank.ItemsSource = db.MASTERBANKs.OrderBy(x => x.BANK_NAME).ToList();
            cbxBank.DisplayMemberPath = "BANK_NAME";
            cbxBank.SelectedValuePath = "BANK_CODE";

            cbxFileType.ItemsSource = db.MonthlySubscriptonFileTypes.ToList();
            cbxFileType.SelectedValuePath = "Id";
            cbxFileType.DisplayMemberPath = "FileType";
        }
      
        private void CdrMonthlySubscription_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            if (cdrMonthlySubscription.DisplayMode == CalendarMode.Month)
            {

                data = new Model.MonthlySubs(cdrMonthlySubscription.DisplayDate);
                this.DataContext = data;
                cdrMonthlySubscription.SelectedDate = data.SelecctedDate;
                cdrMonthlySubscription.DisplayMode = CalendarMode.Year;
                LoadMonthlySubsciption(data.SelecctedDate);                
            }
        }
        void LoadMonthlySubsciption(DateTime dt)
        {
            this.Title = $"Monthly Subscription [ {dt.ToString("MMMM - yyyy")} ]";
            dataMS = db.MonthlySubscriptions.FirstOrDefault(x => x.date == dt);
            if (dataMS?.MonthEndClosed == true)
            {
                stpFileSelect.Visibility = Visibility.Collapsed;
            }
            else
            {
                stpFileSelect.Visibility = Visibility.Visible;
            }
            if (dataMS != null)
            {
                try
                {
                    var lstBank = dataMS.MonthlySubscriptionBanks.Select(x => new
                    Model.MonthlySubsBank{
                        Id = x.Id,
                        BankName = x.MASTERBANK.BANK_NAME,
                        NoOfMember = x.MonthlySubscriptionMembers.Count(),
                        TotalAmount = x.MonthlySubscriptionMembers.Sum(y => y.Amount),
                        ActiveAmount = x.MonthlySubscriptionMembers.Where(y => y.MonthlySubscriptionMemberStatu?.Id == (int)AppLib.MonthlySubscriptionMemberStatus.Active).Sum(z => z.Amount),
                        DefaulterAmount = x.MonthlySubscriptionMembers.Where(y => y.MonthlySubscriptionMemberStatu?.Id == (int)AppLib.MonthlySubscriptionMemberStatus.Defaulter).Sum(z => z.Amount),
                        StruckOffAmount = x.MonthlySubscriptionMembers.Where(y => y.MonthlySubscriptionMemberStatu?.Id == (int)AppLib.MonthlySubscriptionMemberStatus.StruckOff).Sum(z => z.Amount),
                        ResignedAmount = x.MonthlySubscriptionMembers.Where(y => y.MonthlySubscriptionMemberStatu?.Id == (int)AppLib.MonthlySubscriptionMemberStatus.Resigned).Sum(z => z.Amount),
                        SundryCreditorAmount = x.MonthlySubscriptionMembers.Where(y => y.MonthlySubscriptionMemberStatu?.Id == (int)AppLib.MonthlySubscriptionMemberStatus.SundryCreditor).Sum(z => z.Amount)
                    }).ToList();

                    var b = new Model.MonthlySubsBank
                            {
                                BankName = "Total",
                                NoOfMember = lstBank.Sum(x => x.NoOfMember),
                                TotalAmount = lstBank.Sum(x => x.TotalAmount),
                                ActiveAmount = lstBank.Sum(x => x.ActiveAmount),
                                DefaulterAmount = lstBank.Sum(x => x.DefaulterAmount),
                                StruckOffAmount = lstBank.Sum(x => x.StruckOffAmount),
                                ResignedAmount = lstBank.Sum(x => x.ResignedAmount),
                                SundryCreditorAmount = lstBank.Sum(x => x.SundryCreditorAmount)
                            };
                    lstBank.Add(b);

                    dgvBank.ItemsSource = lstBank;
                    lblPaidMembers.Text = dataMS.MonthlySubscriptionBanks.Sum(x => x.MonthlySubscriptionMembers.Count()).ToString();
                    lblPaidAmount.Text = dataMS.MonthlySubscriptionBanks.Sum(x => x.MonthlySubscriptionMembers.Sum(y => y.Amount)).ToString("N2");

                }
                catch (Exception ex)
                {

                }
                try
                {
                    
                    var lstMemberStatus = db.MonthlySubscriptionMemberStatus.ToList().Select(x => new Model.MonthlySubsMemberStatus()
                    {
                        Description= x.Status,
                        NoOfMember = x.MonthlySubscriptionMembers.Count(y => y.MonthlySubscriptionBank.MonthlySubscriptionId == dataMS.Id),
                        Amount = x.MonthlySubscriptionMembers.Where(y => y.MonthlySubscriptionBank.MonthlySubscriptionId == dataMS.Id).Sum(z=> z.Amount),
                    }).ToList();

                    var mStatus = new Model.MonthlySubsMemberStatus()
                    {
                        Description = "Total",
                        NoOfMember = lstMemberStatus.Sum(x => x.NoOfMember),
                        Amount = lstMemberStatus.Sum(x => x.Amount),
                    };
                    lstMemberStatus.Add(mStatus);
                    dgvMemberStatus.ItemsSource = lstMemberStatus;

                }
                catch (Exception ex)
                {

                }
                try
                {

                    var lstMemberMatching = db.MonthlySubscriptionMatchingTypes.ToList().Select(x => new Model.MonthlySubsApprovalStatus()
                    {
                        Description=x.Name,
                        NoOfMember = x.MonthlySubscriptionMemberMatchingResults.Count(y => y.MonthlySubscriptionMember?.MonthlySubscriptionBank?.MonthlySubscriptionId == dataMS.Id),
                        Approved = x.MonthlySubscriptionMemberMatchingResults.Count(y => y.MonthlySubscriptionMember?.MonthlySubscriptionBank?.MonthlySubscriptionId == dataMS.Id && y.ApprovedBy!=null),
                        Pending = x.MonthlySubscriptionMemberMatchingResults.Count(y => y.MonthlySubscriptionMember?.MonthlySubscriptionBank?.MonthlySubscriptionId == dataMS.Id && y.ApprovedBy == null),                        
                    }).ToList();

                    var aStatus = new Model.MonthlySubsApprovalStatus()
                    {
                        Description = "Total",
                        NoOfMember = lstMemberMatching.Sum(x=> x.NoOfMember),
                        Approved = lstMemberMatching.Sum(x => x.Approved),
                        Pending = lstMemberMatching.Sum(x => x.Pending),
                    };
                    lstMemberMatching.Add(aStatus);
                    dgvMemberMatching.ItemsSource = lstMemberMatching;
                }
                catch (Exception ex)
                {

                }                               
            }
            else
            {
                dgvBank.ItemsSource = null;
                dgvMemberStatus.ItemsSource = null;
                dgvMemberMatching.ItemsSource= null;
                lblPaidAmount.Text = "";
                lblPaidMembers.Text = "";
            }
            
        }
        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var bank = cbxBank.SelectedItem as MASTERBANK;
                var fileType = cbxFileType.SelectedItem as MonthlySubscriptonFileType;
                

                if(bank==null)
                {
                    MessageBox.Show("Select Bank");
                    cbxBank.Focus();
                }
                else if (fileType == null)
                {
                    MessageBox.Show("Select file type");
                    cbxFileType.Focus();
                }
                else if (cdrMonthlySubscription.SelectedDate == null)
                {
                    MessageBox.Show("Select Month");
                    cdrMonthlySubscription.Focus();
                }
                else
                {
                    OpenFileDialog ofd = new OpenFileDialog();

                    if (ofd.ShowDialog() == true)
                    {
                        var dt = cdrMonthlySubscription.SelectedDate.Value;
                        var ms = db.MonthlySubscriptions.FirstOrDefault(x => x.date == dt);
                        if(ms == null)
                        {
                            ms = new MonthlySubscription() {date=dt,MonthEndClosed=false };
                            db.MonthlySubscriptions.Add(ms);
                        }

                        var msBank = ms.MonthlySubscriptionBanks.FirstOrDefault(x => x.BankCode == bank.BANK_CODE);
                        if (msBank == null)
                        {
                            msBank = new MonthlySubscriptionBank() {BankCode=bank.BANK_CODE };
                            ms.MonthlySubscriptionBanks.Add(msBank);
                        }
                        else
                        {
                            MessageBox.Show($"{cbxBank.Text} is already done.\r\n delete old entry and retry If you need.");
                            return;
                        }

                        var msAttachment = msBank.MonthlySubscriptionBankAttachments.FirstOrDefault(x => x.MonthlySubscriptionFileTypeId == fileType.Id);
                        if (msAttachment == null)
                        {
                            msAttachment = new MonthlySubscriptionBankAttachment() { MonthlySubscriptionFileTypeId = fileType.Id };
                            msBank.MonthlySubscriptionBankAttachments.Add(msAttachment);
                        }


                        msAttachment.FilePath = ofd.FileName;
                        msAttachment.FileContent = File.ReadAllBytes(ofd.FileName); 


                        if (fileType.Id ==(int) AppLib.MonthlySubscriptionFileType.FromNUBE)
                        {
                            DataTable dataTable = new DataTable();
                            using (var con = new OleDbConnection(""))
                            {
                                string Import_FileName = ofd.FileName;
                                string sheetName = "Sheet1";
                                string fileExtension = System.IO.Path.GetExtension(Import_FileName);
                                if (fileExtension == ".xls")
                                    con.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Import_FileName + ";" + "Extended Properties='Excel 8.0;HDR=YES;'";
                                if (fileExtension == ".xlsx")
                                    con.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Import_FileName + ";" + "Extended Properties='Excel 12.0 Xml;HDR=YES;'";
                                using (OleDbCommand comm = new OleDbCommand())
                                {
                                    comm.CommandText = "Select * from [" + sheetName + "$]";

                                    comm.Connection = con;

                                    using (OleDbDataAdapter da = new OleDbDataAdapter())
                                    {
                                        da.SelectCommand = comm;
                                        da.Fill(dataTable);
                                        for(var i = 0; i < dataTable.Rows.Count; i++)
                                        {
                                            var NRIC = dataTable.Rows[i]["NRIC"].ToString();
                                            var MemberName = dataTable.Rows[i]["MemberName"].ToString();
                                            var Amount = dataTable.Rows[i]["Amount"].ToString();

                                            var msMember = msBank.MonthlySubscriptionMembers.FirstOrDefault(x => x.NRIC == NRIC);
                                            if (msMember == null)
                                            {
                                                msMember = new MonthlySubscriptionMember() { NRIC = NRIC,MonthlySubcriptionMemberStatusId=(int)AppLib.MonthlySubscriptionMemberStatus.SundryCreditor};
                                                msBank.MonthlySubscriptionMembers.Add(msMember);
                                            }

                                            msMember.MemberName = MemberName;
                                            msMember.Amount = Convert.ToDecimal(Amount);                                            
                                        }
                                    }
                                }
                            }
                        }
                        db.SaveChanges();
                        MessageBox.Show("Saved");
                        LoadMonthlySubsciption(cdrMonthlySubscription.SelectedDate.Value);
                    }
                }
                
            }catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void CdrMonthlySubscription_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DgvBank_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Model.MonthlySubsBank d = dgvBank.SelectedItem as Model.MonthlySubsBank;
                if (dataMS != null)
                {
                    if (dataMS.Id != 0)
                    {
                        frmMonthlySubscriptionMembers f = new frmMonthlySubscriptionMembers(dataMS.Id);
                        f.cbxBank.Text = d.BankName;                        
                        f.Search();
                        f.ShowDialog();
                    }
                }
            }
            catch(Exception ex) { }
            
        }

        private void DgvBank_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            }
            catch(Exception ex) { }
            
        }

        private void dgvMemberStatus_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            }
            catch (Exception ex) { }
        }

        private void dgvMemberStatus_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Model.MonthlySubsMemberStatus d = dgvMemberStatus.SelectedItem as Model.MonthlySubsMemberStatus;
                if (d != null)
                {
                    frmMonthlySubscriptionMembers f = new frmMonthlySubscriptionMembers(dataMS.Id);
                    f.cbxMemberStatus.Text = d.Description != "Total" ?d.Description : "";
                    f.Search();
                    f.ShowDialog();                    
                }
            }
            catch (Exception ex) { }
        }

        private void dgvMemberMatching_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                e.Row.Header = (e.Row.GetIndex() + 1).ToString();
            }
            catch (Exception ex) { }
        }

        private void dgvMemberMatching_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Model.MonthlySubsApprovalStatus d = dgvMemberMatching.SelectedItem as Model.MonthlySubsApprovalStatus;
                if (d != null)
                {
                    frmMonthlySubscriptionMembers f = new frmMonthlySubscriptionMembers(dataMS.Id);
                    f.cbxApprovalStatus.Text = d.Description != "Total" ? d.Description : "";
                    f.Search();
                    f.ShowDialog();
                }
            }
            catch (Exception ex) { }
        }
    }
}
