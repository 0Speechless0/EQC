using System;

namespace EQC.Models
{
    public class OccuSafeHealthListTpModel : FlowChartFileModel
    {//職業安全衛生清單範本
        public string ExcelNo { get; set; }
        public string ItemName { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
