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
    public class EPCPriceAdjustmentController : Controller
    {//工程管理 - 物價調整款
        PriceAdjustmentService iService = new PriceAdjustmentService();
        // shioulo20221228
        public JsonResult GetEngItem(int id)
        {
            List<EPCPriceAdjustmentVModel> items = iService.GetEngMainBySeq<EPCPriceAdjustmentVModel>(id);
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
        // shioulo20221228
        public JsonResult UpdateEngAwardDate(EPCPriceAdjustmentVModel engMain)
        {
            engMain.updateDate();

            int result = iService.UpdateEngAwardDate(engMain);
            if (result >= 1)
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存成功"
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
        //重新計算
        public JsonResult CalPriceIndex(int id, string tarDate)
        {
            //if(CalPriceAdjustment(id, tarDate))
            if (iService.DelEngPriceAdjWorkItem(id, tarDate)) //shioulo 20221025 改為清除原有資料
            {
                return Json(new
                {
                    result = 0,
                    msg = "計算完成"
                });
            } else
            {
                return Json(new
                {
                    result = -1,
                    msg = "物價調整款計算失敗"
                });
            }
            //List<EngPriceAdjWorkItemVModel> ceList = iService.GetList<EngPriceAdjWorkItemVModel>(id, tarDate);
        }
        //該日期項目清單
        public JsonResult GetList(int id, string tarDate)
        {
            List<EngPriceAdjWorkItemVModel> ceList = iService.GetList<EngPriceAdjWorkItemVModel>(id, tarDate);
            if(ceList.Count == 0)
            {
                List<EngPriceAdjModel> engPriceAdjs = iService.GetEngPriceAdj<EngPriceAdjModel>(id, tarDate);
                if(engPriceAdjs.Count != 1)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "資料錯誤"
                    });
                }
                if(!iService.InitEngPriceAdjWorkItem(engPriceAdjs[0]))
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "資料建立失敗"
                    });
                }
                if(!CalPriceAdjustment(id, tarDate))
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "物價調整款計算失敗"
                    });
                }
                ceList = iService.GetList<EngPriceAdjWorkItemVModel>(id, tarDate);
            }
            EngPriceAdjWorkItemGroupVModel gModel = new EngPriceAdjWorkItemGroupVModel(ceList);

            return Json(new
            {
                result = 0,
                group = gModel
            });
        }
        //計算物價調整款
        private bool CalPriceAdjustment(int engMainSeq, string adjMonth)
        {
            List<EngPriceAdjWorkItemVModel> ceList = iService.GetList<EngPriceAdjWorkItemVModel>(engMainSeq, adjMonth);
            EngPriceAdjWorkItemGroupVModel gModel = new EngPriceAdjWorkItemGroupVModel(ceList, false, true);
            //個類
            if (gModel.M03310.Count > 0)
            {
                List<PriceIndexItemModel> pi = iService.GetPriceIndex<PriceIndexItemModel>(101, adjMonth);
                decimal priceIndex = 0;
                if (pi.Count >= 2) priceIndex = pi[0].PriceIndex - pi[1].PriceIndex;
                bool adjFlag = (priceIndex > 10);
                foreach (EngPriceAdjWorkItemVModel m in gModel.M03310)
                {
                    if(adjFlag)
                    {//符合
                        m.PriceIndex = priceIndex;
                        //(權重金額*(物價指數 - 10 %) * 1.05)
                        m.PriceAdjustment = m.WeightsAmount * (m.PriceIndex - 10)/100 * decimal.Parse("1.05");
                        m.AdjKind = 101;
                    } else
                    {
                        gModel.M03.Add(m);
                    }
                }
                if (!adjFlag) gModel.M03310.Clear();
            }
            if (gModel.M03210.Count > 0)
            {
                List<PriceIndexItemModel> pi = iService.GetPriceIndex<PriceIndexItemModel>(102, adjMonth);
                decimal priceIndex = 0;
                if (pi.Count >= 2) priceIndex = pi[0].PriceIndex - pi[1].PriceIndex;
                bool adjFlag = (priceIndex > 10);
                foreach (EngPriceAdjWorkItemVModel m in gModel.M03210)
                {
                    if (adjFlag)
                    {//符合
                        m.PriceIndex = priceIndex;
                        //(權重金額*(物價指數 - 10 %) * 1.05)
                        m.PriceAdjustment = m.WeightsAmount * (m.PriceIndex - 10) / 100 * decimal.Parse("1.05");
                        m.AdjKind = 102;
                    }
                    else
                    {
                        gModel.M03.Add(m);
                    }
                }
                if (!adjFlag) gModel.M03210.Clear();
            }
            if (gModel.M02742.Count > 0)
            {
                List<PriceIndexItemModel> pi = iService.GetPriceIndex<PriceIndexItemModel>(103, adjMonth);
                decimal priceIndex = 0;
                if (pi.Count >= 2) priceIndex = pi[0].PriceIndex - pi[1].PriceIndex;
                bool adjFlag = (priceIndex > 10);
                foreach (EngPriceAdjWorkItemVModel m in gModel.M02742)
                {
                    if (adjFlag)
                    {//符合
                        m.PriceIndex = priceIndex;
                        //(權重金額*(物價指數 - 10 %) * 1.05)
                        m.PriceAdjustment = m.WeightsAmount * (m.PriceIndex - 10) / 100 * decimal.Parse("1.05");
                        m.AdjKind = 103;
                    }
                    else
                    {
                        gModel.M027_0296.Add(m);
                    }
                }
                if (!adjFlag) gModel.M02742.Clear();
            }
            //中類
            if (gModel.M03.Count > 0)
            {
                int inx = 0;
                while(inx < gModel.M03.Count)
                {
                    EngPriceAdjWorkItemVModel m = gModel.M03[inx];
                    if (m.PriceIndexKindId >= 200 && m.PriceIndexKindId < 300)
                    {
                        List<PriceIndexItemModel> pi = iService.GetPriceIndex<PriceIndexItemModel>(m.PriceIndexKindId, adjMonth);
                        decimal priceIndex = 0;
                        if (pi.Count >= 2) priceIndex = pi[0].PriceIndex - pi[1].PriceIndex;
                        if (priceIndex > 5)
                        {
                            m.PriceIndex = priceIndex;
                            //(權重金額*(物價指數 - 5 %) * 1.05)
                            m.PriceAdjustment = m.WeightsAmount * (m.PriceIndex - 5) / 100 * decimal.Parse("1.05");
                            m.AdjKind = 201;
                            inx++;
                        }
                        else
                        {
                            gModel.Mxxx.Add(m);
                            gModel.M03.RemoveAt(inx);
                        }
                    }
                    else
                    {
                        gModel.Mxxx.Add(m);
                        gModel.M03.RemoveAt(inx);
                    }
                }
            }
            if (gModel.M05.Count > 0)
            {
                int inx = 0;
                while (inx < gModel.M05.Count)
                {
                    EngPriceAdjWorkItemVModel m = gModel.M05[inx];
                    if (m.PriceIndexKindId >= 200 && m.PriceIndexKindId < 300)
                    {
                        List<PriceIndexItemModel> pi = iService.GetPriceIndex<PriceIndexItemModel>(m.PriceIndexKindId, adjMonth);
                        decimal priceIndex = 0;
                        if (pi.Count >= 2) priceIndex = pi[0].PriceIndex - pi[1].PriceIndex;
                        if (priceIndex > 5)
                        {
                            m.PriceIndex = priceIndex;
                            //(權重金額*(物價指數 - 5 %) * 1.05)
                            m.PriceAdjustment = m.WeightsAmount * (m.PriceIndex - 5) / 100 * decimal.Parse("1.05");
                            m.AdjKind = 202;
                            inx++;
                        }
                        else
                        {
                            gModel.Mxxx.Add(m);
                            gModel.M05.RemoveAt(inx);
                        }
                    }
                    else
                    {
                        gModel.Mxxx.Add(m);
                        gModel.M05.RemoveAt(inx);
                    }
                }
            }
            if (gModel.M02319.Count > 0)
            {
                int inx = 0;
                while (inx < gModel.M02319.Count)
                {
                    EngPriceAdjWorkItemVModel m = gModel.M02319[inx];
                    if (m.PriceIndexKindId >= 200 && m.PriceIndexKindId < 300)
                    {
                        List<PriceIndexItemModel> pi = iService.GetPriceIndex<PriceIndexItemModel>(m.PriceIndexKindId, adjMonth);
                        decimal priceIndex = 0;
                        if (pi.Count >= 2) priceIndex = pi[0].PriceIndex - pi[1].PriceIndex;
                        if (priceIndex > 5)
                        {
                            m.PriceIndex = priceIndex;
                            //(權重金額*(物價指數 - 5 %) * 1.05)
                            m.PriceAdjustment = m.WeightsAmount * (m.PriceIndex - 5) / 100 * decimal.Parse("1.05");
                            m.AdjKind = 203;
                            inx++;
                        }
                        else
                        {
                            gModel.Mxxx.Add(m);
                            gModel.M02319.RemoveAt(inx);
                        }
                    }
                    else
                    {
                        gModel.Mxxx.Add(m);
                        gModel.M02319.RemoveAt(inx);
                    }
                }
            }
            if (gModel.M027_0296.Count > 0)
            {
                int inx = 0;
                while (inx < gModel.M027_0296.Count)
                {
                    EngPriceAdjWorkItemVModel m = gModel.M027_0296[inx];
                    if (m.PriceIndexKindId >= 200 && m.PriceIndexKindId < 300)
                    {
                        List<PriceIndexItemModel> pi = iService.GetPriceIndex<PriceIndexItemModel>(m.PriceIndexKindId, adjMonth);
                        decimal priceIndex = 0;
                        if (pi.Count >= 2) priceIndex = pi[0].PriceIndex - pi[1].PriceIndex;
                        if (priceIndex > 5)
                        {
                            m.PriceIndex = priceIndex;
                            //(權重金額*(物價指數 - 5 %) * 1.05)
                            m.PriceAdjustment = m.WeightsAmount * (m.PriceIndex - 5) / 100 * decimal.Parse("1.05");
                            m.AdjKind = 204;
                            inx++;
                        }
                        else
                        {
                            gModel.Mxxx.Add(m);
                            gModel.M027_0296.RemoveAt(inx);
                        }
                    }
                    else
                    {
                        gModel.Mxxx.Add(m);
                        gModel.M027_0296.RemoveAt(inx);
                    }
                }
            }
            //總類
            if (gModel.Mxxx.Count > 0)
            {
                List<PriceIndexItemModel> pi = iService.GetPriceIndex<PriceIndexItemModel>(999, adjMonth);
                decimal priceIndex = 0;
                if (pi.Count >= 2) priceIndex = pi[0].PriceIndex - pi[1].PriceIndex;
                bool adjFlag = (priceIndex > decimal.Parse("2.5"));
                foreach (EngPriceAdjWorkItemVModel m in gModel.Mxxx)
                {
                    if (adjFlag)
                    {//符合
                        m.PriceIndex = priceIndex;
                        //(權重金額*(物價指數 - 2 %) * 1.05)
                        m.PriceAdjustment = m.WeightsAmount * (m.PriceIndex - decimal.Parse("2.5")) / 100 * decimal.Parse("1.05");
                        m.AdjKind = 999;
                    }
                }
            }

            return iService.UpdateEngPriceAdjWorkItem(gModel);
        }

        //日期清單
        public JsonResult GetDateList(int id)
        {
            List<SupDailyDateModel> maxSupDailyDate = iService.GetMaxSupDailyDate<SupDailyDateModel>(id);
            if (maxSupDailyDate.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程無監造日誌"
                });
            }

            List<DateTime> dateList = new List<DateTime>();
            List<EPCEngPriceAdjVModel> dList = iService.GetDateList<EPCEngPriceAdjVModel>(id);

            if (dList.Count == 0)
            {//初始日期清單
                List<EngMainEditVModel> engItems = new EngMainService().GetItemBySeq<EngMainEditVModel>(id);
                if (engItems.Count != 1)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "工程資料錯誤"
                    });
                }

                EngMainEditVModel eng = engItems[0];
                if (!eng.AwardDate.HasValue)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "決標日期資料錯誤"
                    });
                }

                SchProgressPayItemService schProgressPayItemService = new SchProgressPayItemService();
                List<EPCSchProgressVModel> scList = schProgressPayItemService.GetDateList<EPCSchProgressVModel>(id);
                if (scList.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "預定進度 查無資料"
                    });
                }

                List<SchProgressPayItemModel> cePayItems = new SchEngProgressService().GetList<SchProgressPayItemModel>(id, 9999, 1, "","");
                if (cePayItems.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "前置作業 查無資料"
                    });
                }

                List<SchProgressPayItemModel> spPayItems = schProgressPayItemService.GetList<SchProgressPayItemModel>(id, scList[0].SPDate.ToString("yyyy-MM-dd"));
                if(cePayItems.Count != spPayItems.Count)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "前置作業 與 預定進度 資料項目不一致"
                    });
                }

                for(int i=0; i<cePayItems.Count; i++)
                {
                    SchProgressPayItemModel ceM = cePayItems[i];
                    SchProgressPayItemModel spM = spPayItems[i];

                    /*if(ceM.RefItemCode != spM.RefItemCode || ceM.PayItem != spM.PayItem || ceM.Unit != spM.Unit
                        || ceM.Quantity != spM.Quantity || ceM.Price != spM.Price || ceM.Amount != spM.Amount
                        || ceM.Description != spM.Description)*/
                    if(ceM.Seq != spM.SchEngProgressPayItemSeq) //20230412
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "碳排量計算 與 預定進度 資料內容不一致"
                        });
                    }
                }

                DateTime startDate = new DateTime(eng.AwardDate.Value.Year, eng.AwardDate.Value.Month, 1);
                if (DateTime.Now < startDate)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = String.Format("決標日期 {0} 錯誤", startDate.ToString("yyyy-MM-dd"))
                    });
                }
                
                if (DateTime.Now.Year==startDate.Year && DateTime.Now.Month==startDate.Month)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "目前無符合月份"
                    });
                }
                DateTime nowDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                if (DateTime.Now.Day <10) nowDate = nowDate.AddMonths(-1);
                
                while (startDate <= nowDate)
                {
                    if (maxSupDailyDate[0].ItemDate < startDate) break;
                    dateList.Add(startDate);
                    startDate = startDate.AddMonths(1);
                }

                if (dateList.Count>0 && !iService.initDateList(id, dateList))
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "月份資料初始化失敗"
                    });
                }
                dList = iService.GetDateList<EPCEngPriceAdjVModel>(id);
            }
            else
            {//增加日期清單
                DateTime startDate = new DateTime(dList[dList.Count-1].AdjMonth.Year, dList[dList.Count - 1].AdjMonth.Month, 1);
                startDate = startDate.AddMonths(1);
                if (DateTime.Now > startDate)
                {//是否需要增加月份
                    if (DateTime.Now.Year == startDate.Year && DateTime.Now.Month == startDate.Month)
                    {
                        if (maxSupDailyDate[0].ItemDate >= startDate)
                        {
                            dateList.Add(startDate);
                            if (!iService.AddDateList(id, dateList))
                            {
                                return Json(new
                                {
                                    result = -1,
                                    msg = "月份資料初始化失敗"
                                });
                            }
                        }
                    } else { 
                        DateTime nowDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        if (DateTime.Now.Day < 10) nowDate = nowDate.AddMonths(-1);
                        while (startDate <= nowDate)
                        {
                            if (maxSupDailyDate[0].ItemDate < startDate) break;

                            dateList.Add(startDate);
                            startDate = startDate.AddMonths(1);
                        }
                        if (dateList.Count>0 && !iService.AddDateList(id, dateList))
                        {
                            return Json(new
                            {
                                result = -1,
                                msg = "月份資料初始化失敗"
                            });
                        }
                    }
                    dList = iService.GetDateList<EPCEngPriceAdjVModel>(id);
                }
            }
            //中類指數設定清單
            List<EngPriceAdjLockWorkItemModel> lockWorkItems = iService.GetEngPriceAdjLockWorkItem<EngPriceAdjLockWorkItemModel>(id);
            return Json(new
            {
                result = 0,
                items = dList,
                wItems = lockWorkItems
            });
            /*return Json(new
            {
                result = -1,
                msg = "資料讀取錯誤"
            });*/
        }
        //
        public JsonResult SaveMidKind(List<EngPriceAdjLockWorkItemModel> items)
        {
            if(iService.UpdateEngPriceAdjLockWorkItem(items))
            {
                return Json(new
                {
                    result = 0,
                    msg = "儲存成功"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }        
    }
}