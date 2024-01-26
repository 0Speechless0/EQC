using System;

namespace EQC.Models
{
    public class CarbonEmissionHeaderModel
    {//碳排量計算主檔
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public int State { get; set; }
        public string PccesXMLFile { get; set; }
        public DateTime? PccesXMLDate { get; set; }
    }
}
