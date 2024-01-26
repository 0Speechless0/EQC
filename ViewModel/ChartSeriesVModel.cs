using System.Collections.Generic;

namespace EQC.ViewModel
{
    public class ChartSeriesVModel
    {//chart
        public string name { get; set; }
        public string color { get; set; } //s20230623
        public List<decimal> data = new List<decimal>();
    }
}
