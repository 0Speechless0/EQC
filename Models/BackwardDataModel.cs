using System;

namespace EQC.Models
{
    public class BackwardDataModel
    {//落後資料
        public int Seq { get; set; }
        public int PrjXMLSeq { get; set; }
        public int? BDYear { get; set; }
        public int? BDMonth { get; set; }
        public string BDBackwardFactor { get; set; }
        public string BDAnalysis { get; set; }
        public string BDSolution { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
        public string ImproveDeadline { get; set; }
    }
}
