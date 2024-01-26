
namespace EQC.ViewModel
{
    public class EPCEngPrjVModel
    {//工程 EngMain & PrjXML
        public int Seq { get; set; }
        public string EngNo { get; set; }
        public string EngName { get; set; }
        public string organizerUnitName { get; set; }
        public string execUnitName { get; set; }

        //PrjXML標案編號
        public int? PrjXMLSeq { get; set; }
        public string TenderNo { get; set; }
        public string TenderName { get; set; }
        public string tenderOrgUnitName { get; set; }
        public string tenderExecUnitName { get; set; }
    }
}
