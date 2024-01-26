namespace EQC.Models
{
    public class CarbonEmissionCustomizeModel
    {//自定義 碳排係數維護
        public int Seq { get; set; }
        public string CreateUnit { get; set; }
        public string ItemCode { get; set; }
        public string NameSpec { get; set; }
        public decimal? KgCo2e { get; set; }
        public string Unit { get; set; }
        public string Memo { get; set; }
        public int itemNo { get; set; }
    }
}
