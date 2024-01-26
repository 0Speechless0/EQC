
using System;

namespace EQC.Models
{
    public class EC_SchEngProgressHeaderModel
    {//工程變更進度-主檔
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public int Version { get; set; }
        public int SPState { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? SchCompDate { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int ChangeType { get; set; }
        public int SupDailyReportExtensionSeq { get; set; }
        public int SupDailyReportWorkSeq { get; set; }
    }
}
