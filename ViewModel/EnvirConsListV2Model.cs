using EQC.Models;
using EQC.Services;
using System.Collections.Generic;

namespace EQC.ViewModel
{
    public class EnvirConsListV2Model : EnvirConsListModel
    {//環境保育清單範本
        public List<EnvirConsControlStModel> controlItems;
        public List<FlowChartTpDiagramModel> flowChartItems;
        public void GetQCStd()
        {
            controlItems = new EnvirConsControlStService().GetListAll<EnvirConsControlStModel>(Seq);
        }
        public void GetFlowChart()
        {
            flowChartItems = new FlowChartDiagramService().GetFlowChartDiagram<FlowChartTpDiagramModel>(Seq, "Chapter702");
        }
    }
}
