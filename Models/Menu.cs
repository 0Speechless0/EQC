using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class Menu
    {
        public int Seq { get; set; } //(int, not null)
        public byte? SystemTypeSeq { get; set; } //(tinyint, null)
        public int? ParentSeq { get; set; } //(int, null)
        public string Name { get; set; } //(nvarchar(20), null)
        public string Url { get; set; } //(varchar(100), null)
        public int? OrderNo { get; set; } //(int, null)
        public bool? CanManage { get; set; } //(bit, null)

        public bool? IsEnabled { get; set; }
        public DateTime? CreateTime { get; set; } //(datetime, null)
        public int? CreateUer { get; set; } //(int, null)
        public DateTime? ModifyTime { get; set; } //(datetime, null)
        public int? ModifyUser { get; set; } //(int, null)

        public List<byte> RoleSeqs { get; set; }
    }

}