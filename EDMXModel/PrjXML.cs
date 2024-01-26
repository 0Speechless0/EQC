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
    
    public partial class PrjXML
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PrjXML()
        {
            this.EngMain = new HashSet<EngMain>();
            this.ProgressData = new HashSet<ProgressData>();
        }
    
        public int Seq { get; set; }
        public string TenderNo { get; set; }
        public string ExecUnitCd { get; set; }
        public string ExecUnitName { get; set; }
        public string TenderName { get; set; }
        public string PrjName { get; set; }
        public string EngType { get; set; }
        public string TownName { get; set; }
        public Nullable<decimal> CoordX { get; set; }
        public Nullable<decimal> CoordY { get; set; }
        public string Location { get; set; }
        public string PlanOrganizerName { get; set; }
        public string PlanNo { get; set; }
        public string CompetentAuthority { get; set; }
        public string OrganizerName { get; set; }
        public string FundingSourceName { get; set; }
        public string TenderNoticeUnit { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string Weights { get; set; }
        public string EngOverview { get; set; }
        public Nullable<int> SteelDemand { get; set; }
        public Nullable<int> ConcreteDemand { get; set; }
        public Nullable<int> EarchworkDemand { get; set; }
        public string DurationCategory { get; set; }
        public Nullable<int> TotalDays { get; set; }
        public string DurationDesc { get; set; }
        public string BudgetAccount { get; set; }
        public Nullable<decimal> TotalEngBudget { get; set; }
        public Nullable<decimal> OutsourcingBudget { get; set; }
        public Nullable<int> SupplyMaterialCost { get; set; }
        public Nullable<int> LandPurCompen { get; set; }
        public Nullable<decimal> EngManageFee { get; set; }
        public Nullable<decimal> AirPollutionFee { get; set; }
        public Nullable<decimal> OtherFee { get; set; }
        public Nullable<decimal> PlanningFee { get; set; }
        public string PlanningUnitName { get; set; }
        public string DesignUnitName { get; set; }
        public Nullable<decimal> DesignFee { get; set; }
        public string DesignMemo { get; set; }
        public string SupervisionUnitName { get; set; }
        public Nullable<decimal> SupervisionFee { get; set; }
        public string SupervisionMemo { get; set; }
        public string ContractorName1 { get; set; }
        public string ContractorName2 { get; set; }
        public string InsuranceDate { get; set; }
        public Nullable<decimal> InsuranceAmount { get; set; }
        public string InsuranceNo { get; set; }
        public string ActualAnnoDate { get; set; }
        public string ScheBidReviewDate { get; set; }
        public string ActualBidReviewDate { get; set; }
        public string ScheBidOpeningDate { get; set; }
        public string ActualBidOpeningDate { get; set; }
        public string ScheBidAwardDate { get; set; }
        public string ActualBidAwardDate { get; set; }
        public string ScheBiddingMethod { get; set; }
        public string ActualBiddingMethod { get; set; }
        public string BidAwardMethod { get; set; }
        public string ContractFeePayMethod { get; set; }
        public Nullable<decimal> EstimateBasePrice { get; set; }
        public Nullable<decimal> RendBasePrice { get; set; }
        public Nullable<decimal> BidAmount { get; set; }
        public string ContractNo { get; set; }
        public Nullable<int> Prepayment { get; set; }
        public string ScheStartDate { get; set; }
        public string ActualStartDate { get; set; }
        public string ScheCompletionDate { get; set; }
        public string ScheCompletCloseDate { get; set; }
        public Nullable<decimal> QualityControlFee { get; set; }
        public string QualityPlanApproveUnit { get; set; }
        public string QualityPlanApproveDate { get; set; }
        public string QualityPlanApproveNo { get; set; }
        public string SupervisionPlanApproveUnit { get; set; }
        public string SupervisionPlanApproveDate { get; set; }
        public string SupervisionPlanApproveNo { get; set; }
        public string SiteContactMemo { get; set; }
        public Nullable<bool> IsChangeContract { get; set; }
        public Nullable<bool> IsAppendAgreement { get; set; }
        public Nullable<bool> IsForYear97 { get; set; }
        public string ForMade97Month { get; set; }
        public Nullable<bool> IsAppleBeforeClose { get; set; }
        public Nullable<int> PriceAdjust { get; set; }
        public Nullable<int> BudgetAvail { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> CreateUserSeq { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
        public Nullable<int> ModifyUserSeq { get; set; }
        public short TenderYear { get; set; }
        public string NumOfAnnounce { get; set; }
        public string Chief { get; set; }
        public string ManualBelongPrj { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EngMain> EngMain { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProgressData> ProgressData { get; set; }
        public virtual PrjXMLTag PrjXMLTag { get; set; }
    }
}
