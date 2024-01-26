using EQC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Common;
using EQC.EDMXModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EQC.Controllers
{
    public class SignManagementController : MyController
    {
        // GET: SignMangement

        SignManagementService service;

        public SignManagementController()
        {
            service = new SignManagementService();
        }
        public ActionResult Index()
        {
            ViewBag.Title = "簽核管理";
            object data;
            return View();
        }

        public void getFlowList(string formCode)
        {

            var Flowlist = service.getFormFlowList(formCode);

            ResponseJson(new
            {
                Flowlist = Flowlist
            });

        }
        public JsonResult getFormList(int page, int perPage, string search)
        {
            var list = service
                .getFormList();
            var approverUnitTypes = service.GetApprovingUnitTypes();
            var typeUnitLst = JsonConvert.SerializeObject(service.getTypeUnitList() );
            var userService = new UserService();
            var positionList = userService.GetPositionList();
            return Json(new
            {
                FormList = list.Where(row => row.FormCode == search || row.FormName == search || search == "")
                    .ToList()
                    .getPagination(page, perPage),
                count = list.Count,
                ApprovingUnitList = approverUnitTypes,
                pageCount = list.Count % perPage == 0 ? list.Count/perPage : list.Count / perPage + 1,
                typeUnitList = typeUnitLst,
                positionList =positionList
            });
        }
        
        public JsonResult insertForm(ApprovalModuleList m)
        {
           
            return Json(service.insertForm(m) );
        }
        public JsonResult deleteForm(string formCode)
        {
            service.deleteForm(formCode);
            return Json(true);
        }

        public JsonResult updateFormName(string formCode, string formName)
        {
            service.updateFormName(formCode, formName);
            return Json(true);
        }

        public JsonResult syncFlowList(List<ApprovalModuleList> list, string flowPositions )
        {
            var flowPositionArr = JsonConvert.DeserializeObject<int[][]>(flowPositions);
            if (list != null) service.syncFormFlowList(list, flowPositionArr);
            return Json(true);
        }
    }
}
