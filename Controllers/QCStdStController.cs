using EQC.Common;
using EQC.Models;
using EQC.Services;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class QCStdStController : Controller
    {//抽驗標準管理
        //上傳標準
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
                    result = new EngMaterialDeviceControlStService().ImportStdList(id, items);
                }
                else if (mode == 6)
                {
                    result = new EquOperControlStService().ImportStdList(id, items);
                }
                else if (mode == 701)
                {
                    result = new ConstCheckControlStService().ImportStdList(id, items);
                }
                else if (mode == 702)
                {
                    result = new EnvirConsControlStService().ImportStdList(id, items);
                }
                else if (mode == 703)
                {
                    result = new OccuSafeHealthControlStService().ImportStdList(id, items);
                }
                if (result == 0)
                {
                    return Json(new
                    {
                        result = 0,
                        message = "匯入完成"
                    });
                } else
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
                        } else { 
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
                    stdItems = new EngMaterialDeviceControlStService().GetStdList<QCStdModel>(id);
                }
                else if (mode == 6)
                {
                    stdItems = new EquOperControlStService().GetStdList<QCStdModel>(id);
                }
                else if (mode == 701)
                {
                    stdItems = new ConstCheckControlStService().GetStdList<QCStdModel>(id);
                } 
                else if (mode == 702)
                {
                    stdItems = new EnvirConsControlStService().GetStdList<QCStdModel>(id);
                }
                else if(mode == 703)
                {
                    stdItems = new OccuSafeHealthControlStService().GetStdList<QCStdModel>(id);
                } else
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
            if(mode == 5)
                fName = "材料管理標準"; 
            else if (mode == 6)
                fName = "設備運轉測試標準"; 
            else if (mode == 701)
                fName = "施工抽查管理標準";
            else if (mode == 702)
                fName = "環境保育管理標準";
            else if (mode == 703)
                fName = "職業安全衛生管理標準";

            string filename = Utils.CopyTemplateFile("監造計畫產製-"+fName+ ".xlsx", ".xlsx");
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
                        if(m.FlowType == 1) flowText = "施工前";
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
                        else if(mode == 702)
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
                return File(iStream, "application/blob", "監造計畫產製-" + fName + ".xlsx");
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
        public JsonResult Chapter5(int op1, int pageIndex, int perPage)
        {
            List<EngMaterialDeviceControlStModel> result = new List<EngMaterialDeviceControlStModel>();
            EngMaterialDeviceControlStService service = new EngMaterialDeviceControlStService();
            int total = service.GetEngMaterialDeviceControlStCount(op1);
            if (total > 0)
            {
                result = service.GetList<EngMaterialDeviceControlStModel>(op1, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        //使用者新增
        public JsonResult Chapter5NewItem(int op1)
        {
            EngMaterialDeviceControlStService service = new EngMaterialDeviceControlStService();
            int newSeq = service.NewAdd(op1);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "新增失敗"
                });
            }
            else
            {
                List<EngMaterialDeviceControlStModel> items = service.GetItem<EngMaterialDeviceControlStModel>(newSeq);
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                    item = Json(items[0])
                });
            }
        }
        //
        public JsonResult Chapter5Save(List<EngMaterialDeviceControlStModel> items)
        {
            EngMaterialDeviceControlStService service = new EngMaterialDeviceControlStService();
            if (service.Updates(items))
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成"
                });

            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "資料不可變更"
                });
            }
        }

        public JsonResult Chapter5Del(int seq)
        {
            EngMaterialDeviceControlStService service = new EngMaterialDeviceControlStService();
            int newSeq = service.Delete(seq);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料刪除失敗，或資料不可刪除"
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

        //第六章 設備功能運轉測試抽驗程序及標準
        public JsonResult Chapter6(int op1, int pageIndex, int perPage)
        {
            List<EquOperControlStModel> result = new List<EquOperControlStModel>();
            EquOperControlStService service = new EquOperControlStService();
            int total = service.GetEquOperControlStCount(op1);
            if (total > 0)
            {
                result = service.GetList<EquOperControlStModel>(op1, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        //使用者新增
        public JsonResult Chapter6NewItem(int op1)
        {
            EquOperControlStService service = new EquOperControlStService();
            int newSeq = service.NewAdd(op1);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "新增失敗"
                });
            }
            else
            {
                List<EquOperControlStModel> items = service.GetItem<EquOperControlStModel>(newSeq);
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                    item = Json(items[0])
                });
            }
        }
        //
        public JsonResult Chapter6Save(List<EquOperControlStModel> items)
        {
            EquOperControlStService service = new EquOperControlStService();
            if (service.Updates(items))
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成"
                });

            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
        }

        public JsonResult Chapter6Del(int seq)
        {
            EquOperControlStService service = new EquOperControlStService();
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

        //第七章 701 施工抽查程序及標準
        public JsonResult Chapter701(int op1, int pageIndex, int perPage)
        {
            List<ConstCheckControlStModel> result = new List<ConstCheckControlStModel>();
            ConstCheckControlStService service = new ConstCheckControlStService();
            int total = service.GetConstCheckControlStCount(op1);
            if (total > 0)
            {
                result = service.GetList<ConstCheckControlStModel>(op1, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        //使用者新增
        public JsonResult Chapter701NewItem(int op1)
        {
            ConstCheckControlStService service = new ConstCheckControlStService();
            int newSeq = service.NewAdd(op1);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "新增失敗"
                });
            }
            else
            {
                List<ConstCheckControlStModel> items = service.GetItem<ConstCheckControlStModel>(newSeq);
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                    item = Json(items[0])
                });
            }
        }
        //
        public JsonResult Chapter701Save(List<ConstCheckControlStModel> items)
        {
            ConstCheckControlStService service = new ConstCheckControlStService();
            if (service.Updates(items))
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成"
                });

            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
        }

        public JsonResult Chapter701Del(int seq)
        {
            ConstCheckControlStService service = new ConstCheckControlStService();
            int newSeq = service.Delete(seq);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料刪除失敗，或資料不可刪除"
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

        //第七章 702 環境保育抽查標準範本
        public JsonResult Chapter702(int op1, int pageIndex, int perPage)
        {
            List<EnvirConsControlStModel> result = new List<EnvirConsControlStModel>();
            EnvirConsControlStService service = new EnvirConsControlStService();
            int total = service.GetEnvirConsControlStCount(op1);
            if (total > 0)
            {
                result = service.GetList<EnvirConsControlStModel>(op1, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        //使用者新增
        public JsonResult Chapter702NewItem(int op1)
        {
            EnvirConsControlStService service = new EnvirConsControlStService();
            int newSeq = service.NewAdd(op1);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "新增失敗"
                });
            }
            else
            {
                List<EnvirConsControlStModel> items = service.GetItem<EnvirConsControlStModel>(newSeq);
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                    item = Json(items[0])
                });
            }
        }
        //
        public JsonResult Chapter702Save(List<EnvirConsControlStModel> items)
        {
            EnvirConsControlStService service = new EnvirConsControlStService();
            if (service.Updates(items))
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成"
                });

            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
        }

        public JsonResult Chapter702Del(int seq)
        {
            EnvirConsControlStService service = new EnvirConsControlStService();
            int newSeq = service.Delete(seq);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料刪除失敗，或資料不可刪除"
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

        //第七章 703 職業安全衛生抽查標準範本
        public JsonResult Chapter703(int op1, int pageIndex, int perPage)
        {
            List<OccuSafeHealthControlStModel> result = new List<OccuSafeHealthControlStModel>();
            OccuSafeHealthControlStService service = new OccuSafeHealthControlStService();
            int total = service.GetOccuSafeHealthControlStCount(op1);
            if (total > 0)
            {
                result = service.GetList<OccuSafeHealthControlStModel>(op1, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        //使用者新增
        public JsonResult Chapter703NewItem(int op1)
        {
            OccuSafeHealthControlStService service = new OccuSafeHealthControlStService();
            int newSeq = service.NewAdd(op1);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "新增失敗"
                });
            }
            else
            {
                List<OccuSafeHealthControlStModel> items = service.GetItem<OccuSafeHealthControlStModel>(newSeq);
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                    item = Json(items[0])
                });
            }
        }
        //
        public JsonResult Chapter703Save(List<OccuSafeHealthControlStModel> items)
        {
            OccuSafeHealthControlStService service = new OccuSafeHealthControlStService();
            if (service.Updates(items))
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成"
                });
                
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
        }

        public JsonResult Chapter703Del(int seq)
        {
            OccuSafeHealthControlStService service = new OccuSafeHealthControlStService();
            int newSeq = service.Delete(seq);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料刪除失敗，或資料不可刪除"
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
        /*/
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

        
        */

    }
}