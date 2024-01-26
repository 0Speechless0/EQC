using DocumentFormat.OpenXml.Spreadsheet;
using EQC.ViewModel;
using System.Collections.Generic;

namespace EQC_CarbonEmissionCal
{
    public class CarbonCalInfo : ReturnMessage
    {
        public int totalRows { get; set; }
        public List<CarbonEmissionPayItemVModel> items { get; set; }
        public decimal _co2Total { get; set; }
        public decimal co2Total
        {
            get
            {
                return decimal.Round(_co2Total, 0);
            }
            set
            {
                _co2Total = value;
            }
        }
        public decimal  _co2ItemTotal { get; set; }
        public decimal  co2ItemTotal {
            get { 
                return decimal.Round(_co2ItemTotal, 0);
            }
            set
            {
                _co2ItemTotal = value;
            }

        }
        public decimal dismantlingRate(decimal totlaBudeget)
        {

            return totlaBudeget == 0 ?
                0 :  decimal.Round((co2ItemTotal * 100 / totlaBudeget), 2);
        }
    }
}
