using System;

namespace EQC.ViewModel
{
    public class EPCProgressRSchVModel
    {//預定進度
        public DateTime itemDate { get; set; }
        public decimal rate { get; set; }
        public decimal dayProgress { get; set; }
        public string ItemDateStr
        {
            get
            {
                return this.itemDate.ToString("yyyy-M-d");
            }
        }
    }
}
