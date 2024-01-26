using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQC.ViewModel.Interface
{
    public interface UnitTreeJsonConvertor
    {
        string execUnitName { get; set; }
        List<string> Tree02931JsonList { get; set; }
        List<string> Tree02932JsonList { get; set; }
    }
}
