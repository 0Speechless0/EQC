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
    public class EPCEcologicalCheckController : Controller
    {//工程品管 - 生態檢核(施工階段)上傳
        EcologicalChecklistService iService = new EcologicalChecklistService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
        //生態檢核下載清單
        public JsonResult GetDownloadList(int seq)
        {
            string folder = Utils.GetEcologicalCheckRemoteFolder(seq, "FileUploads/EcologicalCheck2");
            if (!Directory.Exists(Path.Combine(folder, "SelfEvalFiles") ))
            {
                return Json(new List<string>());
            }
            return Json(Directory.GetFiles(Path.Combine(folder, "SelfEvalFiles"))
                .Select(file => Path.GetFileName(file))
                .ToList());
        }
        //生態檢核下載
        public ActionResult OneDownload(int seq, string fileName)
        {
            string folder = Utils.GetEcologicalCheckRemoteFolder(seq, "FileUploads/EcologicalCheck2");
            
            Stream iStream = new FileStream(Path.Combine(folder, "SelfEvalFiles", fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", fileName);
        }
        //生態檢核下載
        public ActionResult DownloadAll(int seq, DownloadArgExtension packageArg = null)
        {
            var m = iService.GetItemByEngMain<EcologicalChecklist2Model>(seq, 2).FirstOrDefault();

            string folder = Utils.GetEcologicalCheckRemoteFolder(seq, "FileUploads/EcologicalCheck2");
            string zipPath = Directory.GetFiles(Path.Combine(folder, "SelfEvalFiles")).ToList().downloadFilesByZip(folder, m.SelfEvalFilename);
            Stream stream = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            packageArg?.targetPathSetting(zipPath);
            return File(stream, "application/blob", Path.GetFileName(m.SelfEvalFilename));
        }

        //生態檢核刪除
        public void DeleteFile(int seq, string fileName)
        {
            string folder = Utils.GetEcologicalCheckRemoteFolder(seq, "FileUploads/EcologicalCheck2"); 
            System.IO.File.Delete(Path.Combine(folder, "SelfEvalFiles", fileName));
        }
        //更新資料
        public JsonResult UpdateRecord(int seq)
        {
            List<EcologicalChecklist2Model> items = iService.GetItem<EcologicalChecklist2Model>(seq, 2);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料錯誤"
                });
            }

            List<string> oldFiles = new List<string>();
            EcologicalChecklist2Model m = items[0];

            string folder = Utils.GetEcologicalCheckRemoteFolder(m.EngMainSeq, "FileUploads/EcologicalCheck2");
            var SelfEvalFiles = Request.Files.GetMultiple("SelfEvalFilename");

            //if(!SelfEvalFiles.UploadFilesTypeCheck(new string[] {
            //    "image/jpeg", "application/pdf"
            //}) )
            //{
            //    return Json(new
            //    {
            //        result = -1,
            //        message = "生態檢核上傳檔案錯誤"
            //    });
            //}


            int fCount = Request.Files.Count;
            if (fCount > 0 && Request.Files[0].ContentLength > 0)
            {
                try
                {
                    var keys = Request.Files.AllKeys
                    .Except(
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
                    //刪除生態檢核資料
                    var selfEvalFolder = Path.Combine(folder, "SelfEvalFiles");
                    if (!Directory.Exists(selfEvalFolder)) Directory.CreateDirectory(selfEvalFolder);


                    //上傳新的生態檢核資料
                    
                    foreach( var file in SelfEvalFiles)
                    {

                        string fileName = Guid.NewGuid().ToString("B").ToUpper() + file.FileName;
                        file.SaveAs(Path.Combine(folder, "SelfEvalFiles", fileName));

                    }
                    ;
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


            
            if( SelfEvalFiles.FirstOrDefault() is HttpPostedFileBase selfEvalFile)
            {
                m.SelfEvalFilename = Path.GetExtension(selfEvalFile.FileName) == ".zip" ?
                    m.SelfEvalFilename :  Guid.NewGuid().ToString("B").ToUpper() + ".zip";
            }

            if (iService.UpdateConstruction(m) == -1)
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
            List<EcologicalChecklistModel> planList = iService.GetListDesign<EcologicalChecklistModel>(id);
            if (planList.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "生態檢核(設計階段) 無資料"
                });
            } 
            //else if(planList[0].ToDoChecklit > 2)
            //{
            //    return Json(new
            //    {
            //        result = 1,
            //        msg = "生態檢核(施工階段) 此處不須上傳文件"
            //    });
            //}
            List<EcologicalChecklist2Model> ceList = iService.GetListConstruction<EcologicalChecklist2Model>(id);
            if(ceList.Count == 0)
            {
                iService.Add(id, EcologicalChecklistService._ConstructionStage);
                ceList = iService.GetListConstruction<EcologicalChecklist2Model>(id);
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
            List<EcologicalChecklist2Model> items = iService.GetItem<EcologicalChecklist2Model>(id, 2);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }

            EcologicalChecklist2Model m = items[0];
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
            string fName = Path.Combine(Utils.GetEcologicalCheckRemoteFolder(m.EngMainSeq, "FileUploads/EcologicalCheck2"), fileName);
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