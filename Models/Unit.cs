using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class Unit
    {
        public int Seq { get; set; } //(int, not null)
        public int? ParentSeq { get; set; } //(int, null)
        public string Code { get; set; } //(varchar(10), null)
        public string Name { get; set; } //(nvarchar(30), null)
        public int? OrderNo { get; set; } //(int, null)
        public bool? IsEnabled { get; set; } //(bit, null)
        public bool? IsSubUnit { get; set; } //(bit, null)
        public bool? IsRegTable { get; set; } //(bit, null)
        public DateTime? CreateTime { get; set; } //(datetime, null)
        public int? CreateUser { get; set; } //(int, null)
        public DateTime? ModifyTime { get; set; } //(datetime, null)
        public int? ModifyUser { get; set; } //(int, null)
    }

}