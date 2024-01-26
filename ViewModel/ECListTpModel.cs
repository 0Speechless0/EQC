
namespace EQC.Models
{
    public class ECListTpModel: EnvirConsListTpModel
    {//環境保育清單範本
        public string Text {
            get { return this.ItemName; }
        }
        public int Value {
            get { return this.Seq; }
        }
    }
}
