
namespace EQC.Models
{
    public class ConstCheckRecOptionVModel : ConstCheckRecModel
    {//抽驗紀錄填報
        public int Value
        {
            get { return this.Seq; }
        }
        public string Text
        {
            get {
                return this.chsCheckDate + " " + this.CCRPosDesc;
            }
        }
    }
}
