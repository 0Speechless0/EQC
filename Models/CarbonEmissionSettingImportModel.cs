namespace EQC.Models
{
    public class CarbonEmissionSettingImportModel
    {//碳排量設定 匯入
        public int Seq { get; set; }
        public int EngYear { get; set; }
        public int EngUnitSeq { get; set; }
        public string EngNo { get; set; }
        public int CarbonDemandQuantity { get; set; }
        public int ApprovedCarbonQuantity { get; set; }
    }
}
