
namespace EQC.ViewModel
{
    public partial class EADPrjXMLVModel
    {//各單位在建工程
        public int Seq { get; set; }
        public int? EngSeq { get; set; }

        public string ExecUnitName { get; set;  }
        public string TenderNo { get; set; }
        public string TenderName { get; set; }
        public decimal? OutsourcingBudget { get; set; }
        public decimal? BidAmount { get; set; }
        public decimal? QualityControlFee { get; set; }
        public string ActualStartDate { get; set; }
        public string ScheCompletionDate { get; set; }
    }
}