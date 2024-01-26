using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Models;
using EQC.ViewModel;
using EQC.Services;
using EQC.Common;
using EQC.ViewModel.Common;

namespace EQC.Controllers
{
    [SessionFilter]
    public class UnitController : Controller
    {
        private UnitService unitService = new UnitService();
        // GET: Unit
        public ActionResult Index()
        {
            return View();
        }

        // GET: Unit/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Unit/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Unit/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                if (int.Parse(collection["_orderNo"]) == 0)
                {
                    collection["_orderNo"] = (unitService.GetMaxOrderNo() + 1).ToString();
                }
                unitService.AddUnit(collection);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Unit/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Unit/Edit/5
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

        // GET: Unit/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: Unit/Delete/5
        [HttpPost]
        public ActionResult Delete(VUnit item)
        {
            try
            {
                unitService.Delete(item);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public JsonResult GetList(int page, int per_page, string sort_by)
        {
            List<VUnit> list = unitService.GetList(page - 1, per_page, sort_by);
            Object totalRows = unitService.GetCount();
            int rows;
            if (totalRows == null)
            {
                rows = 0;
            }
            else
            {
                rows = (int)totalRows;
            }
            List<VUnit> listunit = unitService.GetEnabledUnit();
            var units = from m in listunit.AsEnumerable().ToList()
                        select new
                        {
                            value = m.Seq,
                            text = m.Name
                        };
            return Json(new
            {
                l = list,
                t = rows,
                unit = units
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(VUnit item)
        {
            try
            {
                unitService.Update(item);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult GetEnabledUnit()
        {
            List<VUnit> list = unitService.GetEnabledUnit();
            var unit = from m in list.AsEnumerable().ToList()
                       select new
                       {
                           value = m.Seq,
                           text = m.Name
                       };
            return Json(new
            {
                unit = unit
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary> 取得單位(下拉選單資料) </summary>
        /// <returns></returns>
        public JsonResult GetUnitList(int? parentSeq)
        {
            List<SelectVM> list = unitService
                .GetUnitList(parentSeq)
                .Where(unit => !unit.Text.Contains("清單"))
                .ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUnitListV2(string[] subUnit)
        {
            List<string> list = unitService.GetUnitList(subUnit, subUnit.Length);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
		
        public JsonResult GetUnitListV3(string[] subUnit)
        {
            List<string> list = unitService.GetUnitList(subUnit, subUnit.Length);
            return Json(list, JsonRequestBehavior.AllowGet);
		}
		
        /// <summary> 取得單位(下拉選單資料)(施工風險) </summary>
        /// <returns></returns>
        public JsonResult GetUnitListForRisk(int id)
        {
            List<SelectVM> list = unitService.GetUnitListForRisk(id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
