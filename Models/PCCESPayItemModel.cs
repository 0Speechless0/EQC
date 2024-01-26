using System;
using System.Collections.Generic;

namespace EQC.Models
{
    public class PCCESPayItemModel : FlowChartFileModel
    {//PCCESS詳細表
        public int Seq { get; set; }
        public int PCCESSMainSeq { get; set; }

        public string PayItem { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Amount { get; set; }
        public string ItemKey { get; set; }
        public string ItemNo { get; set; }
        public string RefItemCode { get; set; }

        public List<PCCESWorkItemModel> workItems = new List<PCCESWorkItemModel>();
    }
}
