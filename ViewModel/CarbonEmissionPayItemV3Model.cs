using EQC.Models;
using EQC.Services;

namespace EQC.ViewModel
{
    public class CarbonEmissionPayItemV3Model : CarbonEmissionPayItemVModel
    {//碳排量計算
        public string execUnitName { get; set; }
        public string EngName { get; set; }
        public string EngNo { get; set; }
        public string GreenItemName { get; set; }
    }
}
