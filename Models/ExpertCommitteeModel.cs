using System;

namespace EQC.Models
{//專家委員
    public class ExpertCommitteeModel
    {
        public int Seq { get; set; }
        public string ECName { get; set; }
        public DateTime? ECBirthday { get; set; }
        public string ECId { get; set; }
        public byte? ECKind { get; set; }
        public string ECPosition { get; set; }
        public string ECUnit { get; set; }
        public string ECEmail { get; set; }
        public string ECTel { get; set; }
        public string ECMobile { get; set; }
        public string ECFax { get; set; }
        public string ECAddr1 { get; set; }
        public string ECAddr2 { get; set; }
        public string ECMainSkill { get; set; }
        public string ECSecSkill { get; set; }
        public string ECBankNo { get; set; }
        public byte? ECDiet { get; set; }
        public string ECNeed { get; set; }
        public string ECMemo { get; set; }
        //public int? OrderNo { get; set; }
        //public bool IsDeleted { get; set; }
        //public DateTime? CreateTime { get; set; }
        //public int? CreateUserSeq { get; set; }
        //public DateTime? ModifyTime { get; set; }
        //public int? ModifyUserSeq { get; set; }
    }
}