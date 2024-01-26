namespace EQC.Models
{
    public class EngSamplingInspectionImproveVModel
    {//品質查證-施工抽查改善清單 s20230522
        public int Seq { get; set; }
        public int OrderNo { get; set; }
        public string ItemName { get; set; }
        public int? missingCount { get; set; }//缺失數
        public int? improveCount { get; set; }
        public int? ncrCount { get; set; }
        public int? photoCount { get; set; }
    }
}
