
namespace EQC.Models
{//設備運轉抽查標準
    public class EquOperControlStModel : EquOperControlTpModel
    {
        public bool DataKeep { get; set; }
        public int DataType { get; set; }
    }
}