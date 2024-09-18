using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EQC.EDMXModel
{
    public partial class SystemProblem
    {
        static string rootPath;
        public static void SetFileRoot(string root)
        {
            rootPath = root;
        }

        public string uploadFilesName
        {
            get
            {
                if(rootPath != null)
                {
                    var path = Path.Combine(rootPath, Seq.ToString());
                    if(Directory.Exists(path))
                    {
                        return Directory.GetFiles(path)
                        .Select(r => Path.GetFileName(r))
                        .Aggregate("", (a, c) => a + c + ",");
                    }

                }
   
                return null;
            }
        }
    }
}