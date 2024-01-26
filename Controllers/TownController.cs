using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.ViewModel;
using EQC.Services;
using EQC.Common;

namespace EQC.Controllers
{
    [SessionFilter]
    public class TownController : Controller
    {
        private TownService townService = new TownService();
        // GET: Town
        public ActionResult Index()
        {
            return View();
        }

        // GET: Town/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Town/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Town/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                if (int.Parse(collection["_orderNo"]) == 0)
                {
                    collection["_orderNo"] = (townService.GetMaxOrderNo(int.Parse(collection["_citySeq"])) + 1).ToString();
                }
                townService.AddTown(collection);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Town/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Town/Edit/5
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

        // GET: Town/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: Town/Delete/5
        [HttpPost]
        public ActionResult Delete(VTown item)
        {
            try
            {
                townService.Delete(item);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public JsonResult GetList(int page, int per_page, string sort_by, int citySeq)
        {
            List<VTown> list = townService.GetList(page - 1, per_page, sort_by, citySeq);
            Object totalRows = townService.GetCount(citySeq);
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

        public JsonResult GetEnabledTown(FormCollection collection)
        {
            int citySeq;
            if (collection["_citySeq"] == null)
            {
                citySeq = 0;
            }
            else
            {
                citySeq = Convert.ToInt32(collection["_citySeq"]);
            }
            List<VTown> list = townService.GetEnabledTown(citySeq);
            var town = from m in list.AsEnumerable().ToList()
                       select new
                       {
                           value = m.Seq,
                           text = m.TownName
                       };
            return Json(new
            {
                town = town
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
