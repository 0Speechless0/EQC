using System;

namespace EQC.Models
{
    public class EngMaterialDeviceListTpModel : FlowChartFileModel
    {//材料設備清冊範本
        //public int Seq { get; set; }
        public int? OrderNo { get; set; }
        public int? ParentSeq { get; set; }
        public string ExcelNo { get; set; }
        public string MDName { get; set; }
        //public string FlowCharOriginFileName { get; set; }
        //public string FlowCharUniqueFileName { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
