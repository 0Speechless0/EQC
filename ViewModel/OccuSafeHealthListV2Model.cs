using EQC.Services;
using System.Collections.Generic;

namespace EQC.Models
{
    public class OccuSafeHealthListV2Model : OccuSafeHealthListModel
    {//職業安全衛生清單範本
        public List<OccuSafeHealthControlStModel> controlItems;
        public List<FlowChartTpDiagramModel> flowChartItems;
        public void GetQCStd()
        {
            controlItems = new OccuSafeHealthControlStService().GetListAll<OccuSafeHealthControlStModel>(Seq);
        }
        public void GetFlowChart()
        {
            flowChartItems = new FlowChartDiagramService().GetFlowChartDiagram<FlowChartTpDiagramModel>(Seq, "Chapter703");
        }
    }
}
