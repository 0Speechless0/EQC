using System;

namespace EQC.Models
{
    public class EngMainModel
    {//工程主檔
        public int Seq { get; set; }
        public Int16? EngYear { get; set; }
        public string EngNo { get; set; }
        public string EngName { get; set; }
        public string OrganizerUnitCode { get; set; }
        public Int16? OrganizerUnitSeq { get; set; }
        public Int16? OrganizerSubUnitSeq { get; set; }
        public int? OrganizerUserSeq { get; set; }
        public Int16? ExecUnitSeq { get; set; }
        public Int16? ExecSubUnitSeq { get; set; }
        public string DesignUnitName { get; set; }
        public string DesignManName { get; set; }
        public string DesignUnitTaxId { get; set; }
        public string DesignUnitEmail { get; set; }
        public decimal? TotalBudget { get; set; }
        public decimal? SubContractingBudget { get; set; }
        public decimal? ContractAmountAfterDesignChange { get; set; }
        public byte? PurchaseAmount { get; set; }
        public byte? ExecType { get; set; }
        public string ProjectScope { get; set; }
        public Int16? EngTownSeq { get; set; }
        public int? EngPeriod { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? SchCompDate { get; set; }
        public DateTime? PostCompDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string ApproveNo { get; set; }
        public decimal? AwardAmount { get; set; }
        public string BuildContractorName { get; set; }
        public string BuildContractorContact { get; set; }
        public string BuildContractorTaxId { get; set; }
        public string BuildContractorEmail { get; set; }
        public bool IsNeedElecDevice { get; set; }
        public byte SupervisorExecType { get; set; } //shioulo 20220707
        public string SupervisorUnitName { get; set; }
        public string SupervisorDirector { get; set; }
        public string SupervisorTechnician { get; set; }
        public string SupervisorTaxid { get; set; }
        public string SupervisorContact { get; set; }
        public string SupervisorSelfPerson1 { get; set; }
        public string SupervisorSelfPerson2 { get; set; }
        public string SupervisorCommPerson1 { get; set; }
        public string SupervisorCommPersion2 { get; set; }
        public string ConstructionDirector { get; set; }
        public string ConstructionPerson1 { get; set; }
        public string ConstructionPerson2 { get; set; }

        public DateTime? CreateTime { get; set; }
        public int? CreateUserSeq { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int? ModifyUserSeq { get; set; }
        //PrjXML標案Seq shioulo 20220504
        public int? PrjXMLSeq { get; set; }
        //決標日期 shioulo 20220618
        public DateTime? AwardDate { get; set; }
        //shioulo 20220711
        public string PccesXMLFile { get; set; }
        public DateTime? PccesXMLDate { get; set; }
        
        //s20220825
        public string SupervisorCommPerson3 { get; set; }
        public DateTime? SupervisorCommPerson3LicenseExpires { get; set; }
        public string SupervisorCommPerson4 { get; set; }
        public DateTime? SupervisorCommPerson4LicenseExpires { get; set; }
        public DateTime? EngChangeStartDate { get; set; }
        public DateTime? WarrantyExpires { get; set; }
        //s20230418
        public int? CarbonDemandQuantity { get; set; }
        public int? ApprovedCarbonQuantity { get; set; }
        public bool OfficialApprovedCarbonQuantity { get; set; }
    }
}
