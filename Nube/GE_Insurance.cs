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
    
    public partial class GE_Insurance
    {
        public int Id { get; set; }
        public string SchemeNo { get; set; }
        public string ContractNo { get; set; }
        public string BranchCode { get; set; }
        public string EmployerCode { get; set; }
        public string NRIC_O { get; set; }
        public string NRIC_N { get; set; }
        public string LifeAssuredName { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string MemberReferenceNo { get; set; }
        public string ICNumber { get; set; }
        public string MembershipType { get; set; }
        public Nullable<decimal> SumAssured { get; set; }
        public string Mode { get; set; }
        public Nullable<decimal> PremiumRM { get; set; }
        public string PolicyStatus { get; set; }
        public Nullable<System.DateTime> TerminationDate { get; set; }
        public Nullable<System.DateTime> NextDueDate { get; set; }
        public Nullable<System.DateTime> RiskCommDate { get; set; }
        public Nullable<System.DateTime> ProposalDate { get; set; }
        public Nullable<System.DateTime> UWDecisionDate { get; set; }
        public Nullable<decimal> SuspenseAmtRM { get; set; }
        public Nullable<decimal> TotalPremPaid { get; set; }
        public Nullable<decimal> Bonus { get; set; }
        public Nullable<decimal> Interest { get; set; }
        public Nullable<decimal> UnitBalance { get; set; }
    }
}
