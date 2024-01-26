namespace EQC.Models
{
    public class EngPriceAdjWorkItemModel
    {//工程物價調整款.WorkItem
        public int Seq { get; set; }
        public int EngPriceAdjSeq { get; set; }
        public int EngPriceAdjLockWorkItemSeq { get; set; }
        public decimal MonthQuantity { get; set; }
        public decimal PriceIndex { get; set; }
        public decimal PriceAdjustment { get; set; }
        public int AdjKind { get; set; }
    }
}
