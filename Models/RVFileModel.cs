using System;

namespace EQC.Models
{
    public class RVFileModel
    {//拒絕往來廠商
        public int Seq { get; set; }
        public string Code { get; set; }
        public string Corporation_Number { get; set; }
        public string Corporation_Name { get; set; }
        public string Case_no { get; set; }
        public string Case_Name { get; set; }
        public DateTime? Effective_Date { get; set; }
        public DateTime? Expire_Date { get; set; }
    }
}
