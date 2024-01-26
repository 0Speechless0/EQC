
namespace EQC.Models
{
    public class EngExecUnitsVModel
    {//工程執行機關清單
        public int UnitSeq { get; set; }
        public string UnitName { get; set; }

        public string Text {
            get
            {
                return UnitName;
            }
        }
        public int Value
        {
            get
            {
                return UnitSeq;
            }
        }
    }
}
