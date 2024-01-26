
namespace EQC.ViewModel
{
    public class EADRiskEngVModel
    {//水利工程履約風險分析
        public int Seq { get; set; }
        public string BelongPrj { get; set; }
        public string TenderName { get; set; }
        public string ExecUnitName { get; set; }
        public decimal? BidAmount { get; set; }
        public decimal? PDAccuScheProgress { get; set; }
        public decimal? PDAccuActualProgress { get; set; }
        public decimal? DiffProgress { get; set; }
        public string BDAnalysis { get; set; }
        public string BDSolution { get; set; }
    }
}