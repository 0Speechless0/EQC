using EQC.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    public class TrainingImageController : MyController
    {
        // GET: TrainingImage
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            ViewBag.hasCard = false;
            return View();
        }


        public void GetDirFileName(string dirName = "", string keyWord = "")
        {
            string rootDirName = "trainingImage";
            string path = Path.Combine(Server.MapPath("~/FileUploads"), rootDirName);
            Queue<string> filePathStack = new Queue<string>();
            Dictionary<string, List<string>> DirFilesDic = new Dictionary<string, List<string>>();
            filePathStack.Enqueue(dirName);
            do
            {

                string relative_path = filePathStack.Dequeue();
                
                string current_dirPath =  Path.Combine(path, relative_path);
                List<string> current_dirFilePath =
                    Directory.GetFiles(current_dirPath).ToList();

                List<string> current_dirDirsPath =
                    Directory.GetDirectories(current_dirPath).ToList();

                List<string> current_Files = new List<string>();

                current_dirFilePath.ForEach(filePath => {
                    if (filePath.Contains(keyWord) || keyWord == "")
                        current_Files.Add(new FileInfo(filePath).Name);
                    
                });
                current_dirDirsPath.ForEach(subDirPath => 
                    filePathStack.Enqueue($"{relative_path}"+(relative_path == "" ? "" : "/" ) + $"{new DirectoryInfo(subDirPath).Name}" ));

                DirFilesDic.Add(relative_path, current_Files);

            } while (filePathStack.Count() > 0);

            ResponseJson(DirFilesDic
                .Select(r => new { 
                Key = r.Key,
                Files = r.Value
            }));
        }
    }
}