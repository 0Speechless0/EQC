using System.Collections.Generic;

namespace EQC.ViewModel
{
    public class ECAskPaymentPayItemListVModel
    {//工程變更-估驗請款 清單
        public ECAskPaymentPayItemVModel mainItem;
        public List<ECAskPaymentPayItemVModel> subItems = new List<ECAskPaymentPayItemVModel>();
    }
}
