using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class EngReportModel
    {
        public int Seq { get; set; }
        public Int16? RptYear { get; set; }
        public string RptName { get; set; }
        public Int32? RptTypeSeq { get; set; }
        public Int16? ExecUnitSeq { get; set; }
        public Int16? ExecSubUnitSeq { get; set; }

        public string OriginAndScope { get; set; }
        public Int32? OriginAndScopeState { get; set; }
        public Int32? OriginAndScopeUserSeq { get; set; }
        public DateTime? OriginAndScopeUpdateTime { get; set; }
        public bool OriginAndScopeReviewState { get; set; }
        public Int32? OriginAndScopeAssignReviewUserSeq { get; set; }
        public Int32? OriginAndScopeUpdateReviewUserSeq { get; set; }
        public DateTime? OriginAndScopeReviewTime { get; set; }

        public string RelatedReportResults { get; set; }
        public Int32? RelatedReportResultsState { get; set; }
        public Int32? RelatedReportResultsUserSeq { get; set; }
        public DateTime? RelatedReportResultsUpdateTime { get; set; }
        public bool RelatedReportResultsReviewState { get; set; }
        public Int32? RelatedReportResultsAssignReviewUserSeq { get; set; }
        public Int32? RelatedReportResultsUpdateReviewUserSeq { get; set; }
        public DateTime? RelatedReportResultsReviewTime { get; set; }

        public string FacilityManagement { get; set; }
        public Int32? FacilityManagementState { get; set; }
        public Int32? FacilityManagementUserSeq { get; set; }
        public DateTime? FacilityManagementUpdateTime { get; set; }
        public bool FacilityManagementReviewState { get; set; }
        public Int32? FacilityManagementAssignReviewUserSeq { get; set; }
        public Int32? FacilityManagementUpdateReviewUserSeq { get; set; }
        public DateTime? FacilityManagementReviewTime { get; set; }

        public string ProposalScopeLand { get; set; }
        public Int32? ProposalScopeLandState { get; set; }
        public Int32? ProposalScopeLandUserSeq { get; set; }
        public DateTime? ProposalScopeLandUpdateTime { get; set; }
        public bool ProposalScopeLandReviewState { get; set; }
        public Int32? ProposalScopeLandAssignReviewUserSeq { get; set; }
        public Int32? ProposalScopeLandUpdateReviewUserSeq { get; set; }
        public DateTime? ProposalScopeLandReviewTime { get; set; }

        public string LocationMap { get; set; }
        public string AerialPhotography { get; set; }
        public string ScenePhoto { get; set; }
        public string BaseMap { get; set; }
        public string EngPlaneLayout { get; set; }
        public string LongitudinalSection { get; set; }
        public string StandardSection { get; set; }
        public Int32? NeedAssessmenApproval { get; set; }
        public Int32? EvaluationResult { get; set; }
        public string ER1_1 { get; set; }
        public string ER1_2 { get; set; }
        public string ER2_1 { get; set; }
        public string ER2_2 { get; set; }
        public string ER3 { get; set; }
        public string ER4 { get; set; }
        public string ER6 { get; set; }
        public Int32? ProposalReviewTypeSeq { get; set; }
        public Int32? ProposalReviewAttributesSeq { get; set; }
        public Int32? RiverSeq { get; set; }
        public Int32? DrainSeq { get; set; }
        public string Coastal { get; set; }
        public string LargeSectionChainage { get; set; }
        public Int32? CitySeq { get; set; }
        public Int32? TownSeq { get; set; }
        public decimal? CoordX { get; set; }
        public decimal? CoordY { get; set; }
        public string EngineeringScale { get; set; }
        public string ProcessReason { get; set; }
        public string EngineeringScaleMemo { get; set; }
        public string RelatedReportContent { get; set; }
        public Int32? HistoricalCatastrophe { get; set; }
        public string HistoricalCatastropheMemo { get; set; }
        public string ProtectionTarget { get; set; }
        public string SetConditions { get; set; }
        public string EcologicalConservationD01 { get; set; }
        public string EcologicalConservationD02 { get; set; }
        public string EcologicalConservationD03 { get; set; }
        public string EcologicalConservationD04 { get; set; }
        public string EcologicalConservationD05 { get; set; }
        public string EcologicalConservationD06 { get; set; }
        public decimal? DemandCarbonEmissions { get; set; }
        public string DemandCarbonEmissionsMemo { get; set; }
        public Int32? IsProposalReview { get; set; }
        public Int32? ProposalAuditOpinion { get; set; }
        public Int32? ReviewSort { get; set; }
        public decimal? ApprovedFund { get; set; }
        public decimal? ApprovedCarbonEmissions { get; set; }
        public decimal? Expenditure { get; set; }
        public string Resolution { get; set; }
        public Int32? ResolutionAuditOpinion { get; set; }
        public string EngNo { get; set; }
        public Int32? IsTransfer { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? ModifyUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }

        public Int32? EstimatedLandAcquisitionCosts { get; set; }
        public string ManagementPlanningLayoutSituation { get; set; }
        public Int32? IsFloodControlRecords { get; set; }
        public Int32? EstimatedExpenditureCurrentYear { get; set; }
        public Int32? ExpensesSubsequentYears { get; set; }
        public string BookingProcess_SY { get; set; }
        public string BookingProcess_SM { get; set; }
        public string BookingProcess_EY { get; set; }
        public string BookingProcess_EM { get; set; }
        public string Remark { get; set; }
    }
}