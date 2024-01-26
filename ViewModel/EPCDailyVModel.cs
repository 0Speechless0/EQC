
using EQC.Models;
using System.Collections.Generic;

namespace EQC.ViewModel
{
    public class EPCDailyVModel
    {//施工日誌
        public List<SupDailyReportMiscConstructionModel> miscList;
        public List<EPCSupPlanOverviewVModel> planOverviewList;
        public List<SupDailyReportConstructionMaterialModel> materialList;
        public List<SupDailyReportConstructionPersonModel> personList;
        public List<SupDailyReportConstructionEquipmentModel> equipmentList;
    }
}
