//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class MASTERBANK
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MASTERBANK()
        {
            this.MASTERMEMBERs = new HashSet<MASTERMEMBER>();
        }
    
        public decimal BANK_CODE { get; set; }
        public string BANK_NAME { get; set; }
        public string BANK_USERCODE { get; set; }
        public Nullable<decimal> DELETED { get; set; }
        public Nullable<decimal> MERGED { get; set; }
        public decimal HEADER_BANK_CODE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MASTERMEMBER> MASTERMEMBERs { get; set; }
    }
}
