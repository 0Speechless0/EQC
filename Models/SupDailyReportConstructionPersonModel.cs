namespace EQC.Models
{
    public class SupDailyReportConstructionPersonModel
    {//施工日誌-工地人員概況
        public int Seq { get; set; }
        public int SupDailyDateSeq { get; set; }
        public string KindName { get; set; }
        public decimal? TodayQuantity { get; set; }
        public decimal? AccQuantity { get; set; }
    }
}
