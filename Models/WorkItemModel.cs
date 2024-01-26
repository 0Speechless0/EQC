namespace EQC.Models
{
    public class WorkItemModel
    {//PCCESS詳細表.WorkItem
        public decimal WorkItemQuantity { get; set; }
        public string ItemCode { get; set; }
        public string ItemKind { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public decimal LabourRatio { get; set; }
        public decimal EquipmentRatio { get; set; }
        public decimal MaterialRatio { get; set; }
        public decimal MiscellaneaRatio { get; set; }
    }
}
