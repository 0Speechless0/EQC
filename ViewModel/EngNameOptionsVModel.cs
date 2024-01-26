
using System;

namespace EQC.Models
{
    public class EngNameOptionsVModel : IComparable<EngNameOptionsVModel>
    {//工程清單
        public int Seq { get; set; }
        public string EngName { get; set; }

        public string Text {
            get
            {
                return EngName;
            }
        }
        public int Value
        {
            get
            {
                return Seq;
            }
        }

        public int CompareTo(EngNameOptionsVModel compareM)
        {
            /*if (EngName.Equals(compareM.EngName))
                return 0;
            else if (EngName > compareM.EngName)
                return 1;

            else*/
                return String.Compare(EngName, compareM.EngName, comparisonType: StringComparison.OrdinalIgnoreCase); ;
        }
    }
}
