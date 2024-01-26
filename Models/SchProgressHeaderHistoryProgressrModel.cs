using System;

namespace EQC.Models
{
    public class SchProgressHeaderHistoryProgressModel
    {//工程預定進度
        public int SchProgressHeaderHistorySeq { get; set; }
        public DateTime ProgressDate { get; set; }
        public decimal SchProgress { get; set; }
    }
}
