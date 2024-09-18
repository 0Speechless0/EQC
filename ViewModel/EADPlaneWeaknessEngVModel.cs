
using System;
using System.Collections.Generic;
using System.Linq;

namespace EQC.ViewModel
{
    public class EADPlaneWeaknessEngVModel
    {//品質管制弱面追蹤與分析 工程清單
        public int Seq { get; set; }
        public int TenderYear { get; set; }
        public string TenderName { get; set; }
        public string ExecUnitName { get; set; }
        public decimal? BidAmount { get; set; }
        public string Location { get; set; }
        public string PlaneWeakness { get; set; }

        public HashSet<int> PlaneWeaknessNumArr { 
            get
            {
                return PlaneWeakness?.Split(',')
                .Where(r => r.Length > 0)
                .Select(r => Int32.Parse(r))
                .ToHashSet() ?? new HashSet<int>();

            }
        }
    }
}