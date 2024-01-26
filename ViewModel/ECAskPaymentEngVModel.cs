
namespace EQC.ViewModel
{
    public class ECAskPaymentEngVModel
    {//工程變更 - 工程 EngMain & PrjXML
        public int Seq { get; set; }
        public string EngNo { get; set; }
        public string EngName { get; set; }
        public int? PrjXMLSeq { get; set; }

        //PrjXML標案編號
        public string TenderNo { get; set; }
        public string TenderName { get; set; }
        public string ExecUnitName { get; set; }
        public string Location { get; set; }
        public string ContractorName1 { get; set; }
        public string ContractNo { get; set; }
        public decimal? BidAmount { get; set; }
        public string BelongPrj { get; set; }        
        public decimal? DesignChangeContractAmount { get; set; }
        
    }
}
