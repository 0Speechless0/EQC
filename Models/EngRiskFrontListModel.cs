using System;


namespace EQC.Models
{
    public class EngRiskFrontListModel
    {//施工風險評估主檔
        public int Seq { get; set; }
        public string EngNo { get; set; }
        public string PlanOriginAndTarget { get; set; }
        public string PlanScope { get; set; }
        public string PlanScopeFile { get; set; }
        public string PlanEnvironment { get; set; }
        public string DesignConcept { get; set; }
        public string DesignConceptFile { get; set; }
        public string DesignStudy { get; set; }
        public string DesignPrecautions { get; set; }
        public string DesignSelection { get; set; }
        public string DesignSelectionFile { get; set; }
        public string DesignStageRiskResult { get; set; }
        public string DesignStageRiskResultFile { get; set; }
        public string RiskTracking { get; set; }
        public string RiskTrackingFile { get; set; }
        public string Conclusion { get; set; }
        public string ConclusionFile { get; set; }
        public string FinishFile { get; set; }
        public Int32? IsFinish { get; set; }

        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
        public int LockState { get; set; }
    }
}