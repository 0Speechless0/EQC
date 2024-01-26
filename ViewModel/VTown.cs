using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    public class VTown
    {
        public short Seq { get; set; } //(smallint, not null)
        public string TownName { get; set; } //(nvarchar(30), null)
        public short? CitySeq { get; set; } //(smallint, null)
        public bool? IsEnabled { get; set; } //(bit, null)
        public short? OrderNo { get; set; } //(smallint, null)
        public DateTime? CreateTime { get; set; } //(datetime, null)
        public int? CreateUser { get; set; } //(int, null)
        public DateTime? ModifyTime { get; set; } //(datetime, null)
        public int? ModifyUser { get; set; } //(int, null)
    }

}