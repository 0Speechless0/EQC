using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    [Serializable]
    public class VSystemMenu
    {
        public string PathName { get; set; }
        public string Name { get; set; }
        public int SystemType { get; set; }
        public int OrderNo { get; set; }
    }
}