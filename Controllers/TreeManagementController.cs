using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Controllers.Common;
using EQC.EDMXModel;
using EQC.Common;
namespace EQC.Controllers
{
    public class TreeManagementController : WebBaseController<TreeList>
    {
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            ViewBag.BarTitle = "樹種管理";
            return View();
        }
        public override void GetPagination(int page, int perPage)
        {
            using(var context = new EQC_NEW_Entities())
            {
                var list = context.TreeList.OrderByDescending(r =>r.ModifyTime).ToList();
                ResponseJson(new { 
                    list = list.getPagination(page, perPage),
                    count = list.Count
                });
            }
        }
        public override void GetByKeyWord(string keyWord = null)
        {
            using(var context = new EQC_NEW_Entities())
            {
                var list = context.TreeList.Where(r => r.Name.Contains(keyWord));

                ResponseJson(new
                {
                    list = list
                });
            }
        }
        public override void Insert(TreeList model)
        {
            using(var context = new EQC_NEW_Entities())
            {
                model.CreateTime = DateTime.Now;
                model.ModifyTime = DateTime.Now;
                context.TreeList.Add(model);
                context.SaveChanges();
                ResponseJson(true);
            }
        }
        public override void Update(TreeList model)
        {
            using (var context = new EQC_NEW_Entities())
            {
                model.ModifyTime = DateTime.Now;
                context.Entry(
                    context.TreeList.Find(model.Seq)
                ).CurrentValues.SetValues(model);
                context.SaveChanges();
                ResponseJson(true);
            }
        }
        public override void Delete(int id)
        {
            using (var context = new EQC_NEW_Entities())
            {
                context.TreeList.Remove(
                    context.TreeList.Find(id)
                );
                context.SaveChanges();
                ResponseJson(true);
            }
        }
    }
}