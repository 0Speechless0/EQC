using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EQC.Controllers.InterFaceForFrontEnd;
namespace EQC.Controllers.Common
{
    public abstract class WebBaseWithFileController<T> : WebBaseController<T>, FileIOInterface
    {
        public virtual void DeleteFile(string fileName, string dir = "")
        {
            throw new NotImplementedException();
        }

        public virtual void DownloadAll(string dir = "")
        {
            throw new NotImplementedException();
        }

        public virtual void DownloadFile(string fileName, string dir = "")
        {
            throw new NotImplementedException();
        }

        public virtual void GetDownloadFilesName(string dir = "")
        {
            throw new NotImplementedException();
        }

        public virtual void GetDownloadFilesUrl(string dir = "")
        {
            throw new NotImplementedException();
        }

        public virtual void UploadFile(HttpPostedFileBase file, string dir = "")
        {
            throw new NotImplementedException();
        }
    }
}