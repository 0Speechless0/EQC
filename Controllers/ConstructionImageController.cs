using EQC.Common;
using EQC.Models;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class ConstructionImageController : Controller
    {
        ConstructionImageService service = new ConstructionImageService();
        // GET: ConstructionImage
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("ConstructionImage");
        }

        public JsonResult getEngData(string startYear, string endYear)
        {
            try
            {
                List<object> result = service.getEngData(startYear, endYear);
                return Json(new { status = "success", data = result }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult getEngYearList()
        {
            try
            {
                List<string> result = service.getEngYearList();
                return Json(new { status = "success", data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult updateContent(ConstCheckRecFileModel item )
        {
            try
            {
                service.updateContent(item);
                return Json(new { status = "success" });
            }
            catch (Exception e)
            {
                return Json(new { status = "failed" });
            }
        }
        public JsonResult delete(int id, string UniqueFileName)
        {
            try
            {
                service.delete(UniqueFileName);
                string filePath = Utils.GetEngMainFolder(id);

                string uniqueFileName = UniqueFileName;
                if (uniqueFileName != null && uniqueFileName.Length > 0)
                {
                    string fullPath = Path.Combine(filePath, uniqueFileName);
                    if (System.IO.File.Exists(fullPath))
                    {
                       
                            System.IO.File.Delete(fullPath);
                        

                    }
                }
                return Json(new { status = "success" });
            }
            catch (Exception e)
            {
                return Json(new { status = "failed" });
            }
        }
    }
}