
namespace EQC.Models
{
    public class VCityModel : City
    {//行政區(縣市)
        public string Text {
            get
            {
                return CityName;
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
