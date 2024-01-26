
using System;

namespace EQC.Models
{
    public class ESQEngNameOptionsVModel : IComparable<ESQEngNameOptionsVModel>
    {//PrjXML 工程清單
        public int Seq { get; set; }
        public int? EngMainSeq { get; set; }
        public string EngName { get; set; }

        public string Text {
            get
            {
                return EngName;
            }
        }
        public string Value
        {
            get
            {
                return (EngName == "全部工程") ? "" : EngName;
            }
        }
        public int CompareTo(ESQEngNameOptionsVModel compareM)
        {
            return String.Compare(EngName, compareM.EngName, comparisonType: StringComparison.OrdinalIgnoreCase); ;
        }
    }
}
