
namespace EQC.Models
{
    public class EC_SupDailyReportMiscModel
    {//工程變更-監造日誌_雜項
        //public int EC_SupDailyDateSeq { get; set; }
        public int Seq { get; set; }
        public string DesignDrawingConst { get; set; }
        public string SpecAndQuality { get; set; }
        public string SafetyHygieneMattersOther { get; set; }
        public bool? SafetyHygieneMatters01 { get; set; }
        public string OtherMatters { get; set; }
    }
}
