namespace EQC.Models
{
    public class EC_SupDailyReportConstructionMaterialModel
    {//工程變更-施工日誌-工地材料管理概況
        public int Seq { get; set; }
        public int EC_SupDailyDateSeq { get; set; }
        public string MaterialName { get; set; }
        public string Unit { get; set; }
        public decimal? ContractQuantity { get; set; }
        public decimal? TodayQuantity { get; set; }
        public decimal AccQuantity { get; set; }
        public string Memo { get; set; }
    }
}
