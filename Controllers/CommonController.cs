using EQC.Common;
using EQC.Models;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    //[SessionFilter]
    public class CommonController : Controller
    {
       
        public CommonController()
        {
            
        }

        public JsonResult GetStageList(int stageCode)
        {
            List<Stage> list = CommonService.GetStageList(stageCode);
            var stageList = from m in list.AsEnumerable().ToList()
                            select new
                            {
                                value = m.StageCode,
                                text = m.StageName
                            };
            return Json(new
            {
                stageList = stageList
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
