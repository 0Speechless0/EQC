using System;

namespace EQC.Models
{
    public class EngUploadHistoryModel
    {//監造計畫上傳歷程
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public int VersionNo { get; set; }
        public string Memo { get; set; }
        public string OriginFileName { get; set; }
        public string UniqueFileName { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
        public string ApproveNo { get; set; } //s20230518
    }
}
