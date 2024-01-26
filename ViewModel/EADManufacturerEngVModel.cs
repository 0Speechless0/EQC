
namespace EQC.ViewModel
{
    public class EADManufacturerEngVModel
    {//廠商履歷評估分析 工程清單
        public int Seq { get; set; }
        public int TenderYear { get; set; }
        public string TenderName { get; set; }
        public string ExecUnitName { get; set; }
        public decimal? BidAmount { get; set; }
        public string Location { get; set; }
        public string ActualPerformDesignDate { get; set; }
        public string ActualAacceptanceCompletionDate { get; set; }
        public decimal? PSTotalScore { get; set; }
    }
}