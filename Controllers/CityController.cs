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
    //[SessionFilter]
    public class CityController : Controller
    {
        private CityService cityService = new CityService();
        // GET: City
        public ActionResult Index()
        {
            return View();
        }

        // GET: City/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: City/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: City/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                cityService.AddCity(collection);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: City/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: City/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
       
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: City/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: City/Delete/5
        [HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        public ActionResult Delete(VCity item)
        {
            try
            {
                cityService.Delete(item);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
         public JsonResult GetList(int page, int per_page, string sort_by)
        {
            List<City> list = cityService.GetList(page-1, per_page, sort_by);
            Object totalRows = cityService.GetCount();
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
                t = rows
            }, JsonRequestBehavior.AllowGet);
        }

      [HttpPost]
        public ActionResult Update(VCity item)
        {
            try
            {
                cityService.Update(item);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public JsonResult GetCitySelect()
        {
            List<City> listcity = cityService.GetEnabledCities();
            var cities = from m in listcity.AsEnumerable().ToList()
                         select new
                         {
                             value = m.Seq,
                             text = m.CityName
                         };
            return Json(new
            {
                city = cities
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
