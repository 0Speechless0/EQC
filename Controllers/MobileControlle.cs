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

    public class MobileController : Controller
    {
        // 抽驗填報紀錄

        public ActionResult Index()
        {
            //ViewBag.Title = "抽驗紀錄填報";
            return View();
        }

        // 水利署API儀錶板
        public ActionResult Index2()
        {
            return View("Index2");
        }


    }
}