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
    
    public partial class MonthlySubscriptionMatchingType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MonthlySubscriptionMatchingType()
        {
            this.MonthlySubscriptionMemberMatchingResults = new HashSet<MonthlySubscriptionMemberMatchingResult>();
            this.MonthlySubscriptionMemberUpdates = new HashSet<MonthlySubscriptionMemberUpdate>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MonthlySubscriptionMemberMatchingResult> MonthlySubscriptionMemberMatchingResults { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MonthlySubscriptionMemberUpdate> MonthlySubscriptionMemberUpdates { get; set; }
    }
}
