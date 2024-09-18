using EQC.Controllers.InterFaceForFrontEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Services;
using System.Data.Entity;
using EQC.EDMXModel;
using EQC.Services.Common;
using EQC.Common;

namespace EQC.Controllers.Common
{
    public class ExcelImportController<T> : Controller, IExcelImport<T>
    where T :class
    {

        public ExcelImportService<T> iService { get; set; }

        public Func<EQC_NEW_Entities, List<T>> ListGetter { get; set; }
        public string tableName { get; set; }
        public string tablePrimary { get; set; }
        Func<EQC_NEW_Entities, object, T> targetGetter;
        Func<T, object> targetKeyGetter;
        Action<ExcelProcesser> importPreAction;
        public ExcelImportController(
            string _tableName,
            string _tablePrimary,
            string[,] excelHeaderMap,
            Func<EQC_NEW_Entities, object, T> _targetGetter,
            Func<T, object> _targetKeyGetter,

            Func<EQC_NEW_Entities, List<T>> _ListGetter, 
            Func<T, string ,bool> _keyWordComapre,
            Action<ExcelProcesser> _importPreAction
    
        )
        {
            ListGetter = _ListGetter;
            tableName = _tableName;
            targetKeyGetter = _targetKeyGetter;
            targetGetter = _targetGetter;
            tablePrimary = _tablePrimary;
            importPreAction = _importPreAction;
            iService = new ExcelImportService<T>(excelHeaderMap, _keyWordComapre);
        }
        // GET: Ex    celImport
        public JsonResult GetList(int page, int per_page, string keyWord = null)
        {
            
            List<T> list = iService.GetList(ListGetter, page, per_page, keyWord);
            int totalRows = iService.GetListCount(ListGetter, keyWord);
            int rows;
            return Json(new
            {
                l = list,
                t = totalRows
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult excelUpload()
        {
            try
            {
                var file = Request.Files[0];
                if (file.ContentLength == 0)
                {
                    return Json(new
                    {
                        status = "failed",
                        message = "no Content"
                    }, JsonRequestBehavior.AllowGet);
                };
                ExcelProcesser processer = new ExcelProcesser(file, file.FileName);
                processer.setImportPreAction(importPreAction);
                iService.importExcel(processer, tableName, tablePrimary);

            }
            catch (Exception e)
            {
                return Json(new
                {
                    status = "failed",
                    message = e.Message
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                status = "success"
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult getDemandFields()
        {
            try
            {
                var map = iService.getExcelImportFields();
                return Json(new { status = "success", data = map }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult getLastUpdateTime()
        {
            try
            {
                DateTime time = iService.getLastUpdateTime(tableName);
                return Json(new { status = "success", lastUpdateTime = time }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Update(T m)
        {
            iService.update(m, 
                (context) => targetGetter.Invoke(context, targetKeyGetter.Invoke(m) ) );
            return Json(true);
        }

        public JsonResult Add(T m)
        {
            iService.add(m);
            return Json(true);
        }
        public JsonResult Delete(object id)
        {
            iService.delete((context) => targetGetter.Invoke(context, id));
            return Json(true);
        }


    }
}