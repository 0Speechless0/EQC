using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class EngReportApproveVModel: EngReportApproveModel
    {
        public string FormCode { get; set; }
        public string FormName { get; set; }
        public int? ApprovalWorkFlow { get; set; }
        public int? Approver { get; set; }
        public string ApproverName { get; set; }
        public int? ApprovingUnitSeq { get; set; }
        public string ApprovingUnitName { get; set; }
        public int? ApprovalMethod { get; set; }
        //public string ApprovalMethodName { get; set; }
        public int? EmailNotification { get; set; }
        //public string EmailNotificationName { get; set; }        
        public string ApproveUser { get; set; }
        public int ApproveState
        {
            get
            {
                return string.IsNullOrEmpty(this.Signature) ? 0 : 1;
            }
        }
        public string ApproveStateStr
        {
            get
            {
                return string.IsNullOrEmpty(this.Signature) ? "" : "完成";
            }
        }
        public string ApproveTimeStr
        {
            get
            {
                if (this.ApproveTime.HasValue)
                    return Convert.ToDateTime(this.ApproveTime).ToString("yyyy-MM-dd HH:mm");
                else
                    return string.Empty;
            }
        }
    }
}