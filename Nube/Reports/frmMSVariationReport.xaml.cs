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

namespace Nube.Reports
{
    /// <summary>
    /// Interaction logic for frmMSVariationReport.xaml
    /// </summary>
    public partial class frmMSVariationReport : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        public frmMSVariationReport(List<Model.VariationByBank> variationByBanks,DateTime dtPrevious,DateTime dtCurrent)
        {
            InitializeComponent();
            try
            {                
                List<Model.VariationByBank> lst = new List<Model.VariationByBank>();
                Model.VariationByBank data = new Model.VariationByBank();
                int i = 1;
                foreach (var d in variationByBanks)
                {
                    if (d.BankName.ToLower() == "total") continue;
                    i = 1;
                    foreach (var nric in d.UnpaidNRIC.Split(','))
                    {
                        data = new Model.VariationByBank();                        
                        data.BankName = d.BankName;
                        data.NoOfMemberCurrent = d.NoOfMemberCurrent;
                        data.NoOfMemberPrevious = d.NoOfMemberPrevious;
                        data.Different = d.Different;
                        data.Unpaid = d.Unpaid;
                        data.NewPaid = d.NewPaid;
                        data.SNo = i++;
                        data.NRIC = nric;
                        data.VarStatus = "Previous Subscription Paid - Current Subscription Unpaid";

                        var dPre = db.MonthlySubscriptionMembers.FirstOrDefault(x => x.MonthlySubscriptionBankId == d.MSBankIdPrevious && x.NRIC == nric);
                        if (dPre != null)
                        {
                            data.Membername = dPre.MemberName;
                            data.Amount = dPre.Amount;
                            data.MSStatus = dPre.MonthlySubscriptionMemberStatu.Status;
                            lst.Add(data);
                        }
                    }

                    i = 1;
                    foreach (var nric in d.NewPaidNRIC.Split(','))
                    {
                        data = new Model.VariationByBank();                        

                        data.BankName = d.BankName;
                        data.NoOfMemberCurrent = d.NoOfMemberCurrent;
                        data.NoOfMemberPrevious = d.NoOfMemberPrevious;
                        data.Different = d.Different;
                        data.Unpaid = d.Unpaid;
                        data.NewPaid = d.NewPaid;
                        data.SNo = i++;
                        data.NRIC = nric;
                        data.VarStatus = "Previous Subscription Unpaid - Current Subscription Paid";
                        var dPre = db.MonthlySubscriptionMembers.FirstOrDefault(x => x.MonthlySubscriptionBankId == d.MSBankIdCureent && x.NRIC == nric);
                        if (dPre != null)
                        {
                            data.Membername = dPre.MemberName;
                            data.Amount = dPre.Amount;
                            data.MSStatus = dPre.MonthlySubscriptionMemberStatu.Status;
                            lst.Add(data);
                        }
                    }
                }


                rptViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                rptViewer.Reset();
                rptViewer.LocalReport.DataSources.Add(new ReportDataSource("VariationByBank", lst));
                rptViewer.LocalReport.ReportEmbeddedResource = "Nube.Reports.rptMSVariationReport.rdlc";

                List<ReportParameter> reportParameters = new List<ReportParameter>();
                reportParameters.Add(new ReportParameter("Title", "NUBE Monthly Subscription - Variation Report"));
                reportParameters.Add(new ReportParameter("dtPrevious", $"{dtPrevious:MMM yyyy}"));
                reportParameters.Add(new ReportParameter("dtCurrent", $"{dtCurrent:MMM yyyy}"));

                rptViewer.LocalReport.SetParameters(reportParameters);
                
                rptViewer.RefreshReport();
            }
            catch (Exception ex)
            {

            }
            

        }
    }
}
