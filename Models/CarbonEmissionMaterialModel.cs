namespace EQC.Models
{
    public class CarbonEmissionMaterialModel
    {//材料 碳排係數維護
        public int Seq { get; set; }
        public string Kind { get; set; }
        public string NameSpec { get; set; }
        public decimal? KgCo2e { get; set; }
        public string Unit { get; set; }
        public string Memo { get; set; }
        public int itemNo { get; set; }
    }
}
