using System;

namespace EQC.Models
{
    public class NcrModel
    {//NCR程序追蹤改善表
        public int? Seq { get; set; }
        public int ConstCheckRecSeq { get; set; }
        public string MissingItem { get; set; }
        public string CauseAnalysis { get; set; }
        public string CorrectiveAction { get; set; }
        public string PreventiveAction { get; set; }
        public string CorrPrevImproveResult { get; set; }
        public byte? ImproveAuditResult { get; set; }
        public DateTime? ProcessTrackDate { get; set; }
        public string TrackCont { get; set; }
        public bool? CanClose { get; set; }
        public string CloseMemo { get; set; }
        public byte? FormConfirm { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
        public int? ImproveUserSeq { get; set; }
        public int? ApproveUserSeq { get; set; }
        public DateTime? ApproveDate { get; set; }

        public byte? IncompKind { get; set; }//from 抽驗缺失改善
    }
}
