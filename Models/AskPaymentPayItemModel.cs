
namespace EQC.Models
{
    public class AskPaymentPayItemModel : PayItemModel
    {//估驗請款明細
        public int Seq { get; set; }
        public int AskPaymentHeaderSeq { get; set; }
        public int OrderNo { get; set; }
        public int ItemType { get; set; }
        public decimal AccuQuantity { get; set; }
        public decimal AccuAmount { get; set; }
        public decimal SchProgress { get; set; }
        public string Memo { get; set; }
    }
}
