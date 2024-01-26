using System;

namespace EQC.Models
{
    public class SupervisionProjectTpModel
    {//監造計畫書範本
        public int Seq { get; set; }
        public string Name { get; set; }
        public DateTime? RevisionDate { get; set; }
        public string OriginFileName { get; set; }
        public string UniqueFileName { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
