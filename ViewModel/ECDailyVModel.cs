
using EQC.Models;
using System.Collections.Generic;

namespace EQC.ViewModel
{
    public class ECDailyVModel
    {//工程變更-施工日誌
        public List<EC_SupDailyReportMiscConstructionModel> miscList;
        public List<ECSupPlanOverviewVModel> planOverviewList;
        public List<EC_SupDailyReportConstructionMaterialModel> materialList;
        public List<EC_SupDailyReportConstructionPersonModel> personList;
        public List<EC_SupDailyReportConstructionEquipmentModel> equipmentList;
    }
}
