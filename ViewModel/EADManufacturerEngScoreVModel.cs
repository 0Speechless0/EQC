
namespace EQC.ViewModel
{
    public class EADManufacturerEngScoreVModel
    {//廠商履歷評估分析 履約計分
        public int Seq { get; set; }
        public string TenderName { get; set; }
        public string ExecUnitName { get; set; }
        public decimal? BidAmount { get; set; }
        public decimal? PSTotalScore { get; set; }
        public string ActualAacceptanceCompletionDate { get; set; }
    }
}