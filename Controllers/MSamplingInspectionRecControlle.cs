﻿using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class MSamplingInspectionRecControlle : SamplingInspectionRecController
    {

        public override ActionResult Index()
        {
            //ViewBag.Title = "抽驗紀錄填報";
            Utils.setUserClass(this);
            return View();
        }
        public ActionResult Index2()
        {
            return View("Index2");
        }
        public virtual ActionResult API_Post_Test()
        {
            return View();
        }

        public virtual ActionResult API()
        {
            return View();
        }

        


        public override ActionResult EditEng(int seq)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Edit", "MSamplingInspectionRec");
            string tarUrl = redirectUrl.ToString() + "?id=" + seq;
            return Json(new { Url = tarUrl });
        }
        public override ActionResult Edit()
        {
            //ViewBag.Title = "監造計畫-編輯";
            List<VMenu> menu = new List<VMenu>();
            var link = new UrlHelper(Request.RequestContext).Action("Index", "MSamplingInspectionRec");
            menu.Add(new VMenu() { Name = "抽驗紀錄填報(M)", Url = link.ToString() });
            menu.Add(new VMenu() { Name = "抽驗紀錄填報(M)-編輯", Url = "" });
            ViewBag.breadcrumb = menu;

            return View();
        }

        public virtual ActionResult GetAppData()
        {
            try
            {
                if (TempData["UserNo"] != null)
                {
                    if ((string)TempData["CheckResult"] == "檢查合格")
                    {
                        TempData["CheckResult"] = 1;
                    }else if ((string)TempData["CheckResult"] == "有缺失")
                    {
                        TempData["CheckResult"] = 2;
                    }
                    else
                    {
                        TempData["CheckResult"] = 3;
                    }
                    return new JsonResult()
                    {
                        Data = Json(new
                        {
                            AppList = TempData["AppList"],
                            UserNo = TempData["UserNo"],
                            Mobile = TempData["Mobile"],
                            EngName = TempData["EngName"],
                            EngItemName = TempData["EngItemName"],
                            XYPosLati = TempData["XYPosLati"],
                            XYPosLong = TempData["XYPosLong"],
                            PosDesc = TempData["PosDesc"],
                            CheckDate = TempData["CheckDate"],
                            CheckItem = TempData["CheckItem"],
                            RealCheckCond = TempData["RealCheckCond"],
                            CheckResult = TempData["CheckResult"],
                            CheckImage1 = TempData["CheckImage1"],
                            CheckImage2 = TempData["CheckImage2"],
                            CheckImage3 = TempData["CheckImage3"],
                            CheckUser = TempData["CheckUser"],
                            result = 0,
                            message = "上傳成功",
                        }),
                        MaxJsonLength = int.MaxValue,/*重點在這行*/
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    return new JsonResult()
                    {
                        Data = Json(new
                        {
                            result = -1,
                            message = "上傳資料為空，請重新上傳"
                        }),
                        MaxJsonLength = int.MaxValue,/*重點在這行*/
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };

                }

            }
            catch (Exception ex)
            {
                BaseService.log.Info("Error:" + ex);
                return new JsonResult()
                {
                    Data = Json(new
                    {
                        result = -1,
                        message = "上傳檔案失敗"
                    }),
                    MaxJsonLength = int.MaxValue,/*重點在這行*/
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
             }
        }
        //判斷是否為app進入(有無post)
        public virtual JsonResult CheckPost()
        {
            if (TempData["UserNo"] != null)
            {
                return Json(new
                {
                    result = true
                });
            }
            else
            {
                return Json(new
                {
                    result = false
                });

            }
        }
        public  override ActionResult SIRDownload(int seq, int filetype = 2)
        {
            try
            {
                SignatureFileService signatureFileService = new SignatureFileService();
                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                List<EngConstructionEngInfoVModel> engItems = constCheckRecService.GetEngMainByEngConstructionSeq<EngConstructionEngInfoVModel>(seq);
                EngConstructionEngInfoVModel engItem = engItems[0];

                List<ConstCheckRecSheetModel> recItems = constCheckRecService.GetList<ConstCheckRecSheetModel>(seq);
                ConstCheckRecResultService service = new ConstCheckRecResultService();
                foreach (ConstCheckRecSheetModel item in recItems)
                {
                    item.GetControls(service);
                    JoinCell(item.items);
                }
                foreach (ConstCheckRecSheetModel item in recItems)
                {
                    if (item.CCRCheckType1 == 1)
                        CheckSheet1(engItem, item, uuid, signatureFileService, filetype);
                    else if (item.CCRCheckType1 == 2)
                        CheckSheet2(engItem, item, uuid, signatureFileService, filetype);
                    else if (item.CCRCheckType1 == 3)
                        CheckSheet3(engItem, item, uuid, signatureFileService, filetype);
                    else if (item.CCRCheckType1 == 4)
                        CheckSheet4(engItem, item, uuid, signatureFileService, filetype);
                }
                service.Close();

                Task zipDeleteTask = Task.Run(() => {
                    Thread.Sleep(10000);
                    try
                    {
                        string[] deleteList = Directory.GetDirectories(Path.GetTempPath(), uuid + "*", SearchOption.TopDirectoryOnly);
                        
                        foreach(string path in deleteList)
                        {
                            Directory.Delete(path, true);
                        }
                        deleteList = Directory.GetFiles(Path.GetTempPath(), uuid + "*");
                        foreach (string file in deleteList)
                        {
                            System.IO.File.Delete(file);
                        }
                    }
                    finally { }
                
                });

                if (filetype == 2)
                {
                    string path = Path.Combine(Path.GetTempPath(), uuid) + "pdf";
                    string zipFile = Path.Combine(Path.GetTempPath(), uuid + "pdf.zip");
                    ZipFile.CreateFromDirectory(path, zipFile);// AddFiles(files, "ProjectX");
                    Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", engItem.EngNo + "-抽查紀錄表.zip");

                }
                else if (filetype == 3)
                {
                    string path = Path.Combine(Path.GetTempPath(), uuid) + "odt";
                    string zipFile = Path.Combine(Path.GetTempPath(), uuid + "odt.zip");
                    ZipFile.CreateFromDirectory(path, zipFile);// AddFiles(files, "ProjectX");
                    Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", engItem.EngNo + "-抽查紀錄表.zip");
                }
                else
                {
                    string path = Path.Combine(Path.GetTempPath(), uuid);
                    string zipFile = Path.Combine(Path.GetTempPath(), uuid + ".zip");
                    ZipFile.CreateFromDirectory(path, zipFile);// AddFiles(files, "ProjectX");
                    Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", engItem.EngNo + "-抽查紀錄表.zip");
                }


            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("產製錯誤: " + e.Message);
                return Json(new
                {
                    result = -1,
                    message = "產製錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}