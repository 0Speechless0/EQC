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
    public class EQMConstRiskEvalController : Controller
    {//工程品管 - 設計階段施工風險評估
        ConstRiskEvalService iService = new ConstRiskEvalService();
        public ActionResult Index()
        {
            Utils.setUserClass(this, 2);
            return View("Index");
        }
        //更新資料
        public JsonResult UpdateRecord(EQMConstRiskEvalVModel m)
        {
            List<EngMainEditVModel> engs = new EngMainService().GetItemBySeq<EngMainEditVModel>(m.EngMainSeq);
            if (engs.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            string folder = Utils.GetConstRiskEvalFolder(m.EngMainSeq);
            int fCount = Request.Files.Count;
            if (fCount > 0 && Request.Files[0].ContentLength > 0)
            {
                try
                {
                    var file = Request.Files[0];
                    m.FileName = Guid.NewGuid().ToString("B").ToUpper() + file.FileName;
                    string fullPath = Path.Combine(folder, m.FileName);
                    file.SaveAs(fullPath);
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "上傳檔案失敗"
                    });
                }
            }
            else
                m.FileName = "";
            int state = -1;
            if (m.Seq == -1)
                state = iService.Add(m);
            else
            {
                List<ConstRiskEvalModel> items = iService.GetItem<ConstRiskEvalModel>(m.Seq);
                if (items.Count != 1)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "資料錯誤"
                    });
                }
                state = iService.Update(m);
                if (fCount > 0 && state > 0) System.IO.File.Delete(Path.Combine(folder, items[0].FileName));
            }

            if (state == -1)
            {
                return Json(new
                {
                    result = -1,
                    message = "儲存失敗"
                });
            }
            return Json(new
            {
                result = 0,
                message = "儲存完成"
            });
        }
        //刪除資料
        public JsonResult DelRecord(int id)
        {
            List<ConstRiskEvalModel> items = iService.GetItem<ConstRiskEvalModel>(id);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                });
            }
            if (iService.Del(id) > 0)
            {
                ConstRiskEvalModel m = items[0];
                if (!String.IsNullOrEmpty(m.FileName)) System.IO.File.Delete(Path.Combine(Utils.GetConstRiskEvalFolder(m.EngMainSeq), m.FileName));
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
        public JsonResult GetList(int id)
        {
            List<EQMConstRiskEvalVModel> ceList = iService.GetList<EQMConstRiskEvalVModel>(id);
            return Json(new
            {
                result = 0,
                items = ceList
            });
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
            else
            {
                return Json(new
                {
                    result = 2,
                    msg = "讀取資料錯誤"
                });
            }
        }
        //
        public ActionResult Download(int id)
        {
            List<ConstRiskEvalModel> items = iService.GetItem<ConstRiskEvalModel>(id);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            ConstRiskEvalModel m = items[0];
            string fName = Path.Combine(Utils.GetConstRiskEvalFolder(m.EngMainSeq), m.FileName);
            if (!System.IO.File.Exists(fName))
            {
                return Json(new
                {
                    result = -1,
                    message = "未發現檔案"
                }, JsonRequestBehavior.AllowGet);
            }

            Stream iStream = new FileStream(fName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", m.FileName.Substring(38));
        }
    }
}