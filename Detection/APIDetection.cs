using EQC.Common;
using EQC.EDMXModel;
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
        private static Dictionary<int, APIRecord> threadActions = new Dictionary<int, APIRecord>();
        public static void StartAction(int tid, APIRecord apiRecord)
        {
            if(!threadActions.ContainsKey(tid))
                threadActions.Add(tid, apiRecord);
        }

        public static void FinishAction(int tid)
        {
            using (var context = new EQC_NEW_Entities())
            {
                var record = GetActionRecord(tid);
                record.EndingTime = DateTime.Now;
                context.APIRecord.Add(record);
                context.SaveChanges();

                threadActions.Remove(tid);
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
                    ChangeText = null,
                    ActionTable = null
                };
                var tid = Thread.CurrentThread.ManagedThreadId;
                StartAction(tid, newRecord);
            }
        }
    }
}