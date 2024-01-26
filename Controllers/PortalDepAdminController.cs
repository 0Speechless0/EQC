using EQC.Common;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class PortalDepAdminController : Controller
    {//署管理者
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
    }
}