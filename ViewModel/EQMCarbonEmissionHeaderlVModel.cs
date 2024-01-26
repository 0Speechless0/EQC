using EQC.Common;
using EQC.Models;

namespace EQC.ViewModel
{
    public class EQMCarbonEmissionHeaderVModel : CarbonEmissionHeaderModel
    {//碳排量計算主檔
        public string PccesXMLDateStr
        {
            get
            {
                return PccesXMLDate.HasValue ? Utils.ChsDate(PccesXMLDate.Value) : "";
            }
        }
    }
}
