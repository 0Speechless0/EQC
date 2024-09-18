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
        [SessionFilter]
        public JsonResult GetList(int systemTypeSeq)
        {
            //List<MenuRoleVM> list = menuService.GetList(systemTypeSeq);
            List<Menu> list = menuService.GetListV2(systemTypeSeq);
            return Json(new
            {
                l = list,
            }, JsonRequestBehavior.AllowGet); ;
        }

        /// <summary> 取得MENU權限列表 </summary>
        /// <param name="systemTypeSeq"></param>
        /// <returns> MenuRoleVM </returns>
        [SessionFilter]
        public JsonResult GetMenuWithoutParentZero(int systemTypeSeq)
        {
            using (var context = new EDMXModel.EQC_NEW_Entities())
            {
                List<MenuRoleVM> list = 
                    context.Menu.Where(r => r.SystemTypeSeq == systemTypeSeq && r.ParentSeq != 0)
                    .Select(r => new MenuRoleVM { 
                        MenuSeq = r.Seq,
                        MenuName = r.Name
                    }).ToList();
    
                return Json(new
                {
                    l = list,
                }, JsonRequestBehavior.AllowGet);

            }
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

        public JsonResult GetMenuSeqByUrl(string url)
        {
            using (var context = new EDMXModel.EQC_NEW_Entities())
            {
                var controllerName = url.Remove(0, 1).Split('/')[0];
                var seq = controllerName != "" ? context.Menu.Where(r => 
                r.ParentSeq != null  &&
                (r.PathName.StartsWith(controllerName) )
                
                ).FirstOrDefault()?.Seq : 0;
                return Json(seq);
            }
        }

        public JsonResult GetMenuName(int seq)
        {
            using (var context = new EDMXModel.EQC_NEW_Entities())
            {
                var target = context.Menu.Find(seq);
                if(target != null)
                    return Json($"{target.SystemType.Name}-{target.Name}");
                return Json("");
            }
        }
    }
}