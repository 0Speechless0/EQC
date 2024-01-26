using EQC.Common;
using EQC.Models;

namespace EQC.ViewModel
{
    public class EPCSchEngProgressHeaderVModel : SchEngProgressHeaderModel
    {//前置作業 - 預定進度主檔
        public string PccesXMLDateStr
        {
            get
            {
                return PccesXMLDate.HasValue ? Utils.ChsDate(PccesXMLDate.Value) : "";
            }
        }
    }
}
