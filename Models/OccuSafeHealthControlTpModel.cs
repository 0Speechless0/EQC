using System;

namespace EQC.Models
{//職業安全衛生抽查標準範本
    public class OccuSafeHealthControlTpModel
    {
        public int Seq { get; set; }
        public int OccuSafeHealthListSeq { get; set; }
        public string OSCheckItem1 { get; set; }
        public string OSCheckItem2 { get; set; }
        public string OSStand1 { get; set; }
        public string OSStand2 { get; set; }
        public string OSStand3 { get; set; }
        public string OSStand4 { get; set; }
        public string OSStand5 { get; set; }
        public string OSCheckTiming { get; set; }
        public string OSCheckMethod { get; set; }
        public string OSCheckFeq { get; set; }
        public string OSIncomp { get; set; }
        public string OSManageRec { get; set; }
        public byte? OSType { get; set; }
        public bool OSMemo { get; set; }
        public int? OSCheckFields { get; set; }
        public int? OSManageFields { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}