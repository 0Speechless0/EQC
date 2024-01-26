using EQC.Common;

namespace EQC.Models
{
    public class EngMainVModel : EngMainModel
    {//工程主檔
        public string ExecUnit { get; set; }
        public string ExecSubUnit { get; set; }
        public int? DocState { get; set; }

        public string showApproveDate {
            get
            {
                return Utils.ChsDate(this.ApproveDate);
            }
        }
        //主辦機關
        public string OrganizerUnit { get; set; }
        //材料數
        public int EMDCount { get; set; }
        //審核進度
        public string AuditProgress { get; set; }
        public string PccesXMLDateStr
        {
            get
            {

                return this.PccesXMLDate.HasValue ? Utils.ChsDate(this.PccesXMLDate) : "";
            }
        }
        
    }
}
