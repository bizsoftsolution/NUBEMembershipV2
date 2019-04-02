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
        
        public frmMonthlySubscription()
        {
            InitializeComponent();

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
                var dt = cdrMonthlySubscription.DisplayDate;
                cdrMonthlySubscription.SelectedDate = dt;                
                cdrMonthlySubscription.DisplayMode = CalendarMode.Year;
                LoadMonthlySubsciption(dt);                
            }
        }
        void LoadMonthlySubsciption(DateTime dt)
        {
            this.Title = $"Monthly Subscription [ {dt.ToString("MMMM - yyyy")} ]";
            var ms = db.MonthlySubscriptions.FirstOrDefault(x => x.date == dt);
            if (ms?.MonthEndClosed == true)
            {
                stpFileSelect.Visibility = Visibility.Collapsed;
            }
            else
            {
                stpFileSelect.Visibility = Visibility.Visible;
            }
            if (ms != null)
            {
                dgvBank.ItemsSource = ms.MonthlySubscriptionBanks.Select(x => new
                {
                    x.MASTERBANK.BANK_NAME,
                    NoOfMember = x.MonthlySubscriptionMembers.Count(),
                    TotalAmount =x.MonthlySubscriptionMembers.Sum(y=> y.Amount),
                    ActiveAmount = x.MonthlySubscriptionMembers.Where(y=> y.MASTERMEMBER?.STATUS_CODE==(int)AppLib.MonthlySubscriptionMemberStatus.Active ).Sum(z=> z.Amount)                    
                }).ToList();
            }
            else
            {
                dgvBank.ItemsSource = null;
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
                                                msMember = new MonthlySubscriptionMember() { NRIC = NRIC,MonthlySubcriptionMemberStatusId=(int)AppLib.MonthlySubscriptionMemberStatus.SundryCreditor };
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
                    }
                }
                
            }catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void CdrMonthlySubscription_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        
    }
}
