using System;

namespace EQC.Models
{
    public class ContractorQualityControlModel
    {//廠商聘用品管人員
        public int Seq { get; set; }
        public int PrjXMLSeq { get; set; }
        public string QCName { get; set; }
        public string QCLicenseNo { get; set; }
        public string QCSkill { get; set; }
        public string QCMoveinDate { get; set; }
        public string QCDismissalDate { get; set; }
        public string QCState { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
