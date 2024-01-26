namespace EQC.Models
{
    public class SchEngProgressPayItemModel : PayItemModel
    {//工程進度-PayItem
        public int Seq { get; set; }
        public int SchEngProgressHeaderSeq { get; set; }
        public string Memo { get; set; }
        public decimal? KgCo2e { get; set; }
        public int? GreenFundingSeq { get; set; }
        public string GreenFundingMemo { get; set; }
    }
}
