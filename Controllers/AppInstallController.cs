using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class AppInstallController : Controller
    {
        protected AppInstallService AppInstallService = new AppInstallService();
        public  ActionResult Index()
        {
            Utils.setUserClass(this);
            return View();
        }

        public JsonResult Save(int PhoneType, string Phonemodel)
        {
            int userseq = Utils.getUserSeq();
            if (AppInstallService.Update(PhoneType, userseq, Phonemodel))
            {
                return Json(new
                {
                    result = 0,
                    message = "儲存成功"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "儲存失敗"
                });
            }
        }

        public virtual JsonResult GetLink()
        {
            int userseq = Utils.getUserSeq();
            List<AppInstallModel> subEngNames = AppInstallService.GetLink<AppInstallModel>();
            if (subEngNames.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "ios代碼不足，請聯繫"
                });

            }
            else
            {
                return Json(new
                {
                    result = 0,
                    Link = subEngNames[0].Link
                });
            }
        }

        public JsonResult UpdateLink(string Downloadcode)
        {
            int userseq = Utils.getUserSeq();
            if (AppInstallService.UpdateLink(userseq,Downloadcode))
            {
                return Json(new
                {
                    result = 0,
                    message = "儲存成功"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "儲存失敗"
                });
            }
        }


    }
}
