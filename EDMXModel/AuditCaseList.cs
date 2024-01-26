//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EQC.EDMXModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class AuditCaseList
    {
        public int Seq { get; set; }
        public string EngName { get; set; }
        public string ContractNo { get; set; }
        public string OrganizerUnit { get; set; }
        public string FundingSourceName { get; set; }
        public string ActualBidAwardDate { get; set; }
        public Nullable<decimal> ContractAmount { get; set; }
        public string AuditDate { get; set; }
        public string AuditUnit { get; set; }
        public string InformMethod { get; set; }
        public string InsideCommittee { get; set; }
        public string OutsideCommittee { get; set; }
        public string JoinPerson { get; set; }
        public Nullable<int> Score { get; set; }
        public Nullable<decimal> VendorDeductPoint { get; set; }
        public Nullable<decimal> SupervisionDeductPoint { get; set; }
        public Nullable<decimal> PCMDeductionPoint { get; set; }
        public string ImproveResult { get; set; }
        public string PSIssueNoDate { get; set; }
        public string ApprovalNoDate { get; set; }
        public string AuditUnitSugesstion { get; set; }
        public string ECUnitRealResponse { get; set; }
        public string Momo { get; set; }
        public string EngType { get; set; }
        public string BelongPrj { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> CreateUserSeq { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
        public Nullable<int> ModifyUserSeq { get; set; }
    }
}