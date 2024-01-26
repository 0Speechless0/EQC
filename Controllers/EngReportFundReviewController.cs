using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Common;
using EQC.Services;
using EQC.ViewModel;
using EQC.Models;
using System.Net;
using Newtonsoft.Json;

namespace EQC.Controllers
{
    public class EngReportFundReviewController : Controller
    {
        [SessionFilter]
        public ActionResult Index()
        {
            return View();
        }
    }
}
