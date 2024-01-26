using EQC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Models;
using System.Data;
using EQC.ViewModel;
using System.Web.UI;
using EQC.Common;

namespace EQC.Controllers
{
    //[SessionFilter]
    public class MainTypeController : Controller
    {
        private MainTypeService mainTypeService = new MainTypeService();
        private CityService cityService = new CityService();

        // GET: MainType
        public ActionResult Index()
        {
            return View();
        }

        // GET: MainType/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MainType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MainType/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                if (int.Parse(collection["_orderNo"]) == 0)
                {
                    collection["_orderNo"] = (mainTypeService.GetMaxOrderNo(collection["_citySeq"]) + 1).ToString();
                }
                int result = mainTypeService.AddMainType(collection);
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

        // GET: MainType/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MainType/Edit/5
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

        // GET: MainType/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: MainType/Delete/5
        [HttpPost]
        public ActionResult Delete(VMainType item)
        {
            try
            {
                int result = mainTypeService.Delete(item);
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

        
        public JsonResult GetList(int page, int per_page, string sort_by, FormCollection collection)
        {
            string mainTypeName = collection["mainTypeName"];
            if (mainTypeName == null)
                mainTypeName = string.Empty;
            List<VMainType> list = mainTypeService.GetList(page - 1, per_page, sort_by, collection);
            Object totalRows = mainTypeService.GetCount(collection);
            int rows;
            if (totalRows == null)
            {
                rows = 0;
            }
            else
            {
                rows = (int)totalRows;
            }
            //List<City> listcity = cityService.GetEnabledCities();
            //var cities = from m in listcity.AsEnumerable().ToList()
            //             select new
            //             {
            //                 value = m.Seq,
            //                 text = m.CityName
            //             };
            return Json(new
            {
                l = list,
                t = rows,
                //city = cities
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Update(VMainType item)
        {
            try
            {
                mainTypeService.Update(item);
                return RedirectToAction("Index");
            }
            catch 
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult GetEnabledMainType(int citySeq)
        {
            List<MainType> list = mainTypeService.GetEnabledMainType(citySeq);
            var mainType = from m in list.AsEnumerable().ToList()
                           select new
                           {
                               value = m.Seq,
                               text = m.MainTypeName
                           };
            return Json(new
            {
                maintype = mainType
            }, JsonRequestBehavior.AllowGet);

        }
    }
}
