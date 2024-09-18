using EQC.EDMXModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQC.Common
{

    //包裹式填報下載執行動作參數必須包括以下介面
    public class DownloadArgExtension
    {

        public static string WebRootPath { get; set; }
        private int? CreateUserSeq;
        public void targetPathSetting(string targetPath, string fileName = null)
        {
            if(distFilePath != null)
            {
                System.IO.File.Copy(
                    targetPath,
                    Path.Combine(distFilePath,
                        fileName != null ?
                        Path.GetExtension(fileName).Length > 0 ?
                        fileName : ( fileName + Path.GetExtension(targetPath) )  
                        : Path.GetFileName(targetPath) 
                    ),
                    true
                );
            }


        }
        public static void compressToPackage(string distDir, string sourceDir)
        {
            var fileName = $@"Package-{DateTime.Now.ToString("yyyyMMddHHmmss")}";
            ZipFile.CreateFromDirectory(distDir,Path.Combine(Utils.GetTempFolderForUser(fileName), $"{fileName}.zip"));
            ZipFile.CreateFromDirectory(distDir, sourceDir);
            //System.IO.Directory.Delete(distDir, true);
        }
        public void finalHandle(PackageDownloadAction target)
        {
            if (!Directory.Exists(distFilePath)) return;
            var fileCount = Directory.GetFiles(distFilePath).Length;
            string name = (target.GroupName ?? target.Name).Replace('/', '-');

            if (fileCount > 1 && target.PackageDistPath == null)
            {
                ZipFile.CreateFromDirectory(distFilePath,
                    Path.Combine(Utils.GetTempFolderForUser($@"packageDownload-{name}-{DateTime.Now.ToString("yyyyMMddHHmmss")}", target.createUserSeq),
                    $"{name}.zip"
                    ));
                System.IO.Directory.Delete(distFilePath, true);
            }

            if (fileCount == 0)
            {
                var sw = File.CreateText(Path.Combine(distFilePath, "使用說明.txt"));
                sw.WriteLine("此項目尚不能包裹式下載，請確認以下幾點:");
                sw.WriteLine("1. 該文件已經先上傳系統");
                sw.WriteLine("2. 該文件需要先在系統產製");
                sw.Close();
            }
        }

        public int? GetCreateUser()
        {
            return CreateUserSeq;
        }
        public void setDistFilePath(PackageDownloadAction target)
        {
            var name = (target.GroupName ?? target.Name).Replace('/', '-');
            var uuid =
                target.PackageDistPath !=  null ? 
                $@"{ target.PackageDistPath }/packageDownload-{name}" :
                $"packageDownload-{name}-{target.CreateTime.ToString("yyyyMMddHHmmss")}-One";
            distFilePath = Utils.GetTempFolderForUser(uuid, target.createUserSeq);
            CreateUserSeq = target.createUserSeq;
        }
        private string distFilePath;

        public string DistFilePath
        {
            get
            {
                return distFilePath;
            }
        }


    }
}