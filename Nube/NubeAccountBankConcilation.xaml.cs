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
    /// Interaction logic for NubeAccountBankConcilation.xaml
    /// </summary>
    public partial class NubeAccountBankConcilation : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        string fund = AppLib.sAccFundName.ToUpper();
                
        public NubeAccountBankConcilation()
        {
            InitializeComponent();                                  
            //cmbLedgerName.ItemsSource = db.ViewLedgers.Where(x => x.GroupName == "Bank Accounts").Select(x => x.LedgerNameOP).Distinct().ToList();         
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            DateTime VDate = dtpDateFrom.SelectedDate.Value;
            DateTime VDateTo = dtpDateFrom.SelectedDate.Value;
            string Ledger = cmbLedgerName.Text;

            //dgvDetails.ItemsSource = db.ViewLedgerGroups.Where(x => x.Fund == fund & x.LedgerNameOP==Ledger & x.VoucherDate==VDate & x.VoucherDate==VDateTo).Select(x => new { x.VoucherDate, x.VoucherNo, x.LedgerNameOP, x.Narration, Amount = x.CrAmt + x.DrAmt }).ToList();
               
            //txtOpeningBalance.Text = db.ViewLedgerGroups.Where(x => x.Fund==fund & x.LedgerNameOP == cmbLedgerName.Text.ToString() ).Sum(x => x.DrAmt - x.CrAmt).ToString();
            //txtClearedBalance.Text = string.Format("{0:0.00}", db.ViewLedgerGroups.Where(x => x.Fund == fund & x.LedgerNameOP == Ledger & x.Fund == fund).Sum(x => x.DrAmt - x.CrAmt).ToString());
              
                //findBalance();            
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //Accounts.frmHomeAccReports frm = new Accounts.frmHomeAccReports();
            //frm.Show();
            this.Close();
        }       
    }
}
