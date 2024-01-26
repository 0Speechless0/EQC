using EQC.Models;
using EQC.Services;
using System.Collections.Generic;

namespace EQC.ViewModel
{
    public class ConstCheckListV2Model : ConstCheckListModel
    {//施工抽查清單
        public List<ConstCheckControlStModel> controlItems;
        public List<FlowChartTpDiagramModel> flowChartItems;
        public void GetQCStd()
        {
            controlItems = new ConstCheckControlStService().GetListAll<ConstCheckControlStModel>(Seq);
        }
        public void GetFlowChart()
        {
            flowChartItems = new FlowChartDiagramService().GetFlowChartDiagram<FlowChartTpDiagramModel>(Seq, "Chapter701");
        }
    }
}
