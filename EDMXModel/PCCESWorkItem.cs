//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EQC.EDMXModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class PCCESWorkItem
    {
        public int Seq { get; set; }
        public int PCCESPayItemSeq { get; set; }
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
    
        public virtual PCCESPayItem PCCESPayItem { get; set; }
    }
}
