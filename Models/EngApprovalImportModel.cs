using System;

namespace EQC.Models
{//工程核定資料匯入 s20231006
    public class EngApprovalImportModel
    {
        public int Seq { get; set; }
        public Int16? EngYear { get; set; }
        public string EngNo { get; set; }
        public string EngName { get; set; }
        public decimal? TotalBudget { get; set; }
        public decimal? SubContractingBudget { get; set; }
        public int? CarbonDemandQuantity { get; set; }
        public int? ApprovedCarbonQuantity { get; set; }
        public int? ExecUnitSeq { get; set; }
        public int? ExecSubUnitSeq { get; set; }
        public int? OrganizerUserSeq { get; set; }
        public int? engMainSeq { get; set; }
        public string engExecUnit { get; set; } //s20231106
        public int? pccessMainSeq { get; set; } //s20231105
    }
}