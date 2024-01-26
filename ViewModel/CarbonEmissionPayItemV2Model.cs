using EQC.Models;
using System.Collections.Generic;

namespace EQC.ViewModel
{
    public class CarbonEmissionPayItemV2Model : CarbonEmissionPayItemModel
    {//碳排量計算
        public int WorkItemCnt { get; set; }
        public List<CarbonEmissionWorkItemModel> workItems;
    }
}
