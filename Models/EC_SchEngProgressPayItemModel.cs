namespace EQC.Models
{
    public class EC_SchEngProgressPayItemModel : PayItemModel
    {//工程變更-PayItem
        public int Seq { get; set; }
        public int? EC_SchEngProgressHeaderSeq { get; set; }
        public int? ParentEC_SchEngProgressPayItemSeq { get; set; }
        public int? ParentSchEngProgressPayItemSeq { get; set; }
        public int? RootSeq { get; set; }
        public int OrderNo { get; set; }
        public byte ItemMode { get; set; }
        public string Memo { get; set; }
        public int? GreenFundingSeq { get; set; }
        public string GreenFundingMemo { get; set; }
        public decimal? KgCo2e { get; set; }
        //public decimal? ItemKgCo2e { get; set; }
        //public string RStatus { get; set; }
        //public int RStatusCode { get; set; }
    }
}
