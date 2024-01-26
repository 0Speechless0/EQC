using System;

namespace EQC.Models
{
    public class EngRiskFrontProjectOutlineFunctionModel
    {//工程計畫概要-工程功能需求
        public int Seq { get; set; }
        public int EngRiskFrontSeq { get; set; }
        public string EngFunction { get; set; }
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