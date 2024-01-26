
namespace EQC.Models
{
    public class SupDailyReportMiscModel
    {//監造日誌_雜項
        //public int SupDailyDateSeq { get; set; }
        public int Seq { get; set; }
        public string DesignDrawingConst { get; set; }
        //public bool? IsFollowSkill { get; set; }
        //public int TheConcrete140 { get; set; }
        //public int TheConcrete210 { get; set; }
        //public int Rebar { get; set; }
        public string SpecAndQuality { get; set; }
        public string SafetyHygieneMattersOther { get; set; }
        public bool? SafetyHygieneMatters01 { get; set; }
        public string OtherMatters { get; set; }
    }
}
