using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQC.ViewModel.Interface
{
    public interface CarbonReduction : miniStandardRecord
    {
        int Seq { get; set; }
        string Code { get; set; }
        string Type2 { get; set; }
        string Unit { get; set; }
    }
}
