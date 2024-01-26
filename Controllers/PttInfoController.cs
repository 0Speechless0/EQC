using EQC.Common;
using EQC.EDMXModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    public class PttInfoController : Controller
    {


        public JsonResult GetPttInfo(int seq)
        { 
            PttMain info = null;
            SessionManager sessionManager = new EQC.Common.SessionManager();
            UserInfo userInfo = sessionManager.GetUser();
            using (EQC_NEW_Entities context = new EQC_NEW_Entities())
            {
                DateTime lastCheckTime = context
                    .PttCheckRecord
                    .Where(row => row.UserSeq == userInfo.Seq)
                    .Select(row => row.CheckTime).FirstOrDefault() ?? DateTime.MinValue;

                List<PttMain> infos = context.PttMain
                    .OrderBy(row => row.createTime)
                    .Where(row => row.createTime > lastCheckTime ).ToList();
                int count = infos.Count;
                if(count > seq)
                {
                    info = infos[seq];
                }

            }

            if(info == null)
            {
                return Json(new
                {
                    result = -1,
                    info = info
                });
            } 
            return Json(new
            {
                result = 0,
                info = info,
                infoCreateTime = info.createTime.ToString()
            });
        }

        public JsonResult getHeadingModal()
        {
            List<PttMain> ptts;
            Session["PttInfo"] = false;
            SessionManager sessionManager = new EQC.Common.SessionManager();
            UserInfo userInfo = sessionManager.GetUser();
            PttMain newest;
            using (EQC_NEW_Entities context = new EQC_NEW_Entities())
            {
                DateTime lastCheckTime = context
                   .PttCheckRecord
                   .Where(row => row.UserSeq == userInfo.Seq)
                   .Select(row => row.CheckTime).FirstOrDefault() ?? DateTime.MinValue;

                ptts = context.PttMain
                    .Where(row => row.createTime > lastCheckTime)
                    .ToList();

                newest = ptts.OrderByDescending(row => row.createTime).FirstOrDefault();


                return Json(new
                {
                    result = 0,
                    ptts = ptts,
                    newestDateTime = newest?.createTime.ToString()

                });
            }

        }

        //public JsonResult getInsideModal(int seq)
        //{
        //    PttMain ptt;
        //    SessionManager sessionManager = new EQC.Common.SessionManager();
        //    UserInfo userInfo = sessionManager.GetUser();
        //    using (EQC_NEW_Entities context = new EQC_NEW_Entities())
        //    {
        //        ptt = context.PttMain.Find(seq);
        //    }
        //    return Json(new
        //    {
        //        result = 0,
        //        ptt = ptt

        //    });
        //}


        public JsonResult CheckPtt(DateTime checkTime)
        {
            DateTime d = checkTime;
            SessionManager sessionManager = new EQC.Common.SessionManager();
            UserInfo userInfo = sessionManager.GetUser();

            try
            {
                using (EQC_NEW_Entities context = new EQC_NEW_Entities())
                {
                    PttCheckRecord record = context.PttCheckRecord.Where(row => row.UserSeq == userInfo.Seq).FirstOrDefault();
                    if (record == null)
                    {
                        context.PttCheckRecord.Add(new PttCheckRecord { UserSeq = userInfo.Seq, CheckTime = d.AddSeconds(1) });
                    }
                    else
                    {
                        record.CheckTime = d.AddSeconds(1);
                    }
                    context.SaveChanges();
                }
            }
            catch(Exception e)
            {
                return Json(new
                {
                    result = -1
                });
            }
            return Json(new
            {
                result = 0
            });

        }
    }
}