
namespace EQC.Models
{
    public class EC_SchEngProgressWorkItemModel : WorkItemModel
    {//工程前置作業-PayItem用料清單
        public int Seq { get; set; }
        public int EC_SchEngProgressPayItemSeq { get; set; }
        public int OrderNo { get; set; }
    }
}
