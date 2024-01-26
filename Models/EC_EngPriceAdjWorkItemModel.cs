namespace EQC.Models
{
    public class EC_EngPriceAdjWorkItemModel
    {//工程變更-工程物價調整款.WorkItem
        public int Seq { get; set; }
        public int EC_EngPriceAdjSeq { get; set; }
        public int EC_EngPriceAdjLockWorkItemSeq { get; set; }
        public decimal MonthQuantity { get; set; }
        public decimal PriceIndex { get; set; }
        public decimal PriceAdjustment { get; set; }
        public int AdjKind { get; set; }
    }
}
