using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using EQC_CarbonEmissionCal.Common;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EQMCarbonEmissionController : Controller
    {//工程品管 - 碳排量計算
        CarbonEmissionPayItemService iService = new CarbonEmissionPayItemService();
        public ActionResult Index()
        {
            Utils.setUserClass(this, 2);
            return View("Index");
        }

        //碳交易工程調整 a20230508
        //工程清單
        public JsonResult GetCarbonTradeEngList(int year, int unit, int subUnit)
        {
            return Json(new
            {
                result = 0,
                items = iService.GetCarbonTradeEngList<CEFEngsVModel>(year, unit, subUnit)
            });
        }
        public JsonResult ConfirmTradeAdj(int id)
        {
            List<EQMCarbonEmissionHeaderVModel> ceHaeders = iService.GetHeaderList<EQMCarbonEmissionHeaderVModel>(id);
            if (ceHaeders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            EQMCarbonEmissionHeaderVModel ceHaeder = ceHaeders[0];
            decimal? co2Total = null;
            decimal? co2ItemTotal = null;
            decimal? greenFunding = null;
            iService.CalCarbonTotal(id, ref co2Total, ref co2ItemTotal, ref greenFunding);
            if (!co2Total.HasValue)
            {
                return Json(new
                {
                    result = -1,
                    msg = "錯誤, 無法取得設計總碳排放量"
                });
            }
            List<CarbonTradeEngsVModel> srcItems = iService.GetEngTradeList<CarbonTradeEngsVModel>(ceHaeder.Seq);
            List<CarbonTradeEngsVModel> items = iService.GetTradeAdjList<CarbonTradeEngsVModel>(ceHaeder.Seq);
            if (iService.ConfirmTradeAdj(ceHaeder, items, srcItems))
            {
                return Json(new
                {
                    result = 0,
                    msg = "儲存完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }

        //碳交易調整清單 20230508
        public JsonResult GetTradeAdjList(int id)
        {
            List<CarbonEmissionHeader2Model> ceHaeders = iService.GetTradeHeaders<CarbonEmissionHeader2Model>(id);
            if (ceHaeders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }

            CarbonEmissionHeader2Model ceHaeder = ceHaeders[0];
            if (!ceHaeder.AdjState.HasValue || ceHaeder.AdjState.Value == 1)
            {
                if (!iService.InitEngTradeAdj(id))
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "交易資料初始化失敗"
                    });
                }
            }
            return Json(new
            {
                result = 0,
                items = iService.GetTradeAdjList<CarbonTradeEngsVModel>(ceHaeder.Seq)
            });
        }
        //核定文件清單 s20230508
        public JsonResult DelTradeEng(int id)
        {
            if (iService.DelTradeEng(id)) {
                return Json(new
                {
                    result = 0,
                    msg = "刪除完成"
                });
            } else
            {
                return Json(new
                {
                    result = -1,
                    msg = "刪除失敗"
                });
            }
        }
        //可碳交易工程清單 20230508
        public JsonResult GetTradeEngs(int id)
        {
            return Json(new
            {
                result = 0,
                items = iService.GetTradeEngs(id)
            });
        }
        //碳交易工程 20230508
        public JsonResult GetTradeEng(string eNo)
        {
            List<CarbonTradeEngsVModel> engs = iService.GetTradeEng<CarbonTradeEngsVModel>(eNo);
            if (engs.Count == 1)
            {
                return Json(new
                {
                    result = 0,
                    item = engs[0]
                });
            } else
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料讀取錯誤"
                });
            }
        }
        //更新調整碳交易工程 20230508
        public JsonResult UpdateTradeAdj(int id, CarbonTradeEngsVModel item)
        {
            List<EQMCarbonEmissionHeaderVModel> ceHaeders = iService.GetHeaderList<EQMCarbonEmissionHeaderVModel>(id);
            if (ceHaeders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            EQMCarbonEmissionHeaderVModel ceHaeder = ceHaeders[0];
            if (iService.UpdateEngTradeAdj(ceHaeder.Seq, item))
            {
                return Json(new
                {
                    result = 0,
                    msg = "儲存完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }

        //確認碳交易工程 a20230420
        public JsonResult ConfirmTrade(int id)
        {
            List<EQMCarbonEmissionHeaderVModel> ceHaeders = iService.GetHeaderList<EQMCarbonEmissionHeaderVModel>(id);
            if (ceHaeders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            EQMCarbonEmissionHeaderVModel ceHaeder = ceHaeders[0];
            decimal? co2Total = null;
            decimal? co2ItemTotal = null;
            decimal? greenFunding = null;
            iService.CalCarbonTotal(id, ref co2Total, ref co2ItemTotal, ref greenFunding);
            if (!co2Total.HasValue)
            {
                return Json(new
                {
                    result = -1,
                    msg = "錯誤, 無法取得設計總碳排放量"
                });
            }
            List<CarbonTradeEngsVModel> items = iService.GetEngTradeList<CarbonTradeEngsVModel>(ceHaeder.Seq);
            if (iService.ConfirmTrade(ceHaeder, items, co2Total.Value))
            {
                return Json(new
                {
                    result = 0,
                    msg = "儲存完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }
        public ActionResult delCarbonTradeDoc(string fileName, int eId)
        {
            string fileDir = Utils.GetEngMainFolder(eId);
            iService.RemoveTradeDoc(fileName, eId);
            fileDir.RemoveFile(fileName);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //下載核定檔案 s20230419
        public ActionResult dnCarbonTradeDoc(int id, int eId)
        {
            List<EQMCarbonEmissionTradingDocVModel> docs = iService.GetApproveDocBySeq<EQMCarbonEmissionTradingDocVModel>(id);
            if (docs.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "基本資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EQMCarbonEmissionTradingDocVModel m = docs[0];
            string fName = Path.Combine(Utils.GetEngMainFolder(eId), m.UniqueFileName);
            if (!System.IO.File.Exists(fName))
            {
                return Json(new
                {
                    result = -1,
                    message = "未發現檔案"
                }, JsonRequestBehavior.AllowGet);
            }

            Stream iStream = new FileStream(fName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", m.OriginFileName);
        }
        //上傳碳交易核定文件 s20230419
        public JsonResult UploadTradeDoc(int id, DateTime docDate, string docNo)
        {
            List<CarbonEmissionHeader2Model> ceHeaders = iService.GetTradeHeaders<CarbonEmissionHeader2Model>(id);
            if (ceHeaders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "基本資料錯誤"
                });
            }
            CarbonEmissionHeader2Model ceHeader = ceHeaders[0];
            var file = Request.Files[0];
            string errMsg = "";
            if (file.ContentLength > 0)
            {
                try
                {
                    string uniqueFileName = SaveFile(file, ceHeader, "CarbonTrade-");
                    if (uniqueFileName != null)
                    {
                        string originFileName = file.FileName.ToString().Trim();
                        CarbonEmissionTradingDocModel m = new CarbonEmissionTradingDocModel()
                        {
                            Seq = -1,
                            CarbonEmissionHeaderSeq = ceHeader.Seq,
                            CarbonTradingApprovedDate = docDate,
                            CarbonTradingNo = docNo,
                            UniqueFileName = uniqueFileName,
                            OriginFileName = originFileName

                        };
                        if (iService.AppendTradeDoc(m))
                        {
                            return Json(new
                            {
                                result = 0,
                                message = "儲存完成"
                            });
                        }
                    }

                    return Json(new
                    {
                        result = -1,
                        message = "資料儲存失敗"
                    });
                }
                catch (Exception e)
                {
                    //System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                    errMsg = e.Message;
                }
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗: "+ errMsg
            });
        }
        //核定文件清單 s20230428
        public JsonResult GetApproveDocs(int id)
        {
            return Json(new
            {
                result = 0,
                items = iService.GetApproveDocs<EQMCarbonEmissionTradingDocVModel>(id)
            });
        }
        private string SaveFile(HttpPostedFileBase file, CarbonEmissionHeader2Model m, string fileHeader)
        {
            try
            {
                string filePath = Utils.GetEngMainFolder(m.EngMainSeq);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string uniqueFileName;
                /*//刪除已儲存原始檔案
                string uniqueFileName = m.UniqueFileName;
                if (uniqueFileName != null && uniqueFileName.Length > 0)
                {
                    System.IO.File.Delete(Path.Combine(filePath, uniqueFileName));
                }*/

                string originFileName = file.FileName.ToString().Trim();
                int inx = originFileName.LastIndexOf(".");
                uniqueFileName = String.Format("{0}{1}{2}", fileHeader, Guid.NewGuid().ToString("B").ToUpper(), originFileName.Substring(inx));
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
        //s20230419
        public JsonResult GetCETradeHeader(int id)
        {
            List<CarbonEmissionHeader2Model> list = iService.GetTradeHeaders<CarbonEmissionHeader2Model>(id);
            if (list.Count == 1)
            {
                return Json(new
                {
                    result = 0,
                    item = list[0]
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "基本資料錯誤"
                });
            }
        }
        //更新碳交易工程清單 20230419
        public JsonResult UpdateTrade(int id, List<CarbonTradeEngsVModel> items, string desc)
        {
            List<EQMCarbonEmissionHeaderVModel> ceHaeders = iService.GetHeaderList<EQMCarbonEmissionHeaderVModel>(id);
            if (ceHaeders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            EQMCarbonEmissionHeaderVModel ceHaeder = ceHaeders[0];
            if(iService.UpdateEngTrade(ceHaeder.Seq, items, desc))
            {
                return Json(new
                {
                    result = 0,
                    msg = "儲存完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }
        //可碳交易工程清單 20230419
        public JsonResult GetTradeList(int id)
        {
            List<CarbonEmissionHeader2Model> ceHaeders = iService.GetTradeHeaders<CarbonEmissionHeader2Model>(id);
            if (ceHaeders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            List<CarbonTradeEngsVModel> items ;
            CarbonEmissionHeader2Model ceHaeder = ceHaeders[0];
            if(ceHaeder.State == 0)
            {
                items = iService.GetTradeList<CarbonTradeEngsVModel>(id);
            } else
            {
                items = iService.GetEngTradeList<CarbonTradeEngsVModel>(ceHaeder.Seq);
            }
            var carbonCo2TotalService = new CarbonEmissionPayItemService();
            items.ForEach(e =>
            {
                decimal? Co2Total = null;
                decimal? Co2ItemTotal = null;
                carbonCo2TotalService.CalCarbonTotal(e.EngMainSeq, ref Co2Total, ref Co2ItemTotal);
                e.CarbonDesignQuantity = (int)(Co2Total ?? 0);
            });

            return Json(new
            {
                result = 0,
                items = items
            });

        }
        //綠色經費類型清單 20230418
        public JsonResult GreenFundingList()
        {
            List<SelectOptionModel> list = iService.GreenFundingList<SelectOptionModel>();
            //if (list.Count > 0) list.Insert(0, new SelectOptionModel() { Text = "", Value = "" });
            return Json(new
            {
                result = 0,
                items = list
            });
        }
        //資料檢查
        public JsonResult CheckData(int id)
        {
            bool c10NotMatch = false, notMatch = false, notLongEnough = false, c10NotMatchUnit = false;
            iService.CheckData(id, ref c10NotMatch, ref notMatch, ref notLongEnough, ref c10NotMatchUnit);
            string msg = "資料檢查完成\n無須修改PCCES或填寫理由";
            if (c10NotMatch || notMatch || notLongEnough || c10NotMatchUnit)
            {
                msg = "";
                string sp = "";
                if (c10NotMatch || c10NotMatchUnit)
                {
                    msg += sp + "單位錯誤(第10碼)";
                    sp = ", ";
                }
                if (notMatch)
                {
                    msg += sp + "查無編碼";
                    sp = ", ";
                }
                if (notLongEnough)
                {
                    msg += sp + "編碼不足10碼";
                }
                msg += " 項目\n請填寫理由 或 修改PCCES重新上傳";
            }
            return Json(new
            {
                result = 0,
                msg = msg
            });
        }
        //計算碳排量
        public JsonResult CalCarbonEmissions(int id)
        {
            if (iService.CalCarbonEmissions(id))
            {
                return Json(new
                {
                    result = 0,
                    msg = ""
                });
            }
            return Json(new
            {
                result = -1,
                msg = "計算碳排量發生錯誤"
            });
        }
        //更新資料
        public JsonResult UpdateRecord(CarbonEmissionPayItemModel m)
        {
            switch (m.RStatusCode)
            {
                case CarbonEmissionPayItemService._NotMatch: m.RStatusCode = CarbonEmissionPayItemService._NotMatchReason; break;
                case CarbonEmissionPayItemService._C10NotMatch: m.RStatusCode = CarbonEmissionPayItemService._C10NotMatchReason; break;
                case CarbonEmissionPayItemService._FullMatch: m.RStatusCode = CarbonEmissionPayItemService._FullMatchEdit; break;
                case CarbonEmissionPayItemService._Match: m.RStatusCode = CarbonEmissionPayItemService._MatchEdit; break;
                case CarbonEmissionPayItemService._NotLongEnough: m.RStatusCode = CarbonEmissionPayItemService._NotLongEnoughEdit; break;
                case CarbonEmissionPayItemService._NonTypeMatch: m.RStatusCode = CarbonEmissionPayItemService._NonTypeMatchEdit; break;
                case CarbonEmissionPayItemService._MatchC10_0: m.RStatusCode = CarbonEmissionPayItemService._MatchC10_0Edit; break;
                case CarbonEmissionPayItemService._Tree0Match: m.RStatusCode = CarbonEmissionPayItemService._Tree0MatchEdit; break;
                case CarbonEmissionPayItemService._C10NotMatchUnit: m.RStatusCode = CarbonEmissionPayItemService._C10NotMatchUnitEdit; break;

            }
            if (iService.Update(m) > 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "更新完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "更新失敗"
            });
        }
        //取得第一層大綱資料
        public JsonResult GetLevel1List(int id)
        {
            List<SelectOptionModel> list = iService.GetLevel1Options<SelectOptionModel>(id);
            if (list.Count > 0)
                list.Insert(0, new SelectOptionModel() { Text = "全部", Value = "" });
            return Json(new
            {
                result = 0,
                items = list
            });
        }
        public JsonResult GetList(int id, int perPage, int pageIndex, string keyword)
        {
            List<EngMainEditVModel> items = new EngMainService().GetItemBySeq<EngMainEditVModel>(id);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            EngMainEditVModel eng = items[0];

            int total = iService.GetListTotal(id, keyword);
            List<CarbonEmissionPayItemVModel> ceList = iService.GetList<CarbonEmissionPayItemVModel>(id, perPage, pageIndex, keyword);
            if (ceList.Count == 0 && string.IsNullOrEmpty(keyword))
            {//初始化資料
                if(!iService.CreatePayItems(id))
                {//s20231109
                    return Json(new
                    {
                        result = -1,
                        msg = "無 PCCES(xml) 資料, 請回工程重新匯入"
                    });
                }
                iService.CalCarbonEmissions(id);
                total = iService.GetListTotal(id, "");
                if (total == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "無初始資料, 請重新匯入"
                    });
                }
                ceList = iService.GetList<CarbonEmissionPayItemVModel>(id, perPage, pageIndex, "");
            }
            decimal? co2Total = null;
            decimal? co2ItemTotal = null;
            decimal? dismantlingRate = null;
            decimal? greenFunding = null; //s20230418
            decimal? greenFundingRate = null;
            if (total > 0)
            {//s20230525 改為 包發預算 SubContractingBudget
                iService.CalCarbonTotal(id, ref co2Total, ref co2ItemTotal, ref greenFunding);
                if (eng.SubContractingBudget.HasValue && eng.SubContractingBudget.Value > 0)
                {
                    dismantlingRate = Math.Round(co2ItemTotal.Value * 100 / eng.SubContractingBudget.Value);
                    greenFundingRate = Math.Round(greenFunding.Value * 100 / eng.SubContractingBudget.Value);
                }
            }
            UserInfo userInfo = Utils.getUserInfo();
            return Json(new
            {
                result = 0,
                totalRows = total,
                items = ceList,
                co2Total = co2Total,
                dismantlingRate = dismantlingRate,
                admin = userInfo.IsAdmin || userInfo.IsDepartmentAdmin,
                greenFunding = greenFunding,
                greenFundingRate = greenFundingRate,
            });
        }

        public JsonResult GetCEHeader(int id)
        {
            List<EQMCarbonEmissionHeaderVModel> list = iService.GetHeaderList<EQMCarbonEmissionHeaderVModel>(id);
            if (list.Count == 1)
            {
                return Json(new
                {
                    result = 0,
                    item = list[0]
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無資料"
                });
            }
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
            else if (items.Count > 1)
            {
                return Json(new
                {
                    result = 1,
                    msg = "資料錯誤: 對應到多個工程資料"
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
        //public ActionResult DownloadPccesXMLExport(int engId)
        //{
        //    var carbonHeader = iService.GetHeaderList<CarbonEmissionHeaderModel>(engId).FirstOrDefault();

        //    var engFolder = Utils.GetEngMainFolder(carbonHeader.EngMainSeq);
        //    var outputPoccessor = new PccesXMLUpdatedOuput(Path.Combine(engFolder, carbonHeader.PccesXMLFile));
        //    var payItems = iService.GetPayItemByHeaderSeq(carbonHeader.Seq);

        //    return File(outputPoccessor.exportUpdatedPayItemPCCESFile(payItems), "application/blob", String.Format("{0}-Pcces匯出.xml", engId)); 

        //}
        public ActionResult Download(int id)
        {
            try
            {
                List<EQMEngVModel> items = iService.GetEng<EQMEngVModel>(id);
                if (items.Count != 1)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "工程資料錯誤"
                    });
                }

                EQMEngVModel eng = items[0];

                List<EQMCarbonEmissionHeaderTradeVModel> ceHaeders = iService.GetTradeHeadersAndDoc<EQMCarbonEmissionHeaderTradeVModel>(id);
                if (ceHaeders.Count != 1)
                {//s20230420
                    return Json(new
                    {
                        result = -1,
                        msg = "查無碳排量基本資料"
                    });
                }
                EQMCarbonEmissionHeaderTradeVModel ceHaeder = ceHaeders[0];

                List<CarbonEmissionPayItemVModel> ceList = iService.GetList<CarbonEmissionPayItemVModel>(id, 9999, 1, "");
                if (ceList.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "查無碳排量資料"
                    });
                }

                //decimal? co2Total = null;
                //decimal? co2ItemTotal = null;
                //iService.CalCarbonTotal(id, ref co2Total, ref co2ItemTotal);
                //暫存目錄
                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                string folder = Path.Combine(Path.GetTempPath(), uuid);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                if (!CreateExcel(eng, ceList, folder))
                {
                    return Json(new
                    {
                        result = -1,
                        message = "碳排放量估算表 Excel 製表失敗"
                    }, JsonRequestBehavior.AllowGet);
                }
                List<CECheckTableModel> list = new CECheckTableService().GetList<CECheckTableModel>(id);
                if (list.Count > 0)
                {
                    CECheckTableModel model = list[0];
                    list[0].TreeJsonToList();
                    if (!CreateCheckTableDoc(eng, list[0], folder))
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "節能減碳簡易檢核表 製表失敗"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (!CreateCheckTableExcel(eng, ceList, folder, ceHaeder))
                {
                    return Json(new
                    {
                        result = -1,
                        message = "工程碳排放量檢核表 Excel 製表失敗"
                    }, JsonRequestBehavior.AllowGet);
                }

                string zipFile = Path.Combine(Path.GetTempPath(), uuid + "-碳排係數計算.zip");
                ZipFile.CreateFromDirectory(folder, zipFile);// AddFiles(files, "ProjectX");
                Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("{0} 碳排係數計算.zip", eng.EngNo));
            }
            catch (Exception e)
            {
                BaseService.log.Info("CreateExcel EQMCarbon Err: " + e.Message);
            }


            return Json(new
            {
                result = -1,
                message = "請求錯誤"
            }, JsonRequestBehavior.AllowGet);
        }
        private bool CreateExcel(EQMEngVModel eng, List<CarbonEmissionPayItemVModel> ceList, string folder)
        {
            //string filename = CopyTemplateFile("碳排放量估算表.xlsx", ".xlsx");

            string filename = Path.Combine(folder, "碳排放量估算表.xlsx");
            string srcFile = Path.Combine(Utils.GetTemplateFilePath(), "碳排放量估算表.xlsx");
            System.IO.File.Copy(srcFile, filename);

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
                fillSheet(dict["碳排放量估算表"], eng, ceList);


                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                return true; //return DownloadFile(filename, "xlsx");
            }
            catch (Exception e)
            {
                BaseService.log.Info("CreateExcel EQMCarbon Err: " + e.Message);
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                return false;
                /*return Json(new
                {
                    result = -1,
                    message = "Excel 製表失敗"
                }, JsonRequestBehavior.AllowGet);*/
            }
        }
        private void fillSheet(Worksheet sheet, EQMEngVModel eng, List<CarbonEmissionPayItemVModel> ceList)
        {
            if (sheet == null) return;

            sheet.Cells[1, 1] = "經濟部水利署" + eng.ExecUnitName;// "@執行機關";
            sheet.Cells[4, 2] = eng.EngName;// "@工程名稱";
            sheet.Cells[5, 2] = eng.EngPlace;// "@施工地點";
            sheet.Cells[5, 6] = eng.EngNo;// "@標案編號";

            int row = 7;
            foreach (CarbonEmissionPayItemVModel m in ceList)
            {
                sheet.Cells[row, 1] = m.ItemNo;// 項次";
                sheet.Cells[row, 2] = m.Description;// 項目及說明
                sheet.Cells[row, 3] = m.Unit;
                if (m.Description != "總計")
                    sheet.Cells[row, 4] = m.Quantity;
                sheet.Cells[row, 5] = m.KgCo2e;
                if (m.Description == "總計")
                    sheet.Cells[row, 6] = m.Amount;
                else
                {
                    sheet.Cells[row, 6] = m.ItemKgCo2e;
                    if(m.ItemKgCo2e.HasValue && eng.Co2Total.HasValue && eng.Co2Total.Value > 0) //s20230808
                        sheet.Cells[row, 7] = Math.Round((m.ItemKgCo2e.Value*100 / eng.Co2Total.Value),2).ToString()+"%";
                }
                sheet.Cells[row, 8] = m.Memo;
                if (String.IsNullOrEmpty(m.RStatus))
                    sheet.Cells[row, 9] = m.RStatusCodeStr.Replace("<font color=\"red\">", "").Replace("</font>", "").Replace("<br>", "\n");
                else
                    sheet.Cells[row, 9] = m.RStatus;
                row++;
            }

            sheet.Cells[row - 2, 5] = "總碳排放量";
            sheet.Cells[row - 2, 6] = eng.Co2Total;
            sheet.Cells[row - 2, 8] = "可拆解率%";
            if (eng.Co2ItemTotal.HasValue && eng.SubContractingBudget.HasValue && eng.SubContractingBudget.Value > 0) //s20230808
                sheet.Cells[row - 2, 9] = Math.Round(eng.Co2ItemTotal.Value * 100 / eng.SubContractingBudget.Value).ToString();

            Microsoft.Office.Interop.Excel.Range excelRange = sheet.Range[sheet.Cells[7, 1], sheet.Cells[row - 1, 9]];
            excelRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            excelRange.Borders.ColorIndex = 1;
        }

        //上傳 PCCESS xml
        public JsonResult UploadXML(int id)
        {
            var file = Request.Files[0];
            string errMsg = "";
            if (file.ContentLength > 0)
            {
                try
                {
                    string tempPath = Path.GetTempPath();
                    string fullPath = Path.Combine(tempPath, "CarbonEmission-" + file.FileName);
                    //string fullPath = Utils.GetTempFile(".xml");// Path.Combine(tempPath, uuid + ".xml");
                    file.SaveAs(fullPath);
                    List<EngMainEditVModel> items = new EngMainService().GetItemBySeq<EngMainEditVModel>(id);
                    if (items.Count != 1)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "工程資料讀取錯誤"
                        });
                    }
                    List<CarbonEmissionHeaderModel> hList = iService.GetHeaderList<CarbonEmissionHeaderModel>(id);
                    if (hList.Count != 1)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "主檔資料讀取錯誤"
                        });
                    }
                    CarbonEmissionHeaderModel headerModel = hList[0];
                    headerModel.PccesXMLFile = "CarbonEmission-" + file.FileName;

                    int result = processXML(fullPath, items[0], headerModel, ref errMsg);
                    if (result > 0)
                    {
                        string dir = Utils.GetEngMainFolder(headerModel.EngMainSeq);
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                        System.IO.File.Copy(fullPath, Path.Combine(dir, headerModel.PccesXMLFile), true);
                        iService.CalCarbonEmissions(id);
                        return Json(new
                        {
                            result = 0,
                            message = "上傳檔案完成",
                            item = result
                        });
                    }
                    else if (result == -1)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "資料儲存 錯誤: " + errMsg
                        });
                    }
                    else if (result == -2)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "工程編號/名稱 錯誤"
                        });
                    }
                    else if (result == -3)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "物價調整款已產生 不能再更新"
                        });
                    }
                }
                catch (Exception e)
                {
                    //System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                    errMsg = e.Message;
                }
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗: " + errMsg
            });
        }
        private int processXML(string fileName, EngMainEditVModel eng, CarbonEmissionHeaderModel headerModel, ref string errMsg)
        {
            PccseXML pccseXML = new PccseXML(fileName);
            EngMainModel engMainModel = pccseXML.CreateEngMainModel(ref errMsg);
            if (engMainModel == null || eng.EngNo != engMainModel.EngNo || eng.EngName != engMainModel.EngName) return -2;
            //List<EPCEngPriceAdjVModel> paList = new PriceAdjustmentService().GetDateList<EPCEngPriceAdjVModel>(eng.Seq);
            //List<EPCSchProgressHeaderVModel> scItems = new SchProgressPayItemService().GetHeaderList<EPCSchProgressHeaderVModel>(eng.Seq);
            //if (paList.Count == 0)//s20230428 已分離不需管制
            {
                pccseXML.GetGrandTotalInformation();
                engMainModel.Seq = eng.Seq;//s20230410
                engMainModel.TotalBudget = pccseXML.engMainModel.TotalBudget;
                return iService.UpdatePCCES(engMainModel, pccseXML.payItems, headerModel, ref errMsg);
            }
            //return -3;
        }
        public ActionResult DownloadPccesXML(int id)
        {
            var carbonHeader = iService.GetHeaderList<CarbonEmissionHeaderModel>(id).FirstOrDefault();

            var engFolder = Utils.GetEngMainFolder(carbonHeader.EngMainSeq);
            var outputPoccessor = new PccesXMLUpdatedOuput(Path.Combine(engFolder, carbonHeader.PccesXMLFile));
            var payItems = iService.GetPayItemByHeaderSeq(carbonHeader.Seq);

            return File(outputPoccessor.exportUpdatedPayItemPCCESFile(payItems), "application/blob", String.Format("Pcces匯入-{0}", carbonHeader.PccesXMLFile));
        }
        //下載 PccesXML shioulo 20220712
        public ActionResult DownloadXML(int id)
        {
            List<CarbonEmissionHeaderModel> items = iService.GetHeaderList<CarbonEmissionHeaderModel>(id);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            CarbonEmissionHeaderModel m = items[0];
            string fName = Path.Combine(Utils.GetEngMainFolder(m.EngMainSeq), m.PccesXMLFile);
            if (!System.IO.File.Exists(fName))
            {
                return Json(new
                {
                    result = -1,
                    message = "未發現檔案"
                }, JsonRequestBehavior.AllowGet);
            }

            Stream iStream = new FileStream(fName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", m.PccesXMLFile);
        }

        private ActionResult DownloadFile(string fullPath, string fileExt)
        {
            if (System.IO.File.Exists(fullPath))
            {
                Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", "碳排放量估算表." + fileExt);
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }
        private string CopyTemplateFile(string filename, string extFile)
        {
            string tempFile = Utils.GetTempFile(extFile);
            string srcFile = Path.Combine(Utils.GetTemplateFilePath(), filename);
            System.IO.File.Copy(srcFile, tempFile);
            return tempFile;
        }
        //檢查表資料 s20230313 
        public JsonResult GetCheckTable(int id)
        {
            CECheckTableModel model;
            List<CECheckTableModel> list = new CECheckTableService().GetList<CECheckTableModel>(id);
            if (list.Count > 0)
            {
                model = list[0];
            }
            else
            {
                model = new CECheckTableModel() { Seq = -1 };
            }
            model.TreeJsonToList();
            //
            if (String.IsNullOrEmpty(model.Signature))
            {
                List<SignatureFileVM> signatureFileVMList = new UserService().GetSignatureFileByUserSeq(Utils.getUserSeq());
                if (signatureFileVMList.Count > 0)
                {
                    SignatureFileVM signatureFileVM = signatureFileVMList[0];
                    string fullPath = Path.Combine(Server.MapPath(signatureFileVM.FilePath), signatureFileVM.FileName);
                    //model.Signature = @"data:image/png;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes(fullPath));
                    Image im = Utils.ResizeImage(400, 200, fullPath);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        im.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] imageBytes = ms.ToArray();

                        model.Signature = @"data:image/png;base64," + Convert.ToBase64String(imageBytes);
                    }
                }
            }
            if (String.IsNullOrEmpty(model.Signature)) model.Signature = "";
            return Json(new
            {
                result = 0,
                item = model
            });
        }
        //儲存檢查表 s20230314
        public JsonResult SaveCheckTable(CECheckTableModel item)
        {
            item.TreeListToJson();
            int result;
            if (item.Seq == -1)
            {
                result = new CECheckTableService().AddRecord(item);

            }
            else
            {
                result = new CECheckTableService().UpdateRecord(item);
            }
            if (result == -1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "儲存失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    id = item.Seq,
                    msg = "儲存完成"
                });
            }
        }
        
        //節能減碳簡易檢核表
        public ActionResult DnCheckTable(int id)
        {
            try
            {
                List<EQMEngVModel> items = iService.GetEng<EQMEngVModel>(id);
                if (items.Count != 1)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "工程資料錯誤"
                    });
                }
                EQMEngVModel eng = items[0];

                //暫存目錄
                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                string folder = Path.Combine(Path.GetTempPath(), uuid);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                List<CECheckTableModel> list = new CECheckTableService().GetList<CECheckTableModel>(id);
                if (list.Count > 0)
                {
                    CECheckTableModel model = list[0];
                    list[0].TreeJsonToList();
                    if (!CreateCheckTableDoc(eng, list[0], folder))
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "節能減碳簡易檢核表 製表失敗"
                        }, JsonRequestBehavior.AllowGet);
                    }
                } else
                {
                    return Json(new
                    {
                        result = -1,
                        message = "查無資料"
                    }, JsonRequestBehavior.AllowGet);
                }

                Stream iStream = new FileStream(Path.Combine(folder, "節能減碳簡易檢核表.docx"), FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("{0}-節能減碳簡易檢核表.docx", eng.EngNo));
            }
            catch (Exception e)
            {
                BaseService.log.Info("CreateExcel EQMCarbon Err: " + e.Message);
            }


            return Json(new
            {
                result = -1,
                message = "請求錯誤"
            }, JsonRequestBehavior.AllowGet);
        }
        private bool CreateCheckTableDoc(EQMEngVModel eng, CECheckTableModel ctItem, string folder)
        {
            string filename = Path.Combine(folder, "節能減碳簡易檢核表.docx");
            string srcFile = Path.Combine(Utils.GetTemplateFilePath(), "碳排量計算-附件1-節能減碳簡易檢核表1111019修正.docx");
            System.IO.File.Copy(srcFile, filename);

            Microsoft.Office.Interop.Word.Application wordApp = null;
            Document doc = null;
            try
            {
                wordApp = new Microsoft.Office.Interop.Word.Application();
                doc = wordApp.Documents.Open(filename);
                Table sheet = doc.Tables[1];

                sheet.Cell(2, 2).Range.Text = eng.EngName;
                //規劃、設計階段
                int row = 4;
                setCheckItem(ctItem.F1101, ++row, sheet);
                setCheckItem(ctItem.F1102, ++row, sheet);
                setCheckItem(ctItem.F1103, ++row, sheet);
                setCheckItem(ctItem.F1104, ++row, sheet);
                setCheckItem(ctItem.F1105, ++row, sheet);
                setCheckItem(ctItem.F1106, ++row, sheet);
                string buff = "", Split = "";
                foreach(CECheckTableTreeModel m in ctItem.TreeList)
                {
                    if (m.Seq == 9999)
                    {
                        if (m.Checked)
                            buff += Split+String.Format("■其他 {0} 樹種 {1} 株", m.TreeName, m.Amount);
                        else
                            buff += Split+"□其他________樹種___株";
                    }
                    else
                    {
                        if (m.Checked)
                            buff += Split+String.Format("■{0} {1} 株", m.TreeName, m.Amount);
                        else
                            buff += Split+String.Format("□{0}___株", m.TreeName);
                    }
                    Split = ",";
                }
                sheet.Cell(row, 4).Range.Text = Split + String.Format("是否盡量利用在地物種或碳儲存效果佳之樹種\n(如：高固碳樹種{0})",buff);
                setCheckItem(ctItem.F1107, ++row, sheet);
                sheet.Cell(row, 4).Range.Text = String.Format("其他：{0}", ctItem.F1107Desc);

                setCheckItem(ctItem.F1201, ++row, sheet);
                buff = "□20%，□30%，□50%，□其他____%";
                if (ctItem.F1201Mix.HasValue) {
                    switch (ctItem.F1201Mix.Value)
                    {
                        case 0: buff = String.Format("□20%，□30%，□50%，■其他 {0} %", ctItem.F1201Other); break;
                        case 1: buff = "■20%，□30%，□50%，□其他____%"; break;
                        case 2: buff = "□20%，■30%，□50%，□其他____%"; break;
                        case 3: buff = "□20%，□30%，■50%，□其他____%"; break;
                    }
                }
                sheet.Cell(row, 4).Range.Text = Split + String.Format("是否採用礦物摻料預拌混凝土設計(礦物摻料{0})", buff);
                setCheckItem(ctItem.F1202, ++row, sheet);
                setCheckItem(ctItem.F1203, ++row, sheet);
                setCheckItem(ctItem.F1204, ++row, sheet);
                setCheckItem(ctItem.F1205, ++row, sheet);
                setCheckItem(ctItem.F1206, ++row, sheet);
                sheet.Cell(row, 4).Range.Text = String.Format("其他：{0}", ctItem.F1206Desc);

                setCheckItem(ctItem.F1301, ++row, sheet);
                setCheckItem(ctItem.F1302, ++row, sheet);
                setCheckItem(ctItem.F1303, ++row, sheet);
                setCheckItem(ctItem.F1304, ++row, sheet);
                setCheckItem(ctItem.F1305, ++row, sheet);
                setCheckItem(ctItem.F1306, ++row, sheet);
                buff = "□就地取材□級配粒料□透水材料";
                if (ctItem.F1306Mode.HasValue)
                {
                    switch (ctItem.F1306Mode.Value)
                    {
                        case 1: buff = "■就地取材□級配粒料□透水材料"; break;
                        case 2: buff = "□就地取材■級配粒料□透水材料"; break;
                        case 3: buff = "□就地取材□級配粒料■透水材料"; break;
                    }
                }
                sheet.Cell(row, 4).Range.Text = Split + String.Format("是否選擇材料回填，{0}。", buff);
                setCheckItem(ctItem.F1307, ++row, sheet);
                buff = "□乾砌石□鋪石□漿砌石□混凝土排石";
                if (ctItem.F1307Mode.HasValue)
                {
                    switch (ctItem.F1307Mode.Value)
                    {
                        case 1: buff = "■乾砌石□鋪石□漿砌石□混凝土排石"; break;
                        case 2: buff = "□乾砌石■鋪石□漿砌石□混凝土排石"; break;
                        case 3: buff = "□乾砌石□鋪石■漿砌石□混凝土排石"; break;
                        case 4: buff = "□乾砌石□鋪石□漿砌石■混凝土排石"; break;
                    }
                }
                sheet.Cell(row, 4).Range.Text = Split + String.Format("是否採用砌排石工，{0}。", buff);
                setCheckItem(ctItem.F1308, ++row, sheet);
                setCheckItem(ctItem.F1309, ++row, sheet);
                buff = "□就地取料□外購石料";
                if (ctItem.F1309Mode.HasValue)
                {
                    switch (ctItem.F1309Mode.Value)
                    {
                        case 1: buff = "■就地取料□外購石料"; break;
                        case 2: buff = "□就地取料■外購石料"; break;
                    }
                }
                sheet.Cell(row, 4).Range.Text = Split + String.Format("是否採用箱型石籠工，{0}。", buff);
                setCheckItem(ctItem.F1310, ++row, sheet);
                buff = "□就地取料□外購石料";
                if (ctItem.F1310Mode.HasValue)
                {
                    switch (ctItem.F1310Mode.Value)
                    {
                        case 1: buff = "■就地取料□外購石料"; break;
                        case 2: buff = "□就地取料■外購石料"; break;
                    }
                }
                sheet.Cell(row, 4).Range.Text = Split + String.Format("是否採用拋石工，{0}。", buff);
                setCheckItem(ctItem.F1311, ++row, sheet);
                setCheckItem(ctItem.F1312, ++row, sheet);
                sheet.Cell(row, 4).Range.Text = String.Format("其他：{0}", ctItem.F1312Desc);

                setCheckItem(ctItem.F1401, ++row, sheet);
                setCheckItem(ctItem.F1402, ++row, sheet);
                setCheckItem(ctItem.F1403, ++row, sheet);
                setCheckItem(ctItem.F1404, ++row, sheet);
                sheet.Cell(row, 4).Range.Text = String.Format("其他：{0}", ctItem.F1404Desc);
                //施工階段
                sheet = doc.Tables[2];
                row = 1;
                setCheckItem(ctItem.F2101, ++row, sheet);
                setCheckItem(ctItem.F2102, ++row, sheet);
                setCheckItem(ctItem.F2103, ++row, sheet);
                sheet.Cell(row, 4).Range.Text = String.Format("其他：{0}", ctItem.F2103Desc);

                setCheckItem(ctItem.F2201, ++row, sheet);
                setCheckItem(ctItem.F2202, ++row, sheet);
                setCheckItem(ctItem.F2203, ++row, sheet);
                setCheckItem(ctItem.F2204, ++row, sheet);
                sheet.Cell(row, 4).Range.Text = String.Format("其他：{0}", ctItem.F2204Desc);

                setCheckItem(ctItem.F2301, ++row, sheet);
                setCheckItem(ctItem.F2302, ++row, sheet);
                setCheckItem(ctItem.F2303, ++row, sheet);
                sheet.Cell(row, 4).Range.Text = String.Format("其他：{0}", ctItem.F2303Desc);
                //營運階段
                row++;
                setCheckItem(ctItem.F3101, ++row, sheet);
                setCheckItem(ctItem.F3102, ++row, sheet);
                setCheckItem(ctItem.F3103, ++row, sheet);
                setCheckItem(ctItem.F3104, ++row, sheet);
                sheet.Cell(row, 4).Range.Text = String.Format("其他：{0}", ctItem.F3104Desc);

                setCheckItem(ctItem.F3201, ++row, sheet);
                setCheckItem(ctItem.F3202, ++row, sheet);
                setCheckItem(ctItem.F3203, ++row, sheet);
                setCheckItem(ctItem.F3204, ++row, sheet);
                sheet.Cell(row, 4).Range.Text = String.Format("其他：{0}", ctItem.F3204Desc);

                setCheckItem(ctItem.F3301, ++row, sheet);
                setCheckItem(ctItem.F3302, ++row, sheet);
                sheet.Cell(row, 4).Range.Text = String.Format("其他：{0}", ctItem.F3302Desc);

                setCheckItem(ctItem.F3401, ++row, sheet);
                setCheckItem(ctItem.F3402, ++row, sheet);
                setCheckItem(ctItem.F3403, ++row, sheet);
                sheet.Cell(row, 4).Range.Text = String.Format("其他：{0}", ctItem.F3403Desc);

                //簽章
                if (!String.IsNullOrEmpty(ctItem.Signature))
                {
                    sheet.Cell(++row, 2).Select();
                    byte[] imageBytes = Convert.FromBase64String(ctItem.Signature.Replace("data:image/png;base64,", ""));
                    using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                    {
                        Image image = Image.FromStream(ms, true);
                        string fileName = Utils.GetTempFile(".png");
                        image.Save(fileName);
                        object linkToFile = false;
                        object saveWithDocument = true;
                        object range = wordApp.Selection.Range;
                        InlineShape shape = wordApp.ActiveDocument.InlineShapes.AddPicture(fileName, ref linkToFile, ref saveWithDocument, ref range);
                        shape.Width = 90f;
                        shape.Height = 30f;
                        shape.ConvertToShape().WrapFormat.Type = WdWrapType.wdWrapFront;
                    }
                }

                doc.Save();
                if (doc != null) doc.Close();
                if (wordApp != null) wordApp.Quit();

                return true;
            }
            catch (Exception e)
            {
                BaseService.log.Info("EQMCarbonEmissionController.節能減碳簡易檢核表: " + e.Message);
                if (doc != null) doc.Close();
                if (wordApp != null) wordApp.Quit();
            }

            return false;
        }
        private void setCheckItem(int? data, int row, Table sheet)
        {
            if (data.HasValue) {
                switch (data.Value) {
                    case 1: sheet.Cell(row, 5).Range.Text = "ˇ"; break;
                    case 2: sheet.Cell(row, 6).Range.Text = "ˇ"; break;
                    case 3: sheet.Cell(row, 7).Range.Text = "ˇ"; break;
                }
            }
        }
        //工程碳排放量檢核表
        private bool CreateCheckTableExcel(EQMEngVModel eng, List<CarbonEmissionPayItemVModel> ceList, string folder, EQMCarbonEmissionHeaderTradeVModel ceHaeder)
        {
            string filename = Path.Combine(folder, "工程碳排放量檢核表.xlsx");
            string srcFile = Path.Combine(Utils.GetTemplateFilePath(), "碳排量計算-工程碳排放量檢核表.xlsx");
            System.IO.File.Copy(srcFile, filename);

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
                fillCheckTableSheet(dict["工程碳排放量檢核表"], eng, ceList, ceHaeder);

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                return true;
            }
            catch (Exception e)
            {
                BaseService.log.Info("CreateExcel EQMCarbon Err: " + e.Message);
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                return false;
            }
        }
        private void fillCheckTableSheet(Worksheet sheet, EQMEngVModel eng, List<CarbonEmissionPayItemVModel> ceList, EQMCarbonEmissionHeaderTradeVModel ceHaeder)
        {
            if (sheet == null) return;

            sheet.Cells[1, 1] = "經濟部水利署" + eng.ExecUnitName;// "@執行機關";
            sheet.Cells[6, 2] = eng.EngName;// "@工程名稱";
            sheet.Cells[7, 2] = eng.EngPlace;// "@施工地點";
            sheet.Cells[7, 6] = eng.EngNo;// "@標案編號";
            sheet.Cells[10, 2] = eng.Co2Total;
            //s20230619 包發預算 SubContractingBudget
            if (eng.Co2ItemTotal.HasValue && eng.SubContractingBudget.HasValue && eng.SubContractingBudget.Value > 0)
                sheet.Cells[11, 2] = String.Format("{0}%",Math.Round(eng.Co2ItemTotal.Value * 100 / eng.SubContractingBudget.Value));
            sheet.Cells[15, 1] = ceHaeder.CarbonTradingDesc;
            sheet.Cells[16, 5] = ceHaeder.CarbonTradingApprovedDateStr+" "+ceHaeder.CarbonTradingNo;
        }

    }
}