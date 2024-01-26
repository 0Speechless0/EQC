using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class EngReportOptionModel
    {
        public int Seq { get; set; }
        public Int16? RptYear { get; set; }
        public decimal? CarbonEmissionRatio { get; set; }
        public decimal? RegressionCurve { get; set; }
        public decimal? PriceAdjustmentIndex { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? ModifyUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}