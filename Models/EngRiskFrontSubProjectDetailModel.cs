using System;

namespace EQC.Models
{
    public class EngRiskFrontSubProjectDetailModel
    {//設計方案評選-方案項目
        public int Seq { get; set; }
        public int SubProjectSeq { get; set; }
        public int Level { get; set; }
        public int ParentSeq { get; set; }
        public string GojsFromNo { get; set; }
        public string GojsToNo { get; set; }
        public string StepNo { get; set; }
        public string StepName { get; set; }
        public int HazardTypeSeq { get; set; }
        public string PossibleRiskSituation { get; set; }
        public int Possibility { get; set; }
        public int Severity { get; set; }
        public int IsAcceptable { get; set; }
        public string RisksAndOpportunitiesMeasure { get; set; }
        public int PrincipalSeq { get; set; }
        public string SummaryOfExecutiveResults { get; set; }
        public int IsEffect { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}