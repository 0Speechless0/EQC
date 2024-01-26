using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    public class ThemeTraningNode
    {
        public string path { get; set; }

        public List<FileInfo> files { get; set; }
    }
}