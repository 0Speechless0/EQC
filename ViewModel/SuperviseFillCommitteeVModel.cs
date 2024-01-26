
namespace EQC.ViewModel
{//委員
    public class SuperviseFillCommitteeVModel
    {
        public int mode { get; set; }
        public int Seq { get; set; }
        
        public string CName { get; set; }
        public string Value
        {
            get {
                return string.Format("{0}-{1}", mode, Seq);
            }
        }
        public string Text
        {
            get
            {
                return CName;
            }
        }
    }
}