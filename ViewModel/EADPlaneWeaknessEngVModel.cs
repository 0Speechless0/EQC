
namespace EQC.ViewModel
{
    public class EADPlaneWeaknessEngVModel
    {//品質管制弱面追蹤與分析 工程清單
        public int Seq { get; set; }
        public int TenderYear { get; set; }
        public string TenderName { get; set; }
        public string ExecUnitName { get; set; }
        public decimal? BidAmount { get; set; }
        public string Location { get; set; }
        public string PlaneWeakness { get; set; }
    }
}