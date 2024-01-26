
namespace EQC.Models
{//設備運轉測試清單
    public class EquOperTestListModel : EquOperTestListTpModel
    {
        public int EngMainSeq { get; set; }
        public bool DataKeep { get; set; }
        public int DataType { get; set; }
    }
}