using EQC.Models;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Common;
using EQC.EDMXModel;
using System.IO;
using Newtonsoft.Json;
using EQC.ViewModel;

namespace EQC.Controllers
{


    [APITokenFilter]
    public class MobileAPIController : MyController
    {
        private APIService apiService = new APIService();
        private ConstCheckRecService constCheckRecService = new ConstCheckRecService();
        private TechnicalService service = new TechnicalService();
        private EngMainService engService =  new  EngMainService();

        private ConstCheckRecImproveService constCheckRecImproveService = new ConstCheckRecImproveService();
        // GET: MobileAPI


        public void RegisterLock()
        {

        }

        public JsonResult GetArticals(int count = 10)
        {
            try
            {
                var articals = service.getAllTechnicalArtical();
                var result = articals.OrderByDescending(row => row.ModifyTimeStr ).ToList().GetRange(0, count);
                return Json(new { status = "success", data = result, status_code = 0});
            }
            catch (Exception e)
            {

                return Json(new { status = "failed", status_code = 1 });
            }
        }
        public JsonResult GetArticalData(int technicalArticalSeq)
        {
            try
            {
                List<object> data = service.getArticalCommentWithReply(technicalArticalSeq);
                //return Json(new { status = "success", data = data, isAdmin = new SessionManager().GetUser().IsAdmin }, JsonRequestBehavior.AllowGet);
                return Json(new { status = "success", data = data, status_code = 0});
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" , status_code = 1});
            }
        }
        public JsonResult SignSuper(int recSeq, string userNo)
        {
            try
            {
                if(Utils.CheckDirectorOfSupervision(userNo))
                {
                    var moblieService = new MobileAPIService();
                    moblieService.SignSuper(recSeq, userNo);

                    return Json(new { status = "success", status_code = 0 });

                }


                return Json(new {  message="你不是監造主任", status = "success", status_code = 0 });
            }
            catch (Exception e)
            {

                return Json(new { status = "failed", status_code = 1 });
            }
        }
        public JsonResult GetEngRecSuperUnSign(string engNo)
        {
            try
            {
                var moblieService = new MobileAPIService();
                var recList = moblieService.GetEngRecSuperUnSigin(engNo);

                return Json(new { data = recList, status = "success", status_code = 0 });
            }
            catch (Exception e)
            {

                return Json(new { status = "failed", status_code = 1 });
            }
        }
        public JsonResult GetEngList(string userNo, int engYear, string str = null, bool? hasPackage = null, bool? withDocument = null)
        {
            try
            {
                var mService = new MobileAPIService();
                var moblieService = new MobileAPIService();
                var engList = moblieService.GetEngListByUser(userNo, engYear, str);

                using (var context = new EQC_NEW_Entities())
                {
                    var userSeq = context.UserMain.Where(r => r.UserNo == userNo).FirstOrDefault()?.Seq;
                    var a = context.ConstCheckRec.Except(
                        context.ConstCheckRec
                        .Join(context.constCheckSignatures.Where(r => r.SignatureRole == 3), r1 => r1.Seq, r2 => r2.ConstCheckSeq, (r1, r2) => r1)
                    ).Join(context.EngConstruction, r1 => r1.EngConstructionSeq, r2 => r2.Seq, (r1, r2) => r2)
                    .Join(context.EngMain, r1 => r1.EngMainSeq, r2 => r2.Seq, (r1, r2) => r2)
                    .Where(r =>
                        (
                        r.EngSupervisor.Where(rr => rr.UserMainSeq == userSeq && rr.UserKind == 0).Count() > 0
                        && r.SupervisorExecType == 1)
                        || ( "S" + r.EngNo == userNo && r.SupervisorExecType == 2)
                    )
                    .GroupBy(r => r.Seq)
                    .Select(r => r.FirstOrDefault())
                    .ToList()
                    .Join(engList, r1 => r1.Seq, r2 => r2.Seq, (r1, r2) => r2).ToList();

                    engList = engList.GroupJoin(a,
                        r1 => r1.Seq,
                        r2 => r2.Seq,
                        (r1, r2) =>
                        {
                            r1.recSignNeeded = r2.Count() > 0; return r1;
                        }).ToList();

                    if (hasPackage != null && !hasPackage.Value)
                    {
                        engList = engList.Join(context.ToolPackage, r1 => r1.Seq, r2 => r2.EngSeq, (r1, r2) => r1).ToList();
                    }
                    if (withDocument != null && withDocument.Value)
                    {
                        engList = engList.Where(r => $"MobileConstCheckDocuments1/{r.Seq}"
                            .GetFiles().Length > 0).ToList();
                    }

                    engList = engList.GroupJoin(context.ConstCheckUser.Where(r => r.UserSeq == userSeq), r1 => r1.Seq, r2 => r2.EngSeq, (r1, r2) =>
                    {

                        r1.IsConstCheckOwn = r2.Count() > 0;
                        return r1;
                    }).ToList();
         
                }





                return Json(new { data = engList, status = "success", status_code = 0});
            }
            catch(Exception e)
            {

                return Json(new {  status = "failed" , status_code = 1});
            }
        }
        private void JoinCell(List<ControlStVModel> items)
        {
            if (items.Count == 0) return;

            string checkItem = "----";
            string checkItemStd1 = "----";
            int gInx = -1, gCount = 0;
            int gInxStd1 = -1, gCountStd1 = 0;
            for (int i = 0; i < items.Count; i++)
            {
                string key = items[i].CheckItem1 + items[i].CheckItem2;
                string keyStd1 = items[i].Stand1;
                if (checkItem != key)
                {
                    if (gInx != -1)
                    {
                        items[gInx].rowSpan = gCount;
                    }
                    gInx = i;
                    gCount = 1;
                    checkItem = key;

                    if (gInxStd1 != -1)
                    {
                        items[gInxStd1].rowSpanStd1 = gCountStd1;
                    }
                    gInxStd1 = i;
                    gCountStd1 = 1;
                    checkItemStd1 = keyStd1;
                }
                else
                {
                    items[i].rowSpan = 0;
                    gCount++;

                    if (checkItemStd1 != keyStd1)
                    {
                        if (gInxStd1 != -1)
                        {
                            items[gInxStd1].rowSpanStd1 = gCountStd1;
                        }
                        gInxStd1 = i;
                        gCountStd1 = 1;
                        checkItemStd1 = keyStd1;
                    }
                    else
                    {
                        items[i].rowSpanStd1 = 0;
                        gCountStd1++;
                    }
                }
            }
            items[gInx].rowSpan = gCount;
            items[gInxStd1].rowSpanStd1 = gCountStd1;
        }
        public JsonResult GetRecResult(int recSeq, int checkTypeSeq)
        {
            try
            {
                List<ConstCheckRecImproveVModel> reports = constCheckRecImproveService.GetItemByConstCheckRecSeq<ConstCheckRecImproveVModel>(recSeq);
                if (reports.Count == 0)
                    reports.Add(new ConstCheckRecImproveVModel() { ConstCheckRecSeq = recSeq });

                ConstCheckRecResultService service = new ConstCheckRecResultService();
                List<ControlStVModel> items = new List<ControlStVModel>();
                if (checkTypeSeq == 1)
                {//施工抽查清單
                    items = service.GetConstCheckRecResult<ControlStVModel>(recSeq);
                }
                else if (checkTypeSeq == 2)
                {//設備運轉測試清單
                    items = service.GetEquOperRecResult<ControlStVModel>(recSeq);
                }
                else if (checkTypeSeq == 3)
                {//職業安全衛生清單
                    items = service.GetOccuSafeHealthRecResult<ControlStVModel>(recSeq);
                }
                else
                {//環境保育清單
                    items = service.GetEnvirConsRecResult<ControlStVModel>(recSeq);
                }
                if (items.Count > 0)
                {
                    JoinCell(items);
                    List<object> resultItems = items
                    .Where(row => row.FormConfirm == 0 )
                    .Select(row => new
                    {
                        CheckItem = row.CheckItem1.Trim(),
                        Stand = row.Stand1.Trim()

                    })
                    .ToList<object>();

                    return Json(new
                    {
                        data = resultItems,
                        status = "success",
                        status_code = 0
                    });
                }

                return Json(new { status = "success", status_code = 0 });
            }
            catch (Exception e)
            {
                return Json(new { status = "failed", status_code = 1 });
            }
        }
        public JsonResult GetRecCheckTypeOption(int constructionSeq)
        {
            try
            {
                List<SelectIntOptionModel> items = constCheckRecService.GetRecCheckTypeOption<SelectIntOptionModel>(constructionSeq);
                List<string> options = new List<string>();
                foreach (SelectIntOptionModel item in items)
                {
                    string text;
                    switch (item.Value)
                    {
                        case 1: text = "施工抽查"; break;
                        case 2: text = "設備運轉測試"; break;
                        case 3: text = "職業安全"; break;
                        default: text = "生態保育"; break;
                    }
                    options.Add(text);
                }
                return Json(new { data = options, status = "success", status_code= 0
                });
            }
            catch( Exception e)
            {
                return Json(new { status = "failed", status_code=1 });
            }

        }
        public JsonResult GetRecOption(int constructionSeq, int checkTypeSeq)
        {
            try
            {
                List<string> items = constCheckRecService.GetRecOption(constructionSeq, checkTypeSeq);
                Dictionary<string, int> recSeqMap = constCheckRecService.GetRecSeqMap(constructionSeq, checkTypeSeq);

                return Json(new { data = items, recStrSeqMap  = recSeqMap, status = "success", status_code = 0 });
            }
            catch(Exception e)
            {
                return Json(new { status = "failed" , status_code = 1});
            }
        }
        //分項工程清單
        public  JsonResult GetSubEngList(string engNo)
        {
            try
            {
                List<object> subEngList = constCheckRecService.GetEngCreatedList(engNo);

                return Json(new { data = subEngList, status = "success", status_code =  0 });


            }
            catch(Exception e)
            {
                return Json(new {  status = "failed", status_code = 1 });
            }

        }

        public JsonResult GetEngConstructionList(int engSeq)
        {
            try
            {
                EngConstructionService engConstructionService = new EngConstructionService();
                List<EngConstructionVModel> subEngList = engConstructionService.GetListByEngMainSeq<EngConstructionVModel>(engSeq);

                return Json(new { data = subEngList, status = "success", status_code = 0 });


            }
            catch (Exception e)
            {
                return Json(new { status = "failed", status_code = 1 });
            }
        }


        class ChapterControlModel
        {
            public int Seq { get; set; }
            public string ItemName { get; set; }
            public string ControlInfo { get; set; }
        }

        public JsonResult GetChapterControl(int seq, int chapter, int[] stages =null)
        {

            var service = new MobileAPIService();
            try
            {
                var list = service.GetChapterControl(seq, chapter, stages);
                return Json(new
                {
                    list = list,
                    status = "success",
                    status_code = 0
                });
            }
            catch(Exception e)
            {

                return Json(new
                {
                    status = "failed:"+e.Message,
                    status_code = 1

                });
            }



        }
        public JsonResult GetEngChapterItems(string engNo)
        {
            var service = new MobileAPIService();
            try
            {
                var list = service.GetEngChapterItems(engNo);
                return Json(new
                {
                    status_code = 0,
                    status = "success",
                    list = list
                });

            }
            catch(Exception e)
            {
                return Json(new
                {
                    status = "failed",
                    status_code = 1
                });
            }

        }

        public void GetEnvironment()
        {
            using(var context = new EQC_NEW_Entities())
            {
                try
                {
                    ResponseJson(new { 
                        variables = context.EnvironmentVar.Where(var => !var.DeleteTag).ToDictionary(row => row.Key, row => row.Value),
                        status_code = 0,
                        status = "success"

                    }
                    );
                }
                catch(Exception e)
                {
                    ResponseJson(new
                    {
                        status = "failed",
                        status_code = 1
                    });
                }

            }
        }


        public void GetDocument(int engSeq)
        {
            var service = new MobileAPIService();
            try
            {


                var documentLinks = service.GetDocument(engSeq);
                ResponseJson(new
                {
                    Documents = documentLinks,
                    status = "success",

                    status_code = 0
                });
            }
            catch (Exception e)
            {
                ResponseJson(new
                {
                    status = $"failed :{e.Message}",
                    status_code = 1
                });
            }
        }
        
        public void GetToolPackage(int engSeq)
        {
            var service = new MobileAPIService();

            try
            {


                var packageData = service.GetToolPackage(engSeq);
                ResponseJson(new
                {
                    Documents = packageData["document"],
                    Images = packageData["image"],
                    status = "success",

                    status_code = 0
                });
            }
            catch(Exception e)
            {
                ResponseJson(new
                {
                    status = $"failed :{e.Message}",
                    status_code = 1
                });
            }
        }
        public JsonResult GetCheckSignature()
        {
            var service = new MobileAPIService();
            var token = Request.Headers.Get("token");
            try
            {

                var list = service.GetCheckSignature(token);

                return Json(new
                {
                    data = list,
                    status = "success",
                    status_code = 0

                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    status = "failed :" + e.Message,
                    status_code = 1

                });
            }

        }
        public JsonResult GetSignatureOption(string engNo)
        {
            var service = new MobileAPIService();
            var apiService = new APIService();
            var token = Request.Headers.Get("token");
            var tokenData = apiService.GetTokenData(token);
            try
            {

                var signatureOption = service.GetSignatureOption(engNo, tokenData.userNo);

                return Json(new
                {
                    data = signatureOption
                    ,
                    status = "success",
                    status_code = 0

                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    status = "failed :" + e.Message,
                    status_code = 1

                });
            }

        }
        public JsonResult GetCheckSignatureStatus()
        {
            var service = new MobileAPIService();
            var token = Request.Headers.Get("token");
            try
            {

                var list = service.GetCheckSignature(token);

                return Json(new
                {
                    data = new
                    {
                        signature1 = list.Exists(r => r.SignatureRole ==0),
                        signature2 = list.Exists(r => r.SignatureRole == 1),
                        signature3 = list.Exists(r => r.SignatureRole == 2)
                    },
                    status = "success",
                    status_code = 0

                });
            }
            catch(Exception e)
            {
                return Json(new
                {
                    status = "failed :"+e.Message,
                    status_code = 1

                });
            }

        }
        public JsonResult UploadSignature(constCheckSignatures m)
        {
            try
            {
                using (var context = new EQC_NEW_Entities())
                {
                    var findSignature = context.constCheckSignatures.Find(m.Seq);

                    if (findSignature != null)
                    {
                        findSignature.CreateTime = m.CreateTime;
                        m.ModifyTime = DateTime.Now;
                        context.Entry(findSignature)
                            .CurrentValues.SetValues(m);
                    }
                    else
                    {
                        m.CreateTime = DateTime.Now;
                        context.constCheckSignatures.Add(m);
                    }
                    context.SaveChanges();

                }

                return Json(new
                {
                    status_code = 0,
                    status = "success"
                });
            }
            catch(Exception e)
            {
                return Json(new
                {
                    status = "failed :"+e.Message,
                    status_code = 1
                });
            }

        }

        //public JsonResult UploadConstCheckJson(int engSeq)
        //{
        //    try
        //    {
        //        using(var context = new EQC_NEW_Entities())
        //        {
        //            return Json(new
        //            {
        //                status_code = 0,
        //                status = "success"
        //            });
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new
        //        {
        //            status = "failed :" + e.Message,
        //            staus_code = 1

        //        });
        //    }
        //}
        public JsonResult UploadCheckJson(string json)
        {
            var service = new MobileAPIService();
            var token = Request.Headers.Get("token").ToString();
            var a = JsonConvert.DeserializeObject<object>(json);
            try
            {

                service.UploadCheckJson(json, token);


                return Json(new
                {
                    status_code = 0,
                    status = "success"
                });
            }
            catch (Exception e)
            {
                BaseService.log.Info($"{e.Message}: {e.StackTrace}");
                return Json(new
                {
                    status = "failed :" + e.Message,
                    status_code = 1

                });
            }
        }
        public ActionResult DownloadCheckJson(string engNo)
        {
            var service = new MobileAPIService();
            var apiService = new APIService();
            string token = Request.Headers.Get("token");
            var tokenData = apiService.GetTokenData(token);
            EngConstructionService engConstructionService = new EngConstructionService();
            var eng= service.GetEngSeqByEngNo<EngMainEditVModel>(engNo).FirstOrDefault();
            int engSeq = eng?.Seq ?? 0;
            try
            {
                var engMainService = new EngMainService();
                var userList = service.GetSignatureOption(engNo, tokenData.userNo);
    
                
                var engChapterItems = service.GetEngChapterItems(engNo);
                var engConstructionList = engConstructionService.GetListByEngMainSeq<EngConstructionVModel>(engSeq);
                var engChapterItemsControl =
                    engChapterItems
                    .ToDictionary(row => row.Seq, row => service.GetChapterControl(row.Seq, row.chapter));
                //var engChapterItemsSignature =
                //    engChapterItems
                //    .ToDictionary(row => row.Seq, row => {
                //        var list = service.GetCheckSignature(row.Seq);

                //        return new
                //        {
                //            signature1 = list.Exists(r => r.SignatureRole == 0),
                //            signature2 = list.Exists(r => r.SignatureRole == 1),
                //            signature3 = list.Exists(r => r.SignatureRole == 2)
                //        };
                //    });
                //var toolPackage = service.GetToolPackage(engSeq);

                var str = JsonConvert.SerializeObject(
                    new
                    {
                        Eng = new { 
                            EngName = eng.EngName,
                            EngNo = eng.EngNo,
                            Seq = eng.Seq,
                            EngYear = eng.EngYear,
                            ExecUnitName = eng.execUnitName,
                            ExecType  = eng.SupervisorExecType
                        },
                        EngChapterItems = engChapterItems,
                        EngConstrucitonList = engConstructionList,
                        EngChapterItemsControl = engChapterItemsControl,
                        EngUserList = userList,
                        //ToolPackage = new
                        //{
                        //    Image = toolPackage["image"] ?? new List<object>(),
                        //    Document = toolPackage["document"] ?? new List<object>()
                        //},

                        status_code = 0,
                        status = "success"
                    }

                );
                return Content(str.Replace("\r\n", ""));
            }
            catch (Exception e)
            {
                BaseService.log.Info(e.Message+ ":" +e.StackTrace);
                return Json(new
                {
                    status = "failed :" + e.Message,
                    status_code = 1

                });
            }
        }
    }
}