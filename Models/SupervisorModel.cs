using System;

namespace EQC.Models
{
    public class SupervisorModel
    {//監造單位聘用監工人員
        public int Seq { get; set; }
        public int PrjXMLSeq { get; set; }
        public string SPName { get; set; }
        public string SPLicenseNo { get; set; }
        public string SPSkill { get; set; }
        public string SPMoveinDate { get; set; }
        public string SPDismissalDate { get; set; }
        public string SPState { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
