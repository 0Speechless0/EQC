using System;

namespace EQC.Models
{
    public class PCCESSMainModel
    {//PCCESS主檔
        public int Seq { get; set; }

        public string ProcuringEntity { get; set; }
        public string ProcuringEntityId { get; set; }
        public string ContractTitle { get; set; }
        public string ContractLocation { get; set; }
        public string contractNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
