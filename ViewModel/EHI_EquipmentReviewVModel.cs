using System;

namespace EQC.ViewModel
{
    public class EHI_EquipmentReviewVModel
    {//材料設備送審管制總表
        public int EngMaterialDeviceListSeq { get; set; }
        public string ItemNo { get; set; }
        public string MDName { get; set; }
        public Decimal ContactQty { get; set; }
        public string ContactUnit { get; set; }
        public int IsSampleTestCnt { get; set; }
        public int IsFactoryInspCnt { get; set; }
        public int AuditDateCnt { get; set; }
        public int AuditResultCnt { get; set; }
    }
}