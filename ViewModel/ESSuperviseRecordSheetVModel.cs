using System;

namespace EQC.ViewModel
{
    public class ESSuperviseRecordSheetVModel
    {//督導紀錄表 20230217

        public int SupervisorExecType { get; set; }
        public int EngSeq { get; set; }
        public string PhaseCode { get; set; }
        public byte SuperviseMode { get; set; }
        public string TenderNo { get; set; }
        public string TenderName { get; set; }
        public string OrganizerName { get; set; }
        public string ContactName { get; set; }
        public DateTime? SuperviseDate { get; set; }
        public DateTime? SuperviseEndDate { get; set; } //s20230316
        public string Location { get; set; }
        public string SupervisionUnitName { get; set; }
        public string  SupervisorDirectorOutSide { get; set; }

        public string SupervisorDirectorSelf { get; set; }
        public string SupervisorSelfPerson1 { get; set; }
        public string SupervisorCommPerson1 { get; set; }
        public string ActualStartDate { get; set; }
        public string ScheCompletionDate { get; set; }
        public string ContractorName1 { get; set; }
        public string EngOverview { get; set; }
        public decimal? BidAmount { get; set; }
        public string CommitteeList { get; set; }
        public int? CommitteeAverageScore { get; set; }
        public string Inspect { get; set; }
        public decimal? DeductPoints { get; set; }
        public string ProjectNo { get; set; }
        public string ExecUnitCode { get; set; }
        public decimal? PDAccuActualProgress { get; set; }
        public decimal? PDAccuScheProgress { get; set; }
        public decimal? DiffProgress { get; set; }
        public string ImproveDeadline { get; set; }
    }
}
