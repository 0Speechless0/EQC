using System;

namespace EQC.Models
{
    public class EC_SupDailyDateModel
    {//工程變更-監造(施工)日誌_日誌日期
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public DateTime ItemDate { get; set; }
        public byte DataType { get; set; }
        public int ItemState { get; set; }
        //s20230408
        public string Weather1 { get; set; }
        public string Weather2 { get; set; }
        public DateTime? FillinDate { get; set; }
        //s20230908
        public DateTime? ModifyTime { get; set; }
        public string DisplayName { get; set; }
    }
}
