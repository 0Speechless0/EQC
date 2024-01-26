using EQC.Common;

namespace EQC.Models
{
    public class QualityProjectListVModel : QualityProjectListModel
    {//品質計畫書範本
        public bool edit { get; set; }
        public string showModifyTime
        {
            get
            {
                return Utils.EngDateTime(this.ModifyTime);
            }
        }
        public string showRevisionDate
        {
            get
            {
                return Utils.EngDateTime(this.RevisionDate);
            }
        }
    }
}
