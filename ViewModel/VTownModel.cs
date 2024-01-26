
namespace EQC.Models
{
    public class VTownModel : Town
    {//行政區(縣市)
        public string Text {
            get
            {
                return TownName;
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
