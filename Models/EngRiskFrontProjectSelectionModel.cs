using System;

namespace EQC.Models
{
    public class EngRiskFrontProjectSelectionModel
    {//設計方案評選-方案項目
        public int Seq { get; set; }
        public int EngRiskFrontSeq { get; set; }
        public int PSType { get; set; }
        public string PlanOverview { get; set; }
        public string Weight1 { get; set; }
        public string Weight2 { get; set; }
        public string Weight3 { get; set; }
        public string Weight4 { get; set; }
        public string Weight5 { get; set; }
        public string Weight6 { get; set; }
        public string Weight7 { get; set; }
        public string TWeight { get; set; }
        public int WeightSort { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}