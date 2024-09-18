using EQC.Common;
using EQC.Models;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class ESTempController : Controller
    {
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            //ViewBag.Title = "監造計畫維護";
            return View("ESTemp");
        }
        private string GetTemplateFilePath()
        {
            return Utils.GetTemplateFilePath();
        }
        //範本清單
        public JsonResult GetList()
        {
            List<ESTempModel> result = new List<ESTempModel>();
            ESTempService service = new ESTempService();
            result = service.ListAll();
            return Json(result);
        }
        //上傳
        public JsonResult Upload(int seq)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                try
                {
                    ESTempService service = new ESTempService();
                    List<ESTempModel> items = service.GetItemFileInfoBySeq(seq);
                    if (items.Count == 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "資料錯誤無法上傳檔案"
                        });
                    }
                    string uniqueFileName = SaveFile(file, items[0], "督導統計-");
                    if (uniqueFileName != null)
                    {
                        string originFileName = file.FileName.ToString().Trim();
                        if (service.UpdateUploadFile(seq, originFileName, uniqueFileName) == 0)
                        {
                            return Json(new
                            {
                                result = -1,
                                message = "上傳檔案失敗"
                            });
                        }
                        else
                        {
                            items = service.GetItemBySeq(seq);
                            return Json(new
                            {
                                result = 0,
                                message = "資料已上傳成功",
                                item = Json(items[0])
                            });

                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                }
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }
        //下載
        public ActionResult Download(int seq)
        {
            ESTempService service = new ESTempService();
            List<ESTempModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].UniqueFileName == null || items[0].UniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            return DownloadFile(items[0]);
        }
        //儲存上傳檔案
        private string SaveFile(HttpPostedFileBase file, ESTempModel m, string fileHeader)
        {
            try
            {
                string filePath = GetTemplateFilePath();
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                //刪除已儲存原始檔案
                string uniqueFileName = m.UniqueFileName;
                if (uniqueFileName != null && uniqueFileName.Length > 0)
                {
                    System.IO.File.Delete(Path.Combine(filePath, uniqueFileName));
                }

                string originFileName = file.FileName.ToString().Trim();
                int inx = originFileName.LastIndexOf(".");
                uniqueFileName = String.Format("{0}{1}{2}", fileHeader, m.Name, originFileName.Substring(inx));
                string fullPath = Path.Combine(filePath, uniqueFileName);
                file.SaveAs(fullPath);

                return uniqueFileName;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                return null;
            }
        }
        //下載範例檔
        private ActionResult DownloadFile(ESTempModel m)
        {
            string filePath = GetTemplateFilePath();

            string uniqueFileName = m.UniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", m.OriginFileName);
                }
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }

    }
}