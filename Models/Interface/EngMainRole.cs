using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQC.Interface
{
    public interface EngMainRole
    {
        string engTownName { get; set; }
        string organizerUserName { get; set; }
        string organizerUnitName { get; set; }
        string execUnitName { get; set; }
        string execSubUnitName { get; set; }
    }
}
