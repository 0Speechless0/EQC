using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Controllers.InterFaceForFrontEnd;
using EQC.Common;
using System.IO;

namespace EQC.Controllers
{
    public class TempFileController :  MyController , FileIOInterface
    {
        public void DeleteFile(string fileName, string dir = "")
        {
            dir.DeleteFile(fileName);
            ResponseJson(true);
        }

        public void DownloadAll(string dir = "")
        {
            throw new NotImplementedException();
        }

        public void DownloadFile(string fileName, string dir = "")
        {
            using(var ms = new MemoryStream())
            {
                using(var fs = dir.GetFileStream(fileName))
                {
                    fs.CopyTo(ms);
                    DownloadFile(ms, fileName);
                 }
       
            }
        }

        public void GetDownloadFilesList(string dir = "")
        {
            var files = dir.GetFiles().Select(r => new FileInfo(r));

            ResponseJson(files.Select(file => {
                var _file = file.Name.Split('.');
                return new
                {
                    CreateTime = file.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Name = _file[0] + "." + _file[2],
                    Description = _file[1]
                };
            }));
        }

        public void GetDownloadFilesName(string dir = "")
        {
            throw new NotImplementedException();
        }

        public void GetDownloadFilesUrl(string dir = "")
        {
            throw new NotImplementedException();
        }



        public void UploadFile(HttpPostedFileBase file, string dir = "")
        {
            string path = dir.getUploadDir();
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            file.SaveAs(Path.Combine(path, file.FileName));
            ResponseJson(true);
        }
    }
}