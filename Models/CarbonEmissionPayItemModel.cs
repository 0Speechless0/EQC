namespace EQC.Models
{
    public class CarbonEmissionPayItemModel: PayItemModel
    {//碳排量計算
        public int Seq { get; set; }
        public int CarbonEmissionHeaderSeq { get; set; }
        public decimal? KgCo2e { get; set; }
        public decimal? ItemKgCo2e { get; set; }
        public string Memo { get; set; }
        public string RStatus { get; set; }
        public int RStatusCode { get; set; }
        //s20230418
        public int? GreenFundingSeq { get; set; }
        public string GreenFundingMemo { get; set; }

        /*public string PayItem { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Amount { get; set; }
        public string ItemKey { get; set; }
        public string ItemNo { get; set; }
        public string RefItemCode { get; set; }*/
    }
}
