namespace EQC.Models
{
    public class CarbonEmissionSettingModel
    {//碳排量設定
        public int Seq { get; set; }
        public int EngYear { get; set; }
        public int EngUnitSeq { get; set; }
        public int? CarbonDemandQuantity { get; set; }
        public int? ApprovedCarbonQuantity { get; set; }
        public int? CarbonDesignQuantity { get; set; }
        public int? CarbonConstructionQuantity { get; set; }
    }
}
