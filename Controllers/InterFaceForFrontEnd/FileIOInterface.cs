using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers.InterFaceForFrontEnd
{
    interface FileIOInterface
    {
        void GetDownloadFilesName(string dir = "");
        void GetDownloadFilesUrl(string dir = "");
        void DownloadAll(string dir = "");
        void DeleteFile(string fileName, string dir = "");
        void UploadFile(HttpPostedFileBase file, string dir = "");
        void DownloadFile(string fileName, string dir ="");

   
    }
}
