using EQC.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EQC.ViewModel
{
    public class EPCEngMaterialAdjVModel : EngMaterialAdjModel
    {//物料調整
        public List<string> ForMade97MonthArray
        {
            get
            {
                if (String.IsNullOrEmpty(ForMade97Month))
                    return new List<string>();
                else
                    return ForMade97Month.Split(',').ToList();
            }
        }
    }
}
