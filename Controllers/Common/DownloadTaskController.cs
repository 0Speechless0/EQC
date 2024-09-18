using EQC.Common;
using EQC.Detection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers.Common
{
    [SessionFilter]
    public class DownloadTaskController : MyController
    {
        public JsonResult GetDownloadTaskTag()
        {
            var user = Utils.getUserInfo();
            return Json(new
            {
                downloadTaskTag =
                    DownloadTaskDetection.downloadTaskTag.Contains(user.Seq)
            });
        }

        public void DownloadUserFile(string uuid)
        {
            var targetFilePath = Directory.GetFiles(Path.Combine(Utils.GetTempFolderForUser(), uuid))
               .FirstOrDefault();
            using(var ms = new MemoryStream())
            {
                var fs =  new FileStream(targetFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                fs.CopyTo(ms);
                DownloadFile(ms, Path.GetFileName(targetFilePath));
                fs.Close();
                ms.Close();
            }
        }

        public void DeleteDownloadFile(string uuid = null)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (uuid != null)
            {
                var targetFileDir = Path.Combine(Utils.GetTempFolderForUser(), uuid);
                
                Directory.Delete(targetFileDir, true);
            }
            else
            {
                Directory.Delete(Utils.GetTempFolderForUser(), true);
            }
            ResponseJson(true);
        }
    }
}