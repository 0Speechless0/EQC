using System;
using System.Web;

namespace EQC.Models
{//材料設備抽查管理標準範本
    public class EngMaterialDeviceControlTpModel
    {
        public int Seq { get; set; }
        public int EngMaterialDeviceListTpSeq { get; set; }
        public string MDTestItem { get; set; }
        public string MDTestStand1 { get; set; }
        public string MDTestStand2 { get; set; }
        public string MDTestTime { get; set; }
        public string MDTestMethod { get; set; }
        public string MDTestFeq { get; set; }
        public string MDIncomp { get; set; }
        public string MDManageRec { get; set; }
        public string MDMemo { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}