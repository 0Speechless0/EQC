
using EQC.Models;

namespace EQC.ViewModel
{
    public class ConstCheckRecOption1VModel : ConstCheckRecModel
    {//抽驗紀錄填報
        public string ItemName { get; set; }
        public int Value
        {
            get { return this.Seq; }
        }
        public string Text
        {
            get {
                return ItemName+ "、" + this.chsCheckDate + " " + this.CCRPosDesc;
            }
        }
    }
}
