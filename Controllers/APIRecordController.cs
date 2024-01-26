using EQC.EDMXModel;
using EQC.ViewModel.Common;
using EQC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
        [SessionFilter]
    public class APIRecordController : MyController
    {
        public ActionResult Index()
        {
            return View();
        }
        public void GetAPIRecordAndOption(
            int systemTypeSeq = 0, 
            int currentPage = 1,
            int perPage = 0,
            string controllerName = null, 
            bool? error = null, 
            bool? api = null,
            bool? unusual = null,
            bool optionOnly =false,
            bool? hasChange = null,
            string userKeyWord = null
        )
        {
            using (var context = new EQC_NEW_Entities(false))
            {
      
                List<APIRecord> list = new List<APIRecord>();

                var apiOptions = context.APIRecord
                    .Where(r => r.UserMainSeq == -1)
                    .GroupBy(r => r.ControllerName)
                    .Select(r => new SelectVM { 
                        Text = r.Key,
                        Value= r.Key
                    }).ToList();



                list = 
                    ( api.Value ? context.APIRecord.ToList() :
                        context.APIRecord.Include("UserMain").ToList()
                    )
                    .Where(r =>
                        (r.ControllerName == controllerName || controllerName == null)
                        &&
                        (
                            userKeyWord == null ||
                            r.UserMain == null ||
                            r.UserMain.DisplayName.StartsWith(userKeyWord) ||
                            r.UserMain.UserNo.StartsWith(userKeyWord) 
                       
                        )
                        &&
                        (hasChange == null || (r.ChangeText != null && hasChange.Value) || (r.ChangeText == null && !hasChange.Value)) &&
                        (error == null || (r.ErrorCode != null && error.Value) || (r.ErrorCode == null && !error.Value)) &&
                        (api == null || (r.UserMainSeq == -1 && api.Value) || (r.UserMainSeq != -1 && !api.Value)) &&
                        (unusual == null || (r.UserMainSeq == 0 && unusual.Value) || (r.UserMainSeq != 0 && !unusual.Value))
                    )
                    .OrderByDescending(r => r.CreateTime)
                    .ToList();

                var extendedOptions = 
                    list
                    .GroupJoin(
                        context.Menu,
                        r1 => r1.ControllerName,
                        r2 => r2.PathName.Split('/')[0] + "/",
                        (r1, r2) => {
                            r1.MenuName = r2.FirstOrDefault()?.Name;
                            return r1;
                        }
                    )
                    .Where(r => r.MenuName == null)
                    .GroupBy(r => r.ControllerName)
                    .Select(r => new SelectVM()
                    {
                        Text = r.Key,
                        Value = r.Key
                    });

                ResponseJson(new
                {
                    list = optionOnly ? new List<APIRecord>() : list
                        .getPagination(currentPage, perPage),

                    apiOptions = apiOptions,

                    controllerOption = 
                        context
                        .Menu.Where(r =>  r.SystemTypeSeq == systemTypeSeq && r.ParentSeq != 0)
                    .ToList()
                    .Select(r => new SelectVM()
                    {
                        Text = r.Name,
                        Value = r.PathName.Split('/')[0] + "/"
                    })

                    .GroupBy(r => r.Value)
                    .Select(r => r.First() ),
                    extendedOptions = extendedOptions,
                    count = list.Count()
                }, "yyyy-MM-dd HH:mm:ss.fff");
            }
        }
    }
}