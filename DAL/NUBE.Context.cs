﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class nubebfsEntities : DbContext
    {
        public nubebfsEntities()
            : base("name=nubebfsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<MASTERBANKBRANCH> MASTERBANKBRANCHes { get; set; }
        public virtual DbSet<MASTERCITY> MASTERCITies { get; set; }
        public virtual DbSet<MASTERGUARDIAN> MASTERGUARDIANs { get; set; }
        public virtual DbSet<MASTERNAMESETUP> MASTERNAMESETUPs { get; set; }
        public virtual DbSet<MASTERNOMINEE> MASTERNOMINEEs { get; set; }
        public virtual DbSet<MASTERNUBEBRANCH> MASTERNUBEBRANCHes { get; set; }
        public virtual DbSet<MASTERRACE> MASTERRACEs { get; set; }
        public virtual DbSet<MASTERRELATION> MASTERRELATIONs { get; set; }
        public virtual DbSet<MASTERSTATE> MASTERSTATEs { get; set; }
        public virtual DbSet<GuardianInsertBranch> GuardianInsertBranches { get; set; }
        public virtual DbSet<NomineeInsertBranch> NomineeInsertBranches { get; set; }
        public virtual DbSet<CountrySetup> CountrySetups { get; set; }
        public virtual DbSet<UserAccount> UserAccounts { get; set; }
        public virtual DbSet<MASTERMEMBER> MASTERMEMBERs { get; set; }
        public virtual DbSet<MembershipAttachment> MembershipAttachments { get; set; }
        public virtual DbSet<ActiveMemberHistory> ActiveMemberHistories { get; set; }
        public virtual DbSet<ArrearPostDetail> ArrearPostDetails { get; set; }
        public virtual DbSet<ArrearPostMaster> ArrearPostMasters { get; set; }
        public virtual DbSet<ArrearPreDetail> ArrearPreDetails { get; set; }
        public virtual DbSet<ArrearPreMaster> ArrearPreMasters { get; set; }
        public virtual DbSet<DeaciveMemberHistory> DeaciveMemberHistories { get; set; }
        public virtual DbSet<dtproperty> dtproperties { get; set; }
        public virtual DbSet<EventHistory> EventHistories { get; set; }
        public virtual DbSet<EVENTHSTRY> EVENTHSTRies { get; set; }
        public virtual DbSet<LoginHistory> LoginHistories { get; set; }
        public virtual DbSet<MASTERMEMBERNEW> MASTERMEMBERNEWs { get; set; }
        public virtual DbSet<MasterMemberStatu> MasterMemberStatus { get; set; }
        public virtual DbSet<MASTERMEMBERTYPE> MASTERMEMBERTYPEs { get; set; }
        public virtual DbSet<MASTERRESIGNSTATU> MASTERRESIGNSTATUS { get; set; }
        public virtual DbSet<MASTERSTATU> MASTERSTATUS { get; set; }
        public virtual DbSet<MemberStatusLog> MemberStatusLogs { get; set; }
        public virtual DbSet<MemberTransfer> MemberTransfers { get; set; }
        public virtual DbSet<NameTitleSetup> NameTitleSetups { get; set; }
        public virtual DbSet<RESIGNATION> RESIGNATIONs { get; set; }
        public virtual DbSet<SalutationSetup> SalutationSetups { get; set; }
        public virtual DbSet<StatusModify> StatusModifies { get; set; }
        public virtual DbSet<TempActiveMemberHistory> TempActiveMemberHistories { get; set; }
        public virtual DbSet<TempGE> TempGEs { get; set; }
        public virtual DbSet<Tran_Start> Tran_Start { get; set; }
        public virtual DbSet<UserPrevilage> UserPrevilages { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<AI_Insurance> AI_Insurance { get; set; }
        public virtual DbSet<FeesDetail> FeesDetails { get; set; }
        public virtual DbSet<FeesDetailsNotMatch> FeesDetailsNotMatches { get; set; }
        public virtual DbSet<FeesMaster> FeesMasters { get; set; }
        public virtual DbSet<GE_Insurance> GE_Insurance { get; set; }
        public virtual DbSet<TEMPFEE_STATUS> TEMPFEE_STATUS { get; set; }
        public virtual DbSet<TEMPVIEWMASTERMEMBER> TEMPVIEWMASTERMEMBERs { get; set; }
        public virtual DbSet<TVMASTERMEMBER> TVMASTERMEMBERs { get; set; }
        public virtual DbSet<MASTERBANK> MASTERBANKs { get; set; }
        public virtual DbSet<MemberInsertBranch> MemberInsertBranches { get; set; }
        public virtual DbSet<IRCConfirmation> IRCConfirmations { get; set; }
    
        public virtual ObjectResult<SPMEMBERSHIPTOLIST_Result> SPMEMBERSHIPTOLIST(Nullable<int> aPPROVEDSTATE)
        {
            var aPPROVEDSTATEParameter = aPPROVEDSTATE.HasValue ?
                new ObjectParameter("APPROVEDSTATE", aPPROVEDSTATE) :
                new ObjectParameter("APPROVEDSTATE", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SPMEMBERSHIPTOLIST_Result>("SPMEMBERSHIPTOLIST", aPPROVEDSTATEParameter);
        }
    }
}
