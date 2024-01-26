using System;

namespace EQC.Models
{
    public class ConstCheckRecImproveFileModel : UploadFileModel
    {//抽驗缺失改善檔案上傳
        
        public int ConstCheckRecImproveSeq { get; set; }
        public int ControllStSeq { get; set; }
        //public byte ItemGroup { get; set; }
        public string Memo { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
    }
}
