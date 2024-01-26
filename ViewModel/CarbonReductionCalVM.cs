using EQC.EDMXModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    public class CarbonReductionCalVM<T>
    {
        public int Seq { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public string Description { get; set; }

        public string RefItemCode { get; set; }

        public T factor;

        public CarbonReductionCal carbonReductionCal { get; set; }
    }
}