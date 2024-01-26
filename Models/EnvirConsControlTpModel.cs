using System;

namespace EQC.Models
{//環境保育抽查標準範本
    public class EnvirConsControlTpModel
    {
        public int Seq { get; set; }
        public int EnvirConsListSeq { get; set; }
        public byte? ECCFlow1 { get; set; }
        public string ECCCheckItem1 { get; set; }
        public string ECCCheckItem2 { get; set; }
        public string ECCStand1 { get; set; }
        public string ECCStand2 { get; set; }
        public string ECCStand3 { get; set; }
        public string ECCStand4 { get; set; }
        public string ECCStand5 { get; set; }
        public string ECCCheckTiming { get; set; }
        public string ECCCheckMethod { get; set; }
        public string ECCCheckFeq { get; set; }
        public string ECCIncomp { get; set; }
        public string ECCManageRec { get; set; }
        public byte? ECCType { get; set; }
        public bool ECCMemo { get; set; }
        public int? ECCCheckFields { get; set; }
        public int? ECCManageFields { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}