
namespace EQC.Models
{
    public class PayItemModel
    {//PCCESS PayItem詳細表
        public string PayItem { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Amount { get; set; }
        public string ItemKey { get; set; }
        public string ItemNo { get; set; }
        public string RefItemCode { get; set; }
    }
}
