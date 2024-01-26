using System;

namespace EQC.Models
{
    public class EC_SchProgressPayItemModel : PayItemModel
    {//工程變更-預定進度
        public int Seq { get; set; }
        public int EC_SchEngProgressPayItemSeq { get; set; }
        public DateTime SPDate { get; set; }
        public int OrderNo { get; set; }
        public decimal SchProgress { get; set; }
        public decimal DayProgress { get; set; }
        public decimal DayProgressAfter { get; set; }
        public int Days { get; set; }
    }
}
