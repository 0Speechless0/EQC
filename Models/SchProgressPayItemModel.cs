using System;

namespace EQC.Models
{
    public class SchProgressPayItemModel : PayItemModel
    {//預定進度
        public int Seq { get; set; }
        public int SchProgressHeaderSeq { get; set; }
        public int SchEngProgressPayItemSeq { get; set; } //s20230330
        public DateTime SPDate { get; set; }
        //public string PayItem { get; set; }
        //public string Description { get; set; }
        //public string Unit { get; set; }
        //public decimal? Quantity { get; set; }
        //public decimal? Price { get; set; }
        //public decimal? Amount { get; set; }
        //public string ItemKey { get; set; }
        //public string ItemNo { get; set; }
        //public string RefItemCode { get; set; }
        public int OrderNo { get; set; }
        public decimal SchProgress { get; set; }
        public decimal DayProgress { get; set; }
        public decimal DayProgressAfter { get; set; }
        public int Days { get; set; }
    }
}
