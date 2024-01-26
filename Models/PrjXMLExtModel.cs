using System;

namespace EQC.Models
{
    public class PrjXMLExtModel
    {//標案主檔(從XML匯入)擴充
        public int PrjXMLSeq { get; set; }
        public string BelongPrj { get; set; }
        public decimal? YearBudget { get; set; }
        public string PrjManageUnit { get; set; }
        public string AirPollutionNo { get; set; }
        public string EcologicalCheck { get; set; }
        public string BuildingLicenseNo { get; set; }
        public string BuildingLicenseDate { get; set; }
        public string PunishmentMechanism { get; set; }
        public string InsuranceStatus { get; set; }
        public string AuditDate { get; set; }
        public string Score { get; set; }
        public string SchePerformDesignDate { get; set; }
        public string ActualPerformDesignDate { get; set; }
        public string ScheAnnoDate { get; set; }
        public string ScheChangeCloseDate { get; set; }
        public string ActualCompletionDate { get; set; }
        public string ActualAacceptanceCompletionDate { get; set; }
        public decimal? AcceptanceDeduction { get; set; }
        public decimal? SettlementAmount { get; set; }
        public string ScheSettlementDate { get; set; }
        public string ActualSettlementDate { get; set; }
        public string BidAwardDiff { get; set; }
        public decimal? DesignChangeContractAmount { get; set; }
        public decimal? CementMortar { get; set; }
        public decimal? MachineMixConcrete { get; set; }
        public decimal? ReadyMixedConcrete { get; set; }
        public decimal? AsphaltConcrete { get; set; }
        public decimal? Sand { get; set; }
        public decimal? Gradation { get; set; }
        public decimal? CLSM { get; set; }
        public decimal? EarthWork { get; set; }
        public decimal? ACReduce { get; set; }
        public decimal? BottomAsh { get; set; }
        public decimal? EAF { get; set; }
        public decimal? BOF { get; set; }
        public decimal? Rebar { get; set; }
        public decimal? SteelPlateSection { get; set; }
        public decimal? Template { get; set; }
        public decimal? ACReduceOutput { get; set; }
        public decimal? EarthWorkOutput { get; set; }
        public string ImproveDeadline { get; set; }
    }
}
