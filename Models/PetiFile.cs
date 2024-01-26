using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class PetiFile
    {
        public int Seq { get; set; } //(int, not null)
        public int? PetitionMainSeq { get; set; } //(int, null)
        public string OrginName { get; set; } //(nvarchar(128), null)
        public string UniqueName { get; set; } //(varchar(255), null)
        public byte? OrderNo { get; set; } //(tinyint, null)
        public DateTime? CreateTime { get; set; } //(datetime, null)
        public int? CreateUser { get; set; } //(int, null)
        public DateTime? ModifyTime { get; set; } //(datetime, null)
        public int? ModifyUser { get; set; } //(int, null)
    }

}