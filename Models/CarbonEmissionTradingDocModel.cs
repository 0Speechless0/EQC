using System;

namespace EQC.Models
{
    public class CarbonEmissionTradingDocModel
    {//碳排量計算核定文件
        public int Seq { get; set; }
        public int CarbonEmissionHeaderSeq { get; set; }
        public DateTime? CarbonTradingApprovedDate { get; set; }
        public string CarbonTradingNo { get; set; }
        public string OriginFileName { get; set; }
        public string UniqueFileName { get; set; }

    }
}
