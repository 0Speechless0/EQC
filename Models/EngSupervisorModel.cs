using System;

namespace EQC.Models
{
    public class EngSupervisorModel
    {//自辦監造人員
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public byte UserKind { get; set; }
        public Int16 SubUnitSeq { get; set; }
        public int UserMainSeq { get; set; }
    }
}
