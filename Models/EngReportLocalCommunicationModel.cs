using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{//提案審查-在地溝通辦理情形
    public class EngReportLocalCommunicationModel
    {
        public int Seq { get; set; }
        public int EngReportSeq { get; set; }

        public DateTime Date { get; set; }
        public string FileNumber { get; set; }
        public string FilePath { get; set; }

        public string Memo { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? ModifyUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}