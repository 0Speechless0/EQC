using System;
using System.Web;

namespace EQC.Models
{
    public class EOTListTpModel: EquOperTestListTpModel
    {
        public string Text {
            get { return this.ItemName; }
        }
        public int Value {
            get { return this.Seq; }
        }
    }
}
