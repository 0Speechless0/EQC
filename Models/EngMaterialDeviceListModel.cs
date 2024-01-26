
namespace EQC.Models
{
    public class EngMaterialDeviceListModel : EngMaterialDeviceListTpModel
    {//材料設備送審清冊
        public string ItemNo { get; set; }
        public int EngMainSeq { get; set; }
        public bool DataKeep { get; set; }
        public int DataType { get; set; }
        //s20230327
        public bool IsAuditVendor { get; set; }
        public bool IsAuditCatalog { get; set; }
        public bool IsAuditReport { get; set; }
        public bool IsAuditSample { get; set; }
        public string OtherAudit { get; set; }
    }
}
