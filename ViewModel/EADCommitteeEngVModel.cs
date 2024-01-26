
namespace EQC.ViewModel
{
    public class EADCommitteeEngVModel
    {//工程採購評選委員分析 工程清單
        public int Seq { get; set; }
        public int TenderYear { get; set; }
        public string TenderName { get; set; }
        public string ExecUnitName { get; set; }
        public decimal? BidAmount { get; set; }
        public string Location { get; set; }
        public string committees { get; set; }
    }
}