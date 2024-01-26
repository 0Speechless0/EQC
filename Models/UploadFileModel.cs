using System;

namespace EQC.Models
{
    public class UploadFileModel
    {//上傳檔案
        public int Seq { get; set; }
        public string OriginFileName { get; set; }
        public string UniqueFileName { get; set; }
        public string Memo { get; set; }

        public byte ItemGroup { get; set; }
        public string RESTful { get; set; }
    }
}
