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
    
    public partial class EngReportList
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EngReportList()
        {
            this.EngReportApprove = new HashSet<EngReportApprove>();
            this.EngReportEstimatedCost = new HashSet<EngReportEstimatedCost>();
            this.EngReportLocalCommunication = new HashSet<EngReportLocalCommunication>();
            this.EngReportMainJobDescription = new HashSet<EngReportMainJobDescription>();
            this.EngReportOnSiteConsultation = new HashSet<EngReportOnSiteConsultation>();
        }
    
        public int Seq { get; set; }
        public Nullable<short> RptYear { get; set; }
        public string RptName { get; set; }
        public Nullable<int> RptTypeSeq { get; set; }
        public Nullable<short> ExecUnitSeq { get; set; }
        public Nullable<short> ExecSubUnitSeq { get; set; }
        public string OriginAndScope { get; set; }
        public Nullable<int> OriginAndScopeState { get; set; }
        public Nullable<int> OriginAndScopeUserSeq { get; set; }
        public Nullable<System.DateTime> OriginAndScopeUpdateTime { get; set; }
        public Nullable<bool> OriginAndScopeReviewState { get; set; }
        public Nullable<int> OriginAndScopeAssignReviewUserSeq { get; set; }
        public Nullable<int> OriginAndScopeUpdateReviewUserSeq { get; set; }
        public Nullable<System.DateTime> OriginAndScopeReviewTime { get; set; }
        public string RelatedReportResults { get; set; }
        public Nullable<int> RelatedReportResultsState { get; set; }
        public Nullable<int> RelatedReportResultsUserSeq { get; set; }
        public Nullable<System.DateTime> RelatedReportResultsUpdateTime { get; set; }
        public Nullable<bool> RelatedReportResultsReviewState { get; set; }
        public Nullable<int> RelatedReportResultsAssignReviewUserSeq { get; set; }
        public Nullable<int> RelatedReportResultsUpdateReviewUserSeq { get; set; }
        public Nullable<System.DateTime> RelatedReportResultsReviewTime { get; set; }
        public string FacilityManagement { get; set; }
        public Nullable<int> FacilityManagementState { get; set; }
        public Nullable<int> FacilityManagementUserSeq { get; set; }
        public Nullable<System.DateTime> FacilityManagementUpdateTime { get; set; }
        public Nullable<bool> FacilityManagementReviewState { get; set; }
        public Nullable<int> FacilityManagementAssignReviewUserSeq { get; set; }
        public Nullable<int> FacilityManagementUpdateReviewUserSeq { get; set; }
        public Nullable<System.DateTime> FacilityManagementReviewTime { get; set; }
        public string ProposalScopeLand { get; set; }
        public Nullable<int> ProposalScopeLandState { get; set; }
        public Nullable<int> ProposalScopeLandUserSeq { get; set; }
        public Nullable<System.DateTime> ProposalScopeLandUpdateTime { get; set; }
        public Nullable<bool> ProposalScopeLandReviewState { get; set; }
        public Nullable<int> ProposalScopeLandAssignReviewUserSeq { get; set; }
        public Nullable<int> ProposalScopeLandUpdateReviewUserSeq { get; set; }
        public Nullable<System.DateTime> ProposalScopeLandReviewTime { get; set; }
        public string LocationMap { get; set; }
        public string AerialPhotography { get; set; }
        public string ScenePhoto { get; set; }
        public string BaseMap { get; set; }
        public string EngPlaneLayout { get; set; }
        public string LongitudinalSection { get; set; }
        public string StandardSection { get; set; }
        public Nullable<int> NeedAssessmenApproval { get; set; }
        public Nullable<int> EvaluationResult { get; set; }
        public string ER1_1 { get; set; }
        public string ER1_2 { get; set; }
        public string ER2_1 { get; set; }
        public string ER2_2 { get; set; }
        public string ER3 { get; set; }
        public string ER4 { get; set; }
        public string ER6 { get; set; }
        public Nullable<int> ProposalReviewTypeSeq { get; set; }
        public Nullable<int> ProposalReviewAttributesSeq { get; set; }
        public Nullable<int> RiverSeq { get; set; }
        public Nullable<int> DrainSeq { get; set; }
        public string Coastal { get; set; }
        public string LargeSectionChainage { get; set; }
        public Nullable<int> CitySeq { get; set; }
        public Nullable<int> TownSeq { get; set; }
        public Nullable<decimal> CoordX { get; set; }
        public Nullable<decimal> CoordY { get; set; }
        public string EngineeringScale { get; set; }
        public string ProcessReason { get; set; }
        public string EngineeringScaleMemo { get; set; }
        public string RelatedReportContent { get; set; }
        public Nullable<int> HistoricalCatastrophe { get; set; }
        public string HistoricalCatastropheMemo { get; set; }
        public string ProtectionTarget { get; set; }
        public string SetConditions { get; set; }
        public string EcologicalConservationD01 { get; set; }
        public string EcologicalConservationD02 { get; set; }
        public string EcologicalConservationD03 { get; set; }
        public string EcologicalConservationD04 { get; set; }
        public string EcologicalConservationD05 { get; set; }
        public string EcologicalConservationD06 { get; set; }
        public Nullable<decimal> DemandCarbonEmissions { get; set; }
        public string DemandCarbonEmissionsMemo { get; set; }
        public Nullable<int> IsProposalReview { get; set; }
        public Nullable<int> ProposalAuditOpinion { get; set; }
        public Nullable<int> ReviewSort { get; set; }
        public Nullable<int> ApprovedFund { get; set; }
        public Nullable<decimal> ApprovedCarbonEmissions { get; set; }
        public Nullable<int> Expenditure { get; set; }
        public string Resolution { get; set; }
        public Nullable<int> ResolutionAuditOpinion { get; set; }
        public string EngNo { get; set; }
        public Nullable<int> IsTransfer { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public int CreateUserSeq { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
        public Nullable<int> ModifyUserSeq { get; set; }
        public Nullable<int> EstimatedLandAcquisitionCosts { get; set; }
        public string ManagementPlanningLayoutSituation { get; set; }
        public Nullable<int> IsFloodControlRecords { get; set; }
        public Nullable<int> EstimatedExpenditureCurrentYear { get; set; }
        public Nullable<int> ExpensesSubsequentYears { get; set; }
        public string Remark { get; set; }
        public string BookingProcess_SY { get; set; }
        public string BookingProcess_SM { get; set; }
        public string BookingProcess_EY { get; set; }
        public string BookingProcess_EM { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EngReportApprove> EngReportApprove { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EngReportEstimatedCost> EngReportEstimatedCost { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EngReportLocalCommunication> EngReportLocalCommunication { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EngReportMainJobDescription> EngReportMainJobDescription { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EngReportOnSiteConsultation> EngReportOnSiteConsultation { get; set; }
    }
}