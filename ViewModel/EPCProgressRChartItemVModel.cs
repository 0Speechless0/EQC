using System;

namespace EQC.ViewModel
{
    public class EPCProgressRChartItemVModel
    {//Chart item
        public DateTime ItemDate { get; set; }
        public string itemName { get; set; }
        public decimal quantity { get; set; }
        public string ItemDateStr
        {
            get
            {
                return this.ItemDate.ToString("yyyy-M-d");
            }
        }
    }
}
