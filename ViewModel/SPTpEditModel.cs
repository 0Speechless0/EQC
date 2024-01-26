
using EQC.Common;

namespace EQC.Models
{
    public class SPTpEditModel: SupervisionProjectTpModel
    {//監造計畫書範本
        public string modifyDate
        {
            get
            {
                return Utils.ChsDate(this.RevisionDate);
            }
        }
    }
}
