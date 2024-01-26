using System;

namespace EQC.ViewModel
{
    public class EHI_EngListBModel
    {//工程清單
        public int Seq { get; set; }
        public Int16 EngYear { get; set; }
        public string EngNo { get; set; }
        public string EngName { get; set; }
        public string execUnitName { get; set; }
        public string execSubUnitName { get; set; }
    }
}