using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;
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

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmIRCConfirmationSearch.xaml
    /// </summary>
    public partial class frmIRCConfirmationSearch : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        public IRCConfirmation selectedIRC = new IRCConfirmation();

        public frmIRCConfirmationSearch()
        {
            InitializeComponent();
            rptViewer.SetDisplayMode(DisplayMode.PrintLayout);
            selectedIRC = new IRCConfirmation();
            Search();
        }

        private void dgvDetails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                selectedIRC = dgvDetails.SelectedItem as IRCConfirmation;
                this.Close();
            }
            catch(Exception ex) { }
            
        }

        void Search()
        {
            if(cbxConfirm.IsChecked!=true && cbxPending.IsChecked != true)
            {
                cbxConfirm.IsChecked = true;
                cbxPending.IsChecked = true;
            }

            var lst = db.IRCConfirmations.ToList();
            if (!string.IsNullOrWhiteSpace(txtSearchText.Text))
            {
                var SearchText = txtSearchText.Text.ToLower();
                lst = lst.Where(x => x.ResignMemberName.ToLower().Contains(SearchText) 
                                  || x.ResignMemberNo.Contains(SearchText)
                                  || x.ResignMemberBankName.ToLower().Contains(SearchText)
                                  || x.ResignMemberICNo.Contains(SearchText)
                                ).ToList();
            }
            
            if(cbxConfirm.IsChecked==true && cbxPending.IsChecked == false)
            {
                lst = lst.Where(x => x.Status == "Confirm").ToList();
            }
            if (cbxConfirm.IsChecked == false && cbxPending.IsChecked == true)
            {
                lst = lst.Where(x => x.Status == "Pending").ToList();
            }
            dgvDetails.ItemsSource = lst;

            rptViewer.Reset();            
            rptViewer.LocalReport.DataSources.Add(new ReportDataSource("IRCConfirmation", lst));
            rptViewer.LocalReport.ReportEmbeddedResource = "Nube.Transaction.rptIRC.rdlc";
            rptViewer.RefreshReport();
        }

        private void txtSearchText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Search();
            }
        }

        private void cbxConfirm_Checked(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void cbxConfirm_Unchecked(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void cbxPending_Checked(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void cbxPending_Unchecked(object sender, RoutedEventArgs e)
        {
            Search();
        }
    }
}
