
using System;

namespace EQC.Models
{
    public class SchEngProgressHeaderModel
    {//工程進度-主檔
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public int SPState { get; set; }
        //s20230417
        public string PccesXMLFile { get; set; }
        public DateTime? PccesXMLDate { get; set; }
    }
}
