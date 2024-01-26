using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Services;
using EQC.Models;
using EQC.ViewModel;
using System.Data.SqlClient;
using EQC.Common;
using EQC.ViewModel.Common;

namespace EQC.Controllers
{
    public class MenuController : Controller
    {
        private MenuService menuService = new MenuService();
        // GET: Menu
        [SessionFilter]
        public ActionResult Index()
        {
            return View();
        }
                
        /// <summary> 取得MENU權限列表 </summary>
        /// <param name="systemTypeSeq"></param>
        /// <returns> MenuRoleVM </returns>
        [HttpGet]
        [SessionFilter]
        public JsonResult GetList(int systemTypeSeq)
        {
            List<MenuRoleVM> list = menuService.GetList(systemTypeSeq);
            return Json(new
            {
                l = list,
            }, JsonRequestBehavior.AllowGet); ;
        }

        public JsonResult SetSession(int seq)
        {
            string pathName = menuService.GetMenuParentString(seq);
            Session["pathName"] = pathName;
            return null;
        }

        public string GetPathName()
        {
            if (Session["pathName"] == null)
            {
                return "";
            }
            else
            {
                return Session["pathName"].ToString();
            }
        }

        public void ResetSession()
        {
            return;
        }

        public string GetTopMenuUrl()
        {
            SessionManager _sm = new SessionManager();
            List<VMenu> listMenu = menuService.LoadMenu(_sm.SystemTypeSeq, _sm.GetUser().Seq);
            foreach (var item in listMenu)
            {
                if (item.Url != null && item.Url != "")
                {
                    return item.Url;
                }
            }
            return "";
        }

        public List<VMenu> CheckMenuList()
        {
            SessionManager _sm = new SessionManager();
            List<VMenu> listMenu = menuService.LoadMenu(_sm.SystemTypeSeq, _sm.GetUser().Seq);

            return listMenu;
        }

        /// <summary> 取得系統別下拉 </summary>
        /// <returns> SelectVM </returns>
        [HttpGet]
        public JsonResult GetSystemTypeList()
        {
            List<SelectVM> list = menuService.GetSystemTypeList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary> 儲存MenuRole </summary>
        /// <param name="menuSeq"> menuSeq </param>
        /// <param name="roleSeq"> roleSeq </param>
        /// <param name="chk"> chk </param>
        /// <returns> SaveChangeStatus </returns>
        [HttpPost]
        public ActionResult Save(int menuSeq, int roleSeq, bool chk)
        {
            SaveChangeStatus result = menuService.Save(menuSeq, roleSeq, chk);
            return Json(result);
        }
    }
}