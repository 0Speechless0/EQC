using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.EDMXModel{
    public partial class ExpertCommittee
    {
        public string ECKindName
        {
            get
            {
                switch(ECKind)
                {
                    case 1: return "評選委員";
                    case 2: return "督導委員";
                    default: return "其它";
                }
            }

        }
    }
}