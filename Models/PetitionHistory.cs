using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class PetitionHistory
    {
        public long Seq { get; set; } //(bigint, not null)
        public int? PetitionMainSeq { get; set; } //(int, null)
        public DateTime? OccureDate { get; set; } //(datetime, null)
        public string HistoryStatus { get; set; } //(nvarchar(50), null)
        public string UnitName { get; set; } //(nvarchar(50), null)
        public string UserName { get; set; } //(nvarchar(50), null)
        public string Description { get; set; } //(nvarchar(200), null)
        public DateTime? CreateTime { get; set; } //(datetime, null)
        public int? CreateUser { get; set; } //(int, null)
        public DateTime? ModifyTime { get; set; } //(datetime, null)
        public int? ModifyUser { get; set; } //(int, null)
    }

}