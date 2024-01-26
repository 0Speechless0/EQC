using System;

namespace EQC.Models
{
    public class City
    {
        public short Seq { get; set; } //(smallint, not null)
        public string CityName { get; set; } //(nvarchar(100), null)
        public bool? IsEnabled { get; set; } //(bit, null)
        public short? OrderNo { get; set; } //(smallint, null)
        public DateTime? CreateTime { get; set; } //(datetime, null)
        public int? CreateUser { get; set; } //(int, null)
        public DateTime? ModifyTime { get; set; } //(datetime, null)
        public int? ModifyUser { get; set; } //(int, null)

    }
}