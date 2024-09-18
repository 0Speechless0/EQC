using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Services;
using EQC.Models;
using EQC.ViewModel;
using EQC.Common;
using EQC.ViewModel.Common;

namespace EQC.Controllers
{
    [SessionFilter]
    public class RoleController : MyController
    {
        private RoleService roleService = new RoleService();
        // GET: Role
        public ActionResult Index()
        {
            return View();
        }
        public void GetAll()
        {
            using( var context = new  EDMXModel.EQC_NEW_Entities(false))
            {
                var list = context.Role.Where(r => r.IsDelete == false && r.IsEnabled == true).ToList();

                ResponseJson(new { l = list });
            }
        }

        /// <summary> 取得角色列表 </summary>
        /// <param name="page"> 頁數 </param>
        /// <param name="per_page"> 跳頁 </param>
        /// <returns> 角色列表 </returns>
        public JsonResult GetList(int page, int per_page)
        {
            List<VRole> list = roleService.GetList(page - 1, per_page);
            Object totalRows = roleService.GetCount();
            int rows;
            if (totalRows == null)
            {
                rows = 0;
            }
            else
            {
                rows = (int)totalRows;
            }

            return Json(new
            {
                l = list,
                t = rows,
            }, JsonRequestBehavior.AllowGet); ;
        }

        public void Insert(EDMXModel.Role role)
        {
            using(var context = new EDMXModel.EQC_NEW_Entities() )
            {
                role.IsDefault = false;
                role.IsEnabled = true;
                role.IsDelete = false;
                role.CreateTime = DateTime.Now;
                context.Role.Add(role);
                context.SaveChanges();
                ResponseJson(true);
            }
        }

        /// <summary> 儲存人員資料 </summary>
        /// <param name="vUserMain"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(VRole vRole)
        {
            SaveChangeStatus result = new SaveChangeStatus(true, StatusCode.Save);
            if (vRole.Seq == 0)
            {
                result = roleService.AddRole(vRole);
            }
            else
            {
                result = roleService.UpdateRole(vRole);
            }
            return Json(result);
        }

        /// <summary> 刪除使用者 </summary>
        /// <param name="Seq"> Seq </param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteRole(int Seq)
        {
            var result = roleService.DeleteRole(Seq);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary> 取得角色(下拉選單資料) </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetRoleList()
        {
            List<SelectVM> list = roleService.GetRoleList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRoleListV2()
        {
            List<SelectVM> list = roleService.GetRoleList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
