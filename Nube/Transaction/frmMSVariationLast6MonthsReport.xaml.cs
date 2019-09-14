using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for frmMSVariationLast6MonthsReport.xaml
    /// </summary>
    public partial class frmMSVariationLast6MonthsReport : MetroWindow
    {
        nubebfsEntity db = new nubebfsEntity();
        DateTime dtReport = new DateTime();
        bool? DisplaySubs = false;
        bool? GroupByBank = false;
        bool? GroupByBankBranch = false;
        bool? GroupByNUBEBranch = false;

        public frmMSVariationLast6MonthsReport(DateTime dtReport,bool? DisplaySubs,bool? GroupByBank, bool? GroupByBankBranch, bool? GroupByNUBEBranch)
        {
            InitializeComponent();
            this.dtReport = dtReport;
            this.DisplaySubs = DisplaySubs;
            this.GroupByBank = GroupByBank;
            this.GroupByBankBranch = GroupByBankBranch;
            this.GroupByNUBEBranch = GroupByNUBEBranch;
        }

        void loadReport()
        {
            try
            {
                // DateTime lastSubsDT = db.MonthlySubscriptions.Select(x => x.date).Max();
                DateTime lastSubsDT = this.dtReport;
                DateTime dt = lastSubsDT.AddYears(-1);
                DateTime dtDOJ = lastSubsDT.AddMonths(-4);
                List<Model.VariationReport> lst = new List<Model.VariationReport>();
                Model.VariationReport data = new Model.VariationReport();
                var lstMembers = db.MASTERMEMBERs.Where(x => x.LASTPAYMENT_DATE >= dt && x.DATEOFJOINING<dtDOJ && x.RESIGNED!=1).ToList();

                //DateTime dtMon1 = lastSubsDT.AddMonths(-5);
                //DateTime dtMon2 = lastSubsDT.AddMonths(-4);
                DateTime dtMon3 = lastSubsDT.AddMonths(-3);
                DateTime dtMon4 = lastSubsDT.AddMonths(-2);
                DateTime dtMon5 = lastSubsDT.AddMonths(-1);
                DateTime dtMon6 = lastSubsDT;

                //var lstMon1 = db.MonthlySubscriptionMembers.Where(x => x.MonthlySubscriptionBank.MonthlySubscription.date == dtMon1).Select(x=> new {x.MemberCode,x.Amount }).ToList();
                //var lstMon2 = db.MonthlySubscriptionMembers.Where(x => x.MonthlySubscriptionBank.MonthlySubscription.date == dtMon2).Select(x => new { x.MemberCode, x.Amount }).ToList();
                var lstMon3 = db.MonthlySubscriptionMembers.Where(x => x.MonthlySubscriptionBank.MonthlySubscription.date == dtMon3).Select(x => new { x.MemberCode, x.Amount }).ToList();
                var lstMon4 = db.MonthlySubscriptionMembers.Where(x => x.MonthlySubscriptionBank.MonthlySubscription.date == dtMon4).Select(x => new { x.MemberCode, x.Amount }).ToList();
                var lstMon5 = db.MonthlySubscriptionMembers.Where(x => x.MonthlySubscriptionBank.MonthlySubscription.date == dtMon5).Select(x => new { x.MemberCode, x.Amount }).ToList();
                var lstMon6 = db.MonthlySubscriptionMembers.Where(x => x.MonthlySubscriptionBank.MonthlySubscription.date == dtMon6).Select(x => new { x.MemberCode, x.Amount }).ToList();
                

                foreach (var d in lstMembers)
                {
                    try
                    {
                        var Subs = Math.Round((decimal)((d.Salary ?? 0) / 100), 2);
                        //var Subs1 = lstMon1.FirstOrDefault(x => x.MemberCode == d.MEMBER_CODE)?.Amount ?? 0;
                        //var Subs2 = lstMon2.FirstOrDefault(x => x.MemberCode == d.MEMBER_CODE)?.Amount ?? 0;
                        var Subs3 = lstMon3.FirstOrDefault(x => x.MemberCode == d.MEMBER_CODE)?.Amount ?? 0;
                        var Subs4 = lstMon4.FirstOrDefault(x => x.MemberCode == d.MEMBER_CODE)?.Amount ?? 0;
                        var Subs5 = lstMon5.FirstOrDefault(x => x.MemberCode == d.MEMBER_CODE)?.Amount ?? 0;
                        var Subs6 = lstMon6.FirstOrDefault(x => x.MemberCode == d.MEMBER_CODE)?.Amount ?? 0;

                        if (
                            //Subs != Subs1 || 
                            //Subs != Subs2 || 
                            Subs != Subs3 || Subs != Subs4 || Subs != Subs5 || Subs != Subs6)
                        {
                            data = new Model.VariationReport();
                            data.MemberCode = d.MEMBER_CODE;
                            data.MemberId = d.MEMBER_ID ?? 0;
                            data.MemberName = d.MEMBER_NAME;
                            data.MemberStatus = d.MASTERMEMBERTYPE?.MEMBERTYPE_NAME.Substring(0,1);

                            data.DOJ = d.DATEOFJOINING;
                            data.DOL = d.LASTPAYMENT_DATE;
                            data.DOR = d.RESIGNATIONs?.FirstOrDefault()?.VOUCHER_DATE;
                            data.DueMonth = (int)(d.TOTALMONTHSDUE ?? 0);

                            data.Subs = Subs;
                            //data.Subs1 = Subs1;
                            //data.Subs2 = Subs2;
                            data.Subs3 = Subs3;
                            data.Subs4 = Subs4;
                            data.Subs5 = Subs5;
                            data.Subs6 = Subs6;
                            //data.mm1 = SubsDescription(Subs, Subs1, dtMon1, data.DOJ, data.DOR);
                            //data.mm2 = SubsDescription(Subs, Subs2, dtMon2, data.DOJ, data.DOR);
                            data.mm3 = SubsDescription(Subs, Subs3, dtMon3, data.DOJ, data.DOR);
                            data.mm4 = SubsDescription(Subs, Subs4, dtMon4, data.DOJ, data.DOR);
                            data.mm5 = SubsDescription(Subs, Subs5, dtMon5, data.DOJ, data.DOR);
                            data.mm6 = SubsDescription(Subs, Subs6, dtMon6, data.DOJ, data.DOR);
                            if (GroupByBank == true)
                            {
                                data.GroupName = d.MASTERBANK?.BANK_NAME;
                            }
                            else if (GroupByBankBranch == true)
                            {
                                data.GroupName = $"{d.MASTERBANK?.BANK_NAME } - {d.MASTERBANKBRANCH?.BANKBRANCH_NAME}" ;
                            }
                            else
                            {
                                data.GroupName = $"{d.MASTERBANKBRANCH?.MASTERNUBEBRANCH?.NUBE_BRANCH_NAME}";
                            }
                            
                            if(!( 
                                
                                (data.mm3 == "*" || data.mm3 == "-") &&
                                (data.mm4 == "*" || data.mm4 == "-") &&
                                (data.mm5 == "*" || data.mm5 == "-") &&
                                (data.mm6 == "*" || data.mm6 == "-") 
                                )) lst.Add(data);
                        }
                    }
                    catch(Exception ex)
                    {

                    }
                   
                }


                //rptViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                rptViewer.Reset();
                rptViewer.LocalReport.DataSources.Add(new ReportDataSource("VariationReport", lst));
                rptViewer.LocalReport.ReportEmbeddedResource = "Nube.Reports.rptMSVariationLast6MonthsReport.rdlc";

                List<ReportParameter> reportParameters = new List<ReportParameter>();
                reportParameters.Add(new ReportParameter("Title", $"NUBE Monthly Subscription {lastSubsDT:MMM yyyy} - Variation Report"));
                reportParameters.Add(new ReportParameter("dtMon1", $"{lastSubsDT.AddMonths(-5):MMM yyyy}"));
                reportParameters.Add(new ReportParameter("dtMon2", $"{lastSubsDT.AddMonths(-4):MMM yyyy}"));
                reportParameters.Add(new ReportParameter("dtMon3", $"{lastSubsDT.AddMonths(-3):MMM yyyy}"));
                reportParameters.Add(new ReportParameter("dtMon4", $"{lastSubsDT.AddMonths(-2):MMM yyyy}"));
                reportParameters.Add(new ReportParameter("dtMon5", $"{lastSubsDT.AddMonths(-1):MMM yyyy}"));
                reportParameters.Add(new ReportParameter("dtMon6", $"{lastSubsDT:MMM yyyy}"));

                rptViewer.LocalReport.SetParameters(reportParameters);

                rptViewer.RefreshReport();
            }
            catch (Exception ex)
            {

            }

        }

        string SubsDescription(decimal Subs,decimal Subs1,DateTime SubsDT,DateTime? DOJ,DateTime? DOR )
        {
            if (Subs1 != 0)
            {
                //if (this.DisplaySubs==true)
                //{
                //    return Subs1 - Subs == 0 ? $"{Subs1:N2}" : $"{Subs1:N2}\n({(Subs1 - Subs):+0.00; -#0.00})";
                //}
                //else
                //{
                //    return Subs1 - Subs == 0 ? "-" : $"{(Subs1 - Subs):+0.00; -0.00}";                    
                //}
                var diff = Subs1 - Subs;
                if (diff == 0) {
                    return "-";
                }                 
                else 
                {
                    if (diff > 0 && (SubsDT.Month==1 || SubsDT.Month==7 ))
                    {
                        if(diff ==  (Subs * (5/100)))
                        {
                            return "Inc(5%)";
                        }
                    }  
                    return $"{Math.Abs(diff):0.00}"; 
                }
            }
            //else if (SubsDT.Year == DOJ?.Year && SubsDT.Month == DOJ?.Month)
            //{
            //    return "N";
            //}
            //else if (SubsDT.Year == DOR?.Year && SubsDT.Month == DOR?.Month)
            //{
            //    return "R";
            //}
            //else if(SubsDT<DOJ || SubsDT>DOR)
            //{
            //    return "";
            //}
            else
            {
                return "*";
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(new ThreadStart(loadReport));            
        }
    }
}
