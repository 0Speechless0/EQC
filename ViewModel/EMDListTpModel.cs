using System;
using System.Web;

namespace EQC.Models
{
    public class EMDListTpModel : EngMaterialDeviceListTpModel
    {
        public string Text {
            get { return this.MDName; }
        }
        public int Value {
            get { return this.Seq; }
        }
    }
}
