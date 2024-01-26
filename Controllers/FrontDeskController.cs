using EQC.Common;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class FrontDeskController : Controller
    {
        private string baseLayout = "_LayoutFrontDesk";
        //開發階段暫時 layout
        //private string baseLayout = "_???";//正式

        public ActionResult Index()
        {
            ViewBag.Title = "標案管理";
            return View("Index", baseLayout);//_BackendLayout
        }

        public ActionResult TenderPlanList()
        {
            ViewBag.Title = "建立標案";
            return View("TenderPlanList", baseLayout);
        }
    }
}