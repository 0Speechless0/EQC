using System;

namespace EQC.Models
{
    public class EC_SupPlanOverviewModel
    {//依施工計畫執行按圖施工概況
        public int EC_SupDailyDateSeq { get; set; }
        public int EC_SchEngProgressPayItemSeq { get; set; }
        public int Seq { get; set; }
        public string PayItem { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Amount { get; set; }
        public string ItemKey { get; set; }
        public string ItemNo { get; set; }
        public string RefItemCode { get; set; }
        public decimal TodayConfirm { get; set; }
        public string Memo { get; set; }
        public int OrderNo { get; set; }
        public decimal DayProgress { get; set; }
    }
}
