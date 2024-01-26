using System;

namespace EQC.Models
{
    public class EngRiskFrontProjectOutlineSiteEnvironmentModel
    {//工程計畫概要-工址環境現況
        public int Seq { get; set; }
        public int EngRiskFrontSeq { get; set; }
        public string SiteEnvironment { get; set; }
        public string PotentialHazard { get; set; }
        public string HazardCountermeasures { get; set; }
        public int PrincipalSeq { get; set; }
        public string EngMemo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}