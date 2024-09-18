using EQC.Common;
using EQC.EDMXModel;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace EQC.Detection
{
    public static class APIDetection
    {
        private static Dictionary<int, APIRecord> threadActions = new Dictionary<int, APIRecord>(30000);


        private static Dictionary<int, APIRecord> htmlReadyCapture = new Dictionary<int, APIRecord>(30000);


        public static void StartAction(int tid, APIRecord apiRecord)
        {
            try
            {
                if (!threadActions.ContainsKey(tid))
                    threadActions.Add(tid, apiRecord);
            }
            catch
            {
                BaseService.log.Info($@"軌跡記錄失敗 => 時間 :{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }, 動作: {apiRecord.ActionName}");
            }

        }
        public static bool FinishHtmlCapture(int captureId, HttpSessionStateBase Session)
        {
            using (var context = new EQC_NEW_Entities())
            {


                if (htmlReadyCapture.ContainsKey(captureId) && htmlReadyCapture[captureId] != null)
                {
                    var record = context.APIRecord.Find(htmlReadyCapture[captureId]?.Seq);
 
                    if (record != null && record.ChangeText != null)
                    {
                        var fileName = record.CreateTime?.ToString($"yyyy-MM-dd-HH-mm-ss");
                        var dirName = record.CreateTime?.ToString($"yyyy-MM-dd") ;
                        Session["captureHTMLEncode"]?.ToString().SaveToTxTFile($"HtmlCapturedContent/{dirName}/{record.UserMainSeq}", fileName + ".txt");
                        Session["captureHTMLEncodeLast"]?.ToString().SaveToTxTFile($"HtmlCapturedContent/{dirName}/{record.UserMainSeq}", "org_" + fileName + ".txt");
                        htmlReadyCapture.Remove(captureId);
                        return true;
                    }

                }


            }
            return false;
        }



        public static void StartHtmlCaptureIfNeeded(int captureId)
        {
            if (!htmlReadyCapture.ContainsKey(captureId))
            {
                htmlReadyCapture.Add(captureId, null);
            }
            else
            {
                htmlReadyCapture[captureId] = null;
            }



            

        }
        public static void FinishAction(int tid, int captureId = 0)
        {
            using (var context = new EQC_NEW_Entities())
            {
                var record = GetActionRecord(tid);


                if(record.ChangeText != null)
                {
                    record.EndingTime = DateTime.Now;
                    context.APIRecord.Add(record);
                    StartHtmlCaptureIfNeeded(captureId);
                    htmlReadyCapture[captureId] = record;
                }

                threadActions.Remove(tid);
                context.SaveChanges();
            }
            
        }

        public static APIRecord GetActionRecord(int tid)
        {
            if (threadActions.ContainsKey(tid))
                return threadActions[tid];
            return new APIRecord();
        }

        public static void SetError(int tid, string errorCode)
        {
            using(var context = new EQC_NEW_Entities())
            {
                GetActionRecord(tid).ErrorCode = errorCode;
                context.SaveChanges();
                FinishAction(tid);
            }
        }
        public static void Record(this ActionDescriptor ActionDescriptor, UserInfo userInfo)
        {

            string controllerName = ActionDescriptor.ControllerDescriptor.ControllerName + "/";
            string action = ActionDescriptor.ActionName;
            using (var context = new EQC_NEW_Entities())
            {


                var newRecord = new APIRecord
                {
                    ActionName = action,
                    ControllerName = controllerName,
                    Origin = HttpContext.Current.Request.UserHostAddress,
                    CreateTime = DateTime.Now,
                    UserMainSeq = userInfo?.Seq ?? 0,
                    ActionTable = null
                };
                var tid = Thread.CurrentThread.ManagedThreadId;
                StartAction(tid, newRecord);
            }
        }
    }
}