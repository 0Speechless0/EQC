using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class UnitDispatch
    {
        public int Seq { get; set; } //(int, not null)
        public int? CentralDispatchSeq { get; set; } //(int, null)
        public int? UnitSeq { get; set; } //(int, null)
        public int? UnderTakerSeq { get; set; } //(int, null)
        public DateTime? DispatchTime { get; set; } //(datetime, null)
        public DateTime? CreateTime { get; set; } //(datetime, null)
        public int? CreateUser { get; set; } //(int, null)
        public DateTime? ModifyTime { get; set; } //(datetime, null)
        public int? ModifyUser { get; set; } //(int, null)
    }

}