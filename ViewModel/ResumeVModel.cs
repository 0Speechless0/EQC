using EQC.Common;

namespace EQC.Models
{
    public class ResumeVModel:ResumeModel
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