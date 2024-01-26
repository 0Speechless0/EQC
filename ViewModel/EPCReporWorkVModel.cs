using EQC.Models;

namespace EQC.ViewModel
{
    public class EPCReportWorkVModel : SupDailyReportWorkModel
    {//監造(施工)日誌_停復工
        public string SStopWorkDateStr
        {
            get
            {
                return this.SStopWorkDate.ToString("yyyy-MM-dd");
            }
        }
        public string EStopWorkDateStr
        {
            get
            {
                return this.EStopWorkDate.ToString("yyyy-MM-dd");
            }
        }
        public string BackWorkDateStr
        {
            get
            {
                if (this.BackWorkDate.HasValue)
                    return this.BackWorkDate.Value.ToString("yyyy-MM-dd");
                else
                    return "";
            }
        }

        public int? EC_SchEngProgressHeaderSeq { get; set; } //s20230526
    }
}
