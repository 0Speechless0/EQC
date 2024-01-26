using System;

namespace EQC.Models
{
    public class EngAttachmentModel
    {//上傳監造計畫附件
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public int Chapter { get; set; }
        public byte FileType { get; set; }
        public string Description { get; set; }
        public string OriginFileName { get; set; }
        public string UniqueFileName { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
