using EQC.EDMXModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    public class TreePlantMonthVM : TreePlantMonth
    {
        public string ScheduledPlant { get; set; }
        public int ScheduledPlantNum { get; set; }
    }
}