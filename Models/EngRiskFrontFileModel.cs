using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{//設計方案評選
    public class EngRiskFrontFileModel
    {
        public int Seq { get; set; }
        public int EngRiskFrontSeq { get; set; }
        public int ERFType { get; set; }
        public string FilePath { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? ModifyUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}