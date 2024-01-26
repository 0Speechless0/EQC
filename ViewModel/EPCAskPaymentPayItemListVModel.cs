using EQC.Models;
using System.Collections.Generic;

namespace EQC.ViewModel
{
    public class EPCAskPaymentPayItemListVModel
    {//估驗請款 清單
        public EPCAskPaymentPayItemVModel mainItem;
        public List<EPCAskPaymentPayItemVModel> subItems = new List<EPCAskPaymentPayItemVModel>();
    }
}
