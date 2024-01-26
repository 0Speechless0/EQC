namespace EQC.Models
{
    public class EC_SupDailyReportConstructionPersonModel
    {//工程變更-施工日誌-工地人員概況
        public int Seq { get; set; }
        public int EC_SupDailyDateSeq { get; set; }
        public string KindName { get; set; }
        public decimal? TodayQuantity { get; set; }
        public decimal? AccQuantity { get; set; }
    }
}
