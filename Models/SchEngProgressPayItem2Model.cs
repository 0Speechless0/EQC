using System.Collections.Generic;

namespace EQC.Models
{
    public class SchEngProgressPayItem2Model : SchEngProgressPayItemModel
    {//工程前置作業-PayItem
        public decimal? KgCo2e { get; set; }
        public decimal? ItemKgCo2e { get; set; }
        public string RStatus { get; set; }
        public int RStatusCode { get; set; }
        public int OrderNo { get; set; }
        public int? GreenFundingSeq { get; set; }
        public string GreenFundingMemo { get; set; }
        public List<SchEngProgressWorkItemModel> workItems;
    }
}
