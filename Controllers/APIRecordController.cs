using EQC.EDMXModel;
using EQC.ViewModel.Common;
using EQC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Detection;

namespace EQC.Controllers
{
    [SessionFilterBase]
    public class APIRecordController : MyController
    {
        public ActionResult Index()
        {
            return View();
        }


        public void captureHTML(string HTMLEncode, bool reset = false)
        {
            var userSeq = Utils.getUserSeq();
            Session["captureHTMLEncode"] = HTMLEncode;
            if (APIDetection.FinishHtmlCapture(userSeq, Session) )
            {
                ResponseJson(true);
            }
            else
            {
                ResponseJson(false);
            }

            //if(Request.Cookies.Get("recordId") != null)
            //{
            //    APIDetection.FinishHtmlCapture(Request.Cookies.Get("recordId").ToString(), Session);
            //    Response.Cookies.Remove("recordId");
            //}
            //else
            //{
            //    var recordId = APIDetection.StartHtmlCapture();
            //    Response.Cookies.Add(new HttpCookie("recordId") { Value = recordId });
            //}

        }

        public ActionResult ChangeHTML(int type, int recordId)
        {
            ViewBag.ChangeHTMLType = type;
            using(var context = new EQC_NEW_Entities())
            {
                var record =
                    context.APIRecord.Find(recordId);
                var fileName = record.CreateTime?.ToString($"yyyy-MM-dd-HH-mm-ss") ;
                var dirName = record.CreateTime?.ToString($"yyyy-MM-dd");
                if (type == 1)
                {
                    ViewBag.HTML =
                            UploadFilesProcesser.ReadTxTFromFile($"HtmlCapturedContent/{dirName}/{record.UserMainSeq}",fileName + ".txt")  ;
                    return View("ChangeHTML");
                }
                else
                {
                    ViewBag.HTML = 
                        UploadFilesProcesser.ReadTxTFromFile($"HtmlCapturedContent/{dirName}/{record.UserMainSeq}", "org_" + fileName + ".txt");
                    return View("ChangeHTML");
                }
            }

        }

        public void GetExtendedOptions()
        {
            using (var context = new EQC_NEW_Entities())
            {
                var extendedOptions =
                        context.APIRecord
                        .GroupBy(r => r.ControllerName)
                        .Select(r => r.Key)
                        .ToList()
                        .Except(context.Menu.ToList().Select(r => r.PathName.Split('/')[0] + "/"))
                        .Where(r => r != null)
                        .Select(r => new SelectVM()
                        {
                            Text = r,
                            Value = r
                        }).ToList();
                ResponseJson(new
                {
                    extendedOptions = extendedOptions
                });
            }
 
        }
        public void GetAPIRecordAndOption(
            DateTime startDate,
            int[] systemTypeSeqs , 
            int currentPage = 1,
            int perPage = 0,

            string[] controllerNames = null, 
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
                List<SelectVM> apiOptions = new List<SelectVM>();
                HashSet<string> _controllerNames = new HashSet<string>(controllerNames ?? new string[0]);
                HashSet<int> _systemTypeSeqs = new HashSet<int>(systemTypeSeqs ?? new int[0]);
                if (optionOnly)
                {
                    apiOptions = context.APIRecord
                        .Where(r => r.UserMainSeq == -1)
                        .GroupBy(r => r.ControllerName)
                        .Select(r => new SelectVM
                        {
                            Text = r.Key,
                            Value = r.Key
                        }).ToList();
                }
                else
                {
                    var endDate = startDate.AddDays(1);
                    var menuDic = context
                        .Menu
                        .ToList()
                        .GroupBy(r => r.PathName.Split('/')[0] + "/")
                         .ToDictionary(
                        r => r.Key, r => r.FirstOrDefault() );
                    list =
                        (api.Value ? context.APIRecord :
                            context.APIRecord.Include("UserMain")
                        )
                        .Where(r =>
                            r.CreateTime > startDate && r.CreateTime <  endDate 

                            &&
                            (hasChange == null || (r.ChangeText != null && hasChange.Value) || (r.ChangeText == null && !hasChange.Value)) &&
                            (error == null || (r.ErrorCode != null && error.Value) || (r.ErrorCode == null && !error.Value)) &&
                            (api == null || (r.UserMainSeq == -1 && api.Value) || (r.UserMainSeq != -1 && !api.Value)) &&
                            (unusual == null || (r.UserMainSeq == 0 && unusual.Value) || (r.UserMainSeq != 0 && !unusual.Value))
                        )
                        .ToList()
                        .Where(r =>
                                userKeyWord == null ||
                                r.UserMain == null ||
                                r.UserMain.DisplayName.StartsWith(userKeyWord) ||
                                r.UserMain.UserNo.StartsWith(userKeyWord)

                            )

                        .Where(r => r.ExecSec > 0)
                        .OrderByDescending(r => r.CreateTime)

                        .Where(r => (_controllerNames.Contains(r.ControllerName) || _controllerNames.Count == 0 ))
                        .Select(r =>
                        {
                            if (menuDic.ContainsKey(r.ControllerName))
                            {
                                r.Menu = menuDic[r.ControllerName];
                                r.MenuName = menuDic[r.ControllerName].Name ;
                            }
                               
      
                            return r;
                        })
                        .Where(r => _systemTypeSeqs.Contains(r.Menu?.SystemTypeSeq ?? 0) 
                        )

                        .ToList();
                    list.ForEach(r1 =>
                    {
                        r1.UserMain.APIRecord = null;

                    });
                }





                ResponseJson(new
                {
                    list = optionOnly ? new List<APIRecord>() : list
                        .getPagination(currentPage, perPage),

                    apiOptions = apiOptions,

                    controllerOption = 
                        context
                        .Menu.Where(r => _systemTypeSeqs.Contains(r.SystemTypeSeq ?? 0) && r.ParentSeq != 0)
                    .ToList()
                    .Select(r => new SelectVM()
                    {
                        Text = r.Name,
                        Value = r.PathName.Split('/')[0] + "/"
                    })

                    .GroupBy(r => r.Value)
                    .Select(r => r.First() ),
                    count = list.Count()
                }, "yyyy-MM-dd HH:mm:ss");
            }
        }
    }

}