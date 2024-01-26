using EQC.Common;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EngForumController : Controller
    {//工程資訊交流
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
    }
}