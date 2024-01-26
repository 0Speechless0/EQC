using System.Collections.Generic;

namespace EQC.Models
{
    public class CarbonEmissionPayItemInsertModel: PayItemModel
    {//碳排量計算
        public int Seq { get; set; }
        public int RStatusCode { get; set; }

        public List<CarbonEmissionWorkItemModel> workItems = new List<CarbonEmissionWorkItemModel>();

    }
}
