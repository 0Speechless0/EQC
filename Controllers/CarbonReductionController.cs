using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Common;
using EQC.Services;
using EQC.EDMXModel;
using EQC.ViewModel.Interface;

namespace EQC.Controllers
{
    [SessionFilter]
    public class CarbonReductionController : MyController
    {
        // GET: CarbonReduction
        ExcelImportService excelService;

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ViewIndex()
        {
            return View();
        }
        public ActionResult ViewIndex2()
        {
            return View();
        }
        public void Download()
        {
            using(var stream = new FileStream(Path.Combine(Utils.GetTemplateFilePath(), @"施工減碳係數0830.xlsx"), FileMode.Open))
            {
                using(var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    Response.AddHeader("Content-Disposition", $"attachment; filename=施工減碳係數範例.xlsx");
                    Response.BinaryWrite(ms.ToArray());

                }
            }
        }
        public JsonResult Upload(HttpPostedFileBase file)
        {
            excelService = new ExcelImportService();
            var ms = new MemoryStream();
            file.InputStream.CopyTo(ms);
            var NavvyProcess = new ExcelProcesser(ms, 4, 0);
            NavvyProcess.createDbColumnMapToIndex(new object[,]{
                {"Type1", 1},
                {"Type2", 2},
                {"Description", 3},
                {"Unit", 4},
                {"PC120", 5},
                {"PC200", 6},
                {"PC300", 7},
                {"PC400", 7},
                {"Code", 9},

            });
            file.InputStream.Position = 0;
            ms = new MemoryStream();
            file.InputStream.CopyTo(ms);
            var TruckProcess = new ExcelProcesser(ms, 4, 1);

            TruckProcess.createDbColumnMapToIndex(new object[,]{
                {"Type1", 1},
                {"Type2", 2},
                {"Description", 3},
                {"Unit", 4},
                {"[15T]", 5},
                {"[21T]", 6},
                {"[35T]", 7},
                {"Code", 9},

            });
            file.InputStream.Position = 0;
            ms = new MemoryStream();
            file.InputStream.CopyTo(ms);
            var Process = new ExcelProcesser(ms, 3, 2);

            Process.createDbColumnMapToIndex(new object[,]{
                {"Type1", 1},
                {"Type2", 2},
                {"Description", 3},
                {"Unit", 4},
                {"KgCo2e", 5},
                {"Code", 9},

            });
            excelService.updateOrCreateFromExcel(NavvyProcess, "CarbonReductionNavvyFactor", "Code");
            excelService.updateOrCreateFromExcel(TruckProcess, "CarbonReductionTruckFactor", "Code");
            excelService.updateOrCreateFromExcel(Process, "CarbonReductionFactor", "Description");
            return Json(new { result = 0 });

        }

        // type 
        public JsonResult GetList(int pageRecordCount = 0, string keyWord = "", int pageIndex = -1, int type =0)
        {

            var service = GetService(type);
            var list = service.GetList()
                    .OfType<CarbonReduction>()
                    .Where(row => row.Code?.Contains(keyWord) ?? false)
                    .ToList();
            List<CarbonReduction> pagination = null;
            if(pageIndex > 0)
            {
                pagination =
                    list
                        .getPagination(pageIndex, pageRecordCount);
            }

            return Json(new
            {
                result = 0,
                pTotal = list.Count(),
                items = pageIndex > 0 ?  pagination : list
            });

        }

        public JsonResult Update(
            CarbonReductionNavvyFactor typeM1,
            CarbonReductionTruckFactor typeM2,
            CarbonReductionFactor typeM3, int type =0)
        {
            var service = GetService(type);
            if(type == 0 )service.Update(typeM1.Seq, typeM1);
            if(type == 1 )service.Update(typeM2.Seq, typeM2);
            if(type == 2 )service.Update(typeM3.Seq, typeM3);
            return Json(new
            {
                result = 0,

            });
        }

        public JsonResult Insert(
            CarbonReductionNavvyFactor typeM1,
            CarbonReductionTruckFactor typeM2,
            CarbonReductionFactor typeM3, int type = 0)
        {
            var service = GetService(type);
            int insertSeq =  0;
            if (type == 0) insertSeq =service.Insert(typeM1);
            if (type == 1) insertSeq = service.Insert(typeM2);
            if (type == 2) insertSeq = service.Insert(typeM3);
            return Json(new
            {
                result = 0,
                inserSeq = insertSeq
            });
        }

        public JsonResult Delete(int id, int type = 0)
        {
            var service = GetService(type);
            service.Delete(id);
            return Json(new
            {
                result = 0,

            });
        }

        private WebBaseControlService GetService(int type)
        {
            switch(type)
            {
                case 0: return new WebBaseControlService(db => db.CarbonReductionNavvyFactor);
                case 1: return new WebBaseControlService(db => db.CarbonReductionTruckFactor);
                case 2: return new WebBaseControlService(db => db.CarbonReductionFactor);
                default: return null;
            }
        }
    }
}