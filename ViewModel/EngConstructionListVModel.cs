using EQC.Common;

namespace EQC.Models
{
    public class EngConstructionListVModel : EngMainModel
    {//分項工程
        public string ExecUnit { get; set; }
        public string ExecSubUnit { get; set; }
        public int? DocState { get; set; }
        public string subEngNo { get; set; }//分項工程編號
        public string subEngName { get; set; }//分項工程名稱
        public int subEngNameSeq { get; set; }//EngConstruction.Seq
        public int? constCheckRecCount { get; set; }//抽查紀錄數 shioulo 20221216
        public int? missingCount { get; set; }//缺失數
        public int hasUnderReview { get; set; }//有審核中表單
        public int hasApproved { get; set; }//有同意表單

        public string showApproveDate {
            get
            {
                return Utils.ChsDate(this.ApproveDate);
            }
        }
    }
}
