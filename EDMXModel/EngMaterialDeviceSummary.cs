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
    
    public partial class EngMaterialDeviceSummary
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EngMaterialDeviceSummary()
        {
            this.EngMaterialDeviceSummary1 = new HashSet<EngMaterialDeviceSummary>();
        }
    
        public int Seq { get; set; }
        public Nullable<int> EngMaterialDeviceListSeq { get; set; }
        public Nullable<int> OrderNo { get; set; }
        public string ItemNo { get; set; }
        public string MDName { get; set; }
        public Nullable<decimal> ContactQty { get; set; }
        public string ContactUnit { get; set; }
        public Nullable<bool> IsSampleTest { get; set; }
        public Nullable<System.DateTime> SchAuditDate { get; set; }
        public Nullable<System.DateTime> RealAutitDate { get; set; }
        public Nullable<bool> IsFactoryInsp { get; set; }
        public Nullable<System.DateTime> FactoryInspDate { get; set; }
        public Nullable<bool> IsAuditVendor { get; set; }
        public string VendorName { get; set; }
        public string VendorTaxId { get; set; }
        public Nullable<bool> IsAuditCatalog { get; set; }
        public Nullable<bool> IsAuditReport { get; set; }
        public Nullable<bool> IsAuditSample { get; set; }
        public string OtherAudit { get; set; }
        public Nullable<System.DateTime> AuditDate { get; set; }
        public Nullable<byte> AuditResult { get; set; }
        public string ArchiveNo { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> CreateUserSeq { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
        public Nullable<int> ModifyUserSeq { get; set; }
        public byte ItemType { get; set; }
        public Nullable<int> RefSeq { get; set; }
        public string VendorAddr { get; set; }
        public Nullable<decimal> VendorLng { get; set; }
        public Nullable<decimal> VendorLat { get; set; }
        public string VendorDistance { get; set; }
    
        public virtual EngMaterialDeviceList EngMaterialDeviceList { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EngMaterialDeviceSummary> EngMaterialDeviceSummary1 { get; set; }
        public virtual EngMaterialDeviceSummary EngMaterialDeviceSummary2 { get; set; }
    }
}