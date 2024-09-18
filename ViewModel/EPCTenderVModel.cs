using EQC.Common;
using System;

namespace EQC.ViewModel
{
    public class EPCTendeVModel
    {//工程標案清單
        public int Seq { get; set; }
        public string EngNo { get; set; }
        public string EngName { get; set; }
        public string TenderNo { get; set; }
        public string TenderName { get; set; }
        public string DurationCategory { get; set; }
        public int? EngPeriod { get; set; }
        public decimal? TotalBudget { get; set; } //s20230329
        public DateTime? StartDate { get; set; }
        public DateTime? SchCompDate { get; set; }

        public DateTime? ProgressDoneEarly { get; set; }

        public string StartDateStr
        {
            get
            {
                return this.StartDate.HasValue ? Utils.ChsDate(this.StartDate) : "";
            }
        }
        public string SchCompDateStr {
            get
            {
                return this.SchCompDate.HasValue ? Utils.ChsDate(this.SchCompDate) : "";
            }
        }

        public string prjSchStartDateStr
        {
            get
            {
                return this.prjSchStartDate.HasValue ? Utils.ChsDate(this.prjSchStartDate) : "";
            }
        }
        public string prjSchCompDateStr
        {
            get
            {
                return this.prjSchCompDate.HasValue ? Utils.ChsDate(this.prjSchCompDate) : "";
            }
        }

        public DateTime? prjSchStartDate { get; set; }
        public DateTime? prjSchCompDate { get; set; }
        // from PrjXML
        public string ActualStartDate { get; set; }
        public int? PrjXMLSeq { get; set; }
        public decimal? SchProgress { get; set; }
        public decimal? AcualProgress { get; set; }
        public decimal? SubContractingBudget { get; set; }
        public int? ApprovedCarbonQuantity { get; set; }
    }
}
