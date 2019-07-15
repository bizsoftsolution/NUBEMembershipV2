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
                
                foreach (var d in variationByBanks)
                {
                    if (d.BankName.ToLower() == "total") continue;                    
                    foreach (var nric in d.UnpaidNRIC.Split(','))
                    {
                        data = new Model.VariationByBank();                        
                        data.BankName = d.BankName;
                        data.NoOfMemberCurrent = d.NoOfMemberCurrent;
                        data.NoOfMemberPrevious = d.NoOfMemberPrevious;
                        data.TotalAmountPrevious = d.TotalAmountPrevious;
                        data.TotalAmountCurrent = d.TotalAmountCurrent;
                        data.Different = d.Different;
                        data.Unpaid = d.Unpaid;
                        data.NewPaid = d.NewPaid;                        
                        data.NRIC = nric;
                        

                        var dPre = db.MonthlySubscriptionMembers.FirstOrDefault(x => x.MonthlySubscriptionBankId == d.MSBankIdPrevious && x.NRIC == nric);
                        if (dPre != null)
                        {
                            data.VarStatus = dPre.MASTERMEMBER?.RESIGNED==1?"Resigned Members": "Previous Subscription Paid - Current Subscription Unpaid";
                            data.Membername = dPre.MemberName;
                            data.Amount = 0;
                            data.AmountPrevious = dPre.Amount;
                            data.AmountDifferent = data.Amount - data.AmountPrevious;
                            data.MSStatus = dPre.MonthlySubscriptionMemberStatu.Status;
                            lst.Add(data);
                        }
                    }
                    
                    foreach (var nric in d.NewPaidNRIC.Split(','))
                    {
                        data = new Model.VariationByBank();                        

                        data.BankName = d.BankName;
                        data.NoOfMemberCurrent = d.NoOfMemberCurrent;
                        data.NoOfMemberPrevious = d.NoOfMemberPrevious;
                        data.TotalAmountPrevious = d.TotalAmountPrevious;
                        data.TotalAmountCurrent = d.TotalAmountCurrent;
                        data.Different = d.Different;
                        data.Unpaid = d.Unpaid;
                        data.NewPaid = d.NewPaid;                        
                        data.NRIC = nric;
                        
                        var dPre = db.MonthlySubscriptionMembers.FirstOrDefault(x => x.MonthlySubscriptionBankId == d.MSBankIdCureent && x.NRIC == nric);
                        
                        if (dPre != null)
                        {
                            var doj = dPre.MASTERMEMBER?.DATEOFJOINING??DateTime.Now;

                            data.VarStatus = doj.Year==dtPrevious.Year && doj.Month==dtPrevious.Month?$"New Join on {dtPrevious:MMM yyyy}" : "Previous Subscription Unpaid - Current Subscription Paid";
                            data.Membername = dPre.MemberName;
                            data.Amount = dPre.Amount;
                            data.AmountPrevious = 0;
                            data.AmountDifferent = data.Amount - data.AmountPrevious;
                            data.MSStatus = dPre.MonthlySubscriptionMemberStatu.Status;
                            lst.Add(data);
                        }
                    }

                    try
                    {
                        var lstPrevious = db.MonthlySubscriptionMembers.Where(x => x.MonthlySubscriptionBankId == d.MSBankIdPrevious);
                        var lstCurrent = db.MonthlySubscriptionMembers.Where(x => x.MonthlySubscriptionBankId == d.MSBankIdCureent);

                        var lstMismatch = lstPrevious.Join(lstCurrent, x => x.NRIC, x => x.NRIC, (x, y) => new { NRIC = x.NRIC, MemberName = x.MemberName, AmountPrevious = x.Amount, AmountCurrent = y.Amount }).Where(x => x.AmountPrevious != x.AmountCurrent).ToList();
                        foreach (var mm in lstMismatch)
                        {
                            data = new Model.VariationByBank();

                            data.BankName = d.BankName;
                            data.NoOfMemberCurrent = d.NoOfMemberCurrent;
                            data.NoOfMemberPrevious = d.NoOfMemberPrevious;
                            data.TotalAmountPrevious = d.TotalAmountPrevious;
                            data.TotalAmountCurrent = d.TotalAmountCurrent;
                            data.Different = d.Different;
                            data.Unpaid = d.Unpaid;
                            data.NewPaid = d.NewPaid;
                            data.NRIC = mm.NRIC;
                            data.VarStatus = mm.AmountCurrent<mm.AmountPrevious ? $"Subscription Decrement" : "Subscription Increment";
                            data.Membername = mm.MemberName;
                            data.Amount = mm.AmountCurrent;
                            data.AmountPrevious = mm.AmountPrevious;
                            data.AmountDifferent = data.Amount - data.AmountPrevious;                            
                            lst.Add(data);
                        }



                    }
                    catch (Exception ex) { }
                    

                }


                rptViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                rptViewer.Reset();
                rptViewer.LocalReport.DataSources.Add(new ReportDataSource("VariationByBank", lst));
                rptViewer.LocalReport.ReportEmbeddedResource = "Nube.Reports.rptMSVariationReport.rdlc";

                List<ReportParameter> reportParameters = new List<ReportParameter>();
                reportParameters.Add(new ReportParameter("Title", $"NUBE Monthly Subscription {dtCurrent:MMM yyyy} - Variation Report"));
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
