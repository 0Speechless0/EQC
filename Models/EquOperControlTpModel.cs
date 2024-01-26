using System;

namespace EQC.Models
{//設備運轉抽查標準範本
    public class EquOperControlTpModel
    {
        public int Seq { get; set; }
        public int EquOperTestTpSeq { get; set; }
        public string EPCheckItem1 { get; set; }
        public string EPCheckItem2 { get; set; }
        public string EPStand1 { get; set; }
        public string EPStand2 { get; set; }
        public string EPStand3 { get; set; }
        public string EPStand4 { get; set; }
        public string EPStand5 { get; set; }
        public string EPCheckTiming { get; set; }
        public string EPCheckMethod { get; set; }
        public string EPCheckFeq { get; set; }
        public string EPIncomp { get; set; }
        public string EPManageRec { get; set; }
        public byte? EPType { get; set; }
        public bool? EPMemo { get; set; }
        public int? EPCheckFields { get; set; }
        public int? EPManageFields { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}