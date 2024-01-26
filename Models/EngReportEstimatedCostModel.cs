using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{//提案審查-概估經費
    public class EngReportEstimatedCostModel
    {
        public int Seq { get; set; }
        public int? EngReportSeq { get; set; }

        public int? Year { get; set; }
        public int? AttributesSeq { get; set; }
        public int? Price { get; set; }

        public string Memo { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? ModifyUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}