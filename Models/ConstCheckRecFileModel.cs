using System;

namespace EQC.Models
{
    public class ConstCheckRecFileModel : UploadFileModel
    {//抽驗紀錄上傳檔案
        //public int Seq { get; set; }
        public int ConstCheckRecSeq { get; set; }
        public int ControllStSeq { get; set; }
        public string Memo { get; set; }
        public int? OrderNo { get; set; }
        public bool? IsSign { get; set; }
        //public string OriginFileName { get; set; }
        //public string UniqueFileName { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
        
    }
}
