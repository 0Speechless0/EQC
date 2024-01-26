using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Services;
using EQC.Models;
using EQC.ViewModel;
using EQC.Common;

namespace EQC.Controllers
{
    [SessionFilter]
    public class SubTypeController : Controller
    {
        private CityService cityService = new CityService();
        private SubTypeService subTypeService = new SubTypeService();
        // GET: SubType
        public ActionResult Index()
        {
            return View();
        }

        // GET: SubType/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SubType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SubType/Create
        [HttpPost]
        public JsonResult Create(FormCollection collection)
        {

            if (int.Parse(collection["_orderNo"]) == 0)
            {
                collection["_orderNo"] = (subTypeService.GetMaxOrderNo(int.Parse(collection["_maintypeSeq"])) + 1).ToString();
            }
            int result = subTypeService.AddSubType(collection);
            return Json(new
            {
                result = result
            }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index");

        }

        // GET: SubType/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SubType/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SubType/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: SubType/Delete/5
        [HttpPost]
        public ActionResult Delete(VSubType item)
        {
            try
            {
                int result = subTypeService.Delete(item);

                return Json(new
                {
                    result = result
                }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        //public JsonResult GetList(int page, int per_page, string sort_by, int citySeq, int mainTypeSeq, string subTypeName)
        public JsonResult GetList(int page, int per_page, string sort_by, FormCollection collection)
        {
            string subTypeName = collection["subTypeName"];
            if (subTypeName == null)
                subTypeName = string.Empty;
            //List<VSubType> list = subTypeService.GetList(page - 1, per_page, sort_by, citySeq, mainTypeSeq, subTypeName);
            List<VSubType> list = subTypeService.GetList(page - 1, per_page, sort_by, collection);
            //Object totalRows = subTypeService.GetCount(citySeq, mainTypeSeq, subTypeName);
            Object totalRows = subTypeService.GetCount(collection);
            int rows;
            if (totalRows == null)
            {
                rows = 0;
            }
            else
            {
                rows = (int)totalRows;
            }
            return Json(new
            {
                l = list,
                t = rows,
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEnabledSubType(FormCollection collection)
        {
            int maintypeSeq;
            if (collection["_maintypeSeq"] == null || collection["_maintypeSeq"] == "")
            {
                maintypeSeq = 0;
            }
            else
            {
                maintypeSeq = Convert.ToInt32(collection["_maintypeSeq"]);
            }
            List<VSubType> list = subTypeService.GetEnabledSubType(maintypeSeq);
            var subtype = from m in list.AsEnumerable().ToList()
                          select new
                          {
                              value = m.SubTypeSeq,
                              text = m.SubTypeName
                          };
            return Json(new
            {
                subtype = subtype
            }, JsonRequestBehavior.AllowGet);

        }
    }
}
