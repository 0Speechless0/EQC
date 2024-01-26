using System;

namespace EQC.Models
{
    public class ResumeModel
    {//履歷
        public int Seq { get; set; }
        public int EngMaterialDeviceSummarySeq { get; set; }
        public int ResumeType { get; set; }

        public string ResumeUrl { get; set; }
        public string OriginFileName { get; set; }
        public string UniqueFileName { get; set; }
        public int OrderNo { get; set; }

        public DateTime? CreateTime { get; set; }
        public int CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int ModifyUserSeq { get; set; }
    }
}