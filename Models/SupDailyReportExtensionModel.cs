using System;

namespace EQC.Models
{
    public class SupDailyReportExtensionModel
    {//監造(施工)日誌_設定展延工期
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public int ExtendDays { get; set; }
        public string ApprovalNo { get; set; }
        public DateTime ApprovalDate { get; set; }
        public byte ExtendReason { get; set; }
        public string ExtendReasonOther { get; set; }
        /*public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }*/
    }
}
