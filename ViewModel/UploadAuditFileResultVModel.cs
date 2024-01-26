using EQC.Common;
using EQC.Models;

namespace EQC.ViewModel
{
    public class UploadAuditFileResultVModel : UploadAuditFileResultModel
    {
        public int EngMainSeq { get; set; }
        public string showCreateTime
        {
            get
            {
                return Utils.ChsDate(this.CreateTime);
            }
        }
    }
}