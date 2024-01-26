using System;

namespace EQC.Models
{
    public class EnvirConsListTpModel : FlowChartFileModel
    {//環境保育清單範本
        public string ExcelNo { get; set; }
        public string ItemName { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
