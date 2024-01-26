using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Common;
using EQC.EDMXModel;
namespace EQC.Controllers
{
    [SessionFilter]
    public class CarbonEmissionViewController : Controller
    {
        // GET: CarbonEmissionView
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index2()
        {
            return View("Index2");
        }
        public JsonResult getCarbonEmissionCodeOption(string prefix = "")
        {
            using (var context = new EQC_NEW_Entities())
            {
                var options = context.CarbonEmissionFactor
                    .Where(row => row.Code.StartsWith(prefix) )
                    .GroupBy(row => row.Code.Substring(0, prefix.Length + 1))
                    .Select(row => row.Key)
                    .ToList();
                return Json(options);
            }


        }

        public JsonResult getCarbonEmissionFactor(string prefix)
        {
            using (var context = new EQC_NEW_Entities())
            {
                var list = context.CarbonEmissionFactor
                    .Where(row => row.Code.StartsWith(prefix))
                    .ToList();
                return Json(list);
            }

        }
    }
}