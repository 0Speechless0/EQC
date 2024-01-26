using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class UploadAuditFileModel
    {//送審檔案
        public int Seq { get; set; }
        public int EngMaterialDeviceSummarySeq { get; set; }
        public int FileType { get; set; }

        public string OriginFileName { get; set; }
        public string UniqueFileName { get; set; }
        public int OrderNo { get; set; }

        public DateTime? CreateTime { get; set; }
        public int CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int ModifyUserSeq { get; set; }
    }
}