using EQC.Models;

namespace EQC.ViewModel
{
    public class EPCReportExtensionVModel : SupDailyReportExtensionModel
    {//監造(施工)日誌_設定展延工期
        public string ApprovalDateStr { 
            get {
                return this.ApprovalDate.ToString("yyyy-MM-dd");
            }
        }
        public int? SupDailyReportExtensionSeq { get; set; }
    }
}
