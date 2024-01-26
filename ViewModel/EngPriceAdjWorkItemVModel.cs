using EQC.Models;
using Newtonsoft.Json;

namespace EQC.ViewModel
{
    public class EngPriceAdjWorkItemVModel: EngPriceAdjWorkItemModel
    {//工程物價調整款.WorkItem
        public int Kind { get; set; }
        public string PayItem { get; set; }
        public string Description { get; set; }
        public string ItemCode { get; set; }
        public decimal Amount { get; set; }
        public decimal Weights { get; set; }
        public decimal WorkAmount { get; set; }
        public decimal WeightsAmount { get; set; }
        public int PriceIndexKindId { get; set; }
    }
}
