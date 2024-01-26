using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    public class TenderCalForm
    {
        public string TenderNo { get; set; }
        public string TenderName { get; set; }


        public string EngName { get; set; }

        public int isBuild { get; set; }

        public decimal TotalKgCo2e { get; set; }

        public int MaterialSummaryNum { get; set; }

        public int MaterialSummaryTotal { get; set; }
        public int MaterialTestNum { get; set; }

        public int MaterialTestTotal { get; set; }

        public int FillConstructionDayShouldNum { get; set; }
        public int FillConstructionDayNum { get; set; }
        public int FillSupervisionDayNum { get; set; }

        public int FillSupervisionDayShouldNum { get; set; }

        public int FillDayTotal { get; set; }

        public int checkedDayNum { get; set; }
        public int checkedDayTotal { get; set; }

        public int constCheckShouldNum { get; set; }

        public int neededConstCheckNum { get; set; }
        public string EngNo { get;  set; }
        public string ExecUnitName { get;  set; }
        public int DismantlingRate { get; set; }
    }
}