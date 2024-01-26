using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{//提案審查-主要工作內容
    public class EngReportMainJobDescriptionModel
    {
        public int Seq { get; set; }
        public int? EngReportSeq { get; set; }

        public int? RptJobDescriptionSeq { get; set; }
        public string OtherJobDescription { get; set; }
        public decimal? Num { get; set; }
        public decimal? Cost { get; set; }

        public string Memo { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? ModifyUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}