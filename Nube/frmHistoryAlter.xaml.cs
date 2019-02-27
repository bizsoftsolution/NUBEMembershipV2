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
                
            }
        }
    }
}
