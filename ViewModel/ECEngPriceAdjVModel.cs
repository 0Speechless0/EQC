using EQC.Models;

namespace EQC.ViewModel
{
    public class ECEngPriceAdjVModel: EC_EngPriceAdjModel
    {//工程變更-預定進度 日期清單
        public string AdjMonthStr
        {
            get
            {
                return this.AdjMonth.ToString("yyyy-M-d");
            }
        }
        public string AdjMonthStrYM
        {
            get
            {
                //return this.AdjMonth.ToString("MM月dd日");
                return this.AdjMonth.ToString("yyyy-M");
            }
        }
    }
}
