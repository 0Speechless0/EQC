using System;

namespace EQC.Models
{
    public class EcologicalChecklist2Model
    {//生態檢核 - 施工階段
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public int Stage { get; set; }
        public string ChecklistFilename { get; set; }
        public string SelfEvalFilename { get; set; }
        public string PlanDesignRecordFilename { get; set; }
        public string DataCollectDocFilename { get; set; }
        public string ConservMeasFilename { get; set; }
        public string SOCFilename { get; set; }
        public string LivePhoto { get; set; }
        public string EngDiagram { get; set; }
        public string Other { get; set; }
        public string Other2 { get; set; }

    }
}
