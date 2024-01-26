using System;

namespace EQC.Models
{//施工抽查清單範本
    public class ConstCheckListTpModel : FlowChartFileModel
    {
        public string ExcelNo { get; set; }
        public string ItemName { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
