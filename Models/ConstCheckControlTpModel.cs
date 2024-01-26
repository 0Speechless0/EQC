using System;

namespace EQC.Models
{//施工抽查標準範本
    public class ConstCheckControlTpModel
    {
        public int Seq { get; set; }
        public int ConstCheckListTpSeq { get; set; }
        public byte? CCFlow1 { get; set; }
        public string CCFlow2 { get; set; }
        public string CCManageItem1 { get; set; }
        public string CCManageItem2 { get; set; }
        public string CCCheckStand1 { get; set; }
        public string CCCheckStand2 { get; set; }
        public string CCCheckTiming { get; set; }
        public string CCCheckMethod { get; set; }
        public string CCCheckFeq { get; set; }
        public string CCIncomp { get; set; }
        public string CCManageRec { get; set; }
        public byte? CCType { get; set; }
        public bool CCMemo { get; set; }
        public int? CCCheckFields { get; set; }
        public int? CCManageFields { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}