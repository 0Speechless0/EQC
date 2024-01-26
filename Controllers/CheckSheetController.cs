using EQC.Common;
using EQC.Models;
using EQC.Services;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class CheckSheetController : Controller
    {
        public ActionResult Index()
        {
            //ViewBag.Title = "抽查紀錄表維護";
            return View("CheckSheet");
        }

        private string GetFlowChartPath()
        {
            return Utils.GetTemplateFolder();
        }
        //第五章 材料設備清冊範本
        public JsonResult Chapter5(int pageIndex, int perPage)
        {
            List<EMDListTpEditModel> result = new List<EMDListTpEditModel>();
            EngMaterialDeviceListTpService service = new EngMaterialDeviceListTpService();
            int total = service.GetListCount();
            if (total > 0)
            {
                result = service.ListAll(pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        public ActionResult Chapter5Download(int seq)
        {
            List<EMDCTpModel> items = new QCStdBaseService().GetEngMaterialDeviceControlTpByEngMaterialDeviceListTpSeq<EMDCTpModel>(seq.ToString(), 1, 9999);

            if (items.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "沒有抽檢明細資料"
                }, JsonRequestBehavior.AllowGet);
            }

            string tempFile = CopyTemplateFile("抽查紀錄表維護-5-材料品質抽驗紀錄表.docx");
            string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                Utils.insertRow(table, 3, 4);
            }
            int rowInx = 0;
            foreach (EMDCTpModel m in items)
            {
                table.GetRow(3 + rowInx).GetCell(0).SetText(m.MDTestItem);
                table.GetRow(3 + rowInx).GetCell(1).SetText(m.MDTestStand1);// + " " + m.MDTestStand2);
                rowInx++;
            }

            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();
            Stream iStream = new FileStream(outFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", "材料設備抽驗紀錄表.docx");
        }

        //第六章 設備功能運轉測試抽驗程序及標準
        public JsonResult Chapter6(int pageIndex, int perPage)
        {
            List<EOTListTpEditModel> result = new List<EOTListTpEditModel>();
            EquOperTestListTpService service = new EquOperTestListTpService();
            //result = service.ListAll(pageIndex, perPage);
            //return Json(result);
            int total = service.GetListCount();
            if (total > 0)
            {
                result = service.ListAll(pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        //
        public ActionResult Chapter6Download(int seq)
        {
            List<EOCTpModel> items = new QCStdBaseService().GetEquOperControlTpByEquOperTestTpSeq<EOCTpModel>(seq.ToString(), 1, 9999);

            if (items.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "沒有抽檢明細資料"
                }, JsonRequestBehavior.AllowGet);
            }

            string tempFile = CopyTemplateFile("抽查紀錄表維護-6-設備運轉測試抽驗紀錄表.docx");
            string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                Utils.insertRow(table, 6, 7);
            }
            int rowInx = 0;
            foreach (EOCTpModel m in items)
            {
                table.GetRow(6 + rowInx).GetCell(0).SetText(m.EPCheckItem1);// + m.EPCheckItem2);
                table.GetRow(6 + rowInx).GetCell(1).SetText(m.EPStand1);// + m.EPStand2 + m.EPStand3 + " " + m.EPStand4 + m.EPStand5);
                rowInx++;
            }

            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();

            Stream iStream = new FileStream(outFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", "施設備運轉測試抽驗紀錄表.docx");
        }


        //第七章 701 施工抽查程序及標準
        public JsonResult Chapter701(int pageIndex, int perPage)
        {
            List<CCListTpEditModel> result = new List<CCListTpEditModel>();
            ConstCheckListTpService service = new ConstCheckListTpService();
            int total = service.GetListCount();
            if (total > 0)
            {
                result = service.ListAll(pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }

        public ActionResult Chapter701Download(int seq)
        {
            List<CCCTpModel> items = new QCStdBaseService().GetConstCheckControlTpByConstCheckListTpSeq<CCCTpModel>(seq.ToString(), 1, 9999);

            if(items.Count==0)
            {
                return Json(new
                {
                    result = -1,
                    message = "沒有抽檢明細資料"
                }, JsonRequestBehavior.AllowGet);
            }

            string tempFile = CopyTemplateFile("抽查紀錄表維護-701-施工抽查紀錄表.docx");
            string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                Utils.insertRow(table, 6, 7);
            }
            int rowInx = 0;
            foreach(CCCTpModel m in items)
            {
                string flowName = "施工後";
                if (m.CCFlow1 == 1)
                {
                    flowName = "施工前";
                }
                else if (m.CCFlow1 == 2)
                {
                    flowName = "施工中";
                }
                table.GetRow(6 + rowInx).GetCell(0).SetText(flowName + " " + m.CCFlow2);
                table.GetRow(6 + rowInx).GetCell(1).SetText(m.CCManageItem1);// +" "+ m.CCManageItem2);
                table.GetRow(6 + rowInx).GetCell(2).SetText(m.CCCheckStand1);// + " " + m.CCCheckStand2);
                rowInx++;
            }

            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();

            Stream iStream = new FileStream(outFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", "施工抽查紀錄表.docx");
        }

        //第七章 702 環境保育抽查標準範本
        public JsonResult Chapter702(int pageIndex, int perPage)
        {
            List<ECListTpEditModel> result = new List<ECListTpEditModel>();
            EnvirConsListTpService service = new EnvirConsListTpService();
            int total = service.GetListCount();
            if (total > 0)
            {
                result = service.ListAll(pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }

        public ActionResult Chapter702Download(int seq)
        {
            /*return Json(new
            {
                result = -1,
                message = "尚未實作"
            }, JsonRequestBehavior.AllowGet);*/
            List<ECCTpModel> items = new QCStdBaseService().GetEnvirConsControlTpByEnvirConsListSeq<ECCTpModel>(seq.ToString(), 1, 9999);
            if (items.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "沒有抽檢明細資料"
                }, JsonRequestBehavior.AllowGet);
            }

            string tempFile = CopyTemplateFile("抽查紀錄表維護-702-生態保育措施抽查紀錄表.docx");
            string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                Utils.insertRow(table, 5, 6);
            }
            int rowInx = 0;
            int oldRow = -1;
            byte? oldECCFlow1 = 0;
            
            foreach (ECCTpModel m in items)
            {
                if (oldECCFlow1 != m.ECCFlow1)
                {
                    oldECCFlow1 = m.ECCFlow1;
                    
                    XWPFTableCell cellFirstofThird = table.GetRow(5 + rowInx).GetCell(0);
                    CT_Tc cttcFirstofThird = cellFirstofThird.GetCTTc();
                    CT_TcPr ctPrFirstofThird = cttcFirstofThird.AddNewTcPr();
                    ctPrFirstofThird.AddNewVMerge().val = ST_Merge.restart;//開始合併
                    ctPrFirstofThird.AddNewVAlign().val = ST_VerticalJc.center;//垂直置中
                    cttcFirstofThird.GetPList()[0].AddNewPPr().AddNewJc().val = ST_Jc.center;

                    string flowName = "施工後";
                    if (m.ECCFlow1 == 1)
                    {
                        flowName = "施工前";
                    }
                    else if (m.ECCFlow1 == 2)
                    {
                        flowName = "施工中";
                    }
                    table.GetRow(5 + rowInx).GetCell(0).SetText(flowName);
                } else {
                    XWPFTableCell cellfirstofRow = table.GetRow(5 + rowInx).GetCell(0);
                    CT_Tc cttcfirstofRow = cellfirstofRow.GetCTTc();
                    CT_TcPr ctPrfirstofRow = cttcfirstofRow.AddNewTcPr();
                    ctPrfirstofRow.AddNewVMerge().val = ST_Merge.@continue;//續合併
                    ctPrfirstofRow.AddNewVAlign().val = ST_VerticalJc.center;//垂直置中
                }
                table.GetRow(5 + rowInx).GetCell(1).SetText(m.ECCCheckItem1);// + " " + m.ECCCheckItem2);
                rowInx++;
            }
            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();

            Stream iStream = new FileStream(outFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", "施工抽查(環境保育)紀錄表.docx");
        }

        //第七章 703 職業安全衛生抽查標準範本
        public JsonResult Chapter703(int pageIndex, int perPage)
        {
            List<OSHListTpEditModel> result = new List<OSHListTpEditModel>();
            OccuSafeHealthListTpService service = new OccuSafeHealthListTpService();
            int total = service.GetListCount();
            if (total > 0)
            {
                result = service.ListAll(pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }

        public ActionResult Chapter703Download(int seq)
        {
            List<OSHCTpModel> items = new QCStdBaseService().GetOccuSafeHealthControlTpByOccuSafeHealthListSeq<OSHCTpModel>(seq.ToString(), 1, 9999);

            if (items.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "沒有抽檢明細資料"
                }, JsonRequestBehavior.AllowGet);
            }

            string tempFile = CopyTemplateFile("抽查紀錄表維護-703-汛期工地防災減災抽查紀錄表.docx");
            string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                Utils.insertRow(table, 4, 5);
            }
            int rowInx = 0;
            foreach (OSHCTpModel m in items)
            {
                table.GetRow(4 + rowInx).GetCell(0).SetText(m.OSCheckItem1);// + " " + m.OSCheckItem2);
                table.GetRow(4 + rowInx).GetCell(1).SetText(m.OSStand1);// + " " + m.OSStand2 + " " + m.OSStand3 + " " + m.OSStand4 + " " + m.OSStand5);
                rowInx++;
            }

            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();

            Stream iStream = new FileStream(outFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", "施工抽查(職業安全衛生)紀錄表.docx");
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