
namespace EQC.Models
{
    public class UModel : UnitModel
    {//單位清單
        public string Text {
            get
            {
                return Name;
            }
        }
        public string Value
        {
            get
            {
                return Seq.ToString();
            }
        }
    }
}
