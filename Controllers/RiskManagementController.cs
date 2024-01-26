using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.EDMXModel;
using EQC.Services;
using EQC.ViewModel;
using EQC.Common;
using System.IO;

namespace EQC.Controllers
{
    public class RiskManagementController : Controller
    {
        RiskManagementService service;
        EngRiskFrontManagementService _service;
        public RiskManagementController()
        {
            service = new RiskManagementService();
            _service = new EngRiskFrontManagementService();
        }
        // GET: RiskManagement
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
        public JsonResult importDestruction(
            int subProjectSeq, 
            List<GoJsModel> nodeData, 
            List<GoJsLinkModel> linkData,
            bool normal = false
        )
        {

            var diagramStr = service.importDestruction(subProjectSeq, nodeData, linkData, normal);
            return Json(new { diagramStr = diagramStr });
        }

        public JsonResult getEngRiskSubProject(int page, int perPage)
        {
            var list =
                service
                .getRiskSubProjectList();
            var listPagination = list
                .getPagination(page, perPage);


            return Json(new { 
                list = listPagination,
                pageCount = list.getPageCount(perPage),

            });
        }

        public JsonResult updateEngRiskSubProjectDetail(List<EngRiskFrontSubProjectDetailTp> list, EngRiskFrontSubProjectListTp main)
        {
            
            if(list != null) service.updateEngRiskSubProjectDetail(list);
            service.updateEngRiskSubProject(main);
            return Json(true);
        }
        public JsonResult getEngRiskSubProjectDetail(int subProjectSeq)
        {
            var hazardTypeList = service.getEngRiskHazardType();
            var projectDetail = service.getRiskSubProjectDetailList(subProjectSeq);
            return Json(new { list = projectDetail, hazardTypeList = hazardTypeList });
        }
        public JsonResult updateEngRiskSubProject(EngRiskFrontSubProjectListTp m)
        {

            service.updateEngRiskSubProject(m);
            return Json(true);
        }

        public JsonResult insertEngRiskSubProject(EngRiskFrontSubProjectListTp m)
        {
            var list = service.getRiskSubProjectList();
            if (list.Where(row => row.ExcelNo == m.ExcelNo).Count() > 0 )
                return Json(-1);
            service.insertEngRiskSubProject(m);
            return Json(true);
        }

        public JsonResult deleteEngRiskSubProject(int subProjectSeq)
        {
            service.deleteEngRiskSubProject(subProjectSeq);
            return Json(true);
        }

        public JsonResult UploadGeneratedImg(HttpPostedFileBase file)
        {
            try
            {

                    $@"RiskManagement\Tp".UploadFileToFolder(file);

                return Json(true);
            }
            catch(Exception e)
            {
                return Json(false);
            }

        }
        public void DownloadTp()
        {
            using(var ms = new MemoryStream())
            {
                var targetFile = @"RiskManagement\DocumentTp".GetFiles()
                    .Select(file => new FileInfo(file))
                    .OrderByDescending(file => file.LastWriteTime)
                    .FirstOrDefault();
                using(var fs = new FileStream(targetFile.FullName, FileMode.Open) )
                {
                    fs.CopyTo(ms);
                }

                Response.AddHeader("Content-Disposition", $"attachment; filename={targetFile.Name}");
                Response.BinaryWrite(ms.ToArray());
            }
        }

        public JsonResult GetTp()
        {
            var targetFile = @"RiskManagement\DocumentTp".GetFiles()
                .Select(file => new FileInfo(file))
                .OrderByDescending(file => file.LastWriteTime)
                .FirstOrDefault();

            return Json(new {
                Name = targetFile?.Name ?? "施工風險評估",
                ModifyTime = Utils.ChsDate(targetFile?.LastWriteTime)
            }); ;
        }
        public JsonResult UploadTp(HttpPostedFileBase file)
        {
            try
            {
                @"RiskManagement\DocumentTp".UploadFileToFolder(file);
                return Json(new { result = 0, modifyDate = Utils.ChsDate(DateTime.Now) });
            }
            catch(Exception e)
            {
                return Json(new { result = 1 });
            }

        }
    }
   
}