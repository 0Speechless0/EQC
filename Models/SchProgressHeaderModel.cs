using System;

namespace EQC.Models
{
    public class SchProgressHeaderModel
    {//預定進度主檔
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public int SPState { get; set; }
        public string PccesXMLFile { get; set; }
        public DateTime? PccesXMLDate { get; set; }
        public byte EngChangeCount { get; set; }
        public byte EngChangeState { get; set; }
        public DateTime? EngChangeStartDate { get; set; }
        public DateTime? EngChangeSchCompDate { get; set; }
    }
}
