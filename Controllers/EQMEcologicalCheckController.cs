using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EQMEcologicalCheckController : Controller
    {//工程品管 - 生態檢核(設計階段)上傳
        EcologicalChecklistService iService = new EcologicalChecklistService();
        public ActionResult Index()
        {
            Utils.setUserClass(this, 2);
            return View("Index");
        }


        //生態檢核下載清單
        public JsonResult GetDownloadList(int seq)
        {
            string folder = Utils.GetEcologicalCheckRemoteFolder(seq);
            if (!Directory.Exists(Path.Combine(folder, "SelfEvalFiles")))
            {
                return Json(new List<string>());
            }
            return Json(Directory.GetFiles(Path.Combine(folder, "SelfEvalFiles"))
                .Select(file => Path.GetFileName(file))
                .ToList() );
        }
        //生態檢核下載
        public ActionResult OneDownload(int seq, string fileName)
        {
            string folder = Utils.GetEcologicalCheckRemoteFolder(seq);
            Stream iStream = new FileStream(Path.Combine(folder, "SelfEvalFiles", fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", fileName);
        }
        //生態檢核下載
        public ActionResult DownloadAll(int seq, DownloadArgExtension downloadArg = null)
        {
            var m = iService.GetItemByEngMain<EcologicalChecklistModel>(seq).FirstOrDefault();
            string folder = Utils.GetEcologicalCheckRemoteFolder(seq);
            string zipPath = Directory.GetFiles(Path.Combine(folder, "SelfEvalFiles")).ToList().downloadFilesByZip(folder, m.SelfEvalFilename);
            downloadArg?.targetPathSetting(zipPath);

            using (Stream stream = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return File(stream, "application/blob", Path.GetFileName(m.SelfEvalFilename));
            }


        }

        //生態檢核刪除
        public void DeleteFile(int seq, string fileName)
        {
            string folder = Utils.GetEcologicalCheckRemoteFolder(seq);
            System.IO.File.Delete(Path.Combine(folder,"SelfEvalFiles",fileName));
        }

        //更新資料
        public void UploadFile()
        {
            var SelfEvalFiles = Request.Files.GetMultiple("SelfEvalFilename");
        }
        public JsonResult UpdateRecord(int seq, int toDoChecklit)
        {
            List<EcologicalChecklistModel> items = iService.GetItem<EcologicalChecklistModel>(seq);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料錯誤"
                });
            }

            List<string> oldFiles = new List<string>();
            EcologicalChecklistModel m = items[0];
            m.ToDoChecklit = toDoChecklit;

            string folder = Utils.GetEcologicalCheckRemoteFolder(m.EngMainSeq);
            int fCount = Request.Files.Count;
            var SelfEvalFiles = Request.Files.GetMultiple("SelfEvalFilename");

            //if (!SelfEvalFiles.UploadFilesTypeCheck(new string[] {
            //    "image/jpeg", "application/pdf"
            //}))
            //{
            //    return Json(new
            //    {
            //        result = -1,
            //        message = "生態檢核上傳檔案格式錯誤"
            //    });
            //}



            if (fCount > 0 && Request.Files[0].ContentLength > 0)
            {
                try
                {
                    var keys = Request.Files.AllKeys.Except(
                        new string[]{
                            "SelfEvalFilename"
                        }
                    );
                    foreach (var key in keys)
                    {
                        var file = Request.Files.Get(key);
                        if (file.ContentLength > 0)
                        {
                            System.Reflection.PropertyInfo property = m.GetType().GetProperty(key);
                            if (property != null)
                            {
                                string fileName = Guid.NewGuid().ToString("B").ToUpper() + file.FileName;
                                file.SaveAs(Path.Combine(folder, fileName));


                                string oldValue = (string)property.GetValue(m);
                                property.SetValue(m, fileName, null);
                                if (!String.IsNullOrEmpty(oldValue)) oldFiles.Add(oldValue);

                            }
                        }
                    }
                }
                catch
                {
                    return Json(new
                    {
                        result = -1,
                        message = "上傳檔案失敗"
                    });
                }
            }

            //刪除生態檢核資料
            var selfEvalFolder = Path.Combine(folder, "SelfEvalFiles");
            if (!Directory.Exists(selfEvalFolder)) Directory.CreateDirectory(selfEvalFolder);
            //else
            //{
            //    foreach (var file in Directory.GetFiles(selfEvalFolder))
            //    {
            //        System.IO.File.Delete(file);
            //    }
            //}

            //上傳新的生態檢核資料
            foreach (var file in SelfEvalFiles)
            {

                string fileName = Guid.NewGuid().ToString("B").ToUpper() + file.FileName;
                file.SaveAs(Path.Combine(folder, "SelfEvalFiles", fileName));

            }

            //壓縮生態檢核資料，紀錄即將更新在資料庫的資料，如果上傳的生態檢核檔案是zip則不紀錄
            if (SelfEvalFiles.FirstOrDefault() is HttpPostedFileBase selfEvalFile)
            {
                m.SelfEvalFilename = Path.GetExtension(selfEvalFile.FileName) == ".zip" ?
                    m.SelfEvalFilename : Guid.NewGuid().ToString("B").ToUpper() + ".zip";
            }



            if (iService.UpdateDesign(m) == -1)
            {
                return Json(new
                {
                    result = -1,
                    message = "儲存失敗"
                });
            }
            foreach (string fName in oldFiles)
            {
                System.IO.File.Delete(Path.Combine(folder, fName));
            }
            return Json(new
            {
                result = 0,
                message = "儲存完成"
            });
        }
        public JsonResult GetList(int id)
        {
            List<EcologicalChecklistModel> ceList = iService.GetListDesign<EcologicalChecklistModel>(id);
            if(ceList.Count == 0)
            {
                iService.Add(id, EcologicalChecklistService._DesignStage);
                ceList = iService.GetListDesign<EcologicalChecklistModel>(id);
            }
            if (ceList.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料讀取錯誤"
                });
            }
            return Json(new
            {
                result = 0,
                item = ceList[0]
            });
        }
        //標案 EngMain
        public virtual JsonResult GetEngMain(int id)
        {
            List<EngMainEditVModel> items = new EngMainService().GetItemBySeq<EngMainEditVModel>(id);
            if (items.Count == 1)
            {
                return Json(new
                {
                    result = 0,
                    item = items[0]
                });
            }
            else
            {
                return Json(new
                {
                    result = 2,
                    msg = "讀取資料錯誤"
                });
            }
        }
        //
        public ActionResult Download(int id, string key, DownloadArgExtension downloadArg = null)
        {
            List<EcologicalChecklistModel> items = iService.GetItem<EcologicalChecklistModel>(id);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }

            EcologicalChecklistModel m = items[0];
            System.Reflection.PropertyInfo property = m.GetType().GetProperty(key);
            if (property == null)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }

            string fileName = (string)property.GetValue(m);
            string fName = Path.Combine(Utils.GetEcologicalCheckRemoteFolder(m.EngMainSeq), fileName);
            if (!System.IO.File.Exists(fName))
            {
                return Json(new
                {
                    result = -1,
                    message = "未發現檔案"
                }, JsonRequestBehavior.AllowGet);
            }

            Stream iStream = new FileStream(fName, FileMode.Open, FileAccess.Read, FileShare.Read);
            downloadArg?.targetPathSetting(fName);
            return File(iStream, "application/blob", fileName.Substring(38));
        }
    }
}