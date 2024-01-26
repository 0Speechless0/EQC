using EQC.Common;
using EQC.Models;
using System;

namespace EQC.ViewModel
{
    public class EQMCarbonEmissionHeaderTradeVModel : CarbonEmissionHeader2Model
    {//碳交易主檔
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public int State { get; set; }
        public DateTime? CarbonTradingApprovedDate { get; set; }
        public string CarbonTradingNo { get; set; }
        public string CarbonTradingApprovedDateStr
        {
            get
            {
                return CarbonTradingApprovedDate.HasValue ? CarbonTradingApprovedDate.Value.ToString("yyyy-MM-dd") : "";
            }
        }
    }
}
