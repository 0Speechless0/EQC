using System;

namespace EQC.Models
{
    public class EC_SchProgressPayItem1Model : EC_SchProgressPayItemModel
    {//工程變更-預定進度
        public int EC_SchEngProgressPayItemSeq { get; set; }
        public decimal? SchProgressDayBefore { get; set; }
    }
}
