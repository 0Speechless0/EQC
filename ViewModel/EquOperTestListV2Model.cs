using EQC.Models;
using EQC.Services;
using System.Collections.Generic;

namespace EQC.ViewModel
{
    public class EquOperTestListV2Model : EquOperTestListModel
    {//設備運轉測試清單
        public List<EquOperControlStModel> controlItems;
        public List<FlowChartTpDiagramModel> flowChartItems;
        public void GetQCStd()
        {
            controlItems = new EquOperControlStService().GetListAll<EquOperControlStModel>(Seq);
        }
        public void GetFlowChart()
        {
            flowChartItems = new FlowChartDiagramService().GetFlowChartDiagram<FlowChartTpDiagramModel>(Seq, "Chapter6");
        }
    }
}
