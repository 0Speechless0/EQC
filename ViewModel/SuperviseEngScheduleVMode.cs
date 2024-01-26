using EQC.Models;

namespace EQC.ViewModel
{
    public class SuperviseEngScheduleVModel : SuperviseEngModel
    {//工程督導 - 行程安排
        public string EngNo { get; set; }
        public string EngName { get; set; }
        public string LeaderName { get; set; }
        public string OutCommittee { get; set; }
        public string InsideCommittee { get; set; }
        public string OfficerName { get; set; }
        public string OfficerTel { get; set; }
        public string OfficerMobile { get; set; }
        public string ExecUnitName { get; set; }
        public string OrganizerName { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }

        public string SuperviseStartTimeStr {
            get {
                if (SuperviseStartTime.HasValue)
                    return SuperviseStartTime.Value.ToString().Substring(0,5);
                else
                    return "";
            }
            
        }
    }
}
