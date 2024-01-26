using EQC.Common;
using EQC.Models;

namespace EQC.ViewModel
{
    public class EQMConstRiskEvalVModel : ConstRiskEvalModel
    {//工程標案清單
        public string ApprovedDateStr
        {
            get
            {
                return this.ApprovedDate.ToString("yyyy-MM-dd");
            }
        }
    }
}
