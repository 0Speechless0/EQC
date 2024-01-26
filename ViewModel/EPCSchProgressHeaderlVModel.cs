using EQC.Common;
using EQC.Models;

namespace EQC.ViewModel
{
    public class EPCSchProgressHeaderVModel : SchProgressHeaderModel
    {//預定進度主檔
        public string PccesXMLDateStr
        {
            get
            {
                return PccesXMLDate.HasValue ? Utils.ChsDate(PccesXMLDate.Value) : "";
            }
        }
    }
}
