using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQC.EDMXModel.InterFace
{
    public interface PayItem
    {
        Nullable<decimal> KgCo2e { get; set; }
        Nullable<int> RStatusCode { get; set; }

        Nullable<int> GreenFundingSeq { get; set; }
        Nullable<decimal> Amount { get; set; }
    }
}
