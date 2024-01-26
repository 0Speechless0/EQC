using System;

namespace EQC.Models
{
    public class UnitModel
    {//單位
        public int Seq { get; set; }
        public int? ParentSeq { get; set; }
        public string PCCESSCode { get; set; }
        public string Code { get; set; } 
        public string Name { get; set; }
        public int? OrderNo { get; set; }

        public DateTime? CreateTime { get; set; }
        public int? CreateUser { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUser { get; set; }
    }
}