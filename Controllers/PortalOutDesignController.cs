using EQC.Common;
using System.Web.Mvc;

namespace EQC.Controllers
{
    public class PortalOutDesignController : Controller
    {//委外設計
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return RedirectToAction("Index", "TenderPlan");
        }
    }
}