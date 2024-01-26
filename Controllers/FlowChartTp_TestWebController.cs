using EQC.Common;
using EQC.Models;
using EQC.Services;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class FlowChartTp_TestWebController : Controller
    {
        public ActionResult Index()
        {
            //ViewBag.Title = "清單及流程圖維護";
            Utils.setUserClass(this);
            return View("");
        }

        public ActionResult Index2()
        {
            return View("Index2");
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
                result = service.ListAll(pageIndex, total);
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

        public ActionResult Chapter5Show(int seq)
        {
            EngMaterialDeviceListTpService service = new EngMaterialDeviceListTpService();
            List<EMDListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            string folderName = "/FileUploads/Tp";
            string uniqueFileName = items[0].FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(folderName, uniqueFileName);
                return Json(new
                {
                    url = fullPath
                }) ;
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Chapter5CekDownload(int seq)
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

            //轉圖檔
            var msWordApp = new Microsoft.Office.Interop.Word.Application();
            msWordApp.Visible = true;
            //開啟該Word檔
            Microsoft.Office.Interop.Word.Document doc1 = msWordApp.Documents.Open(outFile);
            //Opens the word document and fetch each page and converts to image
            foreach (Microsoft.Office.Interop.Word.Window window in doc1.Windows)
            {
                foreach (Microsoft.Office.Interop.Word.Pane pane in window.Panes)
                {
                    for (var i = 1; i <= pane.Pages.Count; i++)
                    {
                        var page = pane.Pages[i];
                        object bits = page.EnhMetaFileBits;//Returns a Object that represents a picture 【Read-only】
                                                           //以下Try Catch區段為將圖片的背景填滿為白色，不然輸出的圖片背景會是透明的
                        try
                        {
                            using (var ms = new MemoryStream((byte[])bits))
                            {
                                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

                                using (var backGroundWritePNG = new Bitmap(image.Width, image.Height))
                                {
                                    //設定圖片的解析度
                                    backGroundWritePNG.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                                    using (Graphics graphis = Graphics.FromImage(backGroundWritePNG))
                                    {
                                        graphis.Clear(Color.White);
                                        graphis.DrawImageUnscaled(image, 0, 0);
                                    }
                                    string filePath = GetFlowChartPath();
                                    string outPutFilePath = Path.ChangeExtension(filePath+"Cha5_" + seq +"_"+ i, "png");
                                    backGroundWritePNG.Save(outPutFilePath, ImageFormat.Png);//輸出圖片
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + ex.Message);
                            return null;
                        }
                    }
                }
            }
            //關閉Word，釋放資源
            doc1.Close(Type.Missing, Type.Missing, Type.Missing);
            msWordApp.Quit(Type.Missing, Type.Missing, Type.Missing);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(doc1);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(msWordApp);

            string folderName = "/FileUploads/TpCha5_"+seq+"_1.png";
            return Json(new
            {
                url = folderName
            });

        }

     //  public ActionResult Chapter5WordToPNG(int seq)
     //  {
     //      string docPath = @"D:\DocToImg\Source.docx"; //Doc檔案路徑
     //      string outPutByOfficePath = @"D:\DocToImg\PrintByOffice_Page_"; //Doc圖片輸出路徑 by Office
     //      var msWordApp = new Microsoft.Office.Interop.Word.Application();
     //      msWordApp.Visible = true;
     //      Microsoft.Office.Interop.Word.Document doc = msWordApp.Documents.Open(docPath);
     //
     //
     //  }
            //第六章 設備功能運轉測試抽驗程序及標準
            public JsonResult Chapter6(int pageIndex, int perPage)
        {
            List<EOTListTpEditModel> result = new List<EOTListTpEditModel>();
            EquOperTestListTpService service = new EquOperTestListTpService();
            int total = service.GetListCount();
            if (total > 0)
            {
                result = service.ListAll(pageIndex, total);
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
        //
        public ActionResult Chapter6ShowWord(int seq)
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

            //轉圖檔
            var msWordApp = new Microsoft.Office.Interop.Word.Application();
            msWordApp.Visible = true;
            //開啟該Word檔，天啊！　真的開起Word，我真得是醉了
            Microsoft.Office.Interop.Word.Document doc1 = msWordApp.Documents.Open(outFile);
            //Opens the word document and fetch each page and converts to image
            foreach (Microsoft.Office.Interop.Word.Window window in doc1.Windows)
            {
                foreach (Microsoft.Office.Interop.Word.Pane pane in window.Panes)
                {
                    for (var i = 1; i <= pane.Pages.Count; i++)
                    {
                        var page = pane.Pages[i];
                        object bits = page.EnhMetaFileBits;//Returns a Object that represents a picture 【Read-only】
                                                           //以下Try Catch區段為將圖片的背景填滿為白色，不然輸出的圖片背景會是透明的
                        try
                        {
                            using (var ms = new MemoryStream((byte[])bits))
                            {
                                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

                                using (var backGroundWritePNG = new Bitmap(image.Width, image.Height))
                                {
                                    //設定圖片的解析度
                                    backGroundWritePNG.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                                    using (Graphics graphis = Graphics.FromImage(backGroundWritePNG))
                                    {
                                        graphis.Clear(Color.White);
                                        graphis.DrawImageUnscaled(image, 0, 0);
                                    }
                                    string filePath = GetFlowChartPath();
                                    string outPutFilePath = Path.ChangeExtension(filePath + "Cha6_" + seq + "_" + i, "png");
                                    backGroundWritePNG.Save(outPutFilePath, ImageFormat.Png);//輸出圖片
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + ex.Message);
                            return null;
                        }
                    }
                }
            }
            //關閉Word，釋放資源
            doc1.Close(Type.Missing, Type.Missing, Type.Missing);
            msWordApp.Quit(Type.Missing, Type.Missing, Type.Missing);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(doc1);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(msWordApp);

            string folderName = "/FileUploads/TpCha6_" + seq + "_1.png";
            return Json(new
            {
                url = folderName
            });
        }



        public ActionResult Chapter6Show(int seq)
        {
            EquOperTestListTpService service = new EquOperTestListTpService();
            List<EOTListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            string folderName = "/FileUploads/Tp";
            string uniqueFileName = items[0].FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(folderName, uniqueFileName);
                return Json(new
                {
                    url = fullPath
                });
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }

        //第七章 701 施工抽查程序及標準
        public JsonResult Chapter701(int pageIndex, int perPage)
        {
            List<CCListTpEditModel> result = new List<CCListTpEditModel>();
            ConstCheckListTpService service = new ConstCheckListTpService();
            int total = service.GetListCount();
            if (total > 0)
            {
                result = service.ListAll(pageIndex, total);
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

            if (items.Count == 0)
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
            foreach (CCCTpModel m in items)
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

        public ActionResult Chapter701ShowWord(int seq)
        {
            List<CCCTpModel> items = new QCStdBaseService().GetConstCheckControlTpByConstCheckListTpSeq<CCCTpModel>(seq.ToString(), 1, 9999);

            if (items.Count == 0)
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
            foreach (CCCTpModel m in items)
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

            //轉圖檔
            var msWordApp = new Microsoft.Office.Interop.Word.Application();
            msWordApp.Visible = true;
            //開啟該Word檔，天啊！　真的開起Word，我真得是醉了
            Microsoft.Office.Interop.Word.Document doc1 = msWordApp.Documents.Open(outFile);
            //Opens the word document and fetch each page and converts to image
            foreach (Microsoft.Office.Interop.Word.Window window in doc1.Windows)
            {
                foreach (Microsoft.Office.Interop.Word.Pane pane in window.Panes)
                {
                    for (var i = 1; i <= pane.Pages.Count; i++)
                    {
                        var page = pane.Pages[i];
                        object bits = page.EnhMetaFileBits;//Returns a Object that represents a picture 【Read-only】
                                                           //以下Try Catch區段為將圖片的背景填滿為白色，不然輸出的圖片背景會是透明的
                        try
                        {
                            using (var ms = new MemoryStream((byte[])bits))
                            {
                                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

                                using (var backGroundWritePNG = new Bitmap(image.Width, image.Height))
                                {
                                    //設定圖片的解析度
                                    backGroundWritePNG.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                                    using (Graphics graphis = Graphics.FromImage(backGroundWritePNG))
                                    {
                                        graphis.Clear(Color.White);
                                        graphis.DrawImageUnscaled(image, 0, 0);
                                    }
                                    string filePath = GetFlowChartPath();
                                    string outPutFilePath = Path.ChangeExtension(filePath + "Cha701_" + seq + "_" + i, "png");
                                    backGroundWritePNG.Save(outPutFilePath, ImageFormat.Png);//輸出圖片
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + ex.Message);
                            return null;
                        }
                    }
                }
            }
            //關閉Word，釋放資源
            doc1.Close(Type.Missing, Type.Missing, Type.Missing);
            msWordApp.Quit(Type.Missing, Type.Missing, Type.Missing);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(doc1);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(msWordApp);

            string folderName = "/FileUploads/TpCha701_" + seq + "_1.png";
            return Json(new
            {
                url = folderName
            });
        }



        public ActionResult Chapter701Show(int seq)
        {
            ConstCheckListTpService service = new ConstCheckListTpService();
            List<CCListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            string folderName = "/FileUploads/Tp";
            string uniqueFileName = items[0].FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(folderName, uniqueFileName);
                return Json(new
                {
                    url = fullPath
                });
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }


        //第七章 702 環境保育抽查標準範本
        public JsonResult Chapter702(int pageIndex, int perPage)
        {
            List<ECListTpEditModel> result = new List<ECListTpEditModel>();
            EnvirConsListTpService service = new EnvirConsListTpService();
            int total = service.GetListCount();
            if (total > 0)
            {
                result = service.ListAll(pageIndex, total);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        

        public ActionResult Chapter702Download(int seq)
        {
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
                }
                else
                {
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

        public ActionResult Chapter702ShowWord(int seq)
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
                }
                else
                {
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

            //轉圖檔
            var msWordApp = new Microsoft.Office.Interop.Word.Application();
            msWordApp.Visible = true;
            //開啟該Word檔，天啊！　真的開起Word，我真得是醉了
            Microsoft.Office.Interop.Word.Document doc1 = msWordApp.Documents.Open(outFile);
            //Opens the word document and fetch each page and converts to image
            foreach (Microsoft.Office.Interop.Word.Window window in doc1.Windows)
            {
                foreach (Microsoft.Office.Interop.Word.Pane pane in window.Panes)
                {
                    for (var i = 1; i <= pane.Pages.Count; i++)
                    {
                        var page = pane.Pages[i];
                        object bits = page.EnhMetaFileBits;//Returns a Object that represents a picture 【Read-only】
                                                           //以下Try Catch區段為將圖片的背景填滿為白色，不然輸出的圖片背景會是透明的
                        try
                        {
                            using (var ms = new MemoryStream((byte[])bits))
                            {
                                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

                                using (var backGroundWritePNG = new Bitmap(image.Width, image.Height))
                                {
                                    //設定圖片的解析度
                                    backGroundWritePNG.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                                    using (Graphics graphis = Graphics.FromImage(backGroundWritePNG))
                                    {
                                        graphis.Clear(Color.White);
                                        graphis.DrawImageUnscaled(image, 0, 0);
                                    }
                                    string filePath = GetFlowChartPath();
                                    string outPutFilePath = Path.ChangeExtension(filePath + "Cha702_" + seq + "_" + i, "png");
                                    backGroundWritePNG.Save(outPutFilePath, ImageFormat.Png);//輸出圖片
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + ex.Message);
                            return null;
                        }
                    }
                }
            }
            //關閉Word，釋放資源
            doc1.Close(Type.Missing, Type.Missing, Type.Missing);
            msWordApp.Quit(Type.Missing, Type.Missing, Type.Missing);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(doc1);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(msWordApp);

            string folderName = "/FileUploads/TpCha702_" + seq + "_1.png";
            return Json(new
            {
                url = folderName
            });
        }


        public ActionResult Chapter702Show(int seq)
        {
            EnvirConsListTpService service = new EnvirConsListTpService();
            List<ECListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            string folderName = "/FileUploads/Tp";
            string uniqueFileName = items[0].FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(folderName, uniqueFileName);
                return Json(new
                {
                    url = fullPath
                });
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }


        //第七章 703 職業安全衛生抽查標準範本
        public JsonResult Chapter703(int pageIndex, int perPage)
        {
            List<OSHListTpEditModel> result = new List<OSHListTpEditModel>();
            OccuSafeHealthListTpService service = new OccuSafeHealthListTpService();
            int total = service.GetListCount();
            if (total > 0)
            {
                result = service.ListAll(pageIndex, total);
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

        public ActionResult Chapter703ShowWord(int seq)
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

            //轉圖檔
            var msWordApp = new Microsoft.Office.Interop.Word.Application();
            msWordApp.Visible = true;
            //開啟該Word檔，天啊！　真的開起Word，我真得是醉了
            Microsoft.Office.Interop.Word.Document doc1 = msWordApp.Documents.Open(outFile);
            //Opens the word document and fetch each page and converts to image
            foreach (Microsoft.Office.Interop.Word.Window window in doc1.Windows)
            {
                foreach (Microsoft.Office.Interop.Word.Pane pane in window.Panes)
                {
                    for (var i = 1; i <= pane.Pages.Count; i++)
                    {
                        var page = pane.Pages[i];
                        object bits = page.EnhMetaFileBits;//Returns a Object that represents a picture 【Read-only】
                                                           //以下Try Catch區段為將圖片的背景填滿為白色，不然輸出的圖片背景會是透明的
                        try
                        {
                            using (var ms = new MemoryStream((byte[])bits))
                            {
                                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

                                using (var backGroundWritePNG = new Bitmap(image.Width, image.Height))
                                {
                                    //設定圖片的解析度
                                    backGroundWritePNG.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                                    using (Graphics graphis = Graphics.FromImage(backGroundWritePNG))
                                    {
                                        graphis.Clear(Color.White);
                                        graphis.DrawImageUnscaled(image, 0, 0);
                                    }
                                    string filePath = GetFlowChartPath();
                                    string outPutFilePath = Path.ChangeExtension(filePath + "Cha703_" + seq + "_" + i, "png");
                                    backGroundWritePNG.Save(outPutFilePath, ImageFormat.Png);//輸出圖片
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + ex.Message);
                            return null;
                        }
                    }
                }
            }
            //關閉Word，釋放資源
            doc1.Close(Type.Missing, Type.Missing, Type.Missing);
            msWordApp.Quit(Type.Missing, Type.Missing, Type.Missing);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(doc1);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(msWordApp);

            string folderName = "/FileUploads/TpCha703_" + seq + "_1.png";
            return Json(new
            {
                url = folderName
            });
        }



        public ActionResult Chapter703Show(int seq)
        {
            OccuSafeHealthListTpService service = new OccuSafeHealthListTpService();
            List<OSHListTpEditModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            string folderName = "/FileUploads/Tp";
            string uniqueFileName = items[0].FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(folderName, uniqueFileName);
                return Json(new
                {
                    url = fullPath
                });
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }


        private string SaveFile(HttpPostedFileBase file, FlowChartFileModel m, string fileHeader)
        {
            try
            {
                string filePath = GetFlowChartPath();
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                //刪除已儲存原始檔案
                string uniqueFileName = m.FlowCharUniqueFileName;
                if (uniqueFileName != null && uniqueFileName.Length > 0)
                {
                    System.IO.File.Delete(Path.Combine(filePath, uniqueFileName));
                }

                string originFileName = file.FileName.ToString().Trim();
                int inx = originFileName.LastIndexOf(".");
                uniqueFileName = String.Format("{0}{1}{2}", fileHeader, m.Seq, originFileName.Substring(inx));
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

        private ActionResult DownloadFile(FlowChartFileModel m)
        {
            string filePath = GetFlowChartPath();
            //string filePath1 = "\\FileUploads/Tp/";
            string uniqueFileName = m.FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                
                   return File(iStream, "application/blob", m.FlowCharOriginFileName);
                   // return Json(new
                   // {
                   //     fullPath = fullPath
                   // },JsonRequestBehavior.AllowGet); 
                }
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
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