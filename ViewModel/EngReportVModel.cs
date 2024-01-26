using EQC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class EngReportVModel: EngReportModel
    {
        public string ExecUnit { get; set; }
        public string ExecSubUnit { get; set; }
        public string ExecUser { get; set; }

        //工程狀態（中文名稱）
        public string EngState { get; set; }
        //年度經費檢討會議-審核意見（中文名稱）
        public string ResolutionAuditOpinionName { get; set; }
        //轉入建立案件（中文名稱）
        public string IsTransferName { get; set; }

        public decimal? RefCarbonEmission { get; set; }
        public decimal? CarbonEmissionRatio { get; set; }
        public decimal? RegressionCurve { get; set; }
        public decimal? PriceAdjustmentIndex { get; set; }

        public decimal? EngReportEstimatedCostPrice { get; set; }

        public string ProposalReviewTypeName { get; set; }
        public string ProposalReviewAttributesName { get; set; }

        public Int16? RiverSeq1 { get; set; }
        public Int16? RiverSeq2 { get; set; }
        public Int16? RiverSeq3 { get; set; }

        public string RiverName1 { get; set; }
        public string RiverName2 { get; set; }
        public string RiverName3 { get; set; }

        public string DrainName { get; set; }
        public string CityName { get; set; }
        public string TownName { get; set; }


        public string OriginAndScopeUserName { get; set; }
        public string RelatedReportResultsUserName { get; set; }
        public string FacilityManagementUserName { get; set; }
        public string ProposalScopeLandUserName { get; set; }


        public string OriginAndScopeAssignReviewUserName { get; set; }
        public string RelatedReportResultsAssignReviewUserName { get; set; }
        public string FacilityManagementAssignReviewUserName { get; set; }
        public string ProposalScopeLandAssignReviewUserName { get; set; }


        public string OriginAndScopeUpdateReviewUserName { get; set; }
        public string RelatedReportResultsUpdateReviewUserName { get; set; }
        public string FacilityManagementUpdateReviewUserName { get; set; }
        public string ProposalScopeLandUpdateReviewUserName { get; set; }


        public string OriginAndScopeStateName { get; set; }
        public string RelatedReportResultsStateName { get; set; }
        public string FacilityManagementStateName { get; set; }
        public string ProposalScopeLandStateName { get; set; }


        public string OriginAndScopeReviewStateName { get; set; }
        public string RelatedReportResultsReviewStateName { get; set; }
        public string FacilityManagementReviewStateName { get; set; }
        public string ProposalScopeLandReviewStateName { get; set; }


        public string OriginAndScopeTWDT { get { return this.OriginAndScopeUpdateTime.HasValue ? Utils.ChsDateTime(this.OriginAndScopeUpdateTime) : string.Empty; } }
        public string RelatedReportResultsTWDT { get { return this.RelatedReportResultsUpdateTime.HasValue ? Utils.ChsDateTime(this.RelatedReportResultsUpdateTime) : string.Empty; } }
        public string FacilityManagementTWDT { get { return this.FacilityManagementUpdateTime.HasValue ? Utils.ChsDateTime(this.FacilityManagementUpdateTime) : string.Empty; } }
        public string ProposalScopeLandTWDT { get { return this.ProposalScopeLandUpdateTime.HasValue ? Utils.ChsDateTime(this.ProposalScopeLandUpdateTime) : string.Empty; } }
        public string OriginAndScopeReviewTWDT { get { return this.OriginAndScopeReviewTime.HasValue ? Utils.ChsDateTime(this.OriginAndScopeReviewTime) : string.Empty; } }
        public string RelatedReportResultsReviewTWDT { get { return this.RelatedReportResultsReviewTime.HasValue ? Utils.ChsDateTime(this.RelatedReportResultsReviewTime) : string.Empty; } }
        public string FacilityManagementReviewTWDT { get { return this.FacilityManagementReviewTime.HasValue ? Utils.ChsDateTime(this.FacilityManagementReviewTime) : string.Empty; } }
        public string ProposalScopeLandReviewTWDT { get { return this.ProposalScopeLandReviewTime.HasValue ? Utils.ChsDateTime(this.ProposalScopeLandReviewTime) : string.Empty; } }

        //public bool OriginAndScopeReviewStateCheck { get { return this.OriginAndScopeReviewState == 0 ? false : true; } }
        //public bool RelatedReportResultsReviewStateCheck { get { return this.RelatedReportResultsReviewState == 0 ? false : true; } }
        //public bool FacilityManagementReviewStateCheck { get { return this.FacilityManagementReviewState == 0 ? false : true; } }
        //public bool ProposalScopeLandReviewStateCheck { get { return this.ProposalScopeLandReviewState == 0 ? false : true; } }


        public string LocationMapFileName { get; set; }
        public string AerialPhotographyFileName { get; set; }
        public string ScenePhotoFileName { get; set; }
        public string BaseMapFileName { get; set; }
        public string EngPlaneLayoutFileName { get; set; }
        public string LongitudinalSectionFileName { get; set; }
        public string StandardSectionFileName { get; set; }


        public string D01FileName { get; set; }
        public string D02FileName { get; set; }
        public string D03FileName { get; set; }
        public string D04FileName { get; set; }
        public string D05FileName { get; set; }
        public string D06FileName { get; set; }


        public string FileName { get; set; }

        public int IsEditA { get; set; }
        public int IsEditB { get; set; }
        public int IsEditC { get; set; }
        public int IsEditD { get; set; }
        public int IsEditF { get; set; }
        public int IsEditFile { get; set; }

        public bool edit { get; set; }
        public bool IsShow { get; set; }
        public bool IsCheck { get; set; }

        /// <summary>
        /// 能否顯示送簽或簽核
        /// </summary>
        public bool IsSavaApproval { get; set; }
    }
}