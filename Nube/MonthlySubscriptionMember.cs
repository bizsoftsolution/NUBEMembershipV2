//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nube
{
    using System;
    using System.Collections.Generic;
    
    public partial class MonthlySubscriptionMember
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MonthlySubscriptionMember()
        {
            this.MonthlySubscriptionMemberMatchingResults = new HashSet<MonthlySubscriptionMemberMatchingResult>();
        }
    
        public long Id { get; set; }
        public int MonthlySubscriptionBankId { get; set; }
        public Nullable<decimal> MemberCode { get; set; }
        public string NRIC { get; set; }
        public string MemberName { get; set; }
        public decimal Amount { get; set; }
        public int MonthlySubcriptionMemberStatusId { get; set; }
    
        public virtual MASTERMEMBER MASTERMEMBER { get; set; }
        public virtual MonthlySubscriptionBank MonthlySubscriptionBank { get; set; }
        public virtual MonthlySubscriptionMemberStatu MonthlySubscriptionMemberStatu { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MonthlySubscriptionMemberMatchingResult> MonthlySubscriptionMemberMatchingResults { get; set; }
    }
}
