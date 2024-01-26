using EQC.Common;
using EQC.Models;
using EQC.Services;
using System.Collections.Generic;

namespace EQC.ViewModel
{
    public class EngMaterialDeviceList2VModel : EngMaterialDeviceListModel
    {//材料設備送審清冊 s20230624
        public List<EngMaterialDeviceControlStModel> controlItems;//材料設備抽查管理標準
        public List<EngMaterialDeviceSummaryModel> engMaterialDeviceSummaryItems;//材料設備送審管制總表
        public void GetQCStd()
        {
            controlItems = new EngMaterialDeviceControlStService().GetListAll<EngMaterialDeviceControlStModel>(Seq);
        }
    }
}
