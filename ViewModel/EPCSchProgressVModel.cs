using System;

namespace EQC.ViewModel
{
    public class EPCSchProgressVModel
    {//預定進度 日期清單
        public int Version { get; set; } //s20230411
        public DateTime SPDate { get; set; }
        public string SPDateStr
        {
            get
            {
                return this.SPDate.ToString("MM月dd日");
            }
        }
        public string ItemDate
        {
            get
            {
                return this.SPDate.ToString("yyyy/M/d");
            }
        }
    }
}
