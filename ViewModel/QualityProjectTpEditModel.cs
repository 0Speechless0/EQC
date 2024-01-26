
using EQC.Common;

namespace EQC.Models
{
    public class QualityProjectEditModel : QualityProjectTpModel
    {//品質計畫書範本
        public string modifyDate
        {
            get
            {
                return Utils.ChsDate(this.RevisionDate);
            }
        }
    }
}
