
namespace EQC.Models
{
    public class EC_SupDailyReportMiscConstructionModel
    {//工程變更-工程變更-施工日誌_雜項
        //public int EC_SupDailyDateSeq { get; set; }
        public int Seq { get; set; }
        public bool? IsFollowSkill { get; set; }
        public string SafetyHygieneMattersOther { get; set; }
        public bool? SafetyHygieneMatters01 { get; set; }
        public byte? SafetyHygieneMatters02 { get; set; }
        public bool? SafetyHygieneMatters03 { get; set; }
        public string SamplingTest { get; set; }
        public string NoticeManufacturers { get; set; }
        public string ImportantNotes { get; set; }
    }
}
