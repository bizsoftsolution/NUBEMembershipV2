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
    
    public partial class IRCConfirmation
    {
        public string MemberCode { get; set; }
        public string ResignMemberNo { get; set; }
        public string ResignMemberName { get; set; }
        public string ResignMemberICNo { get; set; }
        public string ResignMemberBankName { get; set; }
        public string ResignMemberBranchName { get; set; }
        public string IRCPosition { get; set; }
        public string MembershipNo { get; set; }
        public string PromotedTo { get; set; }
        public Nullable<System.DateTime> GradeWEF { get; set; }
        public Nullable<bool> NameOfPerson { get; set; }
        public Nullable<bool> WasPromoted { get; set; }
        public Nullable<bool> BeforePromotion { get; set; }
        public Nullable<bool> Attached { get; set; }
        public Nullable<bool> HereByConfirm { get; set; }
        public Nullable<bool> FilledBy { get; set; }
        public Nullable<bool> BranchCommitteeVerification1 { get; set; }
        public Nullable<bool> BranchCommitteeVerification2 { get; set; }
        public string BranchCommitteeName { get; set; }
        public string BranchCommitteeZone { get; set; }
        public Nullable<System.DateTime> BranchCommitteeDate { get; set; }
        public string Remarks { get; set; }
    }
}