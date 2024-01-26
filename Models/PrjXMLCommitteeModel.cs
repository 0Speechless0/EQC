
namespace EQC.Models
{
    public class PrjXMLCommitteeModel
    {//決標委員資料
        public int Seq { get; set; }
        public int PrjXMLSeq { get; set; }
        public string CName { get; set; }
        public byte Kind { get; set; }
        public string Profession { get; set; }
        public string Experience { get; set; }
        public bool? IsPresence { get; set; }
    }
}