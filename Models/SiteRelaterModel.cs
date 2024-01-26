using System;

namespace EQC.Models
{
    public class SiteRelateModel
    {//工地相關人員
        public int Seq { get; set; }
        public int PrjXMLSeq { get; set; }
        public string SRName { get; set; }
        public string SRLicenseKind { get; set; }
        public string SRLicenseNo { get; set; }
        public string SRStartDate { get; set; }
        public string SREndDate { get; set; }
        public string SRMemo { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
