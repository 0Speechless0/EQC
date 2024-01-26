
using System;

namespace EQC.ViewModel
{
    public class EHI_SupervisionVModel : EHI_EngListBModel
    {//督導紀錄
        public DateTime? SuperviseDate { get; set; }
        public int? CommitteeAverageScore { get; set; }
        public decimal? DeductPoint { get; set; }
        public string SuperviseDateStr
        {
            get
            {
                return SuperviseDate.HasValue ? SuperviseDate.Value.ToString("yyyy-MM-dd") : "";
            }
        }
    }
}