using EQC.EDMXModel;
using EQC.Models;
using EQC.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    public class TenderPlanV2Controller : MyController
    {
        // GET: TenderPlanV2

        public TenderPlanV2Controller()
        {
            //setAPIReturnSetting(new JsonSerializerSettings()
            //{
            //    PreserveReferencesHandling = PreserveReferencesHandling.None,
            //    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            //});
        }
        public void GetEngItemPayItemAdded(int id)
        {
            var engMainService = new EngMainService();
            var  item = engMainService.GetItemBySeq<EngMainEditVModel>(id).FirstOrDefault();
            var resultList = new List<object>();
            using(var context = new EQC_NEW_Entities() )
            {
                if(item != null)
                {
                    var engEntity = context.EngMain.Find(item.Seq);
                    resultList.AddRange(engEntity.EngMaterialDeviceList.Where(r => r.DataType == 1).Select(r => r.MDName));
                    resultList.AddRange(engEntity.ConstCheckList.Where(r => r.DataType == 1).Select(r => r.ItemName));
                    resultList.AddRange(engEntity.EnvirConsList.Where(r => r.DataType == 1).Select(r => r.ItemName));
                    resultList.AddRange(engEntity.OccuSafeHealthList.Where(r => r.DataType == 1).Select(r => r.ItemName));
                }
                ResponseJson(resultList);
            }
        }
    }
}