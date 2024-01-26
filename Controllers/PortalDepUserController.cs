using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class PortalDepUserController : Controller
    {//各局首頁
        PortalDepUserService iService = new PortalDepUserService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
        //長官需要知道的事
        public JsonResult GetImportantEventSta()
        {
            List<EADEngStaVModel> engItems = iService.GetImportantEventSta<EADEngStaVModel>();
            return Json(new
            {
                items = engItems
            });
        }
        public JsonResult GetImportantEventList(int mode)
        {
            List<EADPrjXMLVModel> engItems = iService.GetImportantEventList<EADPrjXMLVModel>(mode);
            var roleSeq = new SessionManager().GetUser().RoleSeq;
            return Json(new
            {
                items = engItems,
                roleSeq = roleSeq
            });
        }

        //工程狀態統計
        public JsonResult GetEngStateSta()
        {
            List<EADEngStaVModel> engItems = iService.GetEngStateSta<EADEngStaVModel>();
            return Json(new
            {
                items = engItems
            });
        }
        public JsonResult GetEngStateList(int mode)
        {
            List<EADPrjXMLVModel> engItems = iService.GetEngStateList<EADPrjXMLVModel>(mode);
            return Json(new
            {
                items = engItems
            });
        }

        //工程標案清單
        public virtual JsonResult GetStateList()
        {
            List<SelectOptionModel> engList = iService.GetEngCreatedListState<SelectOptionModel>();
            return Json(new
            {
                items = engList
            });
        }
        public virtual JsonResult GetList(int pageRecordCount, int pageIndex, string state, string engKeyword)
        {
            if (string.IsNullOrEmpty(engKeyword))
                engKeyword = "";
            else
                engKeyword = string.Format("%{0}%", engKeyword);

            List<EADPrjXML2VModel> engList = new List<EADPrjXML2VModel>();
            int total = iService.GetEngCreatedListCount(state, engKeyword);
            if (total > 0)
            {
                engList = iService.GetEngCreatedList<EADPrjXML2VModel>(pageRecordCount, pageIndex, state, engKeyword);
            }
            return Json(new
            {
                pTotal = total,
                items = engList
            });
        }
    }
}