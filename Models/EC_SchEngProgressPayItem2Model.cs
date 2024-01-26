using System.Collections.Generic;

namespace EQC.Models
{
    public class EC_SchEngProgressPayItem2Model : EC_SchEngProgressPayItemModel
    {//工程變更-PayItem
        public decimal? KgCo2e { get; set; }
        public decimal? ItemKgCo2e { get; set; }
        public string RStatus { get; set; }
        public int RStatusCode { get; set; }
        public List<EC_SchEngProgressWorkItemModel> workItems;
    }
}
