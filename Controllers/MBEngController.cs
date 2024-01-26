using EQC.Services;
using System.Collections.Generic;
using EQC.ViewModel;
using System.Web.Mvc;
using System;
using System.Web;
using System.Web.Routing;

namespace EQC.Controllers
{
    public class MBEngController : Controller
    {
        private string baseLayout = "_LayoutMobile";

        public ActionResult Index()
        {            
            ViewBag.account = Request.QueryString["account"];
            ViewBag.mobile = Request.QueryString["mobile"];
            ViewBag.Title = "抽驗記錄填報";
            return View("index", baseLayout);
        }

        public JsonResult checkUser(string account, string mobile)
        {
            MBEngService service = new MBEngService();
            return Json(service.checkUser(account, mobile));
        }

        public JsonResult getEngMain(string mobile)
        {
            MBEngService service = new MBEngService();
            return Json(service.getEngMain(mobile)) ;
        }

        public JsonResult getEngConstruction(int engMainSeq)
        {
            MBEngService service = new MBEngService();
            return Json(service.getEngConstruction(engMainSeq));
        }

        public JsonResult getConstCheckList(int engMainSeq)
        {
            MBEngService service = new MBEngService();
            return Json(service.getConstCheckList(engMainSeq));
        }

        public JsonResult getCCManageItem(int constCheckListSeq, int ccFlow)
        {
            MBEngService service = new MBEngService();
            return Json(service.getCCManageItem(constCheckListSeq, ccFlow));
        }

        public JsonResult drawCCManageItem(int constCheckListSeq, int ccFlow, string ccManageItem1, string ccManageItem2)
        {
            MBEngService service = new MBEngService();
            return Json(service.drawCCManageItem(constCheckListSeq, ccFlow, ccManageItem1, ccManageItem2));
        }

        public JsonResult getEquOperTestList(int engMainSeq)
        {
            MBEngService service = new MBEngService();
            return Json(service.getEquOperTestList(engMainSeq));
        }

        public JsonResult getEPCheckItem(int constCheckListSeq)
        {
            MBEngService service = new MBEngService();
            return Json(service.getEPCheckItem(constCheckListSeq));
        }

        public JsonResult drawEPCheckItem(int constCheckListSeq, string ccManageItem1, string ccManageItem2)
        {
            MBEngService service = new MBEngService();
            return Json(service.drawEPCheckItem(constCheckListSeq, ccManageItem1, ccManageItem2));
        }

        public JsonResult getOccuSafeHealthList(int engMainSeq)
        {
            MBEngService service = new MBEngService();
            return Json(service.getOccuSafeHealthList(engMainSeq));
        }

        public JsonResult getOSCheckItem(int constCheckListSeq)
        {
            MBEngService service = new MBEngService();
            return Json(service.getOSCheckItem(constCheckListSeq));
        }

        public JsonResult drawOSCheckItem(int constCheckListSeq, string ccManageItem1, string ccManageItem2)
        {
            MBEngService service = new MBEngService();
            return Json(service.drawOSCheckItem(constCheckListSeq, ccManageItem1, ccManageItem2));
        }

        public JsonResult getEnvirConsList(int engMainSeq)
        {
            MBEngService service = new MBEngService();
            return Json(service.getEnvirConsList(engMainSeq));
        }

        public JsonResult getECCCheckItem(int constCheckListSeq, int ccFlow)
        {
            MBEngService service = new MBEngService();
            return Json(service.getECCCheckItem(constCheckListSeq, ccFlow));
        }

        public JsonResult drawECCCheckItem(int constCheckListSeq, int ccFlow, string ccManageItem1, string ccManageItem2)
        {
            MBEngService service = new MBEngService();
            return Json(service.drawECCCheckItem(constCheckListSeq, ccFlow, ccManageItem1, ccManageItem2));
        }
    }
}