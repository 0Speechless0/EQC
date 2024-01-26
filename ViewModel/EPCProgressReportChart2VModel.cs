using System;

namespace EQC.ViewModel
{
    public class EPCProgressReportChart2VModel
    {//工程管理-進度報表
        public int OrderNo { get; set; }
        public string Description { get; set; }
        public DateTime minDate { get; set; }
        public DateTime maxDate { get; set; }
        public string minDateStr
        {
            get
            {
                return this.minDate.ToString("yyyy-M-d");
            }
        }
        public string maxDateStr
        {
            get
            {
                return this.maxDate.ToString("yyyy-M-d");
            }
        }
        //s20230227
        public decimal? SchProgress { get; set; }
        public decimal? ActualProgress { get; set; }
        public string Progress {
            get
            {
                if (SchProgress.HasValue && ActualProgress.HasValue)
                {
                    return String.Format("預:{0} 實:{1}", SchProgress.Value, ActualProgress.Value);
                } else
                {
                    return "";
                }
            }
        }
    }
}
