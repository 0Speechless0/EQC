using System;

namespace EQC.Models
{
    public class EcologicalChecklistModel
    {//生態檢核 - 設計階段
        public string EngCreatureNameList { get; set; }

        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public int Stage { get; set; }
        public int ToDoChecklit { get; set; }
        public string ChecklistFilename { get; set; }
        public string SelfEvalFilename { get; set; }
        public string PlanDesignRecordFilename { get; set; }
        public string MemberDocFilename { get; set; }
        public string DataCollectDocFilename { get; set; }
        public string ConservMeasFilename { get; set; }
        public string SOCFilename { get; set; }
    }
}
