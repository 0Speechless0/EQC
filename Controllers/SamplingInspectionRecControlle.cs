using EQC.Common;
using EQC.EDMXModel;
using EQC.EDMXModel.InterFace;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using Microsoft.Office.Interop.Word;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class SamplingInspectionRecController : Controller
    {
        protected ConstCheckRecService constCheckRecService = new ConstCheckRecService();
        //private EngMainService engMainService = new EngMainService();

        public virtual ActionResult Index()
        {
            Utils.setUserClass(this);
            //ViewBag.Title = "抽驗紀錄填報";
            return View();
        }
        public virtual ActionResult GetUserUnit()
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

        public virtual ActionResult EditEng(int seq)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Edit", "SamplingInspectionRec");
            string tarUrl = redirectUrl.ToString() + "?id=" + seq;
            return Json(new { Url = tarUrl });
        }
        public virtual ActionResult Edit()
        {
            Utils.setUserClass(this);
            //ViewBag.Title = "監造計畫-編輯";
            List<VMenu> menu = new List<VMenu>();
            var link = new UrlHelper(Request.RequestContext).Action("Index", "SamplingInspectionRec");
            menu.Add(new VMenu() { Name = "抽查紀錄填報", Url = link.ToString() });
            menu.Add(new VMenu() { Name = "抽查紀錄填報-編輯", Url = "" });
            ViewBag.breadcrumb = menu;

            return View();
        }


        //刪除抽查
        public JsonResult removeRec(int id)
        {
            using(var context = new EQC_NEW_Entities() )
            {
                context.ConstCheckRec.Remove(
                    context.ConstCheckRec.Find(id)
                );
                context.SaveChanges();
            }
            return Json(true);
        }

        //標案年分
        public virtual JsonResult GetYearOptions()
        {
            List<EngYearVModel> years = constCheckRecService.GetEngYearList();
            return Json(years);
        }
        //依年分取執行機關
        public virtual JsonResult GetUnitOptions(string year)
        {
            List<EngExecUnitsVModel> items = constCheckRecService.GetEngExecUnitList(year);
            return Json(items);
        }
        //依年分,機關取執行單位
        public virtual JsonResult GetSubUnitOptions(string year, int parentSeq)
        {
            List<EngExecUnitsVModel> items = constCheckRecService.GetEngExecSubUnitList(year, parentSeq);
            EngExecUnitsVModel m = new EngExecUnitsVModel();
            m.UnitSeq = -1;
            m.UnitName = "全部單位";
            items.Insert(0, m);
            return Json(items);
        }
        //工程名稱清單
        public virtual JsonResult GetEngNameList(string year, int unit, int subUnit, int engMain)
        {
            List<EngNameOptionsVModel> engNames = new List<EngNameOptionsVModel>();
            engNames = constCheckRecService.GetEngCreatedNameList<EngNameOptionsVModel>(year, unit, subUnit);
            engNames.Sort((x, y) => x.CompareTo(y));
            return Json(new
            {
                engNameItems = engNames
            });
        }
        //分項工程清單
        public virtual JsonResult GetSubEngNameList(int engMain)
        {
            List<EngConstructionOptionsVModel> subEngNames = constCheckRecService.GetSubEngList<EngConstructionOptionsVModel>(engMain);
            EngConstructionOptionsVModel m = new EngConstructionOptionsVModel();
            m.Seq = -1;
            m.ItemName = "全部分項工程";
            subEngNames.Insert(0, m);
            //    engNames.Sort((x, y) => x.CompareTo(y));
            return Json(subEngNames);
        }
        //分項工程清單 s20230520
        public virtual JsonResult GetSubEngNameList1(int engMain)
        {
            return Json(new
            {
                result = 0,
                items = constCheckRecService.GetSubEngList1<EngConstructionOptionsVModel>(engMain)
            });
        }

        //抽查清單
        public virtual JsonResult GetList(int engMain)
        {
            return Json(new
            {//s20230520
                //items = constCheckRecService.GetEngCreatedList<EngConstructionListVModel>(engMain),
                cc = constCheckRecService.GetConstCheckList1<EngSamplingInspectionVModel>(engMain), //施工抽查清單
                eot = constCheckRecService.GetEquOperTestList1<EngSamplingInspectionVModel>(engMain), //設備運轉測試清單
                osh = constCheckRecService.GetOccuSafeHealthList1<EngSamplingInspectionVModel>(engMain), //職業安全衛生清單
                ec = constCheckRecService.GetEnvirConsList1<EngSamplingInspectionVModel>(engMain) //環境保育清單

            });
        }
        //有檢驗單之分項工程清單 s20230520
        public virtual JsonResult GetRecEngConstruction(int mode, int eId, int rId)
        {
            return Json(new
            {//s20230520
                result = 0,
                items = constCheckRecService.GetRecEngConstruction(mode, eId, rId)
            });
        }

        //抽查記錄表單列表
        public virtual ActionResult GetSIRlist(int seq)
        {
            try
            {
                SignatureFileService signatureFileService = new SignatureFileService();
                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                List<EngConstructionEngInfoVModel> engItems = constCheckRecService.GetEngMainByEngConstructionSeq<EngConstructionEngInfoVModel>(seq);
                EngConstructionEngInfoVModel engItem = engItems[0];

                List<ConstCheckRecSheetModel> recItems = constCheckRecService.GetList<ConstCheckRecSheetModel>(seq);
                ConstCheckRecResultService service = new ConstCheckRecResultService();

                List<string> SIRlists = new List<string>();
                foreach (ConstCheckRecSheetModel item in recItems)
                {
                    if (item.CCRCheckType1 == 1) { 
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
                System.Diagnostics.Debug.WriteLine("產製錯誤: " + e.Message);
                return Json(new
                {
                    result = -1,
                    message = "產製錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //抽查記錄表單下載 s20230520
        public virtual ActionResult SIRDnDoc(int mode, int seq, int filetype, int eId, DownloadArgExtension downloadArg = null)
        {
            try
            {
                string engNo = "";
                //var a = Utils.getUserSeq();
                SignatureFileService signatureFileService = new SignatureFileService();
                string uuid = Guid.NewGuid().ToString("B").ToUpper();

                List<EngConstructionEngInfoVModel> engItems = constCheckRecService.GetEngConstruction<EngConstructionEngInfoVModel>(eId, mode, seq);
                foreach (EngConstructionEngInfoVModel engItem in engItems)
                {
                    engNo = engItem.EngNo;
                    List<ConstCheckRecSheetModel> recItems = constCheckRecService.GetList1<ConstCheckRecSheetModel>(engItem.subEngNameSeq, seq);
                    ConstCheckRecResultService service = new ConstCheckRecResultService();
                    foreach (ConstCheckRecSheetModel item in recItems)
                    {
                        item.GetControls(service);
                        JoinCell(item.items);
                    }
                    service.Close();

                    foreach (ConstCheckRecSheetModel item in recItems)
                    {
                        string outputFilePath = null;
                        if (item.CCRCheckType1 == 1)
                            outputFilePath = CheckSheet1(engItem, item, uuid, signatureFileService, filetype);
                        else if (item.CCRCheckType1 == 2)
                            outputFilePath = CheckSheet2(engItem, item, uuid, signatureFileService, filetype);
                        else if (item.CCRCheckType1 == 3)
                            outputFilePath = CheckSheet3(engItem, item, uuid, signatureFileService, filetype);
                        else if (item.CCRCheckType1 == 4)
                            outputFilePath = CheckSheet4(engItem, item, uuid, signatureFileService, filetype);

                        downloadArg?.targetPathSetting(outputFilePath);
                    }


                }
                if(downloadArg?.DistFilePath == null)
                {
                    if (filetype == 2)
                    {
                        string path = Path.Combine(Path.GetTempPath(), uuid) + "pdf";
                        string zipFile = Path.Combine(Path.GetTempPath(), uuid + "pdf.zip");
                        ZipFile.CreateFromDirectory(path, zipFile);// AddFiles(files, "ProjectX");
                        Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);

                        return File(iStream, "application/blob", engNo + "-抽查紀錄表.zip");

                    }
                    else if (filetype == 3)
                    {
                        string path = Path.Combine(Path.GetTempPath(), uuid) + "odt";
                        string zipFile = Path.Combine(Path.GetTempPath(), uuid + "odt.zip");
                        ZipFile.CreateFromDirectory(path, zipFile);// AddFiles(files, "ProjectX");
                        Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                        return File(iStream, "application/blob", engNo + "-抽查紀錄表.zip");
                    }
                    else
                    {
                        string path = Path.Combine(Path.GetTempPath(), uuid);
                        string zipFile = Path.Combine(Path.GetTempPath(), uuid + ".zip");
                        ZipFile.CreateFromDirectory(path, zipFile);// AddFiles(files, "ProjectX");
                        Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                        return File(iStream, "application/blob", engNo + "-抽查紀錄表.zip");
                    }
                }
                else
                {
                    return null;
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
        //抽查記錄表單下載(zip)
        public virtual ActionResult SIRDownload(int seq,int filetype)
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
                    string path = Path.Combine(Path.GetTempPath(), uuid)+"pdf";
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
        //抽查記錄表單下載(單一檔案)
        public virtual ActionResult SIROneDownload(int seq,List<string>items, string downloaditem,int num,int filetype)
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
                List<EngConstructionEngInfoVModel> engItems = constCheckRecService.GetEngMainByEngConstructionSeq<EngConstructionEngInfoVModel>(seq);
                EngConstructionEngInfoVModel engItem = engItems[0];

                List<ConstCheckRecSheetModel> recItems = constCheckRecService.GetList<ConstCheckRecSheetModel>(seq);
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
                    Log("單一下載" + filetype);
                    return File(CheckSheet2download(engItem, item, uuid, signatureFileService, filetype), "application/blob", Filename);
                }
                else if (item.CCRCheckType1 == 3)
                {
                    Log("單一下載" + filetype);
                    return File(CheckSheet3download(engItem, item, uuid, signatureFileService, filetype), "application/blob", Filename);
                }
                else if (item.CCRCheckType1 == 4)
                {
                    Log("單一下載" + filetype);
                    return File(CheckSheet4download(engItem, item, uuid, signatureFileService, filetype), "application/blob", Filename);
                }
                else
                {
                    Log("單一下載" + filetype);
                    return File(CheckSheet1download(engItem, item, uuid, signatureFileService, filetype), "application/blob", Filename);

                }
            }
            catch (Exception e)
            {
                Log("產製錯誤: " + e.Message);
                System.Diagnostics.Debug.WriteLine("產製錯誤: " + e.Message);
                return Json(new
                {
                    result = -1,
                    message = "產製錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
        }


        //1-施工抽查-抽查紀錄表
        public string CheckSheet1(EngConstructionEngInfoVModel engItem, ConstCheckRecSheetModel recItem, string uuid, SignatureFileService service,int filetype)
        {
            string tempFile = CopyTemplateFile("1-施工抽查-抽查紀錄表.docx");
            string outFile = recItem.GetFilename(uuid);
            List<ControlStVModel> items = recItem.items;
            //string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            //table.GetRow(0).GetCell(0).SetText("表附四-18　"+recItem.CheckItemName);
            XWPFTableCell cell = table.GetRow(0).GetCell(0);
            foreach (var p in cell.Paragraphs)
            {
                string old = p.ParagraphText;
                if (old.IndexOf("@") == -1) continue;
                p.ReplaceText(old, old.Replace("@", "表附四-18　" + recItem.CheckItemName));
                break;
            }

            table.GetRow(2).GetCell(1).SetText(engItem.EngName);
            table.GetRow(3).GetCell(1).SetText(engItem.subEngName + '-' + engItem.subEngItemNo);
            table.GetRow(4).GetCell(1).SetText(recItem.CCRPosDesc);
            table.GetRow(4).GetCell(3).SetText(recItem.chsCheckDate);
            string t = "□施工前　　　　　□施工中檢查　　　　□施工完成檢查";
            if (recItem.CCRCheckFlow == 1)
                t = "■施工前　　　　　□施工中檢查　　　　□施工完成檢查";
            else if (recItem.CCRCheckFlow == 2)
                t = "□施工前　　　　　■施工中檢查　　　　□施工完成檢查";
            else if (recItem.CCRCheckFlow == 3)
                t = "□施工前　　　　　□施工中檢查　　　　■施工完成檢查";
            table.GetRow(5).GetCell(1).SetText(t);

            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                //Utils.insertRow(table, 8, 9);
                Utils.insertRow(table, 8, 9);
            }
            int rowInx = 0;
            using(var context = new EQC_NEW_Entities())
            {
                foreach (ControlStVModel m in items)
                {
                    if (rowInx == 0)
                    {//流程
                        Utils.rowMergeStart(table.GetRow(8 + rowInx).GetCell(0));
                        t = "";
                        if (recItem.CCRCheckFlow == 1)
                            t = "施工前";
                        else if (recItem.CCRCheckFlow == 2)
                            t = "施工中";
                        else if (recItem.CCRCheckFlow == 3)
                            t = "施工後";
                        table.GetRow(8 + rowInx).GetCell(0).SetText(t);
                    }
                    else
                        Utils.rowMergeContinue(table.GetRow(8 + rowInx).GetCell(0));

                    //管理項目
                    if (m.rowSpan > 0)
                    {
                        if (m.rowSpan > 1)
                        {
                            Utils.rowMergeStart(table.GetRow(8 + rowInx).GetCell(1), "2");
                            Utils.rowMergeStart(table.GetRow(8 + rowInx).GetCell(3));
                            Utils.rowMergeStart(table.GetRow(8 + rowInx).GetCell(4));
                        }
                        table.GetRow(8 + rowInx).GetCell(1).SetText(m.CheckItem1);// + m.CheckItem2);
                        table.GetRow(8 + rowInx).GetCell(3).SetParagraph(Utils.setCellTextLeft(doc, table));
                        table.GetRow(8 + rowInx).GetCell(3).SetText(m.CCRRealCheckCond);
                        table.GetRow(8 + rowInx).GetCell(4).SetText(GetCheckCaption(m.CCRCheckResult));
                    }
                    else
                    {
                        Utils.rowMergeContinue(table.GetRow(8 + rowInx).GetCell(1), "2");
                        Utils.rowMergeContinue(table.GetRow(8 + rowInx).GetCell(3));
                        Utils.rowMergeContinue(table.GetRow(8 + rowInx).GetCell(4));
                    }
                    
                    //檢驗項目
                    if (m.rowSpanStd1 > 0)
                    {
                        Utils.rowMergeStart(table.GetRow(8 + rowInx).GetCell(2));
                        table.GetRow(8 + rowInx).GetCell(2).SetText(m.Stand1.Contains("_") ? m.StandardFilled(context) : m.Stand1 );
                    }
                    else
                    {
                        Utils.rowMergeContinue(table.GetRow(8 + rowInx).GetCell(2));
                    }

                    rowInx++;
                }
            }

            service.RenderConstCheckDoc(doc, recItem, addSignatureFile, addSignatureFileAlter);
            //addSignatureFile(doc.Tables[1].GetRow(0).GetCell(1), recItem.SupervisorUserSeq, service);
            //addSignatureFile(doc.Tables[1].GetRow(0).GetCell(3), recItem.SupervisorDirectorSeq, service);
            //
            List<ControlStVModel> Stitems = new List<ControlStVModel>();
            if (recItem.CCRCheckType1 > 0 && recItem.CCRCheckType1 < 5)
            {
                if (recItem.CCRCheckType1 == 1)
                {//施工抽查清單
                    Stitems = constCheckRecService.GetConstCheckControlSt<ControlStVModel>(recItem.ItemSeq, recItem.CCRCheckFlow);
                }
                else if (recItem.CCRCheckType1 == 2)
                {//設備運轉測試清單
                    Stitems = constCheckRecService.GetEquOperControlSt<ControlStVModel>(recItem.ItemSeq);
                }
                else if (recItem.CCRCheckType1 == 3)
                {//職業安全衛生清單
                    Stitems = constCheckRecService.GetOccuSafeHealthControlSt<ControlStVModel>(recItem.ItemSeq);
                }
                else if (recItem.CCRCheckType1 == 4)
                {//環境保育清單
                    Stitems = constCheckRecService.GetEnvirConsControlSt<ControlStVModel>(recItem.ItemSeq, recItem.CCRCheckFlow);
                }
            }
            FillCheckSheetImage(doc, engItem, recItem, Stitems);
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
                string path = Path.Combine(Path.GetTempPath(), uuid);
                string pdfFiles = recItem.Getdownloadname(uuid);
                string[] pdfFile = pdfFiles.Split('.');
                if (!Directory.Exists(path+"pdf")) Directory.CreateDirectory(path+"pdf");
                string filePatdownloadhName = Path.Combine(path+"pdf", pdfFile[0]+".pdf");

                //匯出為 pdf
                wordDocument.ExportAsFixedFormat(filePatdownloadhName, WdExportFormat.wdExportFormatPDF);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();
                return filePatdownloadhName;
            }
            else if (filetype == 3)
            {
                //建立 word application instance
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                //開啟 word 檔案
                var wordDocument = appWord.Documents.Open(outFile);
                //odt檔路徑
                string path = Path.Combine(Path.GetTempPath(), uuid);
                string odtFiles = recItem.Getdownloadname(uuid);
                string[] odtFile = odtFiles.Split('.');
                if (!Directory.Exists(path + "odt")) Directory.CreateDirectory(path + "odt");
                string filePatdownloadhName = Path.Combine(path + "odt", odtFile[0]+".odt");
                //匯出為 pdf
                wordDocument.SaveAs2(filePatdownloadhName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();
                return filePatdownloadhName;
            }
            return outFile;
        }
        //1-施工抽查-抽查紀錄表下載
        public Stream CheckSheet1download(EngConstructionEngInfoVModel engItem, ConstCheckRecSheetModel recItem, string uuid, SignatureFileService service,int filetype)
        {
            string tempFile = CopyTemplateFile("1-施工抽查-抽查紀錄表.docx");
            string outFile = recItem.GetFilename(uuid);
            List<ControlStVModel> items = recItem.items;
            //string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            //table.GetRow(0).GetCell(0).SetText("表附四-18　"+recItem.CheckItemName);
            XWPFTableCell cell = table.GetRow(0).GetCell(0);
            foreach (var p in cell.Paragraphs)
            {
                string old = p.ParagraphText;
                if (old.IndexOf("@") == -1) continue;
                p.ReplaceText(old, old.Replace("@", "表附四-18　" + recItem.CheckItemName));
                break;
            }

            table.GetRow(2).GetCell(1).SetText(engItem.EngName);
            table.GetRow(3).GetCell(1).SetText(engItem.subEngName + '-' + engItem.subEngItemNo);
            table.GetRow(4).GetCell(1).SetText(recItem.CCRPosDesc);
            table.GetRow(4).GetCell(3).SetText(recItem.chsCheckDate);
            string t = "□施工前　　　　　□施工中檢查　　　　□施工完成檢查";
            if (recItem.CCRCheckFlow == 1)
                t = "■施工前　　　　　□施工中檢查　　　　□施工完成檢查";
            else if (recItem.CCRCheckFlow == 2)
                t = "□施工前　　　　　■施工中檢查　　　　□施工完成檢查";
            else if (recItem.CCRCheckFlow == 3)
                t = "□施工前　　　　　□施工中檢查　　　　■施工完成檢查";
            table.GetRow(5).GetCell(1).SetText(t);

            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                //Utils.insertRow(table, 8, 9);
                Utils.insertRow(table, 8, 9);
            }
            int rowInx = 0;
            using(var context = new EQC_NEW_Entities())
            {
                foreach (ControlStVModel m in items)
                {
                    if (rowInx == 0)
                    {//流程
                        Utils.rowMergeStart(table.GetRow(8 + rowInx).GetCell(0));
                        t = "";
                        if (recItem.CCRCheckFlow == 1)
                            t = "施工前";
                        else if (recItem.CCRCheckFlow == 2)
                            t = "施工中";
                        else if (recItem.CCRCheckFlow == 3)
                            t = "施工後";
                        table.GetRow(8 + rowInx).GetCell(0).SetText(t);
                    }
                    else
                        Utils.rowMergeContinue(table.GetRow(8 + rowInx).GetCell(0));

                    //管理項目
                    if (m.rowSpan > 0)
                    {
                        if (m.rowSpan > 1)
                        {
                            Utils.rowMergeStart(table.GetRow(8 + rowInx).GetCell(1), "2");
                            Utils.rowMergeStart(table.GetRow(8 + rowInx).GetCell(3));
                            Utils.rowMergeStart(table.GetRow(8 + rowInx).GetCell(4));
                        }
                        table.GetRow(8 + rowInx).GetCell(1).SetText(m.CheckItem1);// + m.CheckItem2);
                        table.GetRow(8 + rowInx).GetCell(3).SetParagraph(Utils.setCellTextLeft(doc, table));
                        table.GetRow(8 + rowInx).GetCell(3).SetText(m.CCRRealCheckCond);
                        table.GetRow(8 + rowInx).GetCell(4).SetText(GetCheckCaption(m.CCRCheckResult));
                    }
                    else
                    {
                        Utils.rowMergeContinue(table.GetRow(8 + rowInx).GetCell(1), "2");
                        Utils.rowMergeContinue(table.GetRow(8 + rowInx).GetCell(3));
                        Utils.rowMergeContinue(table.GetRow(8 + rowInx).GetCell(4));
                    }

                    //檢驗項目
                    if (m.rowSpanStd1 > 0)
                    {
                        Utils.rowMergeStart(table.GetRow(8 + rowInx).GetCell(2));
                        table.GetRow(8 + rowInx).GetCell(2).SetText(m.Stand1.Contains("_") ? m.StandardFilled(context) : m.Stand1);
                    }
                    else
                    {
                        Utils.rowMergeContinue(table.GetRow(8 + rowInx).GetCell(2));
                    }

                    rowInx++;
                }
            }

            service.RenderConstCheckDoc(doc, recItem, addSignatureFile, addSignatureFileAlter);
            //addSignatureFile(doc.Tables[1].GetRow(0).GetCell(1), recItem.SupervisorUserSeq, service);
            //addSignatureFile(doc.Tables[1].GetRow(0).GetCell(3), recItem.SupervisorDirectorSeq, service);
            //
            List<ControlStVModel> Stitems = new List<ControlStVModel>();
            if (recItem.CCRCheckType1 > 0 && recItem.CCRCheckType1 < 5)
            {
                if (recItem.CCRCheckType1 == 1)
                {//施工抽查清單
                    Stitems = constCheckRecService.GetConstCheckControlSt<ControlStVModel>(recItem.ItemSeq, recItem.CCRCheckFlow);
                }
                else if (recItem.CCRCheckType1 == 2)
                {//設備運轉測試清單
                    Stitems = constCheckRecService.GetEquOperControlSt<ControlStVModel>(recItem.ItemSeq);
                }
                else if (recItem.CCRCheckType1 == 3)
                {//職業安全衛生清單
                    Stitems = constCheckRecService.GetOccuSafeHealthControlSt<ControlStVModel>(recItem.ItemSeq);
                }
                else if (recItem.CCRCheckType1 == 4)
                {//環境保育清單
                    Stitems = constCheckRecService.GetEnvirConsControlSt<ControlStVModel>(recItem.ItemSeq, recItem.CCRCheckFlow);
                }
            }
            XWPFTable table1 = doc.Tables[2];
            //for (int i = 1; i < Stitems.Count; i++) 
            //{
            //    table1.CreateRow();
            //}
            string check = "";
            int j = 0;
            for (int i = 0; i < Stitems.Count; i++)
            {
                if (check != Stitems[i].CheckItem1)
                {
                    check = Stitems[i].CheckItem1;
                }
                else
                {
                    j++;
                    continue;
                }
                table1.CreateRow();
                table1.GetRow(i - j + 1).GetCell(0).SetText(Stitems[i].CheckItem1);
                //圖片
                List<UploadFileModel> UFitems = new ConstCheckRecFileService().GetPhotos<UploadFileModel>(recItem.Seq, Stitems[i].ControllStSeq);
                string filePath = Utils.GetEngMainFolder(engItem.Seq);
                for (int y = 0; y < UFitems.Count; y++)
                {
                    string filePathName = Path.Combine(filePath, UFitems[y].UniqueFileName);
                    addFile(doc.Tables[2].GetRow(i - j + 1).GetCell(1), filePathName);
                }
            }
            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();
            //轉檔
            if (filetype ==2)
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
            }else if (filetype == 3)
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

        //2-設備運轉-抽查紀錄表
        public string CheckSheet2(EngConstructionEngInfoVModel engItem, ConstCheckRecSheetModel recItem, string uuid, SignatureFileService service, int filetype)
        {
            string tempFile = CopyTemplateFile("2-設備運轉-抽查紀錄表.docx");
            string outFile = recItem.GetFilename(uuid);
            List<ControlStVModel> items = recItem.items;
            //string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            table.GetRow(0).GetCell(1).SetText(engItem.EngName);
            table.GetRow(1).GetCell(1).SetText(engItem.subEngName + '-' + engItem.subEngItemNo);
            table.GetRow(2).GetCell(1).SetText(recItem.CCRPosDesc);
            table.GetRow(2).GetCell(3).SetText(recItem.chsCheckDate);
            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                Utils.insertRow(table, 6, 7);
            }
            int rowInx = 0;
            using(var context = new EQC_NEW_Entities())
            {
                foreach (ControlStVModel m in items)
                {
                    //管理項目
                    if (m.rowSpan > 0)
                    {
                        Utils.rowMergeStart(table.GetRow(6 + rowInx).GetCell(0));
                        Utils.rowMergeStart(table.GetRow(6 + rowInx).GetCell(2));
                        Utils.rowMergeStart(table.GetRow(6 + rowInx).GetCell(3));
                        table.GetRow(6 + rowInx).GetCell(0).SetText(m.CheckItem1);// + m.CheckItem2);
                        table.GetRow(6 + rowInx).GetCell(2).SetText(m.CCRRealCheckCond);
                        table.GetRow(6 + rowInx).GetCell(3).SetText(GetCheckCaption(m.CCRCheckResult));
                    }
                    else
                    {
                        Utils.rowMergeContinue(table.GetRow(6 + rowInx).GetCell(0));
                        Utils.rowMergeContinue(table.GetRow(6 + rowInx).GetCell(2));
                        Utils.rowMergeContinue(table.GetRow(6 + rowInx).GetCell(3));
                    }

                    //檢驗項目
                    if (m.rowSpanStd1 > 0)
                    {
                        Utils.rowMergeStart(table.GetRow(6 + rowInx).GetCell(1));
                        table.GetRow(6 + rowInx).GetCell(1).SetText(m.Stand1.Contains("_") ? m.StandardFilled(context) : m.Stand1);
                    }
                    else
                        Utils.rowMergeContinue(table.GetRow(6 + rowInx).GetCell(1));

                    rowInx++;
                }
            }

            service.RenderConstCheckDoc(doc, recItem, addSignatureFile, addSignatureFileAlter);
            //addSignatureFile(doc.Tables[1].GetRow(0).GetCell(1), recItem.SupervisorUserSeq, service);
            //addSignatureFile(doc.Tables[1].GetRow(0).GetCell(3), recItem.SupervisorDirectorSeq, service);
            //
            List<ControlStVModel> Stitems = new List<ControlStVModel>();
            if (recItem.CCRCheckType1 > 0 && recItem.CCRCheckType1 < 5)
            {
                if (recItem.CCRCheckType1 == 1)
                {//施工抽查清單
                    Stitems = constCheckRecService.GetConstCheckControlSt<ControlStVModel>(recItem.ItemSeq, recItem.CCRCheckFlow);
                }
                else if (recItem.CCRCheckType1 == 2)
                {//設備運轉測試清單
                    Stitems = constCheckRecService.GetEquOperControlSt<ControlStVModel>(recItem.ItemSeq);
                }
                else if (recItem.CCRCheckType1 == 3)
                {//職業安全衛生清單
                    Stitems = constCheckRecService.GetOccuSafeHealthControlSt<ControlStVModel>(recItem.ItemSeq);
                }
                else if (recItem.CCRCheckType1 == 4)
                {//環境保育清單
                    Stitems = constCheckRecService.GetEnvirConsControlSt<ControlStVModel>(recItem.ItemSeq, recItem.CCRCheckFlow);
                }
            }

            FillCheckSheetImage(doc, engItem, recItem, Stitems);

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
                string path = Path.Combine(Path.GetTempPath(), uuid);
                string pdfFiles = recItem.Getdownloadname(uuid);
                string[] pdfFile = pdfFiles.Split('.');
                if (!Directory.Exists(path + "pdf")) Directory.CreateDirectory(path + "pdf");
                string filePatdownloadhName = Path.Combine(path + "pdf", pdfFile[0] + ".pdf");

                //匯出為 pdf
                wordDocument.ExportAsFixedFormat(filePatdownloadhName, WdExportFormat.wdExportFormatPDF);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();
                return filePatdownloadhName;
            }
            else if (filetype == 3)
            {
                //建立 word application instance
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                //開啟 word 檔案
                var wordDocument = appWord.Documents.Open(outFile);
                //odt檔路徑
                string path = Path.Combine(Path.GetTempPath(), uuid);
                string odtFiles = recItem.Getdownloadname(uuid);
                string[] odtFile = odtFiles.Split('.');
                if (!Directory.Exists(path + "odt")) Directory.CreateDirectory(path + "odt");
                string filePatdownloadhName = Path.Combine(path + "odt", odtFile[0] + ".odt");
                //匯出為 pdf
                wordDocument.SaveAs2(filePatdownloadhName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();
                return filePatdownloadhName;
            }

            return outFile;

        }
        //2-設備運轉-抽查紀錄表下載
        public Stream CheckSheet2download(EngConstructionEngInfoVModel engItem, ConstCheckRecSheetModel recItem, string uuid, SignatureFileService service,int filetype)
        {
            string tempFile = CopyTemplateFile("2-設備運轉-抽查紀錄表.docx");
            string outFile = recItem.GetFilename(uuid);
            List<ControlStVModel> items = recItem.items;
            //string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            table.GetRow(0).GetCell(1).SetText(engItem.EngName);
            table.GetRow(1).GetCell(1).SetText(engItem.subEngName + '-' + engItem.subEngItemNo);
            table.GetRow(2).GetCell(1).SetText(recItem.CCRPosDesc);
            table.GetRow(2).GetCell(3).SetText(recItem.chsCheckDate);
            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                Utils.insertRow(table, 6, 7);
            }
            int rowInx = 0;
            foreach (ControlStVModel m in items)
            {
                //管理項目
                if (m.rowSpan > 0)
                {
                    Utils.rowMergeStart(table.GetRow(6 + rowInx).GetCell(0));
                    Utils.rowMergeStart(table.GetRow(6 + rowInx).GetCell(2));
                    Utils.rowMergeStart(table.GetRow(6 + rowInx).GetCell(3));
                    table.GetRow(6 + rowInx).GetCell(0).SetText(m.CheckItem1);// + m.CheckItem2);
                    table.GetRow(6 + rowInx).GetCell(2).SetText(m.CCRRealCheckCond);
                    table.GetRow(6 + rowInx).GetCell(3).SetText(GetCheckCaption(m.CCRCheckResult));
                }
                else
                {
                    Utils.rowMergeContinue(table.GetRow(6 + rowInx).GetCell(0));
                    Utils.rowMergeContinue(table.GetRow(6 + rowInx).GetCell(2));
                    Utils.rowMergeContinue(table.GetRow(6 + rowInx).GetCell(3));
                }

                //檢驗項目
                if (m.rowSpanStd1 > 0)
                {
                    Utils.rowMergeStart(table.GetRow(6 + rowInx).GetCell(1));
                    using (var context = new EQC_NEW_Entities())
                    {
                        table.GetRow(6 + rowInx).GetCell(1).SetText(m.Stand1.Contains("_") ? m.StandardFilled(context) : m.Stand1);
                    }
                }
                else
                    Utils.rowMergeContinue(table.GetRow(6 + rowInx).GetCell(1));

                rowInx++;
            }

           
            //
            List<ControlStVModel> Stitems = new List<ControlStVModel>();
            if (recItem.CCRCheckType1 > 0 && recItem.CCRCheckType1 < 5)
            {
                if (recItem.CCRCheckType1 == 1)
                {//施工抽查清單
                    Stitems = constCheckRecService.GetConstCheckControlSt<ControlStVModel>(recItem.ItemSeq, recItem.CCRCheckFlow);
                }
                else if (recItem.CCRCheckType1 == 2)
                {//設備運轉測試清單
                    Stitems = constCheckRecService.GetEquOperControlSt<ControlStVModel>(recItem.ItemSeq);
                }
                else if (recItem.CCRCheckType1 == 3)
                {//職業安全衛生清單
                    Stitems = constCheckRecService.GetOccuSafeHealthControlSt<ControlStVModel>(recItem.ItemSeq);
                }
                else if (recItem.CCRCheckType1 == 4)
                {//環境保育清單
                    Stitems = constCheckRecService.GetEnvirConsControlSt<ControlStVModel>(recItem.ItemSeq, recItem.CCRCheckFlow);
                }
            }
            XWPFTable table1 = doc.Tables[2];
            //for (int i = 1; i < Stitems.Count; i++)
            //{
            //    table1.CreateRow();
            //}
            string check = "";
            int j = 0;
            for (int i = 0; i < Stitems.Count; i++)
            {
                if (check != Stitems[i].CheckItem1)
                {
                    check = Stitems[i].CheckItem1;
                }
                else
                {
                    j++;
                    continue;
                }
                table1.CreateRow();
                table1.GetRow(i - j + 1).GetCell(0).SetText(Stitems[i].CheckItem1);
                //圖片
                List<UploadFileModel> UFitems = new ConstCheckRecFileService().GetPhotos<UploadFileModel>(recItem.Seq, Stitems[i].ControllStSeq);
                string filePath = Utils.GetEngMainFolder(engItem.Seq);
                for (int y = 0; y < UFitems.Count; y++)
                {
                    string filePathName = Path.Combine(filePath, UFitems[y].UniqueFileName);
                    addFile(doc.Tables[2].GetRow(i - j + 1).GetCell(1), filePathName);
                }
            }
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
                //pdf檔路徑
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

        private void FillCheckSheetImage(XWPFDocument doc, EngConstructionEngInfoVModel engItem, ConstCheckRecSheetModel recItem, List<ControlStVModel> Stitems)
        {
            var table1 = doc.Tables[2];
            string check = "";
            int j = 0;
            string filePathName = null;
            for (int i = 0; i < Stitems.Count; i++)
            {

                if (check != Stitems[i].CheckItem1)
                {
                    check = Stitems[i].CheckItem1;
                }
                else
                {
                    j++;
                    continue;
                }
                List<UploadFileModel> UFitems = new ConstCheckRecFileService().GetPhotos<UploadFileModel>(recItem.Seq, Stitems[i].ControllStSeq);
                if (UFitems.Count == 0) continue;
                table1.CreateRow();
                table1.Rows.LastOrDefault()?.GetCell(0).SetText(Stitems[i].CheckItem1);
                //圖片

                string filePath = Utils.GetEngMainFolder(engItem.Seq);
                for (int y = 0; y < UFitems.Count; y++)
                {
                    filePathName = Path.Combine(filePath, UFitems[y].UniqueFileName);
                    addFile(table1.Rows.LastOrDefault()?.GetCell(1), filePathName);
                }
            }
            if (filePathName == null)
            {
                doc.RemoveBodyElement(doc.GetPosOfParagraph(doc.Paragraphs.Where(r => r.Text.Contains("照片")).FirstOrDefault()));
                doc.Paragraphs.Skip(2).ToList().ForEach(e =>
                {
                    doc.RemoveBodyElement(doc.GetPosOfParagraph(e));
                });

                doc.RemoveBodyElement(doc.GetPosOfTable(doc.Tables[2]));
            }
        }
        //3-職業安全-抽查紀錄表
        public string CheckSheet3(EngConstructionEngInfoVModel engItem, ConstCheckRecSheetModel recItem, string uuid, SignatureFileService service, int filetype)
        {
            string tempFile = CopyTemplateFile("3-職業安全-抽查紀錄表.docx");
            string outFile = recItem.GetFilename(uuid);
            List<ControlStVModel> items = recItem.items;
            //string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            table.GetRow(0).GetCell(1).SetText(engItem.EngName);
            //table.GetRow(1).GetCell(1).SetText(engItem.subEngName);
            table.GetRow(2).GetCell(1).SetText(recItem.CCRPosDesc);
            string[] d = recItem.chsCheckDate.Split('/');
            table.GetRow(2).GetCell(3).SetText(String.Format("{0}年{1}月{2}日", d[0], d[1], d[2]));
            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                Utils.insertRow(table, 4, 5);
            }
            int rowInx = 0;
            using(var context = new EQC_NEW_Entities() )
            {
                foreach (ControlStVModel m in items)
                {
                    //管理項目
                    if (m.rowSpan > 0)
                    {
                        Utils.rowMergeStart(table.GetRow(4 + rowInx).GetCell(0));
                        Utils.rowMergeStart(table.GetRow(4 + rowInx).GetCell(2));
                        Utils.rowMergeStart(table.GetRow(4 + rowInx).GetCell(3));
                        table.GetRow(4 + rowInx).GetCell(0).SetText(m.CheckItem1);// + m.CheckItem2);
                        table.GetRow(4 + rowInx).GetCell(2).SetText(GetCheckCaption(m.CCRCheckResult));// m.CCRRealCheckCond);
                                                                                                       //table.GetRow(4 + rowInx).GetCell(7).SetText(GetCheckCaption(m.CCRCheckResult));
                    }
                    else
                    {
                        Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(0));
                        Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(2));
                        Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(3));
                    }

                    //檢驗項目
                    if (m.rowSpanStd1 > 0)
                    {
                        Utils.rowMergeStart(table.GetRow(4 + rowInx).GetCell(1));
                        table.GetRow(4 + rowInx).GetCell(1).SetText(m.Stand1.Contains("_") ? m.StandardFilled(context) : m.Stand1);
                    }
                    else
                        Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(1));

                    rowInx++;
                }

            }

            service.RenderConstCheckDoc(doc, recItem, addSignatureFile, addSignatureFileAlter);
            //addSignatureFile(doc.Tables[1].GetRow(0).GetCell(1), recItem.SupervisorUserSeq, service);
            //addSignatureFile(doc.Tables[1].GetRow(0).GetCell(3), recItem.SupervisorDirectorSeq, service);
            //
            List<ControlStVModel> Stitems = new List<ControlStVModel>();
            if (recItem.CCRCheckType1 > 0 && recItem.CCRCheckType1 < 5)
            {
                if (recItem.CCRCheckType1 == 1)
                {//施工抽查清單
                    Stitems = constCheckRecService.GetConstCheckControlSt<ControlStVModel>(recItem.ItemSeq, recItem.CCRCheckFlow);
                }
                else if (recItem.CCRCheckType1 == 2)
                {//設備運轉測試清單
                    Stitems = constCheckRecService.GetEquOperControlSt<ControlStVModel>(recItem.ItemSeq);
                }
                else if (recItem.CCRCheckType1 == 3)
                {//職業安全衛生清單
                    Stitems = constCheckRecService.GetOccuSafeHealthControlSt<ControlStVModel>(recItem.ItemSeq);
                }
                else if (recItem.CCRCheckType1 == 4)
                {//環境保育清單
                    Stitems = constCheckRecService.GetEnvirConsControlSt<ControlStVModel>(recItem.ItemSeq, recItem.CCRCheckFlow);
                }
            }
            FillCheckSheetImage(doc, engItem, recItem, Stitems);

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
                string path = Path.Combine(Path.GetTempPath(), uuid);
                string pdfFiles = recItem.Getdownloadname(uuid);
                string[] pdfFile = pdfFiles.Split('.');
                if (!Directory.Exists(path + "pdf")) Directory.CreateDirectory(path + "pdf");
                string filePatdownloadhName = Path.Combine(path + "pdf", pdfFile[0] + ".pdf");

                //匯出為 pdf
                wordDocument.ExportAsFixedFormat(filePatdownloadhName, WdExportFormat.wdExportFormatPDF);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();
                return filePatdownloadhName;
            }
            else if (filetype == 3)
            {
                //建立 word application instance
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                //開啟 word 檔案
                var wordDocument = appWord.Documents.Open(outFile);
                //odt檔路徑
                string path = Path.Combine(Path.GetTempPath(), uuid);
                string odtFiles = recItem.Getdownloadname(uuid);
                string[] odtFile = odtFiles.Split('.');
                if (!Directory.Exists(path + "odt")) Directory.CreateDirectory(path + "odt");
                string filePatdownloadhName = Path.Combine(path + "odt", odtFile[0] + ".odt");
                //匯出為 pdf
                wordDocument.SaveAs2(filePatdownloadhName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();
                return filePatdownloadhName;
            }
            return outFile;

        }
        //3-職業安全-抽查紀錄表下載
        public Stream CheckSheet3download(EngConstructionEngInfoVModel engItem, ConstCheckRecSheetModel recItem, string uuid, SignatureFileService service,int filetype)
        {
            string tempFile = CopyTemplateFile("3-職業安全-抽查紀錄表.docx");
            string outFile = recItem.GetFilename(uuid);
            List<ControlStVModel> items = recItem.items;
            //string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            table.GetRow(0).GetCell(1).SetText(engItem.EngName);
            //table.GetRow(1).GetCell(1).SetText(engItem.subEngName);
            table.GetRow(2).GetCell(1).SetText(recItem.CCRPosDesc);
            string[] d = recItem.chsCheckDate.Split('/');
            table.GetRow(2).GetCell(3).SetText(String.Format("{0}年{1}月{2}日", d[0], d[1], d[2]));
            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                Utils.insertRow(table, 4, 5);
            }
            int rowInx = 0;
            foreach (ControlStVModel m in items)
            {
                //管理項目
                if (m.rowSpan > 0)
                {
                    Utils.rowMergeStart(table.GetRow(4 + rowInx).GetCell(0));
                    Utils.rowMergeStart(table.GetRow(4 + rowInx).GetCell(2));
                    Utils.rowMergeStart(table.GetRow(4 + rowInx).GetCell(3));
                    table.GetRow(4 + rowInx).GetCell(0).SetText(m.CheckItem1);// + m.CheckItem2);
                    table.GetRow(4 + rowInx).GetCell(2).SetText(GetCheckCaption(m.CCRCheckResult));// m.CCRRealCheckCond);
                    //table.GetRow(4 + rowInx).GetCell(7).SetText(GetCheckCaption(m.CCRCheckResult));
                }
                else
                {
                    Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(0));
                    Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(2));
                    Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(3));
                }

                //檢驗項目
                if (m.rowSpanStd1 > 0)
                {
                    Utils.rowMergeStart(table.GetRow(4 + rowInx).GetCell(1));
                    using(var context = new EQC_NEW_Entities())
                    {
                        table.GetRow(4 + rowInx).GetCell(1).SetText(m.Stand1.Contains("_") ? m.StandardFilled(context) : m.Stand1);
                    }

                }
                else
                    Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(1));

                rowInx++;
            }
            service.RenderConstCheckDoc(doc, recItem, addSignatureFile, addSignatureFileAlter);
            //addSignatureFile(doc.Tables[1].GetRow(0).GetCell(1), recItem.SupervisorUserSeq, service);
            //addSignatureFile(doc.Tables[1].GetRow(0).GetCell(3), recItem.SupervisorDirectorSeq, service);
            //
            List<ControlStVModel> Stitems = new List<ControlStVModel>();
            if (recItem.CCRCheckType1 > 0 && recItem.CCRCheckType1 < 5)
            {
                if (recItem.CCRCheckType1 == 1)
                {//施工抽查清單
                    Stitems = constCheckRecService.GetConstCheckControlSt<ControlStVModel>(recItem.ItemSeq, recItem.CCRCheckFlow);
                }
                else if (recItem.CCRCheckType1 == 2)
                {//設備運轉測試清單
                    Stitems = constCheckRecService.GetEquOperControlSt<ControlStVModel>(recItem.ItemSeq);
                }
                else if (recItem.CCRCheckType1 == 3)
                {//職業安全衛生清單
                    Stitems = constCheckRecService.GetOccuSafeHealthControlSt<ControlStVModel>(recItem.ItemSeq);
                }
                else if (recItem.CCRCheckType1 == 4)
                {//環境保育清單
                    Stitems = constCheckRecService.GetEnvirConsControlSt<ControlStVModel>(recItem.ItemSeq, recItem.CCRCheckFlow);
                }
            }
            XWPFTable table1 = doc.Tables[2];
            //for (int i = 1; i < Stitems.Count; i++)
            //{
            //    table1.CreateRow();
            //}
            string check = "";
            int j = 0;
            for (int i = 0; i < Stitems.Count; i++)
            {
                if (check != Stitems[i].CheckItem1)
                {
                    check = Stitems[i].CheckItem1;
                }
                else
                {
                    j++;
                    continue;
                }
                table1.CreateRow();
                table1.GetRow(i - j + 1).GetCell(0).SetText(Stitems[i].CheckItem1);
                //圖片
                List<UploadFileModel> UFitems = new ConstCheckRecFileService().GetPhotos<UploadFileModel>(recItem.Seq, Stitems[i].ControllStSeq);
                string filePath = Utils.GetEngMainFolder(engItem.Seq);
                for (int y = 0; y < UFitems.Count; y++)
                {
                    string filePathName = Path.Combine(filePath, UFitems[y].UniqueFileName);
                    addFile(doc.Tables[2].GetRow(i - j + 1).GetCell(1), filePathName);
                }
            }

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
                //pdf檔路徑
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


        //4-環境保育-抽查紀錄表
        public string CheckSheet4(EngConstructionEngInfoVModel engItem, ConstCheckRecSheetModel recItem, string uuid, SignatureFileService service, int filetype)
        {
            string tempFile = CopyTemplateFile("4-環境保育-抽查紀錄表.docx");
            string outFile = recItem.GetFilename(uuid);
            List<ControlStVModel> items = recItem.items;
            //string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            table.GetRow(0).GetCell(1).SetText(engItem.EngName);
            table.GetRow(1).GetCell(1).SetText(engItem.subEngName + '-' + engItem.subEngItemNo);
            table.GetRow(2).GetCell(1).SetText(recItem.CCRPosDesc);
            string[] d = recItem.chsCheckDate.Split('/');
            table.GetRow(2).GetCell(3).SetText(String.Format("{0}年{1}月{2}日", d[0], d[1], d[2]));
            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                Utils.insertRow(table, 5, 6);
            }
            int rowInx = 0;
            foreach (ControlStVModel m in items)
            {
                table.GetRow(5 + rowInx).GetCell(1).SetText(m.CheckItem1);// + " " + m.CheckItem2);
                table.GetRow(5 + rowInx).GetCell(2).SetText(m.CCRRealCheckCond);
                table.GetRow(5 + rowInx).GetCell(3).SetText(GetCheckCaption(m.CCRCheckResult));

                rowInx++;
            }
            service.RenderConstCheckDoc(doc, recItem, addSignatureFile, addSignatureFileAlter);
            //addSignatureFile(doc.Tables[1].GetRow(0).GetCell(1), recItem.SupervisorUserSeq, service);
            //addSignatureFile(doc.Tables[1].GetRow(0).GetCell(3), recItem.SupervisorDirectorSeq, service);
            //
            List<ControlStVModel> Stitems = new List<ControlStVModel>();
            if (recItem.CCRCheckType1 > 0 && recItem.CCRCheckType1 < 5)
            {
                if (recItem.CCRCheckType1 == 1)
                {//施工抽查清單
                    Stitems = constCheckRecService.GetConstCheckControlSt<ControlStVModel>(recItem.ItemSeq, recItem.CCRCheckFlow);
                }
                else if (recItem.CCRCheckType1 == 2)
                {//設備運轉測試清單
                    Stitems = constCheckRecService.GetEquOperControlSt<ControlStVModel>(recItem.ItemSeq);
                }
                else if (recItem.CCRCheckType1 == 3)
                {//職業安全衛生清單
                    Stitems = constCheckRecService.GetOccuSafeHealthControlSt<ControlStVModel>(recItem.ItemSeq);
                }
                else if (recItem.CCRCheckType1 == 4)
                {//環境保育清單
                    Stitems = constCheckRecService.GetEnvirConsControlSt<ControlStVModel>(recItem.ItemSeq, recItem.CCRCheckFlow);
                }
            }
            FillCheckSheetImage(doc, engItem, recItem, Stitems);

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
                string path = Path.Combine(Path.GetTempPath(), uuid);
                string pdfFiles = recItem.Getdownloadname(uuid);
                string[] pdfFile = pdfFiles.Split('.');
                if (!Directory.Exists(path + "pdf")) Directory.CreateDirectory(path + "pdf");
                string filePatdownloadhName = Path.Combine(path + "pdf", pdfFile[0] + ".pdf");

                //匯出為 pdf
                wordDocument.ExportAsFixedFormat(filePatdownloadhName, WdExportFormat.wdExportFormatPDF);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();
                return filePatdownloadhName;
            }
            else if (filetype == 3)
            {
                //建立 word application instance
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                //開啟 word 檔案
                var wordDocument = appWord.Documents.Open(outFile);
                //odt檔路徑
                string path = Path.Combine(Path.GetTempPath(), uuid);
                string odtFiles = recItem.Getdownloadname(uuid);
                string[] odtFile = odtFiles.Split('.');
                if (!Directory.Exists(path + "odt")) Directory.CreateDirectory(path + "odt");
                string filePatdownloadhName = Path.Combine(path + "odt", odtFile[0] + ".odt");
                //匯出為 pdf
                wordDocument.SaveAs2(filePatdownloadhName, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatOpenDocumentText);

                //關閉 word 檔
                wordDocument.Close();
                //結束 word
                appWord.Quit();
                return filePatdownloadhName;
            }
            return outFile;

        }
        //4-環境保育-抽查紀錄表下載
        public Stream CheckSheet4download(EngConstructionEngInfoVModel engItem, ConstCheckRecSheetModel recItem, string uuid, SignatureFileService service, int filetype)
        {
            string tempFile = CopyTemplateFile("4-環境保育-抽查紀錄表.docx");
            string outFile = recItem.GetFilename(uuid);
            List<ControlStVModel> items = recItem.items;
            //string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            table.GetRow(0).GetCell(1).SetText(engItem.EngName);
            table.GetRow(1).GetCell(1).SetText(engItem.subEngName + '-' + engItem.subEngItemNo);
            table.GetRow(2).GetCell(1).SetText(recItem.CCRPosDesc);
            string[] d = recItem.chsCheckDate.Split('/');
            table.GetRow(2).GetCell(3).SetText(String.Format("{0}年{1}月{2}日", d[0], d[1], d[2]));
            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                Utils.insertRow(table, 5, 6);
            }
            int rowInx = 0;
            foreach (ControlStVModel m in items)
            {
                table.GetRow(5 + rowInx).GetCell(1).SetText(m.CheckItem1);// + " " + m.CheckItem2);
                table.GetRow(5 + rowInx).GetCell(2).SetText(m.CCRRealCheckCond);
                table.GetRow(5 + rowInx).GetCell(3).SetText(GetCheckCaption(m.CCRCheckResult));

                rowInx++;
            }
            service.RenderConstCheckDoc(doc, recItem, addSignatureFile, addSignatureFileAlter);
            //addSignatureFile(doc.Tables[1].GetRow(0).GetCell(1), recItem.SupervisorUserSeq, service);
            //addSignatureFile(doc.Tables[1].GetRow(0).GetCell(3), recItem.SupervisorDirectorSeq, service);
            //
            List<ControlStVModel> Stitems = new List<ControlStVModel>();
            if (recItem.CCRCheckType1 > 0 && recItem.CCRCheckType1 < 5)
            {
                if (recItem.CCRCheckType1 == 1)
                {//施工抽查清單
                    Stitems = constCheckRecService.GetConstCheckControlSt<ControlStVModel>(recItem.ItemSeq, recItem.CCRCheckFlow);
                }
                else if (recItem.CCRCheckType1 == 2)
                {//設備運轉測試清單
                    Stitems = constCheckRecService.GetEquOperControlSt<ControlStVModel>(recItem.ItemSeq);
                }
                else if (recItem.CCRCheckType1 == 3)
                {//職業安全衛生清單 
                    Stitems = constCheckRecService.GetOccuSafeHealthControlSt<ControlStVModel>(recItem.ItemSeq);
                }
                else if (recItem.CCRCheckType1 == 4)
                {//環境保育清單
                    Stitems = constCheckRecService.GetEnvirConsControlSt<ControlStVModel>(recItem.ItemSeq, recItem.CCRCheckFlow);
                }
            }
            XWPFTable table1 = doc.Tables[2];
            //for (int i = 1; i < Stitems.Count; i++)
            //{
            //    table1.CreateRow();
            //}
            string check = "";
            int j = 0;
            for (int i = 0; i < Stitems.Count; i++)
            {
                if (check != Stitems[i].CheckItem1)
                {
                    check = Stitems[i].CheckItem1;
                }
                else
                {
                    j++;
                    continue;
                }
                table1.CreateRow();
                table1.GetRow(i - j + 1).GetCell(0).SetText(Stitems[i].CheckItem1);
                //圖片
                List<UploadFileModel> UFitems = new ConstCheckRecFileService().GetPhotos<UploadFileModel>(recItem.Seq, Stitems[i].ControllStSeq);
                string filePath = Utils.GetEngMainFolder(engItem.Seq);
                for (int y = 0; y < UFitems.Count; y++)
                {
                    string filePathName = Path.Combine(filePath, UFitems[y].UniqueFileName);
                    addFile(doc.Tables[2].GetRow(i - j + 1).GetCell(1), filePathName);
                }
            }

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
                //pdf檔路徑
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



        private string GetCheckCaption(byte? data)
        {
            if (!data.HasValue) return "";
            else if (data.Value == 1) return "○";// 檢查合格";
            else if (data.Value == 2) return "╳";//有缺失";
            else return "／";// 無此項目";
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
        private void addSignatureFileAlter(
            XWPFTableCell cell, string filePath,
            NPOI.XWPF.UserModel.PictureType pictureType = NPOI.XWPF.UserModel.PictureType.JPEG
            )
        {


            var widthPic = NPOI.Util.Units.ToEMU(36 * 2.8);// (int)((double)3000 / 587 * 38.4 * 9525);
            var heightPic = NPOI.Util.Units.ToEMU(12 * 2.8);// (int)((double)900 / 587 * 38.4 * 9525);
            if (System.IO.File.Exists(filePath))
            {
                FileStream img = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                XWPFParagraph p = cell.AddParagraph();
                XWPFRun run = p.CreateRun();
                run.AddPicture(img, (int)pictureType, filePath, widthPic, heightPic);
                //run.AddCarriageReturn();
                //cell.AddParagraph().CreateRun().SetText("");
                img.Close();
            }
        }
        private void addFile(XWPFTableCell cell, string filePath, 
            NPOI.XWPF.UserModel.PictureType pictureType = NPOI.XWPF.UserModel.PictureType.JPEG)
        {
            //if (!userSeq.HasValue) return;
            //string fileName = service.GetFileNameByUser(userSeq.Value);
            //if (fileName == null) return;
            //
            //string filePath = Utils.GetSignatureFile(fileName);
            var widthPic = NPOI.Util.Units.ToEMU(48 * 2.8);// (int)((double)3000 / 587 * 38.4 * 9525);
            var heightPic = NPOI.Util.Units.ToEMU(36 * 2.8);// (int)((double)900 / 587 * 38.4 * 9525);
            var p = cell.GetParagraphArray(0);
            if (System.IO.File.Exists(filePath))
            {
                FileStream img = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                XWPFRun run = p.CreateRun();
                run.AddPicture(img, (int)pictureType, filePath, widthPic, heightPic);
                //run.AddCarriageReturn();
                //cell.AddParagraph().CreateRun().SetText("");
                img.Close();
            }
        }
        //for edit ===========================================================
        public JsonResult GetEngItem(int id)
        {
            List<EngConstructionEngInfoVModel> items = constCheckRecService.GetEngMainByEngConstructionSeq<EngConstructionEngInfoVModel>(id);
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
        public JsonResult GetRecCheckTypeOption(int id)
        {
            List<SelectIntOptionModel> items = constCheckRecService.GetRecCheckTypeOption<SelectIntOptionModel>(id);
            return Json(items);
        }
        //取得 抽查清單
        public JsonResult GetStdTypeOption(int engMain, int checkType)
        {
            List<SelectIntOptionModel> items = new List<SelectIntOptionModel>();
            if (checkType > 0 && checkType < 5)
            {
                if (checkType == 1)
                {//施工抽查清單
                    items = constCheckRecService.GetConstCheckList<SelectIntOptionModel>(engMain);
                }
                else if (checkType == 2)
                {//設備運轉測試清單
                    items = constCheckRecService.GetEquOperTestOption<SelectIntOptionModel>(engMain);
                }
                else if (checkType == 3)
                {//職業安全衛生清單
                    items = constCheckRecService.GetOccuSafeHealthList<SelectIntOptionModel>(engMain);
                }
                else if (checkType == 4)
                {//環境保育清單
                    items = constCheckRecService.GetEnvirConsList<SelectIntOptionModel>(engMain);
                }

                return Json(new
                {
                    result = 0,
                    item = items
                });
            } else {
                return Json(new
                {
                    result = 1,
                    message = "讀取資料錯誤"
                });
            }
        }
        //取得 抽查標準項目清單 
        public JsonResult GetControls(int engMain, int checkType, int stdType, int checkFlow)
        {
            List<ControlStVModel> items = new List<ControlStVModel>();
            if (checkType > 0 && checkType < 5)
            {
                if (checkType == 1)
                {//施工抽查清單
                    items = constCheckRecService.GetConstCheckControlSt<ControlStVModel> (stdType, checkFlow);
                }
                else if (checkType == 2)
                {//設備運轉測試清單
                    items = constCheckRecService.GetEquOperControlSt<ControlStVModel>(stdType);
                }
                else if (checkType == 3)
                {//職業安全衛生清單
                    items = constCheckRecService.GetOccuSafeHealthControlSt<ControlStVModel>(stdType);
                }
                else if (checkType == 4)
                {//環境保育清單
                    items = constCheckRecService.GetEnvirConsControlSt<ControlStVModel>(stdType, checkFlow);
                }
                JoinCell(items);
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

        //搜索 抽檢清單名稱
        public JsonResult SearchCheckTypeByKeyWord(int engMain, string keyWord)
        {
            List<ConstCheckRecModel> items = constCheckRecService.SearchCheckTypeByName<ConstCheckRecModel>(engMain, keyWord);
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
        //新增檢驗單
        public JsonResult NewRec(ConstCheckRecModel item)
        {
            if(constCheckRecService.Add(item))
            {
                //var nItem = item;
                return Json(new
                {
                    result = 0,
                    seq = item.Seq
                });
            }
            else
            {
                return Json(new
                {
                    result = 1,
                    message = "新增失敗"
                });
            }
        }
        //檢驗單表頭
        public JsonResult GetRec(int recSeq)
        {
            List<ConstCheckRecModel> items = constCheckRecService.GetItem<ConstCheckRecModel>(recSeq);
            if(items.Count>0) { 
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
        //檢驗單清單 單一項目
        public JsonResult GetRecOptionByCheckType(int constructionSeq, int checkTypeSeq)
        {
            List<ConstCheckRecOptionVModel> items = constCheckRecService.GetListByCheckType<ConstCheckRecOptionVModel>(constructionSeq, checkTypeSeq);
            return Json(items);
        }
        //檢驗單清單 不分分項工程 s20230524
        public JsonResult GetRecOptionByCheckType1(int id, int checkTypeSeq, int cFlow, int itemSeq, int perPage, int pageIndex)
        {
            int total = constCheckRecService.GetListByCheckType1Count(id, checkTypeSeq, cFlow, itemSeq);
            List<ConstCheckRecOption1VModel> items = constCheckRecService.GetListByCheckType1<ConstCheckRecOption1VModel>(id, checkTypeSeq, cFlow, itemSeq, perPage, pageIndex);
            return Json(new
            {
                result = 0,
                totalRows = total,
                items = items
            });
        }
 
        public JsonResult DeleteRecResultControlSt(int controlStSeq, int recSeq, int span, int CCRCheckType1)
        {
            //using(var context = new EQC_NEW_Entities())
            //{
            //    var  a = context.ConstCheckRecResult
            //        .Where(r => r.ControllStSeq == controlStSeq && r.ConstCheckRec.Seq == recSeq).ToList();
            //    context.ConstCheckRecResult.RemoveRange(
            //    context.ConstCheckRecResult
            //        .Where(r => r.ControllStSeq == controlStSeq && r.ConstCheckRec.Seq == recSeq)
            //    );
            //    context.SaveChanges();
            //}

            if(CCRCheckType1 == 1)
                DeleteRecResultControlSt<ConstCheckControlSt>(controlStSeq, span, recSeq,
                db => db.ConstCheckControlSt, item => item.ConstCheckListSeq);
            if (CCRCheckType1 == 4)
                DeleteRecResultControlSt<EnvirConsControlSt>(controlStSeq, span, recSeq,
                db => db.EnvirConsControlSt, item => item.EnvirConsListSeq);
            if (CCRCheckType1 == 2)
                DeleteRecResultControlSt<EquOperControlSt>(controlStSeq, span, recSeq,
                db => db.EquOperControlSt, item => item.EquOperTestStSeq);
            if (CCRCheckType1 == 3)
                DeleteRecResultControlSt<OccuSafeHealthControlSt>(controlStSeq, span, recSeq,
                db => db.OccuSafeHealthControlSt, item => item.OccuSafeHealthListSeq);
            return Json(true);
        }
        public static void DeleteRecResultControlSt<T>(
            int controlStSeq,
            int span,
            int recSeq,
            Func<EQC_NEW_Entities, DbSet<T>> setF,
            Func<T, int?> itemSeqF
            ) where T : class, ControlSt
        {
            using (var context = new EQC_NEW_Entities())
            {

                var compareArr =
                    context.ConstCheckRecResult
                    .Where(r => r.ConstCheckRec.Seq == recSeq)
                    .ToList()
                    .Join(setF.Invoke(context),
                        r1 => r1.ControllStSeq,
                        r2 => r2.Seq,
                        (r1, r2) =>
                        {
                            r1.OrderNo = r2.OrderNo ?? 0;
                            return r1;
                        }
                    )
                    .OrderBy(r => r.OrderNo)
                    .ToArray();
                var a = compareArr
                    .Where(r => r.ControllStSeq == controlStSeq)
                    .FirstOrDefault();
                var targetIndex = Array.IndexOf(
                    compareArr, a
                );


                if (compareArr.Length > 0)
                {
                    context.ConstCheckRecResult.RemoveRange(
                        compareArr.ToList().Skip(targetIndex).Take(span)

                    );
                }

                context.SaveChanges();
            }
           
        }

        //檢驗單明細
        public JsonResult GetRecResult(ConstCheckRecModel rec)
        {
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

            service.GetRecResultStandard(items);
            
            if (items.Count > 0) {
                JoinCell(items);
                foreach(ControlStVModel item in items)
                {
                    item.changed = true;
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
                    message = "無資料"
                });
            }
        }
        //檢驗單明細 s20230302
        public JsonResult GetRecResultHistory(ConstCheckRecModel rec)
        {
            ConstCheckRecResultService service = new ConstCheckRecResultService();
            List<ControlStVModel> items = new List<ControlStVModel>();
            if (rec.CCRCheckType1 == 1)
            {//施工抽查清單
                items = service.GetConstCheckRecResultHistory<ControlStVModel>(rec.Seq);
            }
            else if (rec.CCRCheckType1 == 2)
            {//設備運轉測試清單
                items = service.GetEquOperRecResultHistory<ControlStVModel>(rec.Seq);
            }
            else if (rec.CCRCheckType1 == 3)
            {//職業安全衛生清單
                items = service.GetOccuSafeHealthRecResultHistory<ControlStVModel>(rec.Seq);
            }
            else if (rec.CCRCheckType1 == 4)
            {//環境保育清單
                items = service.GetEnvirConsRecResultHistory<ControlStVModel>(rec.Seq);
            }
            if (items.Count > 0)
            {
                JoinCell(items);
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
                    message = "無資料"
                });
            }
        }
        //更新檢驗單
        public JsonResult UpdateRec(ConstCheckRecModel recItem, List<ControlStVModel> items)
        {
            var iSignatureFileService = new SignatureFileService();
            //if(iSignatureFileService.GetFileNameByUser(recItem.SupervisorDirectorSeq ?? 0) != null)
            //{
            //    iSignatureFileService.UpdateSignatureFile(recItem.SupervisorDirectorSeq ?? 0, recItem.SupervisionDirectorSignaturePath);
            //}
            //else
            //{
            //    iSignatureFileService.AddSignatureFile(recItem.SupervisorDirectorSeq ?? 0, recItem.SupervisionDirectorSignaturePath);
            //}
            //if (iSignatureFileService.GetFileNameByUser(recItem.SupervisorUserSeq ?? 0) != null)
            //{
            //    iSignatureFileService.UpdateSignatureFile(recItem.SupervisorUserSeq ?? 0, recItem.SupervisionComSignaturePath);
            //}
            //else
            //{
            //    iSignatureFileService.AddSignatureFile(recItem.SupervisorUserSeq ?? 0, recItem.SupervisionComSignaturePath);
            //}
            constCheckRecService.UpdateRecResultStandard(items);
            if (
                constCheckRecService.Update(recItem, items) 

            )
            {
                return Json(new
                {
                    result = 0,
                    message = "儲存成功"
                });
            } else
            {
                return Json(new
                {
                    result = -1,
                    message = "儲存失敗"
                });
            }
        }
        //合併儲存格前置處裡
        public void JoinCell(List<ControlStVModel> items)
        {
            if (items.Count == 0) return;

            string checkItem = "----";
            string checkItemStd1 = "----";
            int gInx = -1, gCount = 0;
            int gInxStd1 = -1, gCountStd1 = 0;
            for (int i = 0; i < items.Count; i++)
            {
                string key = items[i].CheckItem1;// + items[i].CheckItem2;
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
                    items[i].itemType = -1;//s20231016
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
        //抽查單確認
        public JsonResult FormConfirm(EngConstructionEngInfoVModel engMain, int id)
        {
            int count = constCheckRecService.FormConfirm(id, 1);
            if (count == 1)
            {
                if(!String.IsNullOrEmpty(engMain.SupervisorContact))
                {
                    string mailBody = "工程編號:"+ engMain.EngNo+"<br> 分項工程:"+engMain.subEngName+"<br> 有抽查紀錄填報單已確認, 等待審核.";
                    Utils.Email(engMain.SupervisorContact, "抽查紀錄填報-待審核通知", mailBody);
                }
                return Json(new
                {
                    result = 0,
                    message = "確認完成"
                });
            }
            else
            {
                return Json(new
                {
                    result = 1,
                    message = "確認失敗"
                });
            }
        }
        //
        public JsonResult DelRec(int id)
        {
            if (constCheckRecService.DelRec(id))
            {
                return Json(new
                {
                    result = 0,
                    message = "刪除完成"
                });
            }
            else
            {
                return Json(new
                {
                    result = 1,
                    message = "刪除失敗"
                });
            }
        }
        //檢驗照片 上傳
        public JsonResult PhotoUpload(int engMain, int recSeq, int ctlSeq, string photoDesc,string restful)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                ConstCheckRecFileModel model = new ConstCheckRecFileModel();
                model.ConstCheckRecSeq = recSeq;
                model.ControllStSeq = ctlSeq;
                model.Memo = photoDesc;
                model.RESTful = restful;
                try
                {
                    if (SaveFile(file, engMain, model, "RecPhoto-"))
                    {
                        if (new ConstCheckRecFileService().Add(model))
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
        //取得 照片群組清單
        public JsonResult GetImgGroupOption(int recSeq, int checkType)
        {
            List<SelectIntOptionModel> items = new List<SelectIntOptionModel>();
            if (checkType > 0 && checkType < 5)
            {
                if (checkType == 1)
                {//施工抽查
                    items = constCheckRecService.GetConstCheckPhotoGroupOption<SelectIntOptionModel>(recSeq);
                }
                else if (checkType == 2)
                {//設備運轉
                    items = constCheckRecService.GetEquOperPhotoGroupOption<SelectIntOptionModel>(recSeq);
                }
                else if (checkType == 3)
                {//職業安全衛生
                    items = constCheckRecService.GetOccuSafeHealthGroupOption<SelectIntOptionModel>(recSeq);
                }
                else if (checkType == 4)
                {//環境保育
                    items = constCheckRecService.GetEnvirConsGroupOption<SelectIntOptionModel>(recSeq);
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
            List<UploadFileModel> items = new ConstCheckRecFileService().GetPhotos<UploadFileModel>(recSeq, ctlSeq);
            string filePath = Utils.GetEngMainFolder(engMain);
            /*foreach(UploadFileModel item in items)
            {
                item.UniqueFileName = String.Format("../FileUploads/Eng/{0}/{1}", engMain, item.UniqueFileName);
            }*/

            return Json(items);
        }
        //檢驗照片 刪除
        public JsonResult DelResultPhoto(int engMain, int fileSeq)
        {
            ConstCheckRecFileService service = new ConstCheckRecFileService();
            List<ConstCheckRecFileModel> items = service.GetItem(fileSeq);
            if (items.Count > 0)
            {
                ConstCheckRecFileModel model = items[0];
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
        //有無蜂窩欄位 更新
        public JsonResult updataRESTful(string RESTful, int fileSeq, string memo)
        {
            ConstCheckRecFileService service = new ConstCheckRecFileService();
            List<ConstCheckRecFileModel> items = service.GetItem(fileSeq);
            if (items.Count > 0)
            {
                items[0].Memo = memo;
                items[0].RESTful = RESTful;
                ConstCheckRecFileModel model = items[0];
                try
                {
                    if (service.update(model))
                    {
                        return Json(new
                        {
                            result = 0,
                            message = "有無蜂窩更新成功"
                        });
                    }
                    return Json(new
                    {
                        result = -1,
                        message = "有無蜂窩更新失敗",
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
        private bool SaveFile(HttpPostedFileBase file, int engMainSeq, ConstCheckRecFileModel m, string fileHeader)
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
                //HttpClientPost(fullPath);
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

        private bool DelFile(int engMainSeq, ConstCheckRecFileModel m)
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

      //  public JsonResult HttpClientPost(string fullPath)
      //  {
      //     // using (var client = new HttpClient())
      //     // using (formData)
      //     // {
      //          try
      //          {
      //          using (var content = new MultipartFormDataContent())
      //          {
      //              content.Add(new StringContent("image"), "value1", "test.jpg");
      //              var req = new HttpClient();
      //              var a= req.PostAsync("https://211.22.221.188:5001/api", content).Result;
      //          }
      //
      //          //         
      //          //         var response = client.PostAsync("http://211.22.221.188:5001/api",formData).Result;
      //          //         if (!response.IsSuccessStatusCode)
      //          //         {
      //          //             return Json(new
      //          //           {
      //          //               response,
      //          //               message = "上傳失敗"
      //          //             });
      //          //         }
      //      }
      //      catch (Exception Error)
      //          {
      //              return Json(new
      //              {
      //                  message = "上傳失敗"
      //              });
      //          }
      //          return Json(new
      //          {
      //              message = "上傳失敗"
      //          });
      //     // }
      //
      //
      //  }

        private void Log(string msg)
        {
            string path = WebConfigurationManager.AppSettings["PPErrorLogPath"] ?? "";
            path = System.Web.HttpContext.Current.Server.MapPath(path);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, "test");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string file = "Log.txt";

            string pathAndFile = Path.Combine(path, file);

            msg = String.Format("{0} {1}" + Environment.NewLine, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg);
            System.IO.File.AppendAllText(pathAndFile, msg);
        }
        public JsonResult PhotoUpload1(int engMain, int recSeq, int ctlSeq, string photoDesc)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                ConstCheckRecFileModel model = new ConstCheckRecFileModel();
                model.ConstCheckRecSeq = recSeq;
                model.ControllStSeq = ctlSeq;
                model.Memo = photoDesc;
                try
                {
                    if (SaveFile(file, engMain, model, "RecPhoto-"))
                    {
                        if (new ConstCheckRecFileService().Add1(model))
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
    }
}