
using System;

namespace EQC.Models
{
    public class EngConstructionOptionsVModel : IComparable<EngConstructionVModel>
    {//分項工程清單
        public int Seq { get; set; }
        public string ItemName { get; set; }

        public string Text {
            get
            {
                return ItemName;
            }
        }
        public int Value
        {
            get
            {
                return Seq;
            }
        }

        public int CompareTo(EngConstructionVModel compareM)
        {
            /*if (EngName.Equals(compareM.EngName))
                return 0;
            else if (EngName > compareM.EngName)
                return 1;

            else*/
                return String.Compare(ItemName, compareM.ItemName, comparisonType: StringComparison.OrdinalIgnoreCase); ;
        }
    }
}
