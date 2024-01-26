using EQC.Common;
using EQC.Detection;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using Microsoft.Office.Interop.Word;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class SamplingInspectionRecImproveController : Controller
    {
        private ConstCheckRecImproveService constCheckRecImproveService = new ConstCheckRecImproveService();

        virtual public ActionResult Index()
        {
            Utils.setUserClass(this);
            //ViewBag.Title = "抽查缺失改善";
            return View();
        }
        public ActionResult GetUserUnit()
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

        virtual public ActionResult EditEng(int seq)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Edit", "SamplingInspectionRecImprove");
            string tarUrl = redirectUrl.ToString() + "?id=" + seq;
            return Json(new { Url = tarUrl });
        }
        virtual public ActionResult Edit()
        {
            Utils.setUserClass(this);
            //ViewBag.Title = "監造計畫-編輯";
            List<VMenu> menu = new List<VMenu>();
            var link = new UrlHelper(Request.RequestContext).Action("Index", "SamplingInspectionRecImprove");
            menu.Add(new VMenu() { Name = "抽查缺失改善", Url = link.ToString() });
            menu.Add(new VMenu() { Name = "抽查缺失改善-編輯", Url = "" });
            ViewBag.breadcrumb = menu;

            return View();
        }

        //標案年分
        virtual public JsonResult GetYearOptions()
        {
            List<EngYearVModel> years = constCheckRecImproveService.GetEngYearList();
            return Json(years);
        }
        //依年分取執行機關
        virtual public JsonResult GetUnitOptions(string year)
        {
            List<EngExecUnitsVModel> items = constCheckRecImproveService.GetEngExecUnitList(year);
            return Json(items);
        }
        //依年分,機關取執行單位
        virtual public JsonResult GetSubUnitOptions(string year, int parentSeq)
        {
            List<EngExecUnitsVModel> items = constCheckRecImproveService.GetEngExecSubUnitList(year, parentSeq);
            EngExecUnitsVModel m = new EngExecUnitsVModel();
            m.UnitSeq = -1;
            m.UnitName = "全部單位";
            items.Insert(0, m);
            return Json(items);
        }
        //工程名稱清單
        virtual public JsonResult GetEngNameList(string year, int unit, int subUnit, int engMain)
        {
            List<EngNameOptionsVModel> engNames = new List<EngNameOptionsVModel>();
            engNames = constCheckRecImproveService.GetEngCreatedNameList<EngNameOptionsVModel>(year, unit, subUnit);
            engNames.Sort((x, y) => x.CompareTo(y));
            return Json(new
            {
                engNameItems = engNames
            });
        }
        //分項工程清單
        virtual public JsonResult GetSubEngNameList(int engMain)
        {
            List<EngConstructionOptionsVModel> subEngNames = constCheckRecImproveService.GetSubEngList<EngConstructionOptionsVModel>(engMain);
            EngConstructionOptionsVModel m = new EngConstructionOptionsVModel();
            m.Seq = -1;
            m.ItemName = "全部分項工程";
            subEngNames.Insert(0, m);
            //    engNames.Sort((x, y) => x.CompareTo(y));
            return Json(subEngNames);
        }

        //分項工程清單
        public JsonResult GetList(int engMain)
        {
            return Json(new
            {
                //items = constCheckRecImproveService.GetEngCreatedList<EngConstructionListVModel>(engMain),
                cc = constCheckRecImproveService.GetConstCheckList1<EngSamplingInspectionImproveVModel>(engMain), //施工抽查清單 s20230522
                eot = constCheckRecImproveService.GetEquOperTestList1<EngSamplingInspectionImproveVModel>(engMain), //設備運轉測試清單
                osh = constCheckRecImproveService.GetOccuSafeHealthList1<EngSamplingInspectionImproveVModel>(engMain), //職業安全衛生清單
                ec = constCheckRecImproveService.GetEnvirConsList1<EngSamplingInspectionImproveVModel>(engMain) //環境保育清單
            });
        }
        //有缺失項目檢驗單之分項工程清單 s20230523
        public virtual JsonResult GetRecEngConstruction(int mode, int eId, int rId)
        {
            return Json(new
            {
                result = 0,
                items = constCheckRecImproveService.GetRecEngConstruction(mode, eId, rId)
            });
        }
        //抽查記錄表單列表
        public virtual ActionResult GetSIRlist(int seq,int mode)
        {
            try
            {
                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                string folder = Path.Combine(Path.GetTempPath(), uuid);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                List<EngConstructionEngInfoVModel> engItems = constCheckRecImproveService.GetEngMainByEngConstructionSeq<EngConstructionEngInfoVModel>(seq);
                EngConstructionEngInfoVModel engItem = engItems[0];

                string dName = "NG";
                List<string> SIRlists = new List<string>();
                if (mode == 44)
                {
                    dName = engItem.EngNo + "-不符合事項報告.zip";
                    List<NgReportModel> items = constCheckRecImproveService.GetNgReoprtList<NgReportModel>(seq);
                    foreach (NgReportModel item in items)
                    {
                        DateTime dt = item.CreateTime.Value;
                        string fn = String.Format("{0}-{1}{2:00}{3:00}-{4}.docx", "不符合事項報告", dt.Year - 1911, dt.Month, dt.Day, item.Seq);
                        string[] sArray = fn.Split('.');
                        SIRlists.Add(sArray[0]);
                    }
                }
                else if (mode == 45)
                {
                    dName = engItem.EngNo + "-NCR程序追蹤改善表.zip";
                    List<NcrModel> items = constCheckRecImproveService.GetNCRList<NcrModel>(seq);
                    foreach (NcrModel item in items)
                    {
                        DateTime dt = item.CreateTime.Value;
                        string fn = String.Format("{0}-{1}{2:00}{3:00}-{4}.docx", "NCR程序追蹤改善表", dt.Year - 1911, dt.Month, dt.Day, item.Seq);
                        string[] sArray = fn.Split('.');
                        SIRlists.Add(sArray[0]);
                    }
                }
                else if (mode == 46)
                {
                    dName = engItem.EngNo + "-改善照片.zip";
                    List<NgReportModel> items = constCheckRecImproveService.GetNgReoprtList<NgReportModel>(seq);
                    foreach (NgReportModel item in items)
                    {
                        item.GetImgGroupOption(constCheckRecImproveService);
                        foreach (NgReportPhotoGroupModel group in item.photoGroups)
                        {
                            DateTime dt = item.CreateTime.Value;
                            string fn = String.Format("{0}-{1}{2:00}{3:00}-{4}-{5}.docx", "改善照片", dt.Year - 1911, dt.Month, dt.Day, group.Value, item.Seq);
                            string[] sArray = fn.Split('.');
                            SIRlists.Add(sArray[0]);
                        }
                    }
                }
                else
                {
                    return Json(new
                    {
                        result = -1,
                        message = "列表顯示錯誤"
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(SIRlists);
            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = -1,
                    message = "列表顯示錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //抽查記錄表單下載 s20230522
        public ActionResult SIRDnDoc(int mode, int seq, int eId, int docType, int filetype)
        {
            try
            {
                string dName = "NG";
                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                string folder = Path.Combine(Path.GetTempPath(), uuid);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                //List<EngConstructionEngInfoVModel> engItems = constCheckRecImproveService.GetEngMainByEngConstructionSeq<EngConstructionEngInfoVModel>(seq);
                //EngConstructionEngInfoVModel engItem = engItems[0];
                List<EngConstructionEngInfoVModel> engItems = constCheckRecImproveService.GetEngConstruction<EngConstructionEngInfoVModel>(eId, mode, seq);
                foreach (EngConstructionEngInfoVModel engItem in engItems)
                {
                    if (docType == 44)
                    {
                        dName = engItem.EngNo + "-不符合事項報告.zip";
                        List<NgReportModel> items = constCheckRecImproveService.GetNgReoprtList<NgReportModel>(engItem.subEngNameSeq, seq);
                        foreach (NgReportModel item in items)
                        {
                            NgReport(engItem, item, folder, filetype);
                        }
                    }
                    else if (docType == 45)
                    {
                        dName = engItem.EngNo + "-NCR程序追蹤改善表.zip";
                        List<NcrModel> items = constCheckRecImproveService.GetNCRList<NcrModel>(engItem.subEngNameSeq, seq);
                        foreach (NcrModel item in items)
                        {
                            NCRReport(engItem, item, folder, filetype);
                        }
                    }
                    else if (docType == 46)
                    {
                        dName = engItem.EngNo + "-改善照片.zip";
                        List<NgReportModel> items = constCheckRecImproveService.GetNgReoprtList<NgReportModel>(engItem.subEngNameSeq, seq);
                        foreach (NgReportModel item in items)
                        {
                            item.GetImgGroupOption(constCheckRecImproveService);
                            PhotoReport(engItem, item, folder, filetype);
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "產製錯誤"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (filetype == 2)
                {
                    string path = Path.Combine(Path.GetTempPath(), uuid) + "pdf";
                    string zipFile = Path.Combine(Path.GetTempPath(), uuid + "pdf.zip");
                    ZipFile.CreateFromDirectory(path, zipFile);// AddFiles(files, "ProjectX");
                    Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", dName);

                }
                else if (filetype == 3)
                {
                    string path = Path.Combine(Path.GetTempPath(), uuid) + "odt";
                    string zipFile = Path.Combine(Path.GetTempPath(), uuid + "odt.zip");
                    ZipFile.CreateFromDirectory(path, zipFile);// AddFiles(files, "ProjectX");
                    Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", dName);
                }
                else
                {
                    string path = Path.Combine(Path.GetTempPath(), uuid);
                    string zipFile = Path.Combine(Path.GetTempPath(), uuid + ".zip");
                    ZipFile.CreateFromDirectory(path, zipFile);// AddFiles(files, "ProjectX");
                    Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", dName);
                }
            }
            catch
            {
                return Json(new
                {
                    result = -1,
                    message = "產製錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //抽查記錄表單下載
        public ActionResult SIRDownload(int seq, int mode ,int filetype)
        {
            try
            {
                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                string folder = Path.Combine(Path.GetTempPath(), uuid); 
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                List<EngConstructionEngInfoVModel> engItems = constCheckRecImproveService.GetEngMainByEngConstructionSeq<EngConstructionEngInfoVModel>(seq);
                EngConstructionEngInfoVModel engItem = engItems[0];

                string dName = "NG";
                if (mode == 44)
                {
                    dName = engItem.EngNo + "-不符合事項報告.zip";
                    List<NgReportModel> items = constCheckRecImproveService.GetNgReoprtList<NgReportModel>(seq);
                    foreach (NgReportModel item in items)
                    {
                        NgReport(engItem, item, folder, filetype);
                    }
                } else if (mode == 45)
                {
                    dName = engItem.EngNo + "-NCR程序追蹤改善表.zip";
                    List<NcrModel> items = constCheckRecImproveService.GetNCRList<NcrModel>(seq);
                    foreach (NcrModel item in items)
                    {
                        NCRReport(engItem, item, folder, filetype);
                    }
                } else if (mode == 46)
                {
                    dName = engItem.EngNo + "-改善照片.zip";
                    List<NgReportModel> items = constCheckRecImproveService.GetNgReoprtList<NgReportModel>(seq);
                    foreach (NgReportModel item in items)
                    {
                        item.GetImgGroupOption(constCheckRecImproveService);
                        PhotoReport(engItem, item, folder, filetype);
                    }
                } else
                {
                    return Json(new
                    {
                        result = -1,
                        message = "產製錯誤"
                    }, JsonRequestBehavior.AllowGet);
                }

                if (filetype == 2)
                {
                    string path = Path.Combine(Path.GetTempPath(), uuid) + "pdf";
                    string zipFile = Path.Combine(Path.GetTempPath(), uuid + "pdf.zip");
                    ZipFile.CreateFromDirectory(path, zipFile);// AddFiles(files, "ProjectX");
                    Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", dName);

                }
                else if (filetype == 3)
                {
                    string path = Path.Combine(Path.GetTempPath(), uuid) + "odt";
                    string zipFile = Path.Combine(Path.GetTempPath(), uuid + "odt.zip");
                    ZipFile.CreateFromDirectory(path, zipFile);// AddFiles(files, "ProjectX");
                    Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", dName);
                }
                else
                {
                    string path = Path.Combine(Path.GetTempPath(), uuid);
                    string zipFile = Path.Combine(Path.GetTempPath(), uuid + ".zip");
                    ZipFile.CreateFromDirectory(path, zipFile);// AddFiles(files, "ProjectX");
                    Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", dName);
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
        public virtual ActionResult SIROneDownload(int seq, List<string> items, string downloaditem, int num, int filetype,int mode)
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
                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                string folder = Path.Combine(Path.GetTempPath(), uuid);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                List<EngConstructionEngInfoVModel> engItems = constCheckRecImproveService.GetEngMainByEngConstructionSeq<EngConstructionEngInfoVModel>(seq);
                EngConstructionEngInfoVModel engItem = engItems[0];

                string dName = "NG";
                if (mode == 44)
                {
                    dName = engItem.EngNo + "-不符合事項報告.zip";
                    NgReportModel item = constCheckRecImproveService.GetNgReoprtList<NgReportModel>(seq)[fileNum];
                    DateTime dt = item.CreateTime.Value;
                    string fn = String.Format("{0}-{1}{2:00}{3:00}-{4}", "不符合事項報告", dt.Year - 1911, dt.Month, dt.Day, item.Seq);
                    if (filetype == 2)
                    {
                        fn = fn + ".pdf"; 
                    }else if (filetype == 3)
                    {
                        fn = fn + ".odt";
                    }
                    else
                    {
                        fn = fn + ".docx";
                    }
                    return File(NgReportdownload(engItem, item, folder, filetype), "application/blob", fn);
                       
                }
                else if (mode == 45)
                {
                    dName = engItem.EngNo + "-NCR程序追蹤改善表.zip";
                    NcrModel item = constCheckRecImproveService.GetNCRList<NcrModel>(seq)[fileNum];
                    DateTime dt = item.CreateTime.Value;
                    string fn = String.Format("{0}-{1}{2:00}{3:00}-{4}", "不符合事項報告", dt.Year - 1911, dt.Month, dt.Day, item.Seq);
                    if (filetype == 2)
                    {
                        fn = fn + ".pdf";
                    }
                    else if (filetype == 3)
                    {
                        fn = fn + ".odt";
                    }
                    else
                    {
                        fn = fn + ".docx";
                    }
                    return File(NCRReportdownload(engItem, item, folder, filetype), "application/blob", fn);
                }
                else if (mode == 46)
                {
                    //判斷是哪一筆分項工程編號的資料(因一筆分項工程有多筆word，所以無法透過下載的第幾項判斷，需篩出不重複分項工程判斷)
                    string downloaditem1 = downloaditem.Split('-')[3];
                    string[] Array = items[0].Split(',');
                    string[] Array1 = new string[20];
                    int Num46 = 0;
                    for (int i = 0; i < Array.Length; i++)
                    {
                        Array1[i] = Array[i].Split('-')[3];
                    }
                    string[] q = Array1.Distinct().ToArray();
                    for (int i = 0; i < q.Length; i++)
                    {
                        if (downloaditem1 == q[i])
                        {
                            Num46 = i;
                        }
                    }

                    dName = engItem.EngNo + "-改善照片.zip";
                    //shioulo 20220802
                    //NgReportModel item = constCheckRecImproveService.GetNgReoprtList<NgReportModel>(seq)[Num46]; 
                    List<NgReportModel> ms = constCheckRecImproveService.GetNgReoprtList<NgReportModel>(seq);
                    NgReportModel item = null;
                    foreach(NgReportModel m in ms)
                    {
                        if(m.Seq.ToString() == downloaditem1)
                        {//改為核對 Seq 而非 index
                            item = m;
                            break;
                        }
                    }
                    if(item == null)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "資料錯誤, 無法產製"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    //shioulo 20220802 end
                    item.GetImgGroupOption(constCheckRecImproveService);
                    
                    NgReportPhotoGroupModel group = item.photoGroups[0];
                    DateTime dt = item.CreateTime.Value;
                    string fn = String.Format("{0}-{1}{2:00}{3:00}-{4}-{5}.docx", "改善照片", dt.Year - 1911, dt.Month, dt.Day, group.Value, item.Seq);
                    if (filetype == 2)
                    {
                        fn = fn + ".pdf";
                    }
                    else if (filetype == 3)
                    {
                        fn = fn + ".odt";
                    }
                    else
                    {
                        fn = fn + ".docx";
                    }
                    //判斷是分項工程的哪一份word
                    string[] Array5 = new string[20];
                    int y = -1;
                    for (int i = 0; i < Array.Length; i++)
                    {
                        if (downloaditem.Split('-')[3] == Array[i].Split('-')[3]) {
                            y++;
                            if (itemArray[i] == downloaditem)
                            {
                                fileNum = y;
                                break;
                            }
                        }
                    }

                    return File(PhotoReportdownload(engItem, item, folder, filetype, fileNum), "application/blob", fn);
                }
                else
                {
                    return Json(new
                    {
                        result = -1,
                        message = "產製錯誤"
                    }, JsonRequestBehavior.AllowGet);
                }

                string path = Path.Combine(Path.GetTempPath(), uuid);
                string zipFile = Path.Combine(Path.GetTempPath(), uuid + ".zip");
                ZipFile.CreateFromDirectory(path, zipFile);// AddFiles(files, "ProjectX");

                Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", dName);
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


        //表7-44　不符合事項報告
        public void NgReport(EngConstructionEngInfoVModel engItem, NgReportModel item, string folder,int filetype)
        {
            SignatureFileService signatureFileService = new SignatureFileService();
            string tempFile = CopyTemplateFile("表7-44-不符合事項報告.docx");
            DateTime dt = item.CCRCheckDate.Value;
            string fn = String.Format("{0}-{1}{2:00}{3:00}-{4}.docx", "不符合事項報告", dt.Year - 1911, dt.Month, dt.Day, item.Seq);
            string outFile = Path.Combine(folder, fn);
            //List<ControlStVModel> items = recItem.items;
            //string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            try
            {
                XWPFTable table = doc.Tables[0];
                table.GetRow(0).GetCell(1).SetText(engItem.EngName);
                //string[] d = item.chsCreateTime.Split('/');
                table.GetRow(0).GetCell(3).SetText(String.Format("{0}年{1}月{2}日", dt.Year - 1911, dt.Month, dt.Day));
                table.GetRow(1).GetCell(1).SetText(engItem.organizerUnitName);
                table.GetRow(2).GetCell(1).SetText(engItem.SupervisorUnitName);
                table.GetRow(4).GetCell(1).SetText(item.CCRPosDesc);

                string text = "[1]1.施工設備 [2]2.材料設備 [3]3.施工成品 [4]4.施工作業 [5]5.文件、紀錄";
                text = text.Replace("[" + item.CheckItemKind.ToString() + "]", "■");
                for (int i = 1; i <= 5; i++) text = text.Replace("[" + i.ToString() + "]", "□");
                table.GetRow(5).GetCell(1).SetText(text);

                text = "□一般缺失立即改善  □一般缺失追蹤改善";
                if(item.IncompKind.HasValue)
                {
                    if(item.IncompKind == 1)
                        text = "■一般缺失立即改善  □一般缺失追蹤改善";
                    if (item.IncompKind == 2)
                        text = "□一般缺失立即改善  ■一般缺失追蹤改善";
                }
                /*text = text.Replace("[" + item.IncompKind.ToString() + "]", "■");
                if (item.IncompKind == 2 || item.IncompKind == 4)
                    text = text.Replace("[1]", "■");
                for (int i = 1; i <= 4; i++) text = text.Replace("[" + i.ToString() + "]", "□");*/
                table.GetRow(6).GetCell(1).SetText(text);

                text = "[1]施工抽查(監造單位)  [2]自主檢查(承攬廠商)";
                text = text.Replace("[" + item.CheckerKind.ToString() + "]", "■");
                for (int i = 1; i <= 4; i++) text = text.Replace("[" + i.ToString() + "]", "□");
                table.GetRow(7).GetCell(1).SetText(text);

                if (!item.ImproveDeadline.HasValue)
                    table.GetRow(9).GetCell(1).SetText("限期改善完成日期：");
                else
                {
                    dt = item.ImproveDeadline.Value;
                    table.GetRow(9).GetCell(1).SetText(String.Format("限期改善完成日期：{0}/{1}/{2}", dt.Year - 1911, dt.Month, dt.Day));
                }
                table.GetRow(10).GetCell(0).SetText(item.CCRRealCheckCond);
                //table.GetRow(10).GetCell(1).SetText("110/11/11");//抽驗紀錄表的現場人員1.2./監造主任
                addSignatureFile(table.GetRow(10).GetCell(1), item.SupervisorUserSeq, signatureFileService);
                addSignatureFile(table.GetRow(10).GetCell(1), item.SupervisorDirectorSeq, signatureFileService);

                table.GetRow(13).GetCell(0).SetText(item.CauseAnalysis);//一、原因分析
                table.GetRow(15).GetCell(0).SetText(item.Improvement);//二、改善措施
                table.GetRow(17).GetCell(0).SetText(item.ProcessResult);//三、處理結果
                if (item.ImproveUserSeq.HasValue)
                {
                    //table.GetRow(18).GetCell(1).SetText("ImproveUser");//改善者或品管人員
                    addSignatureFile(table.GetRow(18).GetCell(1), item.ImproveUserSeq, signatureFileService);
                    dt = item.ModifyTime.Value;
                    table.GetRow(18).GetCell(3).SetText(String.Format("{0}/{1}/{2}", dt.Year - 1911, dt.Month, dt.Day));//改善完成日期：
                }
                //text = "[1]  □不符合，需再改善";//□不符合，需再改善
                //text = text.Replace("[" + item.ImproveAuditResult.ToString() + "]", "■");
                //for (int i = 1; i <= 2; i++) text = text.Replace("[" + i.ToString() + "]", "□");
                string text20 = "□不符合，需再改善";
                string text24 = "□符合，同意結案";
                if (item.ImproveAuditResult.HasValue) {
                    if (item.ImproveAuditResult.Value == 2)
                    {
                        text20 = "■不符合，需再改善";
                    }
                    else if (item.ImproveAuditResult.Value == 1)
                        text24 = "■符合，同意結案";
                }
                table.GetRow(20).GetCell(0).SetText(text20);
                table.GetRow(24).GetCell(0).SetText(text24);

                if (!item.ProcessTrackDate.HasValue)
                    table.GetRow(21).GetCell(0).SetText("計畫追蹤期限(日期)：");
                else
                {
                    dt = item.ProcessTrackDate.Value;
                    table.GetRow(21).GetCell(0).SetText(String.Format("計畫追蹤期限(日期)：{0}/{1}/{2}", dt.Year - 1911, dt.Month, dt.Day));
                }

                table.GetRow(22).GetCell(0).SetText("追蹤行動內容：" + item.TrackCont);
                if (item.ApproveUserSeq.HasValue)
                {
                    dt = item.ApproveDate.Value;
                    string dateTxt = String.Format("{0}/{1}/{2}", dt.Year - 1911, dt.Month, dt.Day);
                    if (item.ImproveAuditResult.Value == 1)
                    {//符合，同意結案
                        //table.GetRow(25).GetCell(1).SetText("ApproveUser");//檢查人員
                        addSignatureFile(table.GetRow(25).GetCell(1), item.ApproveUserSeq, signatureFileService);
                        table.GetRow(25).GetCell(3).SetText(dateTxt);//結案日期：
                    }
                    else
                    {//不符合，需再改善
                        //table.GetRow(23).GetCell(1).SetText("ApproveUser");//檢查人員
                        addSignatureFile(table.GetRow(23).GetCell(1), item.ApproveUserSeq, signatureFileService);
                        table.GetRow(23).GetCell(3).SetText(dateTxt);//填表日期：
                                                                         
                    }
                    
                }
            } catch (Exception) { }
            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();

            //轉檔
            if (filetype == 2)
            {
                //建立 word application instance
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                //開啟 word 檔案
                var wordDocument = appWord.Documents.Open(outFile);
                //pdf檔路徑
                string pdfFile = String.Format("{0}-{1}{2:00}{3:00}-{4}.pdf", "不符合事項報告", dt.Year - 1911, dt.Month, dt.Day, item.Seq);
                if (!Directory.Exists(folder + "pdf")) Directory.CreateDirectory(folder + "pdf");
                string filePatdownloadhName = Path.Combine(folder + "pdf", pdfFile );

                //匯出為 pdf
                wordDocument.ExportAsFixedFormat(filePatdownloadhName, WdExportFormat.wdExportFormatPDF);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();
            }
            else if (filetype == 3)
            {
                //建立 word application instance
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                //開啟 word 檔案
                var wordDocument = appWord.Documents.Open(outFile);
                //odt檔路徑
                string pdfFile = String.Format("{0}-{1}{2:00}{3:00}-{4}.odt", "不符合事項報告", dt.Year - 1911, dt.Month, dt.Day, item.Seq);
                if (!Directory.Exists(folder + "odt")) Directory.CreateDirectory(folder + "odt");
                string filePatdownloadhName = Path.Combine(folder + "odt", pdfFile);
                //匯出為 pdf
                wordDocument.SaveAs2(filePatdownloadhName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();
            }
        }
        //
        public Stream NgReportdownload(EngConstructionEngInfoVModel engItem, NgReportModel item, string folder, int filetype)
        {
            SignatureFileService signatureFileService = new SignatureFileService();
            string tempFile = CopyTemplateFile("表7-44-不符合事項報告.docx");
            DateTime dt = item.CreateTime.Value;
            string fn = String.Format("{0}-{1}{2:00}{3:00}-{4}.docx", "不符合事項報告", dt.Year - 1911, dt.Month, dt.Day, item.Seq);
            string outFile = Path.Combine(folder, fn);
            //List<ControlStVModel> items = recItem.items;
            //string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            try
            {
                XWPFTable table = doc.Tables[0];
                table.GetRow(0).GetCell(1).SetText(engItem.EngName);
                //string[] d = item.chsCreateTime.Split('/');
                table.GetRow(0).GetCell(3).SetText(String.Format("{0}年{1}月{2}日", dt.Year - 1911, dt.Month, dt.Day));
                table.GetRow(1).GetCell(1).SetText(engItem.organizerUnitName);
                table.GetRow(2).GetCell(1).SetText(engItem.SupervisorUnitName);
                table.GetRow(4).GetCell(1).SetText(item.CCRPosDesc);

                string text = "[1]1.施工設備 [2]2.材料設備 [3]3.施工成品 [4]4.施工作業 [5]5.文件、紀錄";
                text = text.Replace("[" + item.CheckItemKind.ToString() + "]", "■");
                for (int i = 1; i <= 5; i++) text = text.Replace("[" + i.ToString() + "]", "□");
                table.GetRow(5).GetCell(1).SetText(text);

                text = "□一般缺失立即改善  □一般缺失追蹤改善";
                if (item.IncompKind.HasValue)
                {
                    if (item.IncompKind == 1)
                        text = "■一般缺失立即改善  □一般缺失追蹤改善";
                    if (item.IncompKind == 2)
                        text = "□一般缺失立即改善  ■一般缺失追蹤改善";
                }
                /*text = text.Replace("[" + item.IncompKind.ToString() + "]", "■");
                if (item.IncompKind == 2 || item.IncompKind == 4)
                    text = text.Replace("[1]", "■");
                for (int i = 1; i <= 4; i++) text = text.Replace("[" + i.ToString() + "]", "□");*/
                table.GetRow(6).GetCell(1).SetText(text);

                text = "[1]施工抽查(監造單位)  [2]自主檢查(承攬廠商)";
                text = text.Replace("[" + item.CheckerKind.ToString() + "]", "■");
                for (int i = 1; i <= 4; i++) text = text.Replace("[" + i.ToString() + "]", "□");
                table.GetRow(7).GetCell(1).SetText(text);

                if (!item.ImproveDeadline.HasValue)
                    table.GetRow(9).GetCell(1).SetText("限期改善完成日期：");
                else
                {
                    dt = item.ImproveDeadline.Value;
                    table.GetRow(9).GetCell(1).SetText(String.Format("限期改善完成日期：{0}/{1}/{2}", dt.Year - 1911, dt.Month, dt.Day));
                }
                table.GetRow(10).GetCell(0).SetText(item.CCRRealCheckCond);
                //table.GetRow(10).GetCell(1).SetText("110/11/11");//抽驗紀錄表的現場人員1.2./監造主任
                addSignatureFile(table.GetRow(10).GetCell(1), item.SupervisorUserSeq, signatureFileService);
                addSignatureFile(table.GetRow(10).GetCell(1), item.SupervisorDirectorSeq, signatureFileService);

                table.GetRow(13).GetCell(0).SetText(item.CauseAnalysis);//一、原因分析
                table.GetRow(15).GetCell(0).SetText(item.Improvement);//二、改善措施
                table.GetRow(17).GetCell(0).SetText(item.ProcessResult);//三、處理結果
                if (item.ImproveUserSeq.HasValue)
                {
                    //table.GetRow(18).GetCell(1).SetText("ImproveUser");//改善者或品管人員
                    addSignatureFile(table.GetRow(18).GetCell(1), item.ImproveUserSeq, signatureFileService);
                    dt = item.ModifyTime.Value;
                    table.GetRow(18).GetCell(3).SetText(String.Format("{0}/{1}/{2}", dt.Year - 1911, dt.Month, dt.Day));//改善完成日期：
                }
                //text = "[1]  □不符合，需再改善";//□不符合，需再改善
                //text = text.Replace("[" + item.ImproveAuditResult.ToString() + "]", "■");
                //for (int i = 1; i <= 2; i++) text = text.Replace("[" + i.ToString() + "]", "□");
                string text20 = "□不符合，需再改善";
                string text24 = "□符合，同意結案";
                if (item.ImproveAuditResult.HasValue)
                {
                    if (item.ImproveAuditResult.Value == 2)
                    {
                        text20 = "■不符合，需再改善";
                    }
                    else if (item.ImproveAuditResult.Value == 1)
                        text24 = "■符合，同意結案";
                }
                table.GetRow(20).GetCell(0).SetText(text20);
                table.GetRow(24).GetCell(0).SetText(text24);

                if (!item.ProcessTrackDate.HasValue)
                    table.GetRow(21).GetCell(0).SetText("計畫追蹤期限(日期)：");
                else
                {
                    dt = item.ProcessTrackDate.Value;
                    table.GetRow(21).GetCell(0).SetText(String.Format("計畫追蹤期限(日期)：{0}/{1}/{2}", dt.Year - 1911, dt.Month, dt.Day));
                }

                table.GetRow(22).GetCell(0).SetText("追蹤行動內容：" + item.TrackCont);
                if (item.ApproveUserSeq.HasValue)
                {
                    dt = item.ApproveDate.Value;
                    string dateTxt = String.Format("{0}/{1}/{2}", dt.Year - 1911, dt.Month, dt.Day);
                    if (item.ImproveAuditResult.Value == 1)
                    {//符合，同意結案
                        //table.GetRow(25).GetCell(1).SetText("ApproveUser");//檢查人員
                        addSignatureFile(table.GetRow(25).GetCell(1), item.ApproveUserSeq, signatureFileService);
                        table.GetRow(25).GetCell(3).SetText(dateTxt);//結案日期：
                    }
                    else
                    {//不符合，需再改善
                        //table.GetRow(23).GetCell(1).SetText("ApproveUser");//檢查人員
                        addSignatureFile(table.GetRow(23).GetCell(1), item.ApproveUserSeq, signatureFileService);
                        table.GetRow(23).GetCell(3).SetText(dateTxt);//填表日期：

                    }

                }
            }
            catch (Exception) { }
            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();

            //轉檔
            if (filetype == 2)
            {
                //建立 word application instance
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                //開啟 word 檔案
                var wordDocument = appWord.Documents.Open(outFile);
                //pdf檔路徑
                string[] targetfile = outFile.Split('.');
                string targetpdf = targetfile[0] + ".pdf";
                //匯出為 pdf
                wordDocument.ExportAsFixedFormat(targetpdf, WdExportFormat.wdExportFormatPDF);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();

                Stream iStream = new FileStream(targetpdf, FileMode.Open, FileAccess.Read, FileShare.Read);
                return iStream;
            }
            else if (filetype == 3)
            {
                //建立 word application instance
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                //開啟 word 檔案
                var wordDocument = appWord.Documents.Open(outFile);
                //odt檔路徑
                string[] targetfile = outFile.Split('.');
                string targetodt = targetfile[0] + ".odt";
                //匯出為 pdf
                wordDocument.SaveAs2(targetodt, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();

                Stream iStream = new FileStream(targetodt, FileMode.Open, FileAccess.Read, FileShare.Read);
                return iStream;
            }
            else
            {
                Stream iStream = new FileStream(outFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return iStream;

            }

        }


        private void addSignatureFile(XWPFTableCell cell, int? userSeq, SignatureFileService service)
        {
            if (!userSeq.HasValue) return;
            string fileName = service.GetFileNameByUser(userSeq.Value);
            if (fileName == null) return;

            string filePath = Utils.GetSignatureFile(fileName);
            var widthPic = NPOI.Util.Units.ToEMU(36 * 2.8);// (int)((double)3000 / 587 * 38.4 * 9525);
            var heightPic = NPOI.Util.Units.ToEMU(12 * 2.8);// (int)((double)900 / 587 * 38.4 * 9525);
            if (System.IO.File.Exists(filePath))
            {
                FileStream img = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                XWPFParagraph p = cell.AddParagraph();
                XWPFRun run = p.CreateRun();
                run.AddPicture(img, (int)NPOI.XWPF.UserModel.PictureType.JPEG, filePath, widthPic, heightPic);
                //run.AddCarriageReturn();
                //cell.AddParagraph().CreateRun().SetText("");
                img.Close();
            }
        }
        //表7-45-NCR程序追蹤改善表
        public void NCRReport(EngConstructionEngInfoVModel engItem, NcrModel item, string folder,int filetype)
        {
            SignatureFileService signatureFileService = new SignatureFileService();
            string tempFile = CopyTemplateFile("表7-45-NCR程序追蹤改善表.docx");
            DateTime dt = item.CreateTime.Value;
            string fn = String.Format("{0}-{1}{2:00}{3:00}-{4}.docx", "NCR程序追蹤改善表", dt.Year - 1911, dt.Month, dt.Day, item.Seq);
            string outFile = Path.Combine(folder, fn);
            //List<ControlStVModel> items = recItem.items;
            //string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            try { 
                XWPFTable table = doc.Tables[0];

                table.GetRow(0).GetCell(1).SetText(engItem.EngName);
                //string[] d = item.chsCreateTime.Split('/');
                table.GetRow(0).GetCell(3).SetText(String.Format("{0}年{1}月{2}日", dt.Year - 1911, dt.Month, dt.Day));
                table.GetRow(1).GetCell(1).SetText(engItem.organizerUnitName);
                table.GetRow(2).GetCell(1).SetText(engItem.SupervisorUnitName);

                string text = "□一般缺失立即改善  □一般缺失追蹤改善";
                if (item.IncompKind.HasValue)
                {
                    if (item.IncompKind == 1)
                        text = "■一般缺失立即改善  □一般缺失追蹤改善";
                    if (item.IncompKind == 2)
                        text = "□一般缺失立即改善  ■一般缺失追蹤改善";
                }
                table.GetRow(4).GetCell(1).SetText(text);

                //table.GetRow(6).GetCell(1).SetText("限期改善完成日期：");
                table.GetRow(7).GetCell(0).SetText(item.MissingItem);//一、缺失事項
                table.GetRow(10).GetCell(0).SetText(item.CauseAnalysis);//二、原因分析
                table.GetRow(13).GetCell(0).SetText(item.CorrectiveAction);//(一)矯正措施
                table.GetRow(15).GetCell(0).SetText(item.PreventiveAction);//(二)預防措施
                table.GetRow(17).GetCell(0).SetText(item.CorrPrevImproveResult);//四、矯正預防措施與改善結果
                //table.GetRow(7).GetCell(1).SetText("110/11/11");//檢查人員簽名
                addSignatureFile(table.GetRow(7).GetCell(1), item.ImproveUserSeq, signatureFileService);

                //table.GetRow(18).GetCell(1).SetText("ImproveUser");//改善者或品管人員
                addSignatureFile(table.GetRow(18).GetCell(1), item.ImproveUserSeq, signatureFileService);
                if (item.ModifyTime.HasValue)
                {
                    dt = item.ModifyTime.Value;
                    table.GetRow(18).GetCell(3).SetText(String.Format("{0}/{1}/{2}", dt.Year - 1911, dt.Month, dt.Day));//改善完成日期：
                }
                /*string text = "[1]  符合     [2] 需再行改善";
                text = text.Replace("[" + item.ImproveAuditResult.ToString() + "]", "■");
                for (int i = 1; i <= 2; i++) text = text.Replace("[" + i.ToString() + "]", "□");
                table.GetRow(20).GetCell(0).SetText(text);*/
                string text20 = "□不符合，需再改善";
                string text24 = "□符合，同意結案";
                if (item.ImproveAuditResult.HasValue)
                {
                    if (item.ImproveAuditResult.Value == 2)
                    {
                        text20 = "■不符合，需再改善";
                    }
                    else if (item.ImproveAuditResult.Value == 1)
                        text24 = "■符合，同意結案";
                }
                table.GetRow(20).GetCell(0).SetText(text20);
                table.GetRow(24).GetCell(0).SetText(text24);

                if (!item.ProcessTrackDate.HasValue)
                    table.GetRow(21).GetCell(0).SetText("計畫追蹤日期：");
                else
                {
                    dt = item.ProcessTrackDate.Value;
                    table.GetRow(21).GetCell(0).SetText(String.Format("計畫追蹤日期：{0}/{1}/{2}", dt.Year - 1911, dt.Month, dt.Day));
                }

                table.GetRow(22).GetCell(0).SetText("追蹤行動內容：" + item.TrackCont);

                if (item.ApproveUserSeq.HasValue)
                {
                    dt = item.ApproveDate.Value;
                    string dateTxt = String.Format("{0}/{1}/{2}", dt.Year - 1911, dt.Month, dt.Day);
                    if (item.ImproveAuditResult.Value == 1)
                    {//符合，同意結案
                        //table.GetRow(25).GetCell(1).SetText("ApproveUser");//檢查人員
                        addSignatureFile(table.GetRow(25).GetCell(1), item.ApproveUserSeq, signatureFileService);
                        table.GetRow(25).GetCell(3).SetText(dateTxt);//結案日期：
                    }
                    else
                    {//不符合，需再改善
                        //table.GetRow(23).GetCell(1).SetText("ApproveUser");//檢查人員
                        addSignatureFile(table.GetRow(23).GetCell(1), item.ApproveUserSeq, signatureFileService);
                        table.GetRow(23).GetCell(3).SetText(dateTxt);//填表日期：

                    }

                }
            } catch (Exception) { }
            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();

            //轉檔
            if (filetype == 2)
            {
                //建立 word application instance
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                //開啟 word 檔案
                var wordDocument = appWord.Documents.Open(outFile);
                //pdf檔路徑
                string pdfFile = String.Format("{0}-{1}{2:00}{3:00}-{4}.pdf", "NCR程序追蹤改善表", dt.Year - 1911, dt.Month, dt.Day, item.Seq);
                if (!Directory.Exists(folder + "pdf")) Directory.CreateDirectory(folder + "pdf");
                string filePatdownloadhName = Path.Combine(folder + "pdf", pdfFile);

                //匯出為 pdf
                wordDocument.ExportAsFixedFormat(filePatdownloadhName, WdExportFormat.wdExportFormatPDF);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();
            }
            else if (filetype == 3)
            {
                //建立 word application instance
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                //開啟 word 檔案
                var wordDocument = appWord.Documents.Open(outFile);
                //odt檔路徑
                string pdfFile = String.Format("{0}-{1}{2:00}{3:00}-{4}.odt", "NCR程序追蹤改善表", dt.Year - 1911, dt.Month, dt.Day, item.Seq);
                if (!Directory.Exists(folder + "odt")) Directory.CreateDirectory(folder + "odt");
                string filePatdownloadhName = Path.Combine(folder + "odt", pdfFile);
                //匯出為 pdf
                wordDocument.SaveAs2(filePatdownloadhName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();
            }

        }

        //表7-45-NCR程序追蹤改善表
        public Stream NCRReportdownload(EngConstructionEngInfoVModel engItem, NcrModel item, string folder, int filetype)
        {
            SignatureFileService signatureFileService = new SignatureFileService();
            string tempFile = CopyTemplateFile("表7-45-NCR程序追蹤改善表.docx");
            DateTime dt = item.CreateTime.Value;
            string fn = String.Format("{0}-{1}{2:00}{3:00}-{4}.docx", "NCR程序追蹤改善表", dt.Year - 1911, dt.Month, dt.Day, item.Seq);
            string outFile = Path.Combine(folder, fn);
            //List<ControlStVModel> items = recItem.items;
            //string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            try
            {
                XWPFTable table = doc.Tables[0];

                table.GetRow(0).GetCell(1).SetText(engItem.EngName);
                //string[] d = item.chsCreateTime.Split('/');
                table.GetRow(0).GetCell(3).SetText(String.Format("{0}年{1}月{2}日", dt.Year - 1911, dt.Month, dt.Day));
                table.GetRow(1).GetCell(1).SetText(engItem.organizerUnitName);
                table.GetRow(2).GetCell(1).SetText(engItem.SupervisorUnitName);

                string text = "□一般缺失立即改善  □一般缺失追蹤改善";
                if (item.IncompKind.HasValue)
                {
                    if (item.IncompKind == 1)
                        text = "■一般缺失立即改善  □一般缺失追蹤改善";
                    if (item.IncompKind == 2)
                        text = "□一般缺失立即改善  ■一般缺失追蹤改善";
                }
                table.GetRow(4).GetCell(1).SetText(text);

                //table.GetRow(6).GetCell(1).SetText("限期改善完成日期：");
                table.GetRow(7).GetCell(0).SetText(item.MissingItem);//一、缺失事項
                table.GetRow(10).GetCell(0).SetText(item.CauseAnalysis);//二、原因分析
                table.GetRow(13).GetCell(0).SetText(item.CorrectiveAction);//(一)矯正措施
                table.GetRow(15).GetCell(0).SetText(item.PreventiveAction);//(二)預防措施
                table.GetRow(17).GetCell(0).SetText(item.CorrPrevImproveResult);//四、矯正預防措施與改善結果
                //table.GetRow(7).GetCell(1).SetText("110/11/11");//檢查人員簽名
                addSignatureFile(table.GetRow(7).GetCell(1), item.ImproveUserSeq, signatureFileService);

                //table.GetRow(18).GetCell(1).SetText("ImproveUser");//改善者或品管人員
                addSignatureFile(table.GetRow(18).GetCell(1), item.ImproveUserSeq, signatureFileService);
                if (item.ModifyTime.HasValue)
                {
                    dt = item.ModifyTime.Value;
                    table.GetRow(18).GetCell(3).SetText(String.Format("{0}/{1}/{2}", dt.Year - 1911, dt.Month, dt.Day));//改善完成日期：
                }
                /*string text = "[1]  符合     [2] 需再行改善";
                text = text.Replace("[" + item.ImproveAuditResult.ToString() + "]", "■");
                for (int i = 1; i <= 2; i++) text = text.Replace("[" + i.ToString() + "]", "□");
                table.GetRow(20).GetCell(0).SetText(text);*/
                string text20 = "□不符合，需再改善";
                string text24 = "□符合，同意結案";
                if (item.ImproveAuditResult.HasValue)
                {
                    if (item.ImproveAuditResult.Value == 2)
                    {
                        text20 = "■不符合，需再改善";
                    }
                    else if (item.ImproveAuditResult.Value == 1)
                        text24 = "■符合，同意結案";
                }
                table.GetRow(20).GetCell(0).SetText(text20);
                table.GetRow(24).GetCell(0).SetText(text24);

                if (!item.ProcessTrackDate.HasValue)
                    table.GetRow(21).GetCell(0).SetText("計畫追蹤日期：");
                else
                {
                    dt = item.ProcessTrackDate.Value;
                    table.GetRow(21).GetCell(0).SetText(String.Format("計畫追蹤日期：{0}/{1}/{2}", dt.Year - 1911, dt.Month, dt.Day));
                }

                table.GetRow(22).GetCell(0).SetText("追蹤行動內容：" + item.TrackCont);

                if (item.ApproveUserSeq.HasValue)
                {
                    dt = item.ApproveDate.Value;
                    string dateTxt = String.Format("{0}/{1}/{2}", dt.Year - 1911, dt.Month, dt.Day);
                    if (item.ImproveAuditResult.Value == 1)
                    {//符合，同意結案
                        //table.GetRow(25).GetCell(1).SetText("ApproveUser");//檢查人員
                        addSignatureFile(table.GetRow(25).GetCell(1), item.ApproveUserSeq, signatureFileService);
                        table.GetRow(25).GetCell(3).SetText(dateTxt);//結案日期：
                    }
                    else
                    {//不符合，需再改善
                        //table.GetRow(23).GetCell(1).SetText("ApproveUser");//檢查人員
                        addSignatureFile(table.GetRow(23).GetCell(1), item.ApproveUserSeq, signatureFileService);
                        table.GetRow(23).GetCell(3).SetText(dateTxt);//填表日期：

                    }

                }
            }
            catch (Exception) { }
            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();

            //轉檔
            if (filetype == 2)
            {
                //建立 word application instance
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                //開啟 word 檔案
                var wordDocument = appWord.Documents.Open(outFile);
                //pdf檔路徑
                string[] targetfile = outFile.Split('.');
                string targetpdf = targetfile[0] + ".pdf";
                //匯出為 pdf
                wordDocument.ExportAsFixedFormat(targetpdf, WdExportFormat.wdExportFormatPDF);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();

                Stream iStream = new FileStream(targetpdf, FileMode.Open, FileAccess.Read, FileShare.Read);
                return iStream;
            }
            else if (filetype == 3)
            {
                //建立 word application instance
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                //開啟 word 檔案
                var wordDocument = appWord.Documents.Open(outFile);
                //odt檔路徑
                string[] targetfile = outFile.Split('.');
                string targetodt = targetfile[0] + ".odt";
                //匯出為 pdf
                wordDocument.SaveAs2(targetodt, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();

                Stream iStream = new FileStream(targetodt, FileMode.Open, FileAccess.Read, FileShare.Read);
                return iStream;
            }
            else
            {
                Stream iStream = new FileStream(outFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return iStream;

            }
        }


        //表7-46-改善照片
        public void PhotoReport(EngConstructionEngInfoVModel engItem, NgReportModel item, string folder,int filetype)
        {
            string tempFile = CopyTemplateFile("表7-46-改善照片.docx");
            string filePath = Utils.GetEngMainFolder(engItem.Seq);
            var widthPic = (int)((double)6000 / 587 * 38.4 * 9525);
            var heightPic = (int)((double)3600 / 587 * 38.4 * 9525);

            foreach (NgReportPhotoGroupModel group in item.photoGroups)
            {
                DateTime dt = item.CreateTime.Value;
                string fn = String.Format("{0}-{1}{2:00}{3:00}-{4}-{5}.docx", "改善照片", dt.Year - 1911, dt.Month, dt.Day, group.Value, item.Seq);
                string outFile = Path.Combine(folder, fn);

                FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
                FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
                XWPFDocument doc = new XWPFDocument(fs);
                try { 
                    XWPFTable table = doc.Tables[0];
                    table.GetRow(0).GetCell(0).SetText(engItem.EngName);
                    
                    table.GetRow(1).GetCTRow().AddNewTrPr().AddNewTrHeight().val = (ulong)(4000);
                    table.GetRow(2).GetCTRow().AddNewTrPr().AddNewTrHeight().val = (ulong)(4000);
                    table.GetRow(3).GetCTRow().AddNewTrPr().AddNewTrHeight().val = (ulong)(4000);
                    XWPFTableCell cell;
                    foreach (UploadFileModel m in group.photos)
                    {
                        FileStream img = new FileStream(Path.Combine(filePath, m.UniqueFileName), FileMode.Open, FileAccess.Read);
                        if (m.ItemGroup == 0)
                        {
                            cell = table.GetRow(1).GetCell(1);
                        }
                        else if (m.ItemGroup == 1)
                        {
                            cell = table.GetRow(2).GetCell(1);
                        }
                        else
                        {
                            cell = table.GetRow(3).GetCell(1);
                        }
                        XWPFParagraph p = cell.AddParagraph();
                        XWPFRun run = p.CreateRun();
                        run.AddPicture(img, (int)NPOI.XWPF.UserModel.PictureType.JPEG, m.UniqueFileName, widthPic, heightPic);
                        //run.AddCarriageReturn();
                        cell.AddParagraph().CreateRun().SetText(m.Memo);
                        img.Close();
                    }
                } catch (Exception) { }
                
                doc.Write(fsOut);
                fsOut.Close();
                doc.Close();
                fs.Close();

                //轉檔
                if (filetype == 2)
                {
                    //建立 word application instance
                    Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                    //開啟 word 檔案
                    var wordDocument = appWord.Documents.Open(outFile);
                    //pdf檔路徑
                    string pdfFile = String.Format("{0}-{1}{2:00}{3:00}-{4}-{5}.pdf", "改善照片", dt.Year - 1911, dt.Month, dt.Day, group.Value, item.Seq);
                    if (!Directory.Exists(folder + "pdf")) Directory.CreateDirectory(folder + "pdf");
                    string filePatdownloadhName = Path.Combine(folder + "pdf", pdfFile);

                    //匯出為 pdf
                    wordDocument.ExportAsFixedFormat(filePatdownloadhName, WdExportFormat.wdExportFormatPDF);

                    //關閉 word 檔
                    wordDocument.Close();
                    //結束 word
                    appWord.Quit();
                }
                else if (filetype == 3)
                {
                    //建立 word application instance
                    Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                    //開啟 word 檔案
                    var wordDocument = appWord.Documents.Open(outFile);
                    //odt檔路徑
                    string pdfFile = String.Format("{0}-{1}{2:00}{3:00}-{4}-{5}.odt", "改善照片", dt.Year - 1911, dt.Month, dt.Day, group.Value, item.Seq);
                    if (!Directory.Exists(folder + "odt")) Directory.CreateDirectory(folder + "odt");
                    string filePatdownloadhName = Path.Combine(folder + "odt", pdfFile);
                    //匯出為 pdf
                    wordDocument.SaveAs2(filePatdownloadhName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);

                    //關閉 word 檔
                    wordDocument.Close();
                    //結束 word
                    appWord.Quit();
                }

            }
        }

        public Stream PhotoReportdownload(EngConstructionEngInfoVModel engItem, NgReportModel item, string folder, int filetype,int num)
        {
            string tempFile = CopyTemplateFile("表7-46-改善照片.docx");
            string filePath = Utils.GetEngMainFolder(engItem.Seq);
            var widthPic = (int)((double)6000 / 587 * 38.4 * 9525);
            var heightPic = (int)((double)3600 / 587 * 38.4 * 9525);
            NgReportPhotoGroupModel group = item.photoGroups[num];

            DateTime dt = item.CreateTime.Value;
            string fn = String.Format("{0}-{1}{2:00}{3:00}-{4}-{5}.docx", "改善照片", dt.Year - 1911, dt.Month, dt.Day, group.Value, item.Seq);
            string outFile = Path.Combine(folder, fn);

            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            try
            {
                XWPFTable table = doc.Tables[0];
                table.GetRow(0).GetCell(0).SetText(engItem.EngName);

                table.GetRow(1).GetCTRow().AddNewTrPr().AddNewTrHeight().val = (ulong)(4000);
                table.GetRow(2).GetCTRow().AddNewTrPr().AddNewTrHeight().val = (ulong)(4000);
                table.GetRow(3).GetCTRow().AddNewTrPr().AddNewTrHeight().val = (ulong)(4000);
                XWPFTableCell cell;
                foreach (UploadFileModel m in group.photos)
                {
                    FileStream img = new FileStream(Path.Combine(filePath, m.UniqueFileName), FileMode.Open, FileAccess.Read);
                    if (m.ItemGroup == 0)
                    {
                        cell = table.GetRow(1).GetCell(1);
                    }
                    else if (m.ItemGroup == 1)
                    {
                        cell = table.GetRow(2).GetCell(1);
                    }
                    else
                    {
                        cell = table.GetRow(3).GetCell(1);
                    }
                    XWPFParagraph p = cell.AddParagraph();
                    XWPFRun run = p.CreateRun();
                    run.AddPicture(img, (int)NPOI.XWPF.UserModel.PictureType.JPEG, m.UniqueFileName, widthPic, heightPic);
                    //run.AddCarriageReturn();
                    cell.AddParagraph().CreateRun().SetText(m.Memo);
                    img.Close();
                }
            }
            catch (Exception) { }

            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();

            //轉檔
            if (filetype == 2)
            {
                //建立 word application instance
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                //開啟 word 檔案
                var wordDocument = appWord.Documents.Open(outFile);
                //pdf檔路徑
                string[] targetfile = outFile.Split('.');
                string targetpdf = targetfile[0] + ".pdf";

                //匯出為 pdf
                wordDocument.ExportAsFixedFormat(targetpdf, WdExportFormat.wdExportFormatPDF);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();
                Stream iStream = new FileStream(targetpdf, FileMode.Open, FileAccess.Read, FileShare.Read);
                return iStream;
            }
            else if (filetype == 3)
            {
                //建立 word application instance
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                //開啟 word 檔案
                var wordDocument = appWord.Documents.Open(outFile);
                //odt檔路徑
                string[] targetfile = outFile.Split('.');
                string targetodt = targetfile[0] + ".odt";
                //匯出為 pdf
                wordDocument.SaveAs2(targetodt, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();
                Stream iStream = new FileStream(targetodt, FileMode.Open, FileAccess.Read, FileShare.Read);
                return iStream;
            }
            else
            {
                Stream iStream = new FileStream(outFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return iStream;

            }
        }


        //for edit ===========================================================
        public JsonResult GetEngItem(int id)
        {
            List<EngConstructionEngInfoVModel> items = constCheckRecImproveService.GetEngMainByEngConstructionSeq<EngConstructionEngInfoVModel>(id);
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
                    result = 1,
                    message = "讀取資料錯誤"
                });
            }
        }

        //已有檢驗單之檢驗項目
        virtual public JsonResult GetRecCheckTypeOption(int id)
        {
            List<SelectIntOptionModel> items = constCheckRecImproveService.GetRecCheckTypeOption<SelectIntOptionModel>(id);
            return Json(items);
        }
        //檢驗單清單 單一項目
        virtual public JsonResult GetRecOptionByCheckType(int constructionSeq, int checkTypeSeq)
        {
            List<ConstCheckRecOptionVModel> items = constCheckRecImproveService.GetListByCheckType<ConstCheckRecOptionVModel>(constructionSeq, checkTypeSeq);
            return Json(items);
        }
        //檢驗單清單 s20230920
        virtual public JsonResult GetRecOptionByCheckType1(int id, int checkTypeSeq, int itemSeq, string itemName)
        {
            List<ConstCheckRecOption1VModel> items = constCheckRecImproveService.GetListByCheckType1<ConstCheckRecOption1VModel>(id, checkTypeSeq, itemSeq);
            SuggestionDectection.InsertAfterDetectSuggestion(1, "混凝土", itemName, itemSeq);
            return Json(items);
        }
        //檢驗單表頭
        public JsonResult GetRec(int recSeq)
        {
            List<ConstCheckRecModel> items = new ConstCheckRecService().GetItem<ConstCheckRecModel>(recSeq);

            if (items.Count > 0)
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
                    result = 1,
                    message = "無資料"
                });
            }
        }
        //檢驗單明細
        public JsonResult GetRecResult(ConstCheckRecModel rec)
        {
            List<ConstCheckRecImproveVModel> reports = constCheckRecImproveService.GetItemByConstCheckRecSeq<ConstCheckRecImproveVModel>(rec.Seq);
            if (reports.Count == 0)
                reports.Add(new ConstCheckRecImproveVModel() { ConstCheckRecSeq = rec.Seq });

            ConstCheckRecResultService service = new ConstCheckRecResultService();
            List<ControlStVModel> items = new List<ControlStVModel>();
            if (rec.CCRCheckType1 == 1)
            {//施工抽查清單
                items = service.GetConstCheckRecResult<ControlStVModel>(rec.Seq);
            }
            else if (rec.CCRCheckType1 == 2)
            {//設備運轉測試清單
                items = service.GetEquOperRecResult<ControlStVModel>(rec.Seq);
            }
            else if (rec.CCRCheckType1 == 3)
            {//職業安全衛生清單
                items = service.GetOccuSafeHealthRecResult<ControlStVModel>(rec.Seq);
            }
            else if (rec.CCRCheckType1 == 4)
            {//環境保育清單
                items = service.GetEnvirConsRecResult<ControlStVModel>(rec.Seq);
            }
            if (items.Count > 0)
            {
                JoinCell(items);
                bool show = false;
                foreach(ControlStVModel m in items)
                {
                    if(m.rowSpan>0)
                    {
                        show = (m.CCRCheckResult==2);
                    }
                    m.rowShow = show;
                }
                return Json(new
                {
                    result = 0,
                    item = items,
                    report = reports[0]
                });
            }
            else
            {
                return Json(new
                {
                    result = 1,
                    message = "無資料"
                });
            }
        }

        //更新 不符合事項報告
        public JsonResult UpdateReport(ConstCheckRecModel recItem, ConstCheckRecImproveVModel report)
        {
            report.updateDate();
            if (constCheckRecImproveService.Update(recItem, report))
            {
                return Json(new
                {
                    result = 0,
                    reportSeq = report.Seq,
                    message = "儲存成功"
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

        //不符合事項報告 確認
        public JsonResult ReportConfirm(int id)
        {
            if (constCheckRecImproveService.ReportConfirm(id, 1)==1)
            {
                return Json(new
                {
                    result = 0,
                    message = "確認成功"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "確認失敗"
                });
            }
        }

        //改善照片 上傳
        public JsonResult PhotoUpload(int engMain, int improveSeq, int ctlSeq, byte photoGroup, string photoDesc)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                ConstCheckRecImproveFileModel model = new ConstCheckRecImproveFileModel();
                model.ConstCheckRecImproveSeq = improveSeq;
                model.ControllStSeq = ctlSeq;
                model.ItemGroup = photoGroup;
                model.Memo = photoDesc;
                try
                {
                    if (SaveFile(file, engMain, model, "RecImprove-"))
                    {
                        if (new ConstCheckRecImproveFileService().Add(model))
                        {
                            return Json(new
                            {
                                result = 0,
                                message = "上傳成功"
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                result = -1,
                                message = "上傳失敗",
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


        //NCR程序追蹤改善表
        public JsonResult GetNCR(ConstCheckRecModel rec)
        {
            List<NcrVModel> items = new NcrService().GetItemByConstCheckRecSeq<NcrVModel>(rec.Seq);
            if (items.Count == 0)
                items.Add(new NcrVModel() { ConstCheckRecSeq = rec.Seq });

            return Json(new
            {
                result = 0,
                ncr = items[0]
            });
        }
        //更新 NCR
        public JsonResult UpdateNCR(ConstCheckRecModel recItem, NcrVModel ncr)
        {
            ncr.updateDate();
            if (new NcrService().Update(recItem, ncr))
            {
                return Json(new
                {
                    result = 0,
                    ncrSeq = ncr.Seq,
                    message = "儲存成功"
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
        //NCR 確認
        public JsonResult NCRConfirm(int id)
        {
            if (new NcrService().NCRConfirm(id, 1) == 1)
            {
                return Json(new
                {
                    result = 0,
                    message = "確認成功"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "確認失敗"
                });
            }
        }

        //取得 照片群組清單
        public JsonResult GetImgGroupOption(int recSeq, int checkType)
        {
            List<SelectIntOptionModel> items = new List<SelectIntOptionModel>();
            if (checkType > 0 && checkType < 5)
            {
                if (checkType == 1)
                {//施工抽查
                    items = constCheckRecImproveService.GetConstCheckPhotoGroupOption<SelectIntOptionModel>(recSeq);
                }
                else if (checkType == 2)
                {//設備運轉
                    items = constCheckRecImproveService.GetEquOperPhotoGroupOption<SelectIntOptionModel>(recSeq);
                }
                else if (checkType == 3)
                {//職業安全衛生
                    items = constCheckRecImproveService.GetOccuSafeHealthGroupOption<SelectIntOptionModel>(recSeq);
                }
                else if (checkType == 4)
                {//環境保育
                    items = constCheckRecImproveService.GetEnvirConsGroupOption<SelectIntOptionModel>(recSeq);
                }

                return Json(new
                {
                    result = 0,
                    item = items
                });
            }
            else
            {
                return Json(new
                {
                    result = 1,
                    message = "讀取資料錯誤"
                });
            }
        }
        //取得 照片清單 
        public JsonResult GetRecResultPhotos(int engMain, int recSeq, int ctlSeq)
        {
            List<UploadFileModel> items = new ConstCheckRecImproveFileService().GetPhotos<UploadFileModel>(recSeq, ctlSeq);
            //string filePath = Utils.GetEngMainFolder(engMain);
            //foreach(UploadFileModel item in items)
            //{
            //    item.UniqueFileName = String.Format("../FileUploads/Eng/{0}/{1}", engMain, item.UniqueFileName);
            //}

            return Json(items);
        }
        //檢驗照片 刪除
        public JsonResult DelResultPhoto(int engMain, int fileSeq)
        {
            ConstCheckRecImproveFileService service = new ConstCheckRecImproveFileService();
            List<ConstCheckRecImproveFileModel> items = service.GetItem(fileSeq);
            if (items.Count > 0)
            {
                ConstCheckRecImproveFileModel model = items[0];
                try
                {
                    if (service.Del(model))
                    {
                        DelFile(engMain, model);
                        return Json(new
                        {
                            result = 0,
                            message = "刪除成功"
                        });
                    }
                    return Json(new
                    {
                        result = -1,
                        message = "刪除失敗",
                    });
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                    return Json(new
                    {
                        result = -1,
                        message = "刪除失敗",
                    });
                }
            }

            return Json(new
            {
                result = 0,
                message = "刪除成功"
            });
        }

        // 共用 =============================
        //合併儲存格前置處裡
        private void JoinCell(List<ControlStVModel> items)
        {
            if (items.Count == 0) return;

            string checkItem = "----";
            string checkItemStd1 = "----";
            int gInx = -1, gCount = 0;
            int gInxStd1 = -1, gCountStd1 = 0;
            for (int i = 0; i < items.Count; i++)
            {
                string key = items[i].CheckItem1 + items[i].CheckItem2;
                string keyStd1 = items[i].Stand1;
                if (checkItem != key)
                {
                    if (gInx != -1)
                    {
                        items[gInx].rowSpan = gCount;
                    }
                    gInx = i;
                    gCount = 1;
                    checkItem = key;

                    if (gInxStd1 != -1)
                    {
                        items[gInxStd1].rowSpanStd1 = gCountStd1;
                    }
                    gInxStd1 = i;
                    gCountStd1 = 1;
                    checkItemStd1 = keyStd1;
                }
                else
                {
                    items[i].rowSpan = 0;
                    gCount++;

                    if (checkItemStd1 != keyStd1)
                    {
                        if (gInxStd1 != -1)
                        {
                            items[gInxStd1].rowSpanStd1 = gCountStd1;
                        }
                        gInxStd1 = i;
                        gCountStd1 = 1;
                        checkItemStd1 = keyStd1;
                    }
                    else
                    {
                        items[i].rowSpanStd1 = 0;
                        gCountStd1++;
                    }
                }
            }
            items[gInx].rowSpan = gCount;
            items[gInxStd1].rowSpanStd1 = gCountStd1;
        }

        private bool SaveFile(HttpPostedFileBase file, int engMainSeq, ConstCheckRecImproveFileModel m, string fileHeader)
        {
            try
            {
                string filePath = Utils.GetEngMainFolder(engMainSeq);
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
                uniqueFileName = String.Format("{0}{1}{2}", fileHeader, Guid.NewGuid(), originFileName.Substring(inx));
                
                string fullPath = Path.Combine(filePath, uniqueFileName);
                file.SaveAs(fullPath);

                m.OriginFileName = originFileName;
                m.UniqueFileName = uniqueFileName;
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                return false;
            }
        }

        private bool DelFile(int engMainSeq, ConstCheckRecImproveFileModel m)
        {
            string filePath = Utils.GetEngMainFolder(engMainSeq);

            string uniqueFileName = m.UniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    try
                    {
                        System.IO.File.Delete(fullPath);
                    } catch(Exception e)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private string CopyTemplateFile(string filename)
        {
            string tempFile = GetTempFile();
            string srcFile = Path.Combine(Utils.GetTemplateFilePath(), filename);
            System.IO.File.Copy(srcFile, tempFile);
            return tempFile;
        }

        private string GetTempFile()
        {
            string uuid = Guid.NewGuid().ToString("B").ToUpper();
            string tempPath = Path.GetTempPath();
            return Path.Combine(tempPath, uuid + ".docx");
        }
    }
}