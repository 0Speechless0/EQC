using EQC.Common;
using System.Web.Mvc;

namespace EQC.Controllers
{
    public class PortalSupervisorController : Controller
    {//委外監造
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return RedirectToAction("Index3", "EPCTender");
        }
    }
}