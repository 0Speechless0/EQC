
namespace EQC.ViewModel
{
    public class EPCQualityVerifyDoc2VMode
    {//品質查證-工程資料表
        public string TenderNo { get; set; }
        public string BelongPrj { get; set; }
        public string TenderName { get; set; }
        public decimal? OutsourcingBudget { get; set; }
        public decimal? RendBasePrice { get; set; }
        public decimal? BidAmount { get; set; }
        public string EngType { get; set; }
        public string AuditDate { get; set; }
        public string ActualStartDate { get; set; }
        public string ScheCompletionDate { get; set; }
        public int? TotalDays { get; set; }
        public string DurationCategory { get; set; }
        public string CompetentAuthority { get; set; }
        public string OrganizerName { get; set; }
        public string PrjManageUnit { get; set; }
        public string DesignUnitName { get; set; }
        public string SupervisionUnitName { get; set; }
        public string ContractorName1 { get; set; }
        public string EngOverview { get; set; }
        public string BDBackwardFactor { get; set; }
        public string BDAnalysis { get; set; }
        public string BDSolution { get; set; }
        public decimal? PDAccuScheProgress { get; set; }
        public decimal? PDAccuActualProgress { get; set; }
        public decimal? PDAccuEstValueAmount { get; set; }
    }
}
