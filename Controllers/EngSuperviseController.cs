using EQC.Common;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EngSuperviseController : Controller
    {//工程督導
        public ActionResult Index()
        {
            Utils.setUserClass(this, 1);
            return View("Index");
        }
    }
}