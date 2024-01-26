using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.EDMXModel;
using EQC.Services;
using EQC.ViewModel;
using EQC.Common;
using EQC.Models;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EngRiskFrontManagementController : Controller
    {
        EngRiskFrontManagementService service;

        public EngRiskFrontManagementController()
        {
            service = new EngRiskFrontManagementService();
        }
        public ActionResult destruct()
        {
            Utils.setUserClass(this);
            ViewBag.Title = "施工風險分項工程拆解";
            ViewBag.hasCard = false;
            return View("Index");
        }
        public ActionResult lookup()
        {
            Utils.setUserClass(this);
            ViewBag.Title = "施工風險評估";
            ViewBag.hasCard = false;
            return View("Index");
        }
        public JsonResult importDestruction(int subProjectSeq,List<GoJsModel> nodeData,List<GoJsLinkModel> linkData, bool normal = false)
        {

 
            var diagramStr = service.importDestruction(subProjectSeq, nodeData, linkData, normal);
            return Json(new { diagramStr = diagramStr });
        }
        public JsonResult UploadGeneratedImg(HttpPostedFileBase file,int engRiskSeq)
        {
            try
            {
                $@"EngRiskFront\{engRiskSeq}".UploadFileToFolder(file);
                return Json(true);
            }
            catch (Exception e)
            {
                return Json(false);
            }
        }

        public JsonResult updateEngRiskSubProjectDetail(List<EngRiskFrontSubProjectDetail> list, EngRiskFrontSubProjectList main)
        {

            if (list != null) service.updateEngRiskSubProjectDetail(list);
            service.updateEngRiskSubProject(main);
            return Json(true);
        }
        public JsonResult getEngRiskSubProjectDetail(int subProjectSeq)
        {
            var hazardTypeList = service.getEngRiskHazardType();
            var projectDetail = service.getRiskSubProjectDetailList(subProjectSeq);
            return Json(new { list = projectDetail, hazardTypeList = hazardTypeList });
        }

        public JsonResult getEngRiskSubProject(int page, int perPage)
        {
            var list =
                service
                .getRiskSubProjectList();
            var listPagination = list
                .getPagination(page, perPage);


            return Json(new
            {
                list = listPagination,
                pageCount = list.getPageCount(perPage),

            });
        }

        public JsonResult insertEngRiskSubProject(EngRiskFrontSubProjectList m)
        {
            var list = new EngRiskFrontService().GetEngRiskFrontSubProjectList<EngRiskFrontSubProjectListModel>(Convert.ToInt16(m.EngRiskFrontSeq));
            //var list = service.getRiskSubProjectList();
            if (list.Where(row => row.ExcelNo == m.ExcelNo).Count() > 0)
                return Json(-1);
            service.insertEngRiskSubProject(m);
            return Json(true);
        }

        public JsonResult updateEngRiskSubProject(EngRiskFrontSubProjectList m)
        {

            service.updateEngRiskSubProject(m);
            return Json(true);
        }

        public JsonResult deleteEngRiskSubProject(int subProjectSeq)
        {
            service.deleteEngRiskSubProject(subProjectSeq);
            return Json(true);
        }
    }
}