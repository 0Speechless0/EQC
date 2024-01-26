using System;

namespace EQC.Models
{
    public class EquOperTestListTpModel : FlowChartFileModel
    {//設備運轉測試清單範本
        public byte? EPKind { get; set; }
        public string ItemName { get; set; }
        public int? OrderNo { get; set; }
        public string ExcelNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
