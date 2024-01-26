using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Common;
using EQC.Services;
using EQC.Models;
using EQC.ViewModel;

namespace EQC.Controllers
{
    public class SharedController : Controller
    {

        private MenuService menuService = new MenuService();
        private SessionManager _sm;
        // GET: Share

        public ActionResult _Menu()
        {
            _sm = new SessionManager();
            List<VMenu> listMenu = menuService.LoadMenu(_sm.SystemTypeSeq, _sm.GetUser().Seq);
            Session["UserMenu"] = listMenu;
            string url = Request.Url.LocalPath;
            foreach (var item in listMenu)
            {
                if (item.Url == null)
                    item.Url = "";
                if (url.Contains(item.Url) && item.Url != "")
                {
                    item.IsSelected = true;
                }
                else
                {
                    item.IsSelected = false;
                }
            }
            return View(listMenu);
        }

        public string GetLogin()
        {
            return new SessionManager().GetUser().UserNo;
        }

        public string GetRoleName()
        {
            string result = string.Empty;
            foreach(Role role in new SessionManager().GetUser().Role)
            {
                result += role.Name + ',';
            }
            return result.Remove(result.Length - 1);
        }
    }
}