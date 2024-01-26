namespace EQC.Models
{
    public class CarbonEmissionFactorDetailModel
    {//碳排係數維護-明細
        public int Seq { get; set; }
        public int CarbonEmissionFactorSeq { get; set; }
        public bool IsDel { get; set; }
        public string NameSpec { get; set; }
        public string Unit { get; set; }
        public decimal? Amount { get; set; }
        public decimal? KgCo2e { get; set; }
        public decimal? Quantity { get; set; }
        public string Memo { get; set; }
        public int ItemMode { get; set; }
    }
}
