using EQC.Common;
using EQC.Models;
using EQC.Services;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class QCStdTpController : Controller
    {

        public ActionResult Index()
        {
            //ViewBag.Title = "抽驗標準管理維護";
            return View("QCStdBase");
        }

        //匯入 更新excel s20230415
        public JsonResult UploadStd(int mode, int id)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                string fileName;
                try
                {
                    fileName = SaveFile(file);
                }
                catch
                {
                    return Json(new
                    {
                        result = -1,
                        message = "上傳檔案儲存失敗"
                    });
                }

                List<QCStdModel> items = new List<QCStdModel>();
                int result = readExcelData(fileName, mode, items);
                if (result < 0)
                {
                    string msg;
                    switch (result)
                    {
                        case -1: msg = "Excel解析發生錯誤"; break;
                        default:
                            msg = "未知錯誤:" + result.ToString(); break;
                    }
                    return Json(new
                    {
                        result = -1,
                        message = msg
                    });
                }

                if (mode == 5)
                {
                    result = new QCStdBaseService().ImportChapter5StdList(id, items);
                }
                else if (mode == 6)
                {
                    result = new QCStdBaseService().ImportChapter6StdList(id, items);
                }
                else if (mode == 701)
                {
                    result = new QCStdBaseService().ImportChapter701StdList(id, items);
                }
                else if (mode == 702)
                {
                    result = new QCStdBaseService().ImportChapter702StdList(id, items);
                }
                else if (mode == 703)
                {
                    result = new QCStdBaseService().ImportChapter703StdList(id, items);
                }
                if (result == 0)
                {
                    return Json(new
                    {
                        result = 0,
                        message = "匯入完成"
                    });
                }
                else
                {
                    return Json(new
                    {
                        result = -1,
                        message = "匯入失敗"
                    });
                }

            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }
        //初始預定進度
        private int readExcelData(string filename, int mode, List<QCStdModel> items)
        {
            string fName = "";
            if (mode == 5)
                fName = "材料管理標準";
            else if (mode == 6)
                fName = "設備運轉測試標準";
            else if (mode == 701)
                fName = "施工抽查管理標準";
            else if (mode == 702)
                fName = "環境保育管理標準";
            else if (mode == 703)
                fName = "職業安全衛生管理標準";

            Microsoft.Office.Interop.Excel.Application appExcel = null;
            Workbook workbook = null;
            //開啟 Excel 檔案
            try
            {
                appExcel = new Microsoft.Office.Interop.Excel.Application();
                workbook = appExcel.Workbooks.Open(filename);

                Dictionary<string, Worksheet> dict = new Dictionary<string, Worksheet>();
                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    dict.Add(worksheet.Name, worksheet);
                }
                Worksheet sheet = dict[fName];

                int row = 2;

                while (!String.IsNullOrEmpty(sheet.Cells[row, 1].Formula.ToString()) && !String.IsNullOrEmpty(sheet.Cells[row, 2].Formula.ToString()))
                {
                    QCStdModel m = new QCStdModel();
                    if (mode == 5 || mode == 6)
                    {
                        m.ManageItem = sheet.Cells[row, 1].Formula.ToString();
                        m.Stand = sheet.Cells[row, 2].Formula.ToString();
                        m.CheckTiming = sheet.Cells[row, 3].Formula.ToString();
                        m.CheckMethod = sheet.Cells[row, 4].Formula.ToString();
                        m.CheckFeq = sheet.Cells[row, 5].Formula.ToString();
                        m.Incomp = sheet.Cells[row, 6].Formula.ToString();
                        m.ManageRec = sheet.Cells[row, 7].Formula.ToString();
                        m.Memo = sheet.Cells[row, 8].Formula.ToString();
                    }
                    else
                    {
                        if (mode == 703)
                        {
                            m.ManageItem = sheet.Cells[row, 1].Formula.ToString();
                            m.Stand = sheet.Cells[row, 2].Formula.ToString();
                            m.CheckTiming = sheet.Cells[row, 3].Formula.ToString();
                            m.CheckMethod = sheet.Cells[row, 4].Formula.ToString();
                            m.CheckFeq = sheet.Cells[row, 5].Formula.ToString();
                            m.Incomp = sheet.Cells[row, 6].Formula.ToString();
                            m.ManageRec = sheet.Cells[row, 7].Formula.ToString();
                        }
                        else
                        {
                            string flowText = sheet.Cells[row, 1].Formula.ToString();
                            if (flowText == "施工中") m.FlowType = 2;
                            else if (flowText == "施工後") m.FlowType = 3;
                            else m.FlowType = 1;
                            if (mode == 701)
                            {
                                m.Flow = sheet.Cells[row, 2].Formula.ToString();
                                m.ManageItem = sheet.Cells[row, 3].Formula.ToString();
                                m.Stand = sheet.Cells[row, 4].Formula.ToString();
                                m.CheckTiming = sheet.Cells[row, 5].Formula.ToString();
                                m.CheckMethod = sheet.Cells[row, 6].Formula.ToString();
                                m.CheckFeq = sheet.Cells[row, 7].Formula.ToString();
                                m.Incomp = sheet.Cells[row, 8].Formula.ToString();
                                m.ManageRec = sheet.Cells[row, 9].Formula.ToString();
                            }
                            else if (mode == 702)
                            {
                                m.ManageItem = sheet.Cells[row, 2].Formula.ToString();
                                m.Stand = sheet.Cells[row, 3].Formula.ToString();
                                m.CheckTiming = sheet.Cells[row, 4].Formula.ToString();
                                m.CheckMethod = sheet.Cells[row, 5].Formula.ToString();
                                m.CheckFeq = sheet.Cells[row, 6].Formula.ToString();
                                m.Incomp = sheet.Cells[row, 7].Formula.ToString();
                                m.ManageRec = sheet.Cells[row, 8].Formula.ToString();
                            }
                        }
                    }
                    items.Add(m);
                    row++;
                }

                workbook.Close();
                appExcel.Quit();

                return 0;
            }
            catch (Exception e)
            {
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                return -1;
            }
        }
        private string SaveFile(HttpPostedFileBase file)
        {
            string filePath = Path.GetTempPath();
            string originFileName = file.FileName.ToString().Trim();
            int inx = originFileName.LastIndexOf(".");
            string uniqueFileName = String.Format("{0}{1}", Guid.NewGuid(), originFileName.Substring(inx));
            string fullPath = Path.Combine(filePath, uniqueFileName);
            file.SaveAs(fullPath);

            return fullPath;
        }

        //下載標準 s20230414
        public ActionResult DnStdDoc(int mode, int id)
        {
            try
            {
                List<QCStdModel> stdItems = null;
                if (mode == 5)
                {
                    stdItems = new QCStdBaseService().GetChapter5StdList<QCStdModel>(id);
                }
                else if (mode == 6)
                {
                    stdItems = new QCStdBaseService().GetChapter6StdList<QCStdModel>(id);
                }
                else if (mode == 701)
                {
                    stdItems = new QCStdBaseService().GetChapter701StdList<QCStdModel>(id);
                }
                else if (mode == 702)
                {
                    stdItems = new QCStdBaseService().GetChapter702StdList<QCStdModel>(id);
                }
                else if (mode == 703)
                {
                    stdItems = new QCStdBaseService().GetChapter703StdList<QCStdModel>(id);
                }
                else
                {
                    return Json(new
                    {
                        result = -1,
                        message = "請求錯誤"
                    }, JsonRequestBehavior.AllowGet);
                }
                return CreateExcelDoc(stdItems, mode);
            }
            catch
            {
                return Json(new
                {
                    result = -1,
                    message = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        private ActionResult CreateExcelDoc(List<QCStdModel> stdItems, int mode)
        {
            string fName = "";
            if (mode == 5)
                fName = "材料管理標準";
            else if (mode == 6)
                fName = "設備運轉測試標準";
            else if (mode == 701)
                fName = "施工抽查管理標準";
            else if (mode == 702)
                fName = "環境保育管理標準";
            else if (mode == 703)
                fName = "職業安全衛生管理標準";

            string filename = Utils.CopyTemplateFile("抽驗標準管理維護-" + fName + ".xlsx", ".xlsx");
            Dictionary<string, Worksheet> dict = new Dictionary<string, Worksheet>();
            Microsoft.Office.Interop.Excel.Application appExcel = null;
            Workbook workbook = null;
            //開啟 Excel 檔案
            try
            {
                appExcel = new Microsoft.Office.Interop.Excel.Application();
                workbook = appExcel.Workbooks.Open(filename);
                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    dict.Add(worksheet.Name, worksheet);
                }
                Worksheet sheet = dict[fName];

                if (mode == 701)
                {
                    int cnt = 1;
                    while (cnt < stdItems.Count)
                    {
                        sheet.Rows[3].Insert();
                        cnt++;
                    }
                }

                //string s = sheet.Cells[5, 1].Formula.ToString();

                int row = 2;
                foreach (QCStdModel m in stdItems)
                {
                    if (mode == 5 || mode == 6)
                    {
                        sheet.Cells[row, 1] = m.ManageItem;
                        sheet.Cells[row, 2] = m.Stand;
                        sheet.Cells[row, 3] = m.CheckTiming;
                        sheet.Cells[row, 4] = m.CheckMethod;
                        sheet.Cells[row, 5] = m.CheckFeq;
                        sheet.Cells[row, 6] = m.Incomp;
                        sheet.Cells[row, 7] = m.ManageRec;
                        sheet.Cells[row, 8] = m.Memo;
                    }
                    else
                    {
                        string flowText = "";
                        if (m.FlowType == 1) flowText = "施工前";
                        else if (m.FlowType == 2) flowText = "施工中";
                        else if (m.FlowType == 3) flowText = "施工後";
                        if (mode == 701)
                        {
                            sheet.Cells[row, 1] = flowText;
                            sheet.Cells[row, 2] = m.Flow;
                            sheet.Cells[row, 3] = m.ManageItem;
                            sheet.Cells[row, 4] = m.Stand;
                            sheet.Cells[row, 5] = m.CheckTiming;
                            sheet.Cells[row, 6] = m.CheckMethod;
                            sheet.Cells[row, 7] = m.CheckFeq;
                            sheet.Cells[row, 8] = m.Incomp;
                            sheet.Cells[row, 9] = m.ManageRec;
                        }
                        else if (mode == 702)
                        {
                            sheet.Cells[row, 1] = flowText;
                            sheet.Cells[row, 2] = m.ManageItem;
                            sheet.Cells[row, 3] = m.Stand;
                            sheet.Cells[row, 4] = m.CheckTiming;
                            sheet.Cells[row, 5] = m.CheckMethod;
                            sheet.Cells[row, 6] = m.CheckFeq;
                            sheet.Cells[row, 7] = m.Incomp;
                            sheet.Cells[row, 8] = m.ManageRec;
                        }
                        else if (mode == 703)
                        {
                            sheet.Cells[row, 1] = m.ManageItem;
                            sheet.Cells[row, 2] = m.Stand;
                            sheet.Cells[row, 3] = m.CheckTiming;
                            sheet.Cells[row, 4] = m.CheckMethod;
                            sheet.Cells[row, 5] = m.CheckFeq;
                            sheet.Cells[row, 6] = m.Incomp;
                            sheet.Cells[row, 7] = m.ManageRec;
                        }
                    }
                    row++;
                }

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", "抽驗標準管理維護-" + fName + ".xlsx");
            }
            catch (Exception e)
            {
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                return Json(new
                {
                    result = -1,
                    message = "Excel 製表失敗"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //第五章 材料設備清冊範本
        public JsonResult EMDListTp()
        {
            List<EMDListTpModel> result = new List<EMDListTpModel>();
            QCStdBaseService service = new QCStdBaseService();
            result = service.GetEngMaterialDeviceListTpList<EMDListTpModel>();
            return Json(result);
        }
        //
        public JsonResult Chapter5(string op1, int pageIndex, int perPage)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + op1);
            List<EMDCTpModel> result = new List<EMDCTpModel>();
            QCStdBaseService service = new QCStdBaseService();
            int total = service.GetEngMaterialDeviceControlTpCount(op1);
            if (total > 0)
            {
                result = service.GetEngMaterialDeviceControlTpByEngMaterialDeviceListTpSeq<EMDCTpModel>(op1, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }

        public JsonResult Chapter5_ImportToSt(string code = "", int seq = 0)
        {
            QCStdBaseService service = new QCStdBaseService();
            int total;
            List<EMDCTpModel> result = new List<EMDCTpModel>();
            total = service.GetEngMaterialDeviceControlTpCount("0", code);
            if (total > 0)
            {
                result = service.GetEngMaterialDeviceControlTpByEngMaterialDeviceListTpSeq<EMDCTpModel>("0", 1, total, code);
            }

            EngMaterialDeviceControlStService ControlStSerivce = new EngMaterialDeviceControlStService();
            foreach(var item in result)
            {
                ControlStSerivce.Insert(seq, item);
            }

            if (result.Count == 0)
            {//s20230415
                return Json(new
                {
                    result = -1,
                    msg = "查無資料"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    msg = "複製完成"
                });
            }
        }
        public JsonResult Chapter6_ImportToSt(string code = "", int seq = 0)
        {
            QCStdBaseService service = new QCStdBaseService();
            int total;
            List<EOCTpModel> result = new List<EOCTpModel>();
            total = service.GetEquOperControlTpCount("0", code);
            if (total > 0)
            {
                result = service.GetEquOperControlTpByEquOperTestTpSeq<EOCTpModel>("0", 1, total, code);
            }
            EquOperControlStService ControlStSerivce = new EquOperControlStService();
            foreach (var item in result)
            {
                ControlStSerivce.Insert(seq, item);
            }
            if (result.Count == 0)
            {//s20230415
                return Json(new
                {
                    result = -1,
                    msg = "查無資料"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    msg = "複製完成"
                });
            }
        }  
        public JsonResult Chapter701_ImportToSt(string code = "", int seq = 0)
        {
            QCStdBaseService service = new QCStdBaseService();
            int total;
            List<CCCTpModel> result = new List<CCCTpModel>();
            total = service.GetConstCheckControlTpCount("0", code);
            if (total > 0)
            {
                result = service.GetConstCheckControlTpByConstCheckListTpSeq<CCCTpModel>("0", 1, total, code);
            }
            ConstCheckControlStService ControlStSerivce = new ConstCheckControlStService();
            foreach (var item in result)
            {
                ControlStSerivce.Insert(seq, item);
            }

            if (result.Count == 0)
            {//s20230415
                return Json(new
                {
                    result = -1,
                    msg = "查無資料"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    msg = "複製完成"
                });
            }
        }
        public JsonResult Chapter702_ImportToSt(string code = "", int seq = 0)
        {
            QCStdBaseService service = new QCStdBaseService();
            int total;
            List<ECCTpModel> result = new List<ECCTpModel>();
            total = service.GetEnvirConsControlTpCount("0", code);
            if (total > 0)
            {
                result = service.GetEnvirConsControlTpByEnvirConsListSeq<ECCTpModel>("0", 1, total, code);
            }
            EnvirConsControlStService ControlStSerivce = new EnvirConsControlStService();
            foreach (var item in result)
            {
                ControlStSerivce.Insert(seq, item);
            }

            if (result.Count == 0)
            {//s20230415
                return Json(new
                {
                    result = -1,
                    msg = "查無資料"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    msg = "複製完成"
                });
            }
        }
        public JsonResult Chapter703_ImportToSt(string code = "", int seq = 0)
        {
            QCStdBaseService service = new QCStdBaseService();
            int total;
            List<OSHCTpModel> result = new List<OSHCTpModel>();
            total = service.GetOccuSafeHealthControlTpCount("0", code);
            if (total > 0)
            {
                result = service.GetOccuSafeHealthControlTpByOccuSafeHealthListSeq<OSHCTpModel> ("0", 1, total, code);
            }
            OccuSafeHealthControlStService ControlStSerivce = new OccuSafeHealthControlStService();
            foreach (var item in result)
            {
                ControlStSerivce.Insert(seq, item);
            }

            if (result.Count == 0)
            {//s20230415
                return Json(new
                {
                    result = -1,
                    msg = "查無資料"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    msg = "複製完成"
                });
            }
        }

        public JsonResult Chapter5_AllList(string op1 = "0", string code ="", int pageIndex = 1,int perPage=0)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + op1);
            List<EMDCTpModel> result = new List<EMDCTpModel>();
            QCStdBaseService service = new QCStdBaseService();
            int total = service.GetEngMaterialDeviceControlTpCount(op1, code);
            if (total > 0)
            {
                result = service.GetEngMaterialDeviceControlTpByEngMaterialDeviceListTpSeq<EMDCTpModel>(op1, pageIndex, total, code);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        //
        public JsonResult Chapter5Save(EMDCTpModel item)
        {
            
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);
            EngMaterialDeviceControlTpService service = new EngMaterialDeviceControlTpService();
            if (service.Update(item) == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成"
                });
                    
            }
        }
        //
        public JsonResult Chapter5Add(EMDCTpModel item)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);
            EngMaterialDeviceControlTpService service = new EngMaterialDeviceControlTpService();
            int newSeq = service.Add(item);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                    Seq = newSeq
                });
            }
        }

        public JsonResult Chapter5Del(int seq)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);
            EngMaterialDeviceControlTpService service = new EngMaterialDeviceControlTpService();
            int newSeq = service.Delete(seq);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料刪除失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "資料刪除完成",
                    Seq = newSeq
                });
            }
        }

        //6 設備運轉測試清單範本
        public JsonResult EOTListTp()
        {
            List<EOTListTpModel> result = new List<EOTListTpModel>();
            QCStdBaseService service = new QCStdBaseService();
            result = service.GetEquOperTestListTpList<EOTListTpModel>();
            return Json(result);
        }
        //第六章 設備功能運轉測試抽驗程序及標準
        public JsonResult Chapter6(string op1, int pageIndex, int perPage)
        {
            List<EOCTpModel> result = new List<EOCTpModel>();
            QCStdBaseService service = new QCStdBaseService();
            int total = service.GetEquOperControlTpCount(op1);
            if (total > 0)
            {
                result = service.GetEquOperControlTpByEquOperTestTpSeq<EOCTpModel>(op1, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }

        public JsonResult Chapter6_AllList(string op1, int pageIndex, int perPage)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + op1);
            List<EOCTpModel> result = new List<EOCTpModel>();
            QCStdBaseService service = new QCStdBaseService();
            int total = service.GetEquOperControlTpCount(op1);
            if (total > 0)
            {
                result = service.GetEquOperControlTpByEquOperTestTpSeq<EOCTpModel>(op1, pageIndex, total);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }

        //
        public JsonResult Chapter6Save(EOCTpModel item)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);
            EquOperControlTpService service = new EquOperControlTpService();
            if (service.Update(item) == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成"
                });

            }
        }
        //
        public JsonResult Chapter6Add(EOCTpModel item)
        {
            EquOperControlTpService service = new EquOperControlTpService();
            int newSeq = service.Add(item);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                    Seq = newSeq
                });
            }
        }

        public JsonResult Chapter6Del(int seq)
        {
            EquOperControlTpService service = new EquOperControlTpService();
            int newSeq = service.Delete(seq);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料刪除失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "資料刪除完成",
                    Seq = newSeq
                });
            }
        }

        //701 施工抽查清單範本
        public JsonResult CCListTp()
        {
            List<CCListTpModel> result = new List<CCListTpModel>();
            QCStdBaseService service = new QCStdBaseService();
            result = service.GetConstCheckListTpList<CCListTpModel>();
            return Json(result);
        }

        //第七章 701 施工抽查程序及標準
        public JsonResult Chapter701(string op1, int pageIndex, int perPage)
        {
            List<CCCTpModel> result = new List<CCCTpModel>();
            QCStdBaseService service = new QCStdBaseService();
            int total = service.GetConstCheckControlTpCount(op1);
            if (total > 0)
            {
                result = service.GetConstCheckControlTpByConstCheckListTpSeq<CCCTpModel>(op1, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }

        public JsonResult Chapter701_AllList(string op1, int pageIndex, int perPage)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + op1);
            List<CCCTpModel> result = new List<CCCTpModel>();
            QCStdBaseService service = new QCStdBaseService();
            int total = service.GetConstCheckControlTpCount(op1);
            if (total > 0)
            {
                result = service.GetConstCheckControlTpByConstCheckListTpSeq<CCCTpModel>(op1, pageIndex, total);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }

        //
        public JsonResult Chapter701Save(CCCTpModel item)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);
            ConstCheckControlTpService service = new ConstCheckControlTpService();
            if (service.Update(item) == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成"
                });

            }
        }
        //
        public JsonResult Chapter701Add(CCCTpModel item)
        {
            ConstCheckControlTpService service = new ConstCheckControlTpService();
            int newSeq = service.Add(item);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                    Seq = newSeq
                });
            }
        }

        public JsonResult Chapter701Del(int seq)
        {
            ConstCheckControlTpService service = new ConstCheckControlTpService();
            int newSeq = service.Delete(seq);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料刪除失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "資料刪除完成",
                    Seq = newSeq
                });
            }
        }

        //702 環境保育清單範本
        public JsonResult ECListTp()
        {
            List<ECListTpModel> result = new List<ECListTpModel>();
            QCStdBaseService service = new QCStdBaseService();
            result = service.GetEnvirConsListTpList<ECListTpModel>();
            return Json(result);
        }

        //第七章 702 環境保育抽查標準範本
        public JsonResult Chapter702(string op1, int pageIndex, int perPage)
        {
            List<ECCTpModel> result = new List<ECCTpModel>();
            QCStdBaseService service = new QCStdBaseService();
            int total = service.GetEnvirConsControlTpCount(op1);
            if (total > 0)
            {
                result = service.GetEnvirConsControlTpByEnvirConsListSeq<ECCTpModel>(op1, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        //
        public JsonResult Chapter702_AllList(string op1, int pageIndex, int perPage)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + op1);
            List<ECCTpModel> result = new List<ECCTpModel>();
            QCStdBaseService service = new QCStdBaseService();
            int total = service.GetEnvirConsControlTpCount(op1);
            if (total > 0)
            {
                result = service.GetEnvirConsControlTpByEnvirConsListSeq<ECCTpModel>(op1, pageIndex, total);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }


        public JsonResult Chapter702Save(ECCTpModel item)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);
            EnvirConsControlTpService service = new EnvirConsControlTpService();
            if (service.Update(item) == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成"
                });

            }
        }
        //
        public JsonResult Chapter702Add(ECCTpModel item)
        {
            EnvirConsControlTpService service = new EnvirConsControlTpService();
            int newSeq = service.Add(item);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                    Seq = newSeq
                });
            }
        }

        public JsonResult Chapter702Del(int seq)
        {
            EnvirConsControlTpService service = new EnvirConsControlTpService();
            int newSeq = service.Delete(seq);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料刪除失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "資料刪除完成",
                    Seq = newSeq
                });
            }
        }

        //703 職業安全衛生清單範本
        public JsonResult OSHListTp()
        {
            List<ECListTpModel> result = new List<ECListTpModel>();
            QCStdBaseService service = new QCStdBaseService();
            result = service.GetOccuSafeHealthListTpList<ECListTpModel>();
            return Json(result);
        }
        //第七章 703 職業安全衛生抽查標準範本
        public JsonResult Chapter703(string op1, int pageIndex, int perPage)
        {
            List<OSHCTpModel> result = new List<OSHCTpModel>();
            QCStdBaseService service = new QCStdBaseService();
            int total = service.GetOccuSafeHealthControlTpCount(op1);
            if (total > 0)
            {
                result = service.GetOccuSafeHealthControlTpByOccuSafeHealthListSeq<OSHCTpModel>(op1, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        //

        public JsonResult Chapter703_AllList(string op1, int pageIndex, int perPage)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + op1);
            List<OSHCTpModel> result = new List<OSHCTpModel>();
            QCStdBaseService service = new QCStdBaseService();
            int total = service.GetOccuSafeHealthControlTpCount(op1);
            if (total > 0)
            {
                result = service.GetOccuSafeHealthControlTpByOccuSafeHealthListSeq<OSHCTpModel>(op1, pageIndex, total);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }

        public JsonResult Chapter703Save(OSHCTpModel item)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);
            OccuSafeHealthControlTpService service = new OccuSafeHealthControlTpService();
            if (service.Update(item) == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成"
                });

            }
        }
        //
        public JsonResult Chapter703Add(OSHCTpModel item)
        {
            OccuSafeHealthControlTpService service = new OccuSafeHealthControlTpService();
            int newSeq = service.Add(item);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                    Seq = newSeq
                });
            }
        }

        public JsonResult Chapter703Del(int seq)
        {
            OccuSafeHealthControlTpService service = new OccuSafeHealthControlTpService();
            int newSeq = service.Delete(seq);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料刪除失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "資料刪除完成",
                    Seq = newSeq
                });
            }
        }


    }
}