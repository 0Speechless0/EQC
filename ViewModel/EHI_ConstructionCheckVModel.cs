using System;

namespace EQC.ViewModel
{
    public class EHI_ConstructionCheckVModel : EHI_EngListBModel
    {//施工抽查紀錄
        public int constCheckRecCount { get; set; }
        public int missingCount { get; set; }
    }
}