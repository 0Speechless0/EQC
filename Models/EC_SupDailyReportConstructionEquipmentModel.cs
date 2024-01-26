namespace EQC.Models
{
    public class EC_SupDailyReportConstructionEquipmentModel
    {//工程變更-施工日誌-機具管理
        public int Seq { get; set; }
        public int EC_SupDailyDateSeq { get; set; }
        public string EquipmentName { get; set; }
        public decimal? TodayQuantity { get; set; }
        public decimal? AccQuantity { get; set; }
        public decimal? TodayHours { get; set; }
        public decimal? AccHours { get; set; }
        public string EquipmentModel { get; set; }
        public decimal? KgCo2e { get; set; } //s20230502
    }
}
