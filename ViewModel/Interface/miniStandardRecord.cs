using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQC.ViewModel.Interface
{
    public interface miniStandardRecord
    {

        int Seq { get; set; }
        Nullable<System.DateTime> CreateTime { get; set; }
        Nullable<System.DateTime> ModifyTime { get; set; }
    }
}
