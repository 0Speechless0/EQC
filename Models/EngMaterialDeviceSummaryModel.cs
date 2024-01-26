using System;

namespace EQC.Models
{
    public class EngMaterialDeviceSummaryModel
    {//材料設備送審管制總表
        public int Seq { get; set; }
        public int EngMaterialDeviceListSeq { get; set; }
        public int OrderNo { get; set; }
        public string ItemNo { get; set; }
        public string MDName { get; set; }

        public Decimal ContactQty { get; set; }
        public string ContactUnit { get; set; }
        public bool IsSampleTest { get; set; }
        public DateTime? SchAuditDate { get; set; }
        public DateTime? RealAutitDate { get; set; }
        public bool IsFactoryInsp { get; set; }
        public DateTime? FactoryInspDate { get; set; }
        public bool IsAuditVendor { get; set; }

        public string VendorName { get; set; }
        public string VendorTaxId { get; set; }
        public string VendorAddr { get; set; } //s20230502
        public decimal? VendorLng { get; set; } //s20231106
        public decimal? VendorLat { get; set; } //s20231106
        public string VendorDistance { get; set; } //s20231106
        public bool IsAuditCatalog { get; set; }
        public bool IsAuditReport { get; set; }
        public bool IsAuditSample { get; set; }
        public string OtherAudit { get; set; }
        public DateTime? AuditDate { get; set; }
        public int AuditResult { get; set; }
        public string ArchiveNo { get; set; }

        public DateTime? CreateTime { get; set; }
        public int CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int ModifyUserSeq { get; set; }
        public byte ItemType { get; set; } //s20230308
        public int RefSeq { get; set; }//s20230308
    }
}