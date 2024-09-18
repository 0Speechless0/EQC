using EQC.Models;

namespace EQC.ViewModel
{
    public class EPCSupDailyDate1VModel : EPCSupDailyDateVModel
    {//日誌日期
        public int dailyCount { get; set; }

        public int OrderNo { get; set; }
        
        public EPCDailyVModel daily { get; set; }

        public EPCDailySVModel daily2 { get; set; }
    }
}
