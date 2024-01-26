using EQC.Models;

namespace EQC.ViewModel
{
    public class EPCReportNoDurationVModel : SupDailyReportNoDurationModel
    {//監造(施工)日誌_設定不計工期
        public string StartDateStr
        {
            get
            {
                return this.StartDate.ToString("yyyy-MM-dd");
            }
        }
        public string EndDateStr
        {
            get
            {
                return this.EndDate.ToString("yyyy-MM-dd");
            }
        }
    }
}
