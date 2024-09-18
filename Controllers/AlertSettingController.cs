using EQC.EDMXModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EQC.Common;
using System.Web.Mvc;
using System.IO;

namespace EQC.Controllers
{
    [SessionFilter]
    public class AlertSettingController : MyController
    {


        public ActionResult Index()
        {
            return View();
        }
        public void GetCurrentSettingPos(string path)
        {
            if (Session["AlertSetting"] != null)
            { 
                var target = (AlertSetting)Session["AlertSetting"];
                var No = target.No;
                using(var context = new EQC_NEW_Entities() )
                {
                    if(target.MenuPath == path)
                        ResponseJson(context.AlertSetting.Find(No)?.Position);
                    else
                    {
                        ResponseJson(null);
                    }
                }

                    

            }
            else
            {
                ResponseJson(null);
            }
          
        }
        public void UploadVideo(string No)
        {

            var file = HttpContext.Request.Files.Get("file");
            using(var context = new EQC_NEW_Entities() )
            {
                var fileName = No; 
                context.AlertSetting.Find(No).VideoUrl = $"FileUploads/AlertVideo/{fileName}.mp4" ;
                $@"AlertVideo".UploadFileToFolder(file, $"{fileName}.mp4");
                context.SaveChanges();
            }

        }
        public ActionResult Setting()
        {
            return View();
        }
        public ActionResult StartPosSetting(string No)
        {

            using(var context = new EQC_NEW_Entities())
            {
                var target = context.AlertSetting.Find(No);
                if (Utils.getUserInfo().RoleSeq == 1)
                    Session["AlertSetting"] = target;

                //if(target.MenuPath != null)
                //{
                //    return Redirect( target.MenuPath);
                //}
                //else
                //{
                //    var path = target.Menu.PathName.Split('/');
                //    return RedirectToAction(path[1], path[0], new { posCatch = true });
                //}
                return RedirectToAction("Setting", "AlertSetting", new 
                { redirect = $"{target.Menu.PathName}?id=" + context.EngMain.FirstOrDefault()?.Seq });
   
            }
       
        }

        public void GetByMenuPath(string menuPath)
        {
            using (var context = new EQC_NEW_Entities(false))
            {
                menuPath = menuPath.Replace("/Index", "");
                ResponseJson(context.AlertSetting.Where(r => r.MenuPath.Replace("/Index", "") == menuPath));
            }

        }
        public void SetPos(string pos, string menuPath)
        {
            AlertSetting item = (AlertSetting)Session["AlertSetting"] ;
          
            if (item != null)
            {
                using (var context = new EQC_NEW_Entities())
                {
                    var targetItem = context.AlertSetting.Find(item.No);
                    targetItem.Position = pos;
                    targetItem.MenuPath = menuPath;
                    targetItem.ModifyTime = DateTime.Now;
                    context.SaveChanges();
                }
                Session["AlertSetting"] = null;
                ResponseJson(true);
            }
            else
            {
                ResponseJson(false);
            }
          
        }
        public void Delete(string No)
        {
            using(var context = new EQC_NEW_Entities())
            {
                context.Entry(context.AlertSetting.Find(No))
                    .State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
                ResponseJson(true);

            }
        }

        public void UpdateOrCreate(AlertSetting item, int systemTypeSeq)
        {
            using(var context = new EQC_NEW_Entities())
            {
                string topSubNoStr = context.AlertSetting
                    .Where(r => r.Menu.SystemTypeSeq == systemTypeSeq)
                    .ToList()
                    .Select(r => r.No.Split('.')[1])
                    .OrderByDescending(no => Int32.Parse(no) )
                    .FirstOrDefault();
                Int32.TryParse(topSubNoStr, out int topSubNo);




                if (item.No == null)
                {
                    item.No = $"{systemTypeSeq}.{topSubNo + 1}";
                    item.CreateTime = DateTime.Now;
                    item.ModifyTime = DateTime.Now;
                    context.AlertSetting.Add(item);
                }
                else
                {

                    if(item.VideoUrl != null && item.VideoUrl.Contains("youtube.com/watch"))
                    {
                        var uri = new Uri(item.VideoUrl);
                        string videoId = HttpUtility.ParseQueryString(uri.Query).Get("v");
                        item.VideoUrl = item.VideoUrl.Substring(0, item.VideoUrl.IndexOf("?"));
                        item.VideoUrl = item.VideoUrl.Replace("watch", "embed/");
                        item.VideoUrl += "/" + videoId + "?autoplay=1";
                    }
                    var target = context.AlertSetting.Find(item.No);
                    if (target != null)
                    {
                        item.No = target.No;
                        item.ModifyTime = DateTime.Now;
                        //item.VideoUrl = target.VideoUrl;
                        context.Entry(target).CurrentValues.SetValues(item);
                    }
                }
      
                //var tagedItems = items.GroupJoin(context.AlertSetting,
                //    r1 => r1.No,
                //    r2 => r2.No, (r1, r2) => new AlertSetting { 
                //        Origin = r2.FirstOrDefault()
                //    });

                context.SaveChanges();
                item.No = item.No.Trim();
                ResponseJson(item, "yyyy-MM-dd HH:mm:ss");

            }

        }

        public void GetList(int systemSeq, int page = 1, int perPage = 0)
        {
            using(var context = new EQC_NEW_Entities(false))
            {
                var list = context.AlertSetting
                    .Include("Menu")
                    .Where(r => r.Menu.SystemTypeSeq == systemSeq)
                    .OrderBy(r => r.ModifyTime)
                    .ToList();
                var pList = list.getPagination(page, perPage);
                pList.ForEach(e =>
                {
                    e.MenuName = e.Menu.Name;
                });
                ResponseJson(new { 
                    list = pList,
                    count = list.Count
                }, "yyyy-MM-dd HH:mm:ss");
            }
           
        }
    }
}
