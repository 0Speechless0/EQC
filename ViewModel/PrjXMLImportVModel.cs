using EQC.Common;
using System;

namespace EQC.Models
{
    public class PrjXMLImportVModel
    {//匯入狀態統計
        public int Success { get; set; }
        public int Failure { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateDate {
            get {
                return CreateTime.ToString("yyyy/MM/dd hh:mm");
            }
        }
        public string CreateUser { get; set; }
    }
}
