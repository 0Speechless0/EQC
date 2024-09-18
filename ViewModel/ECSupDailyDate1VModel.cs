
namespace EQC.ViewModel
{
    public class ECSupDailyDate1VModel : ECSupDailyDateVModel
    {//日誌日期
        public int dailyCount { get; set; }

        public ECDailyVModel daily { get; set; }
        public ECDailySVModel daily2 { get; set; }
        public int OrderNo { get; set; }
    }
}
