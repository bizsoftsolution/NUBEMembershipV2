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
using Microsoft.Reporting.WinForms;

namespace Nube.Transaction
{
    /// <summary>
    /// Interaction logic for frmIRCPrint.xaml
    /// </summary>
    public partial class frmIRCPrint : Window
    {
        public frmIRCPrint()
        {
            InitializeComponent();
            rptViewer.SetDisplayMode(DisplayMode.PrintLayout);           
        }

        public void loadData(frmResingation frm)
        {
            try
            {
                rptViewer.Reset();
                
                rptViewer.LocalReport.ReportEmbeddedResource = "Nube.Transaction.rptIRCPrint.rdlc";
                string IRCPosition = "";
                if (frm.rbtChariman.IsChecked == true)
                {
                    IRCPosition = frm.rbtChariman.Content as string;
                }
                else if (frm.rbtSecretary.IsChecked == true)
                {
                    IRCPosition = frm.rbtSecretary.Content as string;
                }
                else if (frm.rbtCommitteMember.IsChecked == true)
                {
                    IRCPosition = frm.rbtCommitteMember.Content as string;
                }

                string IRCMsg1, IRCMsg2, IRCMsg3, IRCMsg4, IRCMsg5, IRCMsg6;
                IRCMsg1 = frm.cbxNameOfPerson.Content as string;
                IRCMsg2 = string.Format("{0} {1} grade w.e.f. {2}", frm.cbxPromotedTo.Content,frm.txtIRCPromotedTo.Text,frm.dtpGrade.Text);
                IRCMsg3 = frm.cbxBeforePromotion.Content as string;
                IRCMsg4 = frm.cbxAttached.Content as string;
                IRCMsg5 = frm.cbxHereByConfirm.Content as string;
                IRCMsg6 = frm.cbxFilledBy.Content as string;

                List<ReportParameter> rp = new List<ReportParameter>();
                rp.Add(new ReportParameter("IRCMemberName", frm.txtIRCName.Text));
                rp.Add(new ReportParameter("IRCPosition", IRCPosition));
                rp.Add(new ReportParameter("IRCMembershipNo", frm.txtIRCMemberNo.Text));
                rp.Add(new ReportParameter("IRCBankName", frm.txtIRCBankName.Text));
                rp.Add(new ReportParameter("IRCBankAddress", frm.txtIRCBankAddress.Text));
                rp.Add(new ReportParameter("IRCTelephoneNo", frm.txtIRCTelephoneNo.Text));
                rp.Add(new ReportParameter("IRCMobileNo", frm.txtIRCMobileNo.Text));
                rp.Add(new ReportParameter("IRCFaxNo", frm.txtIRCFax.Text));

                rp.Add(new ReportParameter("DispMessage1", IRCMsg1));
                rp.Add(new ReportParameter("DispMessage2", IRCMsg2));
                rp.Add(new ReportParameter("DispMessage3", IRCMsg3));
                rp.Add(new ReportParameter("DispMessage4", IRCMsg4));
                rp.Add(new ReportParameter("DispMessage5", IRCMsg5));
                rp.Add(new ReportParameter("DispMessage6", IRCMsg6));

                rp.Add(new ReportParameter("BCName", frm.txtBranchCommitteeName.Text));
                rp.Add(new ReportParameter("BCZone", frm.txtBranchCommitteeZone.Text));
                rp.Add(new ReportParameter("BCDate", frm.dtpBranchCommitteeDate.SelectedDate.Value.ToString()));

                rp.Add(new ReportParameter("IRCCBX1", frm.cbxNameOfPerson.IsChecked==true?"1":"0"));
                rp.Add(new ReportParameter("IRCCBX2", frm.cbxPromotedTo.IsChecked == true ? "1" : "0"));
                rp.Add(new ReportParameter("IRCCBX3", frm.cbxBeforePromotion.IsChecked == true ? "1" : "0"));
                rp.Add(new ReportParameter("IRCCBX4", frm.cbxAttached.IsChecked == true ? "1" : "0"));
                rp.Add(new ReportParameter("IRCCBX5", frm.cbxHereByConfirm.IsChecked == true ? "1" : "0"));
                rp.Add(new ReportParameter("IRCCBX6", frm.cbxFilledBy.IsChecked == true ? "1" : "0"));
                rp.Add(new ReportParameter("BCCBX1", frm.cbxBranchCommitteeVerification1.IsChecked == true ? "1" : "0"));
                rptViewer.LocalReport.SetParameters(rp);
                rptViewer.RefreshReport();
            }
            catch(Exception ex) { }
            
            
        }
    }
}
