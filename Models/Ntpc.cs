using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class Ntpc
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }

        public bool IsHoliday { get; set; }
    
        public string HolidayCategory { get; set; }

        public string Description { get; set; }

        public bool IsSync { get; set; }
    }
}