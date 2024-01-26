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
    public class EPCEngMaterialAdjController : Controller
    {//工程管理 - 物料調整
        PrjXMLMaterialAdjService iService = new PrjXMLMaterialAdjService();

        public JsonResult GetItem(int id)
        {
            EPCPrjXMLMaterialAdjVModel item;
            List<EPCPrjXMLMaterialAdjVModel> items = iService.GetItemByPrjXMLSeq<EPCPrjXMLMaterialAdjVModel>(id);
            if (items.Count == 0)
            {
                iService.NewItem(id);
                items = iService.GetItemByPrjXMLSeq<EPCPrjXMLMaterialAdjVModel>(id);
                if (items.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "資料讀取錯誤"
                    });
                }
            }
            item = items[0];
            return Json(new
            {
                result = 0,
                item = item
            });
        }

        public JsonResult UpdateItem(PrjXMLMaterialAdjModel item)
        {
            if (iService.Update(item) >0)
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
    }
}