using EQC.Common;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class PortalCommitteeController : Controller
    {//委員
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return RedirectToAction("Index", "EngSupervise");
        }
    }
}