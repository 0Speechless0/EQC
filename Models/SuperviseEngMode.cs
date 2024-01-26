using System;

namespace EQC.Models
{
    public class SuperviseEngModel
    {//督導工程
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public byte? SuperviseMode { get; set; }
        public DateTime? SuperviseDate { get; set; }
        public DateTime? SuperviseEndDate { get; set; } //s20230316
        public int? LeaderSeq { get; set; }
        public string Memo { get; set; }
        public string BriefingPlace { get; set; }
        public string BriefingAddr { get; set; }
        public bool IsVehicleDisp { get; set; }
        public bool IsTHSR { get; set; }
        public string AdminContact { get; set; }
        public string AdminTel { get; set; }
        public string AdminMobile { get; set; }
        public string RiverBureauContact { get; set; }
        public string RiverBureauTel { get; set; }
        public string RiverBureauMobile { get; set; }
        public string LocalGovContact { get; set; }
        public string LocalGovTel { get; set; }
        public string LocalGovMobile { get; set; }
        public int ToBriefingDrive { get; set; }
        public TimeSpan? SuperviseStartTime { get; set; }
        public byte? SuperviseOrder { get; set; }
    }
}
