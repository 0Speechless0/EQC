using System;

namespace EQC.Models
{
    public class FullTimeEngineerModel
    {//專任工程人員
        public int Seq { get; set; }
        public int PrjXMLSeq { get; set; }
        public string FEName { get; set; }
        public string FELicenseKind { get; set; }
        public string FELicenseNo { get; set; }
        public string FEStartDate { get; set; }
        public string FEEndDate { get; set; }
        public string FEMemo { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
