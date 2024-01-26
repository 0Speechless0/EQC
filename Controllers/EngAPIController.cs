using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Common;
using EQC.Services;

namespace EQC.Controllers
{
    [APIUserFilter]
    public class EngAPIController : MyController
    {
        // GET: EngAPI

        EngOpenAPIService service = new EngOpenAPIService();
        public JsonResult GetEngData(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var list = service.GetEngDataForOpenNetWork(startDate, endDate);
                return Json(new
                {
                    data = list,
                    status_code = 0
                });
            }
            catch(Exception e)
            {
                return Json(new
                {
                    status_code = 1,
                    message = "failed"
                });     
            }
        }

        public void GetGISData()
        {
            try
            {
                var list = service.GetGIS();
                ResponseJson(new
                {
                    data = list,
                    status_code = 0
                });
            }
            catch (Exception e)
            {
                ResponseJson(new
                {
                    status_code = 1,
                    message = "failed"
                });
            }
        }

        public JsonResult GetPassword(string[] permissoinList)
        {
            try
            {
                string _permissoinList = permissoinList.Aggregate("", (a, c) => $"{a}{c},");
                _permissoinList = _permissoinList.Remove(_permissoinList.Length - 1, 1);
                var code = new Encryption().encryptCode(_permissoinList + ":"+ DateTime.Now.ToOADate() );
                return Json(new
                {
                    status_code = 0,
                    password = code
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    status_code = 1,
                    message = "failed :" + e.Message
                });
            }
        }


    }
}