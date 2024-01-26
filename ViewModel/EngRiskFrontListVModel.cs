using EQC.Common;
using System;

namespace EQC.Models
{
    public class EngRiskFrontListVModel : EngRiskFrontListModel
    {//施工風險評估主檔
        public string PlanScopeFileName { get; set; }
        public string DesignConceptFileName { get; set; }
        public string DesignSelectionFileName { get; set; }
        public string DesignStageRiskResultFileName { get; set; }
        public string RiskTrackingFileName { get; set; }
        public string ConclusionFileName { get; set; }
        public string FinishFileName { get; set; }
        public string FileName { get; set; }
    }
}