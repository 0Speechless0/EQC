using EQC.Models;

namespace EQC.ViewModel
{
    public class EPCAskPaymentHeaderVModel : AskPaymentHeaderModel
    {//估驗請款 日期清單
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
