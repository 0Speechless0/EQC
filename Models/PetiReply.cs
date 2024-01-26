using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class PetiReply
    {
        public int Seq { get; set; } //(int, not null)
        public int? PetitionMainSeq { get; set; } //(int, null)
        public int? ReplyUnitSeq { get; set; } //(int, null)
        public DateTime? ReplyTime { get; set; } //(datetime, null)
        public string ReplyContent { get; set; } //(nvarchar(2000), null)
        public int? OrderNo { get; set; } //(int, null)
        public DateTime? CreateTime { get; set; } //(datetime, null)
        public int? CreateUser { get; set; } //(int, null)
        public DateTime? ModifyTime { get; set; } //(datetime, null)
        public int? ModifyUser { get; set; } //(int, null)
    }

}