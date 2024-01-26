using EQC.Models;

namespace EQC.ViewModel
{
    public class ECAskPaymentHeaderVModel : EC_AskPaymentHeaderModel
    {//工程變更-估驗請款 日期清單
        public string APDateStr
        {
            get
            {
                return this.APDate.ToString("MM月dd日");
            }
        }
        public string ItemDate
        {
            get
            {
                return this.APDate.ToString("yyyy-MM-dd");
            }
        }
    }
}
