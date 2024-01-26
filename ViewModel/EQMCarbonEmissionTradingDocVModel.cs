using EQC.Models;

namespace EQC.ViewModel
{
    public class EQMCarbonEmissionTradingDocVModel : CarbonEmissionTradingDocModel
    {//碳排量計算核定文件
        public string CarbonTradingApprovedDateStr
        {
            get
            {
                return CarbonTradingApprovedDate.HasValue ? CarbonTradingApprovedDate.Value.ToString("yyyy-MM-dd") : "";
            }
        }
    }
}
