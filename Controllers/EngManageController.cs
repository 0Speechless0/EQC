using EQC.Common;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EngManageController : Controller
    {//工程管理
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
    }
}