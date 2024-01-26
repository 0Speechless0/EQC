using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    public class GoJsModel
    {
        public int key { get; set; }
        public string text { get; set; }
        public string no { get; set; }

        public int level { get; set; }

        public int tableSeq { get; set; }
        public string loc { get; set; }
        public string category { get; set; } = "";
        
    }
}