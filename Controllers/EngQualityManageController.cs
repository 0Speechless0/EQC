using EQC.Common;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EngQualityManageController : Controller
    {//工程品管
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
    }
}