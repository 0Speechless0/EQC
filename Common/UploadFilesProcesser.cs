using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;

namespace EQC.Common
{
    public static class UploadFilesProcesser
    {


        public static FileStream getFileStreamWithConvert(this string pathName, Func<string, string> convert, string ext)
        {
           
            string exportPath;
            var file = System.IO.Path.ChangeExtension(pathName, "."+ext);
            if (!System.IO.File.Exists(file))
                exportPath = convert.Invoke(null);
            else exportPath = file;
            return new FileStream(exportPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        }
        /// <summary>
        /// 壓縮檔案成zip
        /// </summary>
        /// <param name="files">壓縮成zip檔案陣列，來自http post request 的file</param>
        /// <param name="zipfileName">壓縮黨名稱</param>
        /// <param name="zipFilePath"></param>
        /// <returns></returns>
        /// 
        public static string downloadFilesByZip(this IEnumerable<string> files, string zipFilePath, string zipFileName = null)
        {
            if (!Directory.Exists(zipFilePath))
            {
                zipFilePath = Path.Combine(rootPath, zipFilePath);
                if (!Directory.Exists(zipFilePath))
                    Directory.CreateDirectory(zipFilePath);

            }
            if (zipFileName == null)
            {
                zipFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";
            }
            else if (Path.GetExtension(zipFileName) != ".zip")
            {
                zipFileName += ".zip";
            }

            try
            {
                using (ZipOutputStream s = new ZipOutputStream(File.Create(Path.Combine(zipFilePath, zipFileName))))
                {
                    s.SetLevel(9); // 壓縮級別 0-9
                                   //s.Password = "123"; //Zip壓縮檔案密碼
                    byte[] buffer = new byte[4096]; //緩衝區大小
                    foreach (string file in files)
                    {

                        ZipEntry entry = new ZipEntry(Path.GetFileName(file) );
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (Stream fs = File.OpenRead(file) )
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                            fs.Close();
                            fs.Dispose();
                        }

                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception during processing");
            }

            return Path.Combine(zipFilePath, zipFileName);
        }
        public static string compressionZipFromUpload(this IEnumerable<HttpPostedFileBase> files, string zipFilePath , string zipFileName = null)
        {
            if (!Directory.Exists(zipFilePath))
            {
                throw new Exception("zip target not exist");
                
            }
            if(zipFileName == null)
            {
                zipFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip"; 
            }
            else
            {
                zipFileName += ".zip";
            }

            try
            {
                using (ZipOutputStream s = new ZipOutputStream(File.Create( Path.Combine(zipFilePath, zipFileName) )))
                {
                    s.SetLevel(9); // 壓縮級別 0-9
                                   //s.Password = "123"; //Zip壓縮檔案密碼
                    byte[] buffer = new byte[4096]; //緩衝區大小
                    foreach (HttpPostedFileBase file in files)
                    {
                       
                            ZipEntry entry = new ZipEntry(file.FileName);
                            entry.DateTime = DateTime.Now;
                            s.PutNextEntry(entry);
                            using (Stream fs = file.InputStream)
                            {
                                int sourceBytes;
                                do
                                {
                                    sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                    s.Write(buffer, 0, sourceBytes);
                                } while (sourceBytes > 0);
                                fs.Close();
                                fs.Dispose();
                            }
                        
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception during processing");
            }
            
            return zipFileName;
        }
        /// <summary>
        /// 壓縮檔案成zip
        /// </summary>
        /// <param name="usingTypes">MIME內容類型陣列</param>
        public static bool UploadFilesTypeCheck(this IEnumerable<HttpPostedFileBase> files, string[] usingTypes)
        {
            foreach(var file in files)
            {
                var a = usingTypes.Contains(file.ContentType) ? 1 : 0 ;
                if (a == 0) return false;
              
            }
            return true;
        }
        static string rootPath = Path.Combine(HttpContext.Current.Server.MapPath("~"), "FileUploads");
       public static void UploadFilesToFolder(this IEnumerable<HttpPostedFileBase> files, string paths)
       {
            string fileFolderPath = Path.Combine(rootPath, paths);
            if(! System.IO.Directory.Exists(fileFolderPath) )
            {
                System.IO.Directory.CreateDirectory(fileFolderPath);
            }
            foreach(var file in files)
            {
                file.SaveAs(Path.Combine(fileFolderPath, file.FileName));
            }
       }
        public static void UploadFileToFolder(this string path, HttpPostedFileBase file, string name = null)
        {
            string fileFolderPath = Path.Combine(rootPath, path);
            if (!System.IO.Directory.Exists(fileFolderPath))
            {
                System.IO.Directory.CreateDirectory(fileFolderPath);
            }

            file.SaveAs(Path.Combine(fileFolderPath, name ?? file.FileName));

        }
        public static void RemoveFile(this string path, string name)
        {
            string fileFolderPath = Path.Combine(rootPath, path, name);
            if (System.IO.File.Exists(fileFolderPath))
            {
                File.Delete(fileFolderPath);
            }
        }
        public static string getUploadDir(this string relativePath)
        {
            string Dir = Path.Combine(rootPath, relativePath);
            if (!Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);
            return Dir;
        }
        public static List<string> GetDownloadListByFolder(this string paths, bool fullPath = false)
        {
            var folderPath = Path.Combine(rootPath, paths);
            if (System.IO.Directory.Exists(folderPath))
            {
                return System.IO.Directory.GetFiles(folderPath)
                    .Select(row => fullPath ? row : Path.GetFileName(row))
                    .ToList<string>();
            }
            else
            {
                return new List<string>();
            }
        }
        public static string[] GetFiles(this string path)
        {
            var _path = Path.Combine(rootPath, path);
            if (!Directory.Exists(_path)) return new string[0];
            return Directory.GetFiles(_path);
        }
        public static FileStream GetFileStream(this string paths, string fileName)
        {
            return new FileStream(Path.Combine(rootPath, paths, fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        public static void DeleteFile(this string paths, string fileName)
        {
            System.IO.File.Delete(Path.Combine(rootPath, paths, fileName));
        }
        public static bool deepCopyAllFileInDir(this string originPath, string destPath, bool recursive = true)
        {
            try
            {
                // Get information about the source directory
                var dir = new DirectoryInfo(originPath);

                // Check if the source directory exists
                if (!dir.Exists)
                    throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

                // Cache directories before we start copying
                DirectoryInfo[] dirs = dir.GetDirectories();

                // Create the destination directory
                Directory.CreateDirectory(destPath);

                // Get the files in the source directory and copy to the destination directory
                foreach (FileInfo file in dir.GetFiles())
                {
                    string targetFilePath = Path.Combine(destPath, file.Name);
                    file.CopyTo(targetFilePath, true);
                }

                // If recursive and copying subdirectories, recursively call this method
                if (recursive)
                {
                    foreach (DirectoryInfo subDir in dirs)
                    {
                        string newDestinationDir = Path.Combine(destPath, subDir.Name);
                        subDir.FullName.deepCopyAllFileInDir(newDestinationDir, true);
                    }
                }
            }
            catch(Exception e)
            {
                throw e;

            }
            return true;
        }

        public static string ConvertToBase64(this string path)
        {
            var fullPath = Path.Combine(rootPath, path);
            byte[] bytes = File.ReadAllBytes(fullPath);
            return Convert.ToBase64String(bytes);
        }
        public static void SaveImageByBase64(this string filePath, string str, int imageFormat = 0)
        {
            byte[] data = Convert.FromBase64String(str);
            //var compressed;
            //var from = new MemoryStream(compressed);
            //var to = new MemoryStream();
            // var gZipStream = new GZipStream(from, CompressionMode.Decompress);
            //gZipStream.CopyTo(to);
            //var a = to.ToString();
            using (var stream = new MemoryStream(data, 0, data.Length))
            {
                string fullPath = Path.Combine(rootPath, filePath);
                var dir = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                Image image = Image.FromStream(stream);
                image.Save(Path.Combine(rootPath, filePath), imageFormat == 0  ? ImageFormat.Jpeg : ImageFormat.Png );
                //TODO: do something with image
            }
        }

        public static string RootPath
        {
            get
            {
                return rootPath;
            }

        }
        public static void removeFile(this string path)
        {
            File.Delete(Path.Combine(rootPath, path));
        }
        public static string GetUniqueFileName()
        {

            return String.Format("{0}", Guid.NewGuid());
        }
        //public void deleteUploadFile(int id)
        //{
        //    foreach (string typeDir in Directory.GetDirectories(Path.Combine(saveDir)))
        //    {
        //        foreach (string seqDir in Directory.GetDirectories(Path.Combine(saveDir, typeDir)))
        //        {
        //            if (
        //                Path.Combine(saveDir, typeDir, seqDir.ToString()) ==
        //                Path.Combine(saveDir, typeDir, id.ToString()))
        //            {
        //                Directory.Delete(Path.Combine(saveDir, typeDir, id.ToString()), true);
        //            }
        //        }
        //    }
        //}
        //public void saveUploadFile(HttpPostedFileBase file, int id)
        //{
        //    string classDir ;
        //    if (file.ContentType.Split('/')[0] == "video")
        //    {
        //        classDir = "vdo";
        //    }
        //    else if (file.ContentType.Split('/')[0] == "image")
        //    {
        //        classDir = "img";
        //    }
        //    else
        //    {
        //        classDir = "doc";
        //    }
        //    Directory.CreateDirectory(Path.Combine(saveDir, classDir, id.ToString()));
        //    string fullPath = Path.Combine(saveDir, classDir, id.ToString(), file.FileName);
        //    file.SaveAs(fullPath);
        //}
        //public List<object> getPath(int id)
        //{
        //    List<object> filePathList = new List<object>();
        //    foreach (string typeDir in Directory.GetDirectories(Path.Combine(saveDir)))
        //    {
        //        foreach (string seqDir in Directory.GetDirectories(Path.Combine(saveDir, typeDir)))
        //        {
        //            if (
        //                Path.Combine(saveDir, typeDir, seqDir.ToString()) ==
        //                Path.Combine(saveDir, typeDir, id.ToString()))
        //            {
        //                filePathList.AddRange(Directory.GetFiles(Path.Combine(saveDir, typeDir, id.ToString())));
        //            }
        //        }
        //    }



        //    return filePathList;
        //}
    }
}