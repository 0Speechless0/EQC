using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQC.ViewModel.Interface
{
    public interface StandardRecord : miniStandardRecord
    {

        Nullable<int> CreateUser { get; set; }
        Nullable<int> ModifyUser { get; set; }
    }
}
