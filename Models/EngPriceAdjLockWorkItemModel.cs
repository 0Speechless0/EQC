namespace EQC.Models
{
    public class EngPriceAdjLockWorkItemModel
    {//工程物價調整款.WorkItem鎖定資料
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public int SchEngProgressWorkItemSeq { get; set; }
        public int Kind { get; set; }
        public string PayItem { get; set; }
        public string Description { get; set; }
        public string ItemCode { get; set; }
        public decimal Weights { get; set; }
        public decimal Amount { get; set; }
        public int? PriceIndexKindId { get; set; }
    }
}
