
namespace EQC.Models
{
    public class OccuSafeHealthListModel : OccuSafeHealthListTpModel
    {//職業安全衛生清單範本
        public int EngMainSeq { get; set; }
        public bool DataKeep { get; set; }
        public int DataType { get; set; }
    }
}
