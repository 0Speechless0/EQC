using System;

namespace EQC.Models
{
    public class EngConstructionModel
    {//工程主要施工項目及數量
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public int? ItemQty { get; set; }
        public string ItemUnit { get; set; }
        public int? OrderNo { get; set; }

        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
