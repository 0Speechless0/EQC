using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EPCSchEngProgressController : Controller
    {//預定進度 - 前置作業
        SchEngProgressService iService = new SchEngProgressService();

        //下載 PccesXML s20230417
        public ActionResult DownloadPccesXML(int id)
        {
            List<EPCSchEngProgressHeaderVModel> items = iService.GetHeaderList<EPCSchEngProgressHeaderVModel>(id);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EPCSchEngProgressHeaderVModel m = items[0];
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
        //上傳 PCCESS xml s20230417
        public JsonResult UploadXML(int id)
        {
            var file = Request.Files[0];
            string errMsg = "";
            if (file.ContentLength > 0)
            {
                try
                {
                    string tempPath = Path.GetTempPath();
                    string fullPath = Path.Combine(tempPath, "SchEngProgress-" + file.FileName);
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
                    List<SchEngProgressHeaderModel> hList = iService.GetHeaderList<SchEngProgressHeaderModel>(id);
                    if (hList.Count != 1)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "主檔資料讀取錯誤"
                        });
                    }
                    SchEngProgressHeaderModel headerModel = hList[0];
                    headerModel.PccesXMLFile = "SchEngProgress-" + file.FileName;

                    int result = processXML(fullPath, items[0], headerModel, ref errMsg);
                    if (result > 0)
                    {
                        string dir = Utils.GetEngMainFolder(headerModel.EngMainSeq);
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                        System.IO.File.Copy(fullPath, Path.Combine(dir, headerModel.PccesXMLFile), true);
                        new CarbonEmissionPayItemService().CalCarbonEmissionsForSchEngProgress(id);
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
        private int processXML(string fileName, EngMainEditVModel eng, SchEngProgressHeaderModel headerModel, ref string errMsg)
        {
            PccseXML pccseXML = new PccseXML(fileName);
            EngMainModel engMainModel = pccseXML.CreateEngMainModel(ref errMsg);
            if (engMainModel == null || eng.EngNo != engMainModel.EngNo || eng.EngName != engMainModel.EngName) return -2;
            pccseXML.GetGrandTotalInformation();
            engMainModel.Seq = eng.Seq;
            engMainModel.TotalBudget = pccseXML.engMainModel.TotalBudget;
            return iService.UpdatePCCES(engMainModel, pccseXML.payItems, headerModel, ref errMsg);
        }

        //填報完成
        public JsonResult FillCompleted(int id)
        {
            List<EPCSchEngProgressHeaderVModel> sepHeaders = iService.GetHeaderList<EPCSchEngProgressHeaderVModel>(id);
            if (sepHeaders.Count != 1)
            {//s20230420
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤,無法作業"
                });
            }
            EPCSchEngProgressHeaderVModel sepHeader = sepHeaders[0];
            decimal? co2Total = null;
            decimal? co2ItemTotal = null;
            iService.CalCarbonTotal(id, ref co2Total, ref co2ItemTotal);
            if (!co2Total.HasValue)
            {//s20230420
                return Json(new
                {
                    result = -1,
                    msg = "碳排量資料錯誤"
                });
            }
            if (iService.FillCompleted(sepHeader, co2Total.Value) > 0)
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
        //更新資料
        public JsonResult UpdateRecord(SchEngProgressPayItemModel m)
        {
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
        //刪除資料
        public JsonResult DelRecord(int id)
        {
            if (iService.DelRecord(id) == 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "刪除完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "刪除失敗"
            });
        }
        //還原刪除
        public JsonResult UnDelRecord(int id)
        {
            if (iService.UnDelRecord(id) == 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "還原完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "還原失敗"
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
        //取得二層大綱資料
        public JsonResult GetLevel2List(int id, string key)
        {
            List<SelectOptionModel> list = iService.GetLevel2Options<SelectOptionModel>(id, key);
            if (list.Count > 0)
                list.Insert(0, new SelectOptionModel() { Text = "全部", Value = "" });
            return Json(new
            {
                result = 0,
                items = list
            });
        }
        public JsonResult GetList(int id, int perPage, int pageIndex, string fLevel, string keyword)
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

            int resultCode = 0;
            int total = iService.GetListTotal(id, fLevel, keyword);
            List<SchEngProgressPayItemModel> ceList = iService.GetList<SchEngProgressPayItemModel>(id, perPage, pageIndex, fLevel, keyword);
            if (ceList.Count == 0 && string.IsNullOrEmpty(keyword))
            {//初始化資料
                List<CarbonEmissionPayItemV2Model> cepList = iService.GetCarbonEmissionList<CarbonEmissionPayItemV2Model>(id);
                if (cepList.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "無碳排量計算的初始資料, 請確認"
                    });
                }
                iService.GetCarbonEmissionWorkItemList(cepList);
                if(!iService.InitData(id, cepList))
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "初始化資料, 建立失敗"
                    });
                }
                resultCode = 1;
                total = iService.GetListTotal(id, "", "");
                ceList = iService.GetList<SchEngProgressPayItemModel>(id, perPage, pageIndex, "", "");
            }
            decimal? co2Total = null;
            decimal? co2ItemTotal = null;
            decimal? dismantlingRate = null;
            //s20230528
            decimal? co2TotalDesign = null;
            decimal? greenFunding = null;
            decimal? greenFundingRate = null;
            new CarbonEmissionPayItemService().CalCarbonTotal(id, ref co2TotalDesign, ref co2ItemTotal, ref greenFunding); //s20230528 碳排量計算

            co2Total = null;
            co2ItemTotal = null;
            dismantlingRate = null;
            greenFunding = null;
            greenFundingRate = null;
            if (total > 0)
            {
                iService.CalCarbonTotal(id, ref co2Total, ref co2ItemTotal, ref greenFunding);
                if (eng.SubContractingBudget.HasValue && eng.SubContractingBudget.Value > 0)
                {
                    dismantlingRate = Math.Round(co2ItemTotal.Value * 100 / eng.SubContractingBudget.Value);
                    greenFundingRate = Math.Round(greenFunding.Value * 100 / eng.SubContractingBudget.Value);
                }
            }
            return Json(new
            {
                result = resultCode,
                totalRows = total,
                items = ceList,
                co2Total = co2Total,
                dismantlingRate = dismantlingRate,
                co2TotalDesign = co2TotalDesign,
                greenFunding = greenFunding,
                greenFundingRate = greenFundingRate,
            });
        }
        //刪除清單 s20230721
        public JsonResult GetDelList(int id)
        {
            int resultCode = 0;
            List<SchEngProgressPayItemModel> ceList = iService.GetDelList<SchEngProgressPayItemModel>(id);

            return Json(new
            {
                result = 0,
                items = ceList
            });
        }
        //工程進度-主檔
        public JsonResult GetSEPHeader(int id)
        {
            List<EPCSchEngProgressHeaderVModel> list = iService.GetHeaderList<EPCSchEngProgressHeaderVModel>(id);
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
    }
}