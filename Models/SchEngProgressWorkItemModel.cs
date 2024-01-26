
namespace EQC.Models
{
    public class SchEngProgressWorkItemModel : WorkItemModel
    {//工程前置作業-PayItem用料清單
        public int Seq { get; set; }
        public int SchEngProgressPayItemSeq { get; set; }
        public int OrderNo { get; set; }
    }
}
