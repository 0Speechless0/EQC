using System;

namespace EQC.ViewModel
{
    public class SuperviseEngSchedule2VModel
    {//工程督導 - 行程安排
        public string PhaseCode { get; set; }
        public string OrganizerName { get; set; }

        public string ExecUnitName { get; set; }
        public string BelongPrj { get; set; }
        public string TenderName { get; set; }
        public string Location { get; set; }
        public DateTime? SuperviseDate { get; set; }
        public DateTime? SuperviseEndDate { get; set; } //s20230316
        public decimal BidAmount { get; set; }
        public string ScheCompletionDate { get; set; }
        public decimal? ActualProgress { get; set; }
        public decimal? DiffProgress { get; set; }
        public string LeaderName { get; set; }
        public string OutCommittee { get; set; }
        public string InsideCommittee { get; set; }
        public string OfficerName { get; set; }
        public string Memo { get; set; }
    }
}
