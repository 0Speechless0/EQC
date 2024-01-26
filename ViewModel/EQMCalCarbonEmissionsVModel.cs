
namespace EQC.ViewModel
{
    public class EQMCalCarbonEmissionsVModel
    {//碳排係數比對
        public int RStatusCode { get; set; }
        public int Seq { get; set; }
        public string RefItemCode { get; set; }
        public string Code { get; set; }
        public string RStatus { get; set; }
        public decimal KgCo2e { get; set; }
    }
}
