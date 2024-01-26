
namespace EQC.Models
{//抽查標準範本
    public class QCStdModel
    {
        public byte FlowType { get; set; }
        public string Flow { get; set; }
        public string ManageItem { get; set; }
        public string Stand { get; set; }
        public string CheckTiming { get; set; }
        public string CheckMethod { get; set; }
        public string CheckFeq { get; set; }
        public string Incomp { get; set; }
        public string ManageRec { get; set; }
        public string Memo { get; set; }
    }
}