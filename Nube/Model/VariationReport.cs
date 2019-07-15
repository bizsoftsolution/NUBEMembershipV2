using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nube.Model
{
    public class VariationReport
    {
        public decimal MemberCode { get; set; }
        public decimal MemberId { get; set; }
        public String MemberName { get; set; }
        public DateTime? DOJ { get; set; }
        public DateTime? DOR { get; set; }
        public DateTime? DOL { get; set; }
        public int DueMonth { get; set; }
        public string MemberStatus { get; set; }
        public decimal Subs { get; set; }
        public decimal Subs1 { get; set; }
        public decimal Subs2 { get; set; }
        public decimal Subs3 { get; set; }
        public decimal Subs4 { get; set; }
        public decimal Subs5 { get; set; }
        public decimal Subs6 { get; set; }

        public string mm1 { get; set; }
        public string mm2 { get; set; }
        public string mm3 { get; set; }
        public string mm4 { get; set; }
        public string mm5 { get; set; }
        public string mm6 { get; set; }

        public string GroupName { get; set; }
    }
}
