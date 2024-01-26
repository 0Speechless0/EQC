using System;

namespace EQC.Models
{
    public class AskPaymentHeaderModel
    {//估驗請款
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public int APState { get; set; }
        public DateTime APDate { get; set; }
        public decimal CurrentAccuAmount { get; set; }
        public int Period { get; set; }
        public DateTime SupDailyDate { get; set; }
    }
}
