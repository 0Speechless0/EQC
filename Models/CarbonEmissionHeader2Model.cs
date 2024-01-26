using System;

namespace EQC.Models
{
    public class CarbonEmissionHeader2Model
    {//碳排量計算主檔
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public int State { get; set; }
        public int? AdjState { get; set; }
        //public DateTime? PccesXMLDate { get; set; }
        //public DateTime? CarbonTradingApprovedDate { get; set; }
        //public string CarbonTradingNo { get; set; }
        public string CarbonTradingDesc { get; set; }
  
    }
}
