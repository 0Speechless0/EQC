using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    public class VSubType
    {

        public short? SubTypeSeq { get; set; } //(smallint, null)
        public short CitySeq { get; set; } //(smallint, not null)
        public string CityName { get; set; } //(nvarchar(100), null)
        public short? MainTypeSeq { get; set; } //(smallint, null)
        public string MainTypeName { get; set; } //(nvarchar(100), null)
        public string SubTypeName { get; set; } //(nvarchar(100), null)
        public short? DeadLine { get; set; } //(smallint, null)
        public bool? IsEnabled { get; set; } //(bit, null)
        public short? OrderNo { get; set; } //(smallint, null)
        public DateTime? CreateTime { get; set; } //(datetime, null)
        public int? CreateUser { get; set; } //(int, null)
        public DateTime? ModifyTime { get; set; } //(datetime, null)
        public int? ModifyUser { get; set; } //(int, null)
    }
}