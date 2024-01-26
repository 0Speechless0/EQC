using EQC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class APIController : MSamplingInspectionRecController
    {
        // GET: API
        public override ActionResult Index()
        {
            //ViewBag.Title = "抽驗紀錄填報";
            return View();
        }

        public virtual ActionResult API_Post_Test()
        {
            return View();
        }
    }
}