using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    public class ThsrFilterVM
    {
        public string carNo { get; set; } = "";
        public string start { get; set; } = "";
        public string end { get; set; } = "";
        public int direction { get; set; }
    }
}