using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EPCSchEngProgressSubController : Controller
    {//預定進度 - 前置作業 - 分項工程
        SchEngProgressSubService iService = new SchEngProgressSubService();

        //填報完成
        public JsonResult FillCompleted(int id, int eId)
        {
            List<EPCSchEngProgressSubVModel> ceList = iService.GetList<EPCSchEngProgressSubVModel>(eId);
            if(ceList.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "沒有分項工程"
                });
            } else
            {
                List<SchEngProgressPayItemModel> payItems = iService.GetPayItemList<SchEngProgressPayItemModel>(eId, 9999, 1, "", "");
                if (payItems.Count > 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "所有碳排項目全部必須勾稽"
                    });
                }
                int weights = 0;
                foreach(EPCSchEngProgressSubVModel m in ceList)
                {
                    if(m.Amount == 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "分項工程金額為0, 請修正"
                        });
                    }
                    weights += m.Weights;
                }
                /* 20230623 weights改系統自算 取消檢查
                if (weights != 100)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "權重總和必須為 100, 請修正"
                    });
                }*/
            }

            if (iService.FillCompleted(id) > 0)
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

        //分項工程 PayItem清單
        public JsonResult GetSubPayItemList(int id)
        {
            List<SchEngProgressPayItemModel> ceList = iService.GetSubPayItemList<SchEngProgressPayItemModel>(id);
            return Json(new
            {
                result = 0,
                items = ceList
            });
        }
        //分項工程 PayItem 刪除
        public JsonResult DelSubPayItem(int id, int pId)
        {
            if (iService.DelSubPayItem(id, pId))
            {
                return Json(new
                {
                    result = 0,
                    msg = "移除成功"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "移除失敗"
                });
            }
        }
        //分項工程 PayItem 加入
        public JsonResult AddSubPayItem(int id, int pId)
        {
            if (iService.AddSubPayItem(id, pId))
            {
                return Json(new
                {
                    result = 0,
                    msg = "加入成功"
                });
            } else
            {
                return Json(new
                {
                    result = -1,
                    msg = "加入失敗"
                });
            }
        }

        //分項工程清單
        public JsonResult GetList(int id)
        {
            List<EPCSchEngProgressSubVModel> ceList = iService.GetList<EPCSchEngProgressSubVModel>(id);
            return Json(new
            {
                result = 0,
                items = ceList
            });
        }
        //分項工程 新增
        public JsonResult AddSunEng(int id)
        {
            if (iService.AddSubEng(id)) {
                return Json(new
                {
                    result = 0,
                    msg = "新增分項工程成功, 請編修資料"
                });
            }else {
                return Json(new
                {
                    result = -1,
                    msg = "新增分項工程失敗"
                });
            }
        }
        //分項工程 刪除
        public JsonResult DelRecord(int id)
        {
            if (iService.DelRecord(id))
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
        //更新資料
        public JsonResult UpdateRecord(SchEngProgressSubModel m)
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
        public JsonResult GetPayItemList(int id, int perPage, int pageIndex, string fLevel, string keyword)
        {
            int total = iService.GetListTotal(id, fLevel, keyword);
            List<SchEngProgressPayItemModel> ceList = iService.GetPayItemList<SchEngProgressPayItemModel>(id, perPage, pageIndex, fLevel, keyword);

            return Json(new
            {
                result = 0,
                totalRows = total,
                items = ceList
            });
        }
    }
}