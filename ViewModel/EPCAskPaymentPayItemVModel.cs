using EQC.Models;

namespace EQC.ViewModel
{
    public class EPCAskPaymentPayItemVModel : AskPaymentPayItemModel
    {//估驗請款 清單
        public int ItemType { get; set; }
        public decimal subTotalAmount { get; set; }
        public int subCount { get; set; }
        public int level { get; set; }
        public decimal PreviousSchProgress { get; set; }
        public decimal PreviousAccuQuantity { get; set; }
        public decimal PreviousAccuAmount { get; set; }
    }
}
