using EQC.Common;
using EQC.Models;
using EQC.Services;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class SIRApproveController : SamplingInspectionRecController
    {
        protected ConstCheckRecApproveService constCheckRecApproveService = new ConstCheckRecApproveService();

        public override ActionResult Index()
        {
            //ViewBag.Title = "抽驗紀錄填報";
            Utils.setUserClass(this);
            return View();
        }
        public override ActionResult GetUserUnit()
        {
            string unitSubSeq = "";
            string unitSeq = "";
            Utils.GetUserUnit(ref unitSeq, ref unitSubSeq);
            return Json(new
            {
                result = 0,
                unit = unitSeq,
                unitSub = unitSubSeq
            });
        }
        //標案年分
        public override JsonResult GetYearOptions()
        {
            List<EngYearVModel> years = constCheckRecApproveService.GetEngYearList();
            return Json(years);
        }
        //依年分取執行機關
        public override JsonResult GetUnitOptions(string year)
        {
            List<EngExecUnitsVModel> items = constCheckRecApproveService.GetEngExecUnitList(year);
            return Json(items);
        }
        //依年分,機關取執行單位
        public override JsonResult GetSubUnitOptions(string year, int parentSeq)
        {
            List<EngExecUnitsVModel> items = constCheckRecApproveService.GetEngExecSubUnitList(year, parentSeq);
            EngExecUnitsVModel m = new EngExecUnitsVModel();
            m.UnitSeq = -1;
            m.UnitName = "全部單位";
            items.Insert(0, m);
            return Json(items);
        }
        //工程名稱清單
        public override JsonResult GetEngNameList(string year, int unit, int subUnit, int engMain)
        {
            List<EngNameOptionsVModel> engNames = new List<EngNameOptionsVModel>();
            engNames = constCheckRecApproveService.GetEngCreatedNameList<EngNameOptionsVModel>(year, unit, subUnit);
            engNames.Sort((x, y) => x.CompareTo(y));
            return Json(new
            {
                engNameItems = engNames
            });
        }
        //分項工程清單
        public override JsonResult GetSubEngNameList(int engMain)
        {
            List<EngConstructionOptionsVModel> subEngNames = constCheckRecApproveService.GetSubEngList<EngConstructionOptionsVModel>(engMain);
            EngConstructionOptionsVModel m = new EngConstructionOptionsVModel();
            m.Seq = -1;
            m.ItemName = "全部分項工程";
            subEngNames.Insert(0, m);
            //    engNames.Sort((x, y) => x.CompareTo(y));
            return Json(subEngNames);
        }

        //分項工程清單
        public JsonResult GetListLightly(int engMain, int subEngMain, int pageRecordCount, int pageIndex)
        {
            List<EngConstructionListVModel> engList = new List<EngConstructionListVModel>();
            int total = constCheckRecApproveService.GetEngCreatedListCount(engMain, subEngMain);
            if(total > 0)
            {
                engList = constCheckRecApproveService.GetEngCreatedList<EngConstructionListVModel>(engMain, subEngMain, pageRecordCount, pageIndex);
            }
            
        
            return Json(new
            {
                pTotal = total,
                items = engList,
            });
        }

        //抽驗單同意
        public JsonResult Approved(int seq, string sdName)
        {
            UserInfo userInfo = Utils.getUserInfo();
            //if(!userInfo.UserNo.Equals(sdName))
            //{
            //    return Json(new
            //    {
            //        result = -1,
            //        message = "僅工程監造主任才可作業"
            //    });
            //}
            if (constCheckRecApproveService.EngConstructionFormApproved(seq) > 0)
            {
                return Json(new
                {
                    result = 0,
                    message = "儲存完成"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "儲存失敗"
                });
            }
        }
        //抽驗單不同意
        public JsonResult Disagree(int seq, string sdName)
        {
            UserInfo userInfo = Utils.getUserInfo();
            if (!userInfo.UserNo.Equals(sdName))
            {
                return Json(new
                {
                    result = -1,
                    message = "僅工程監造主任才可作業"
                });
            }
            if (constCheckRecApproveService.EngConstructionFormDisagree(seq) > 0)
            {
                return Json(new
                {
                    result = 0,
                    message = "儲存完成"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "儲存失敗"
                });
            }
        }

        //抽查記錄表單列表
        public virtual ActionResult GetSIRlistApprove(int seq)
        {
            try
            {
                SignatureFileService signatureFileService = new SignatureFileService();
                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                List<EngConstructionEngInfoVModel> engItems = constCheckRecApproveService.GetEngMainByEngConstructionSeq<EngConstructionEngInfoVModel>(seq);
                EngConstructionEngInfoVModel engItem = engItems[0];

                List<ConstCheckRecSheetModel> recItems = constCheckRecApproveService.GetList<ConstCheckRecSheetModel>(seq);
                ConstCheckRecResultService service = new ConstCheckRecResultService();

                List<string> SIRlists = new List<string>();
                foreach (ConstCheckRecSheetModel item in recItems)
                {
                    if (item.CCRCheckType1 == 1)
                    {
                        string outFile = item.Getdownloadname(uuid);
                        string[] sArray = outFile.Split('.');
                        SIRlists.Add(sArray[0]);
                    }
                    else if (item.CCRCheckType1 == 2)
                    {
                        string outFile = item.Getdownloadname(uuid);
                        string[] sArray = outFile.Split('.');
                        SIRlists.Add(sArray[0]);
                    }
                    //CheckSheet2(engItem, item, uuid, signatureFileService);
                    else if (item.CCRCheckType1 == 3)
                    {
                        string outFile = item.Getdownloadname(uuid);
                        string[] sArray = outFile.Split('.');
                        SIRlists.Add(sArray[0]);
                    }
                    //CheckSheet3(engItem, item, uuid, signatureFileService);
                    else if (item.CCRCheckType1 == 4)
                    {
                        string outFile = item.Getdownloadname(uuid);
                        string[] sArray = outFile.Split('.');
                        SIRlists.Add(sArray[0]);
                    }
                    //CheckSheet4(engItem, item, uuid, signatureFileService);
                }
                return Json(SIRlists);
                // string path = Path.Combine(Path.GetTempPath(), uuid);
                // string zipFile = Path.Combine(Path.GetTempPath(), uuid + ".zip");
                // ZipFile.CreateFromDirectory(path, zipFile);// AddFiles(files, "ProjectX");
                //
                // Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                // return File(iStream, "application/blob", engItem.EngNo + "-抽查紀錄表.zip");
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = -1,
                    message = "產製錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
        }


        //抽查記錄表單下載
        public override ActionResult SIRDownload(int seq,int filetype)
        {
            try
            {
                SignatureFileService signatureFileService = new SignatureFileService();
                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                List<EngConstructionEngInfoVModel> engItems = constCheckRecApproveService.GetEngMainByEngConstructionSeq<EngConstructionEngInfoVModel>(seq);
                EngConstructionEngInfoVModel engItem = engItems[0];

                List<ConstCheckRecSheetModel> recItems = constCheckRecApproveService.GetList<ConstCheckRecSheetModel>(seq);
                ConstCheckRecResultService service = new ConstCheckRecResultService();
                foreach (ConstCheckRecSheetModel item in recItems)
                {
                    item.GetControls(service);
                    JoinCell(item.items);
                }
                service.Close();

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
                return Json(new
                {
                    result = -1,
                    message = "產製錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //抽查記錄表單下載(單一檔案)
        public virtual ActionResult SIROneDownloadApprove(int seq, List<string> items, string downloaditem, int num, int filetype)
        {
            try
            {
                //取得要下載第幾項
                int fileNum = 0;
                string[] itemArray = items[0].Split(',');
                for (int i = 0; i <= num; i++)
                {
                    if (itemArray[i] == downloaditem)
                    {
                        fileNum = i;
                        break;
                    }
                }
                //
                SignatureFileService signatureFileService = new SignatureFileService();
                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                List<EngConstructionEngInfoVModel> engItems = constCheckRecApproveService.GetEngMainByEngConstructionSeq<EngConstructionEngInfoVModel>(seq);
                EngConstructionEngInfoVModel engItem = engItems[0];

                List<ConstCheckRecSheetModel> recItems = constCheckRecApproveService.GetList<ConstCheckRecSheetModel>(seq);
                ConstCheckRecResultService service = new ConstCheckRecResultService();
                ConstCheckRecSheetModel item = recItems[fileNum];
                item.GetControls(service);
                JoinCell(item.items);
                service.Close();
                //輸出檔名
                string Filename = item.Getdownloadname(uuid);
                string[] FileOriginal = Filename.Split('.');
                if (filetype == 1)
                {
                    Filename = FileOriginal[0] + ".docx";
                }
                else if (filetype == 2)
                {
                    Filename = FileOriginal[0] + ".pdf";
                }
                else if (filetype == 3)
                {
                    Filename = FileOriginal[0] + ".odt";
                }
                //
                if (item.CCRCheckType1 == 2)
                {
                    return File(CheckSheet2download(engItem, item, uuid, signatureFileService, filetype), "application/blob", Filename);
                }
                else if (item.CCRCheckType1 == 3)
                {
                    return File(CheckSheet3download(engItem, item, uuid, signatureFileService, filetype), "application/blob", Filename);
                }
                else if (item.CCRCheckType1 == 4)
                {
                    return File(CheckSheet4download(engItem, item, uuid, signatureFileService, filetype), "application/blob", Filename);
                }
                else
                {
                    return File(CheckSheet1download(engItem, item, uuid, signatureFileService, filetype), "application/blob", Filename);

                }
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = -1,
                    message = "產製錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}