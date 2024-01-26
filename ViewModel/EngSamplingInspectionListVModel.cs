using EQC.Common;

namespace EQC.Models
{
    public class EngSamplingInspectionVModel
    {//品質查證-施工抽查清單 s20230520
        public int Seq { get; set; }
        public int OrderNo { get; set; }
        public string ItemName { get; set; }
        public int? constCheckRecCount { get; set; }//抽查紀錄數 shioulo 20221216
        public int? missingCount { get; set; }//缺失數
    }
}
