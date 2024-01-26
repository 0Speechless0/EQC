using System;
using System.Web;

namespace EQC.Models
{
    public class CCListTpModel: ConstCheckListTpModel
    {
        public string Text {
            get { return this.ItemName; }
        }
        public int Value {
            get { return this.Seq; }
        }
    }
}
