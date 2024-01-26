using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.EDMXModel;
using EQC.Common;
namespace EQC.Controllers
{
    public class ConstCheckUserController : Controller
    {
        // GET: ConstCheckUser

        public ActionResult Index()
        {
            Utils.setUserClass(this);
            ViewBag.hasCard = false;
            ViewBag.Title = "抽查者設定";
            return View();
        }
        public JsonResult GetList(int selectYear, int selectUnit, int pageCount, int pageIndex, string keyWord = "")
        {
            using(var context = new EQC_NEW_Entities() )
            {
                var list = context.ConstCheckUser
                    .Where(
                        row =>
                            row.EngMain.EngYear == selectYear &&
                            row.EngMain.ExecUnitSeq == selectUnit &&
                            (
                                keyWord == "" ||
                                (row.EngMain.EngName.Contains(keyWord) || row.EngMain.EngNo.Contains(keyWord) )
                            ) 
                    )
                    .Select(row => new 
                    {
                       EngSeq = row.EngSeq,
                       UserName = row.UserMain.DisplayName,
                       ExecSubUnit = row.Unit.Name
                    })
                    .ToList()
                    
                    .getPagination(pageIndex, pageCount);

                return Json(list);
            }
        }

        public JsonResult Update(ConstCheckUser m)
        {
            using (var context = new EQC_NEW_Entities())
            {
                if( context.ConstCheckUser.Find(m.EngSeq) == null)
                {
                    context.ConstCheckUser.Add(m);
                }
                else
                {
                    context.Entry(context.ConstCheckUser.Find(m.EngSeq)).CurrentValues.SetValues(m);
                }

                context.SaveChanges();
                return Json(true);
            }
        }


    }
}