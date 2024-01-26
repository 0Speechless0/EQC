
namespace EQC.Models
{//抽查標準
    public class ControlStVModel : ConstCheckRecResultModel
    {
        public string CheckItem1 { get; set; }
        public string CheckItem2 { get; set; }
        public string Stand1 { get; set; }
        public string Stand2 { get; set; }
        public string Stand3 { get; set; }
        public string Stand4 { get; set; }
        public string Stand5 { get; set; }
        public int CheckFields { get; set; }

        public int rowSpan { get; set; }
        public int rowSpanStd1 { get; set; }
        public bool rowShow { get; set; }
        public bool changed { get; set; }
        public int itemType { get; set; } //s20231016
        public string RecResultRemark { get;  set; }
    }
}