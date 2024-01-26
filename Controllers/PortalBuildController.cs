using EQC.Common;
using System.Web.Mvc;

namespace EQC.Controllers
{
    public class PortalBuildController : Controller
    {//施工廠商
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return RedirectToAction("Index3", "EPCTender");
        }
    }
}