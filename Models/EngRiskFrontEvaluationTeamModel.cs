using System;

namespace EQC.Models
{
    public class EngRiskFrontEvaluationTeamModel
    {//準備作業 - 施工風險評估小組
        public int Seq { get; set; }
        public int EngRiskFrontSeq { get; set; }
        public string JobTitle { get; set; }
        public int OrganizerUnitSeq { get; set; }
        public int UnitSeq { get; set; }
        public int PrincipalSeq { get; set; }
        public string Memo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}