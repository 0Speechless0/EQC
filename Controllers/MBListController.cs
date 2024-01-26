using EQC.Services;
using System.Collections.Generic;
using EQC.ViewModel;
using System.Web.Mvc;

namespace EQC.Controllers
{
    public class MBListController : Controller
    {
        private string baseLayout = "_LayoutMobile";

        public ActionResult Index()
        {
            ViewBag.account = Request.QueryString["account"];
            ViewBag.mobile = Request.QueryString["mobile"];
            ViewBag.Title = "我的抽驗清單";
            return View("index", baseLayout);
        }

        public JsonResult checkUser(string account, string mobile)
        {
            MBListService service = new MBListService();
            return Json(service.checkUser(account, mobile));
        }

        public JsonResult getEngMain(string mobile)
        {
            MBListService service = new MBListService();
            return Json(service.getEngMain(mobile));
        }

        public JsonResult getEngConstruction(int engMainSeq)
        {
            MBListService service = new MBListService();
            return Json(service.getEngConstruction(engMainSeq));
        }
    }
}