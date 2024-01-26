using System;

namespace EQC.Models
{
    public class ConstCheckRecImproveModel
    {//抽驗缺失改善
        public int? Seq { get; set; }
        public int ConstCheckRecSeq { get; set; }
        public byte? CheckItemKind { get; set; }
        public byte? IncompKind { get; set; }
        public byte? CheckerKind { get; set; }
        public DateTime? ImproveDeadline { get; set; }
        public string CauseAnalysis { get; set; }
        public string Improvement { get; set; }
        public string ProcessResult { get; set; }
        public byte? ImproveAuditResult { get; set; }
        public DateTime? ProcessTrackDate { get; set; }
        public string TrackCont { get; set; }
        public bool? CanClose { get; set; }
        public string CloseMemo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }

        public byte? FormConfirm { get; set; }
        public int? ImproveUserSeq { get; set; }
        public int? ApproveUserSeq { get; set; }
        public DateTime? ApproveDate { get; set; }
    }
}
