using System;

namespace EQC.Models
{
    public class ConstRiskEvalModel
    {//設計階段施工風險評估
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string Descr { get; set; }
        public string FileName { get; set; }
    }
}
