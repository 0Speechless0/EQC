
namespace EQC.ViewModel
{
    public class EADCommitteeVModel
    {//工程採購評選委員分析 清單
        public string CName { get; set; }
        public byte Kind { get; set; }
        public string Profession { get; set; }
        public int totalCount { get; set; }
        public int presence { get; set; }
        public decimal presenceRate { get; set; }
    }
}