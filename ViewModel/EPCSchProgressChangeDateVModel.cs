using System;

namespace EQC.ViewModel
{
    public class EPCSchProgressChangeDateVModel
    {//預定進度 工程變更日期
        public DateTime? EngChangeStartDate { get; set; }
        public DateTime? ScheChangeCloseDate { get; set; }
        public string EngChangeStartDateStr
        {
            get
            {
                return this.EngChangeStartDate.HasValue ? this.EngChangeStartDate.Value.ToString("yyy-MM-dd") : "";
            }
        }
        public string ScheChangeCloseDateStr
        {
            get
            {
                return this.ScheChangeCloseDate.HasValue ? this.ScheChangeCloseDate.Value.ToString("yyy-MM-dd") : "";
            }
        }
    }
}
