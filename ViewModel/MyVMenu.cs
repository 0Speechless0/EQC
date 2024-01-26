using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    [Serializable]
    public class VMenu
    {
        public int? Seq { get; set; } //(int, null)
        public int? Level { get; set; } //(int, null)
        public string Name { get; set; } //(nvarchar(20), null)
        public string Url { get; set; } //(varchar(100), null)
        public int? OrderNo { get; set; } //(int, null)
        public string Path { get; set; } //(varchar(30), null)
        public bool? IsEnabled { get; set; } //(bit, null)

        public bool IsSelected { get; set; }
        public int ParentSeq { get; set; } //(int, null)
        public byte? SystemTypeSeq { get; set; } //(int, null)
    }
}