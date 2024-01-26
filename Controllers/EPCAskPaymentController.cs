using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EPCAskPaymentController : Controller
    {//進度管理 - 估驗請款
        AskPaymentService iService = new AskPaymentService();

        //日期清單
        public JsonResult GetDateList(int id)
        {
            List<EPCAskPaymentHeaderVModel> dList = iService.GetDateList<EPCAskPaymentHeaderVModel>(id);
            return Json(new
            {
                result = 0,
                items = dList
            });
        }
        public JsonResult AddAskPayment(int id, DateTime tarDate)
        {
            List<EPCProgressEngChangeListVModel> engChanges = new SchEngChangeService().GetEngChangeList<EPCProgressEngChangeListVModel>(id);
            if (engChanges.Count > 0)
            {//s20230412
                EPCProgressEngChangeListVModel engChange = engChanges[0];
                if (tarDate.Subtract(engChange.StartDate.Value).Days >= 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = String.Format("日期{0}(含)之後的請款, 請至 進度管理-工程變更 頁面作業", engChange.StartDate.Value.ToString("yyyy-M-d"))
                    });
                }
            }

            List<EPCSchProgressHeaderVModel> scItems = new SchProgressPayItemService().GetHeaderList<EPCSchProgressHeaderVModel>(id);
            if (scItems.Count == 0 || scItems[0].SPState != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "預訂定度作業未完成, 無法作業"
                });
            }

            List<EPCAskPaymentEngVModel> tenders = iService.GetEngItem<EPCAskPaymentEngVModel>(id);
            if (tenders.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            
            List<EPCAskPaymentHeaderVModel> dates = iService.CheckDate<EPCAskPaymentHeaderVModel>(id, tarDate);
            if (dates.Count > 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "請款日期錯誤, 不得小於已請款日期"
                });
            }
            List<SupDailyDateModel> dailys = iService.CheckDailyDate<SupDailyDateModel>(id, tarDate);
            if (dailys.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "請款日期, 無符合的監造日誌"
                });
            }
            SupDailyDateModel daily = dailys[0];
            if (iService.AddItems(id, tarDate, daily))
            {
                return Json(new
                {
                    result = 0,
                    msg = "新增完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "新增失敗"
            });
        }
        //該日期項目清單
        public JsonResult GetList(int id)
        {
            List<AskPaymentPayItemModel> ceList = iService.GetList<AskPaymentPayItemModel>(id);
            return Json(new
            {
                result = 0,
                items = ceList
            });
        }
        //刪除請款單
        public JsonResult DelAPItem(int id)
        {
            if (iService.Del(id))
            {
                return Json(new
                {
                    result = 0,
                    msg = "刪除成功"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "刪除失敗"
            });
        }
        //更新
        public JsonResult APSave(List<AskPaymentPayItemModel> items)
        {
            if (iService.UpdateAccu(items))
            {
                return Json(new
                {
                    result = 0,
                    msg = "儲存完成"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "儲存失敗"
                });
            }
        }
        //請款完成
        public JsonResult APCompleted(int id)
        {
            if (iService.APCompleted(id, 1) > 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "請款已完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "請款已完成 變更失敗"
            });
        }

        //下載估驗請款單
        public ActionResult DownloadInvoice(int id)
        {
            List<EPCAskPaymentHeaderVModel> hList = iService.GetAskPaymentHeader<EPCAskPaymentHeaderVModel>(id);
            if (hList.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                });
            }
            EPCAskPaymentHeaderVModel apHeader = hList[0];
            List<EPCAskPaymentPayItemVModel> apPayItemList = iService.GetAskPaymentList<EPCAskPaymentPayItemVModel>(apHeader.Seq);
            //前期
            EPCAskPaymentHeaderVModel apPreviousHeader = new EPCAskPaymentHeaderVModel() { CurrentAccuAmount = 0, Period = 0 };
            List<EPCAskPaymentPayItemVModel> apPreviousPayItemList = new List<EPCAskPaymentPayItemVModel>();
            if (apHeader.Period>1)
            {
                hList = iService.GetAskPaymentHeader<EPCAskPaymentHeaderVModel>(apHeader.EngMainSeq, apHeader.Period-1);
                if (hList.Count == 1)
                {
                    apPreviousHeader = hList[0];
                    apPreviousPayItemList = iService.GetAskPaymentList<EPCAskPaymentPayItemVModel>(apPreviousHeader.Seq);
                }
            }
            if(apPreviousPayItemList.Count >0 && apPreviousPayItemList.Count != apPayItemList.Count) { 
                return Json(new
                {
                    result = -1,
                    msg = "前期資料錯誤"
                });
            }

            List<EPCAskPaymentPayItemListVModel> apPayItems = new List<EPCAskPaymentPayItemListVModel>();
            EPCAskPaymentPayItemListVModel item = null;
            string mKey = "";
            for (int i=0; i< apPayItemList.Count;i++)
            {
                EPCAskPaymentPayItemVModel m = apPayItemList[i];
                if (m.level==1)
                {
                    item = new EPCAskPaymentPayItemListVModel();
                    if (apPreviousPayItemList.Count > 0)
                    {
                        m.PreviousSchProgress = apPreviousPayItemList[i].SchProgress;
                        m.PreviousAccuQuantity = 1;
                        m.PreviousAccuAmount = apPreviousPayItemList[i].subTotalAmount;
                    }
                    m.AccuAmount = m.subTotalAmount;
                    item.mainItem = m;
                    apPayItems.Add(item);
                } else if(item != null)
                {
                    if (apPreviousPayItemList.Count > 0)
                    {
                        m.PreviousSchProgress = apPreviousPayItemList[i].SchProgress;
                        m.PreviousAccuQuantity = apPreviousPayItemList[i].AccuQuantity;
                        m.PreviousAccuAmount = apPreviousPayItemList[i].AccuAmount;
                    }

                    item.subItems.Add(m);
                }
            }

            List<EPCAskPaymentEngVModel> items = iService.GetEngItem<EPCAskPaymentEngVModel>(apHeader.EngMainSeq);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            EPCAskPaymentEngVModel eng = items[0];

            List<AskPaymentPayItemModel> apList = iService.GetList<AskPaymentPayItemModel>(id);
            if (apList.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無資料"
                });
            }
            return CreateInvoiceExcel(eng, apHeader, apPreviousHeader, apList, apPayItems);
        }
        private ActionResult CreateInvoiceExcel(EPCAskPaymentEngVModel eng, EPCAskPaymentHeaderVModel apHeader, EPCAskPaymentHeaderVModel apPreviousHeader, List<AskPaymentPayItemModel> miscList, List<EPCAskPaymentPayItemListVModel> apPayItems)
        {
            string filename = Utils.CopyTemplateFile("進度管理-估驗請款單.xlsx", ".xlsx");
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
                
                PayitemInvoiceHeaderSheet(dict["請(總表)"], eng, apHeader, apPreviousHeader);

                int totalPages = 0; //apPayItems.Count
                foreach (EPCAskPaymentPayItemListVModel m in apPayItems)
                {//僅有細項
                    if (m.subItems.Count > 0) totalPages++;
                }

                Worksheet sheet0 = dict["估0"];
                sheet0.Cells[4, 10] = String.Format("第1頁共{0}頁", totalPages+1);
                sheet0.Cells[5, 2] = String.Format("施工地點：{0}", eng.Location);
                Worksheet sheetLast = dict["工作表1"];
                
                int pageIndex = 0;
                for (int i = 0; i < apPayItems.Count; i++)
                {
                    if (apPayItems[i].subItems.Count > 0) {
                        Worksheet sheetSub = workbook.Worksheets.Add(Before: sheetLast);
                        sheetSub.Name = String.Format("估{0}", i + 1);
                        dict.Add(sheetSub.Name, sheetSub);
                        sheet0.Columns.Copy(sheetSub.Columns);
                        sheetSub.Cells[4, 10] = String.Format("第{0}頁共{1}頁", pageIndex + 2, totalPages + 1);
                        pageIndex++;
                    }
                    /*//加入所有不管有無細項
                    Worksheet sheetSub = workbook.Worksheets.Add(Before: sheetLast);
                    sheetSub.Name = String.Format("估{0}", i + 1);
                    dict.Add(sheetSub.Name, sheetSub);
                    sheet0.Columns.Copy(sheetSub.Columns);
                    sheetSub.Cells[4, 10] = String.Format("第{0}頁共{1}頁", i + 2, apPayItems.Count + 1);*/
                }
                PayitemInvoice0(sheet0, apPayItems);
                for (int i = 0; i < apPayItems.Count; i++)
                {
                    if (apPayItems[i].subItems.Count > 0)//僅有細項
                        PayitemInvoiceSub(dict[String.Format("估{0}", i + 1)], apPayItems[i]);
                }
                /*
                */
                /*Range rangeSrc = sheet0.get_Range("A1", "J44");
                Range rangeSub = sheetSub.get_Range("A1", "J44");
                rangeSrc.Copy(rangeSub);*/
                //ns.Index = 3;
                //ns.Copy(sheet);
                //PayitemInvoiceSheet(sheet, eng, apHeader, miscList);

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("{0} 估驗請款單[{1}].xlsx", eng.EngNo, apHeader.ItemDate));
            }
            catch (Exception e)
            {
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                BaseService.log.Info("EPCAskPaymentController.CreateInvoiceExcel: " + e.Message);
                return Json(new
                {
                    result = -1,
                    message = "Excel 製表失敗"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //總表
        private void PayitemInvoiceHeaderSheet(Worksheet sheet, EPCAskPaymentEngVModel eng, EPCAskPaymentHeaderVModel apHeader, EPCAskPaymentHeaderVModel apPreviousHeader)
        {
            if (sheet == null) return;

            sheet.Cells[1, 1] = eng.ExecUnitName;
            sheet.Cells[3, 8] = DateTime.Now.ToString("yyyy/M/d");
            sheet.Cells[4, 1] = String.Format("工程名稱：{0}", eng.EngName);
            sheet.Cells[4, 5] = eng.ContractorName1;
            sheet.Cells[4, 8] = String.Format("會計科目：{0}", eng.BelongPrj);
            sheet.Cells[7, 3] = String.Format("{0}", apPreviousHeader.CurrentAccuAmount);
            sheet.Cells[7, 8] = String.Format("{0}", apHeader.CurrentAccuAmount);
            sheet.Cells[5, 1] = String.Format("契約編號：{0}", eng.ContractNo);
            sheet.Cells[5, 4] = String.Format("承包總價：{0}\n第一次變更：{1}", eng.BidAmount, eng.DesignChangeContractAmount);
            sheet.Cells[5, 8] = String.Format("請款期數：{0}", apHeader.Period);
        }
        //估0
        private void PayitemInvoice0(Worksheet sheet, List<EPCAskPaymentPayItemListVModel> apPayItems)
        {
            if (sheet == null) return;
            int cnt = apPayItems.Count - 1;
            if (cnt < 28) cnt = 28;
            int rowCount = cnt + 10;
            while (cnt > 0)
            {
                sheet.Rows[11].Insert();
                cnt--;
            }
            int row = 10;
            decimal totalPreviousAccuAmount = 0, totalPreviousSchAmount = 0, totalAccuAmount = 0, total = 0, totalSchAmount = 0;
            foreach (EPCAskPaymentPayItemListVModel m in apPayItems)
            {
                sheet.Cells[row, 2] = m.mainItem.Description;
                sheet.Cells[row, 3] = "全";
                sheet.Cells[row, 4] = m.mainItem.Price;
                sheet.Cells[row, 6] = m.mainItem.PreviousAccuAmount;
                sheet.Cells[row, 8] = m.mainItem.AccuAmount - m.mainItem.PreviousAccuAmount;
                sheet.Cells[row, 10] = m.mainItem.AccuAmount;
                foreach (EPCAskPaymentPayItemVModel item in m.subItems)
                {
                    if (item.ItemType != -1)
                    {
                        total += item.Amount.Value;
                        totalSchAmount += item.Amount.Value * item.SchProgress / 100;
                        totalAccuAmount += item.AccuAmount;
                        totalPreviousSchAmount += item.Amount.Value * item.PreviousSchProgress / 100;
                        totalPreviousAccuAmount += item.PreviousAccuAmount;
                    }
                }
                row++;
            }
            sheet.Cells[row, 2] = "合計";
            sheet.Cells[row, 6] = totalPreviousAccuAmount;
            sheet.Cells[row, 8] = totalAccuAmount - totalPreviousAccuAmount;
            sheet.Cells[row, 10] = totalAccuAmount;
            sheet.Cells[rowCount + 2, 5] = total == 0 ? 0 : totalPreviousSchAmount / total;
            sheet.Cells[rowCount + 2, 10] = total == 0 ? 0 : totalSchAmount / total;
            sheet.Cells[rowCount + 3, 5] = total == 0 ? 0 : totalPreviousAccuAmount / total;
            sheet.Cells[rowCount + 3, 10] = total == 0 ? 0 : totalAccuAmount / total;
        }
        //估1..n
        private void PayitemInvoiceSub(Worksheet sheet, EPCAskPaymentPayItemListVModel apPayItems)
        {
            if (sheet == null) return;

            sheet.Cells[9, 2] = String.Format("{0}:{1}", apPayItems.mainItem.PayItem, apPayItems.mainItem.Description);
            int cnt = apPayItems.subItems.Count - 1;
            if (cnt < 28) cnt = 28;
            int rowCount = cnt+10;
            while (cnt > 0)
            {
                sheet.Rows[11].Insert();
                cnt--;
            }
            int row = 10;
            decimal totalPreviousAccuAmount = 0, totalPreviousSchAmount = 0, totalAccuAmount = 0, total = 0, totalSchAmount = 0;
            foreach (EPCAskPaymentPayItemVModel m in apPayItems.subItems)
            {
                if (m.ItemType == -1)
                {
                    sheet.Cells[row, 2] = String.Format("{0}:{1}", m.PayItem.Replace(apPayItems.mainItem.PayItem+",", ""), m.Description);
                } else {
                    total += m.Amount.Value;
                    totalSchAmount += m.Amount.Value * m.SchProgress / 100;
                    totalAccuAmount += m.AccuAmount;
                    totalPreviousSchAmount += m.Amount.Value * m.PreviousSchProgress / 100;
                    totalPreviousAccuAmount += m.PreviousAccuAmount;
                    sheet.Cells[row, 2] = m.Description;
                    sheet.Cells[row, 3] = m.Unit;
                    sheet.Cells[row, 4] = m.Price;
                    sheet.Cells[row, 5] = m.PreviousAccuQuantity;
                    sheet.Cells[row, 6] = m.PreviousAccuAmount;
                    sheet.Cells[row, 7] = m.AccuQuantity - m.PreviousAccuQuantity;
                    sheet.Cells[row, 8] = m.AccuAmount - m.PreviousAccuAmount;
                    sheet.Cells[row, 9] = m.AccuQuantity;
                    sheet.Cells[row, 10] = m.AccuAmount;
                }
                row++;
            }
            sheet.Cells[row, 2] = "合計";
            sheet.Cells[row, 6] = totalPreviousAccuAmount;
            sheet.Cells[row, 8] = totalAccuAmount - totalPreviousAccuAmount;
            sheet.Cells[row, 10] = totalAccuAmount;
            sheet.Cells[rowCount + 2, 5] = total==0 ? 0 : totalPreviousSchAmount / total;
            sheet.Cells[rowCount + 2, 10] = total == 0 ? 0 : totalSchAmount / total;
            sheet.Cells[rowCount + 3, 5] = total == 0 ? 0 : totalPreviousAccuAmount / total;
            sheet.Cells[rowCount + 3, 10] = total == 0 ? 0 : totalAccuAmount / total;
        }

        //下載請款調整
        public ActionResult DownloadAdj(int id)
        {
            List<EPCAskPaymentHeaderVModel> hList = iService.GetAskPaymentHeader<EPCAskPaymentHeaderVModel>(id);
            if (hList.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                });
            }
            EPCAskPaymentHeaderVModel askPaymentHeader = hList[0];

            List<EPCAskPaymentEngVModel> items = iService.GetEngItem<EPCAskPaymentEngVModel>(askPaymentHeader.EngMainSeq);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            EPCAskPaymentEngVModel eng = items[0];

            List<AskPaymentPayItemModel> apList = iService.GetList<AskPaymentPayItemModel>(id);
            if (apList.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無資料"
                });
            }
            return CreateAdjExcel(eng, askPaymentHeader, apList);
        }
        private ActionResult CreateAdjExcel(EPCAskPaymentEngVModel eng, EPCAskPaymentHeaderVModel askPaymentHeader, List<AskPaymentPayItemModel> miscList)
        {
            string filename = Utils.CopyTemplateFile("進度管理-估驗請款調整.xlsx", ".xlsx");
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
                Worksheet sheet = dict["估驗請款"];
                PayitemAdjSheet(sheet, eng, askPaymentHeader, miscList);

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("{0} 估驗請款調整[{1}].xlsx", eng.EngNo, askPaymentHeader.ItemDate));
            }
            catch (Exception e)
            {
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                BaseService.log.Info("EPCAskPaymentController.CreateInvoiceExcel: " + e.Message);
                return Json(new
                {
                    result = -1,
                    message = "Excel 製表失敗"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //payitem
        private void PayitemAdjSheet(Worksheet sheet, EPCAskPaymentEngVModel eng, EPCAskPaymentHeaderVModel askPaymentHeader, List<AskPaymentPayItemModel> spList)
        {
            if (sheet == null) return;

            sheet.Cells[2, 3] = eng.EngName;// "@工程名稱";
            sheet.Cells[3, 3] = eng.EngNo;// "@標案編號";
            sheet.Cells[4, 8] = askPaymentHeader.ItemDate;// 日期
            int row = 6;
            foreach (AskPaymentPayItemModel m in spList)
            {
                sheet.Cells[row, 1] = m.OrderNo;
                sheet.Cells[row, 2] = m.PayItem.Trim();// === 會與 Excel 通用格式衝突";
                sheet.Cells[row, 3] = m.Description;// 項目及說明
                sheet.Cells[row, 4] = m.Unit;
                sheet.Cells[row, 5] = m.Quantity;
                sheet.Cells[row, 6] = m.Price;
                sheet.Cells[row, 7] = m.Amount;
                if (m.ItemType == -1)
                {
                    sheet.Cells[row, 8] = -1;
                    sheet.Cells[row, 9] = -1;
                    sheet.Cells[row, 10] = "";
                    sheet.Cells[row, 8].Interior.Color = sheet.Cells[row, 7].Interior.Color;
                    sheet.Cells[row, 9].Interior.Color = sheet.Cells[row, 7].Interior.Color;
                    sheet.Cells[row, 10].Interior.Color = sheet.Cells[row, 7].Interior.Color;
                }
                else
                {
                    sheet.Cells[row, 8] = m.AccuQuantity;
                    sheet.Cells[row, 9] = m.AccuAmount;
                    sheet.Cells[row, 10] = m.Memo;
                }
                
                row++;
            }
        }

        //匯入excel 更新估驗請款調整
        public JsonResult UploadAccu(int id)
        {
            List<EPCAskPaymentHeaderVModel> hList = iService.GetAskPaymentHeader<EPCAskPaymentHeaderVModel>(id);
            if (hList.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                });
            }
            EPCAskPaymentHeaderVModel askPaymentHeader = hList[0];

            List<EPCAskPaymentEngVModel> items = iService.GetEngItem<EPCAskPaymentEngVModel>(askPaymentHeader.EngMainSeq);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            EPCAskPaymentEngVModel eng = items[0];

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
                List<AskPaymentPayItemModel> apList = iService.GetList<AskPaymentPayItemModel>(id);
                if (apList.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "查無資料"
                    });
                }

                int result = readExcelData(fileName, eng, askPaymentHeader, apList);
                if (result < 0)
                {
                    string msg;
                    switch (result)
                    {
                        case -1: msg = "Excel解析發生錯誤"; break;
                        case -2: msg = "工程名稱/編號 資料錯誤"; break;
                        case -3: msg = "施工項目 資料錯誤"; break;
                        case -4: msg = "日期項目 資料錯誤"; break;
                        case -5: msg = "更新資料錯誤"; break;
                        default:
                            msg = "未知錯誤:" + result.ToString(); break;
                    }
                    return Json(new
                    {
                        result = -1,
                        message = msg
                    });
                }

                return Json(new
                {
                    result = 0,
                    message = "更新完成"
                }); ;

            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }
        private int readExcelData(string filename, EPCAskPaymentEngVModel eng, EPCAskPaymentHeaderVModel askPaymentHeader, List<AskPaymentPayItemModel> items)
        {
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
                Worksheet sheet = dict["估驗請款"];

                if (sheet.Cells[2, 3].Value.ToString() != eng.EngName || sheet.Cells[3, 3].Value.ToString() != eng.EngNo) return -2;

                //檢查 PayItem 是否一致
                int row = 6;
                foreach (AskPaymentPayItemModel m in items)
                {
                    if (sheet.Cells[row, 1].Value.ToString() != m.OrderNo.ToString() || sheet.Cells[row, 2].Value.ToString() != m.PayItem
                        || sheet.Cells[row, 3].Value.ToString() != m.Description || sheet.Cells[row, 4].Value.ToString() != m.Unit) return -3;
                    if (m.ItemType != -1)
                    {
                        decimal accuV;
                        if (decimal.TryParse(sheet.Cells[row, 8].Value.ToString(), out accuV))
                        {
                            m.AccuQuantity = accuV;
                        }
                        if (decimal.TryParse(sheet.Cells[row, 9].Value.ToString(), out accuV))
                        {
                            m.AccuAmount = accuV;
                        }
                        if(sheet.Cells[row, 10].Value != null) m.Memo = sheet.Cells[row, 10].Value.ToString();
                    }
                    row++;
                }
                //檢查 日期 是否一致
                DateTime d = DateTime.Parse(sheet.Cells[4, 8].Value.ToString());
                //System.Diagnostics.Debug.WriteLine(d.ToString("yyyy/M/d") + " "+ m.ItemDate);
                if (d.ToString("yyyy-MM-dd") != askPaymentHeader.ItemDate) return -4;
                //
                workbook.Close();
                appExcel.Quit();

                if (iService.UpdateAccu(items))
                    return 1;
                else
                    return -5;
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
    }
}