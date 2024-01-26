using EQC.Common;
using EQC.Models;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class DelEngReportController : Controller
    {
        public ActionResult Index()
        {
            //ViewBag.Title = "刪除工程";
            return View("");
        }
        public JsonResult Del(string engNo, int mode)
        {
            EngMainService service = new EngMainService();

            List<EngMainEditVModel> items = service.GetEngSeqByEngNo<EngMainEditVModel>(engNo);
            if (items.Count == 1)
            {
                if (mode == 0 && service.DelEng(items[0].Seq, engNo))
                {
                    return Json(new
                    {
                        result = 0,
                        message = "刪除成功"
                    });
                }
                else if (mode == 1 && service.DelEng1(items[0].Seq, engNo))
                {
                    return Json(new
                    {
                        result = 0,
                        message = "刪除成功"
                    });
                }
                else
                {
                    return Json(new
                    {
                        result = 0,
                        message = "刪除失敗"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    result = 1,
                    message = "查無工程資料:" + engNo
                });
            }
        }
    }
}