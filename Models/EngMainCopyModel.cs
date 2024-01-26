using EQC.Common;
using EQC.ViewModel;
using System;
using System.Collections.Generic;

namespace EQC.Models
{
    public class EngMainCopyModel : EngMainEditVModel
    {//s20230624 複製工程用
        public List<EngConstructionModel> engConstructionItems; //工程主要施工項目及數量 清單
        public List<EngSupervisorModel> engSupervisorItems; //自辦監造人員清單
        public List<EngAttachmentModel> engAttachmentItems; //監造計畫附件 圖/表
        public List<CarbonEmissionPayItemV2Model> carbonEmissionPayItems; //碳排量清單
        public List<EngMaterialDeviceList2VModel> engMaterialDeviceItems; //第五章 材料設備清冊
        public List<EquOperTestListV2Model> equOperTestItems; //第六章 設備功能運轉測試抽驗程序及標準
        public List<ConstCheckListV2Model> constCheckItems; //第七章 701 施工抽查程序及標準
        public List<EnvirConsListV2Model> envirConsItems; //第七章 702 環境保育抽查標準
        public List<OccuSafeHealthListV2Model> occuSafeHealthItems; //第七章 703 職業安全衛生抽查標準
    }
}
