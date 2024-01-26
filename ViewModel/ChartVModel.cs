using System.Collections.Generic;

namespace EQC.ViewModel
{
    public class ChartVModel
    {//chart
        public List<string> categories = new List<string>();
        public List<string> seriesName = new List<string>();
        public List<ChartSeriesVModel> series = new List<ChartSeriesVModel>();
    }
}
