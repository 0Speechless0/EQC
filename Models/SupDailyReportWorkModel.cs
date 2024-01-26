using System;

namespace EQC.Models
{
    public class SupDailyReportWorkModel
    {//監造(施工)日誌_停復工
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public DateTime SStopWorkDate { get; set; }
        public DateTime EStopWorkDate { get; set; }
        public string StopWorkReason { get; set; }
        public string StopWorkNo { get; set; }
        public string StopWorkApprovalFile { get; set; }
        public DateTime? BackWorkDate { get; set; }
        public string BackWorkNo { get; set; }
        public string BackWorkApprovalFile { get; set; }

        /*public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }*/
    }
}
