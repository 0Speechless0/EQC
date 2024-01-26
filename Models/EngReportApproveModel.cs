using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{//簽核歷程
    public class EngReportApproveModel
    {
        public int Seq { get; set; }
        public int? EngReportSeq { get; set; }
        public int? GroupId { get; set; }
        public int? ApprovalModuleListSeq { get; set; }
        public Int16? UnitSeq { get; set; }
        public Int16? SubUnitSeq { get; set; }
        public Int16? PositionSeq { get; set; }
        public int? UserMainSeq { get; set; }
        public int? ApproveUserSeq { get; set; }
        public DateTime? ApproveTime { get; set; }
        public string Signature { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? ModifyUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}