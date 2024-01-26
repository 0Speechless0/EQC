
namespace EQC.ViewModel
{
    public class EADManufacturerEngSafetyVModel
    {//廠商履歷評估分析 重大事故
        public int Seq { get; set; }
        public string TenderName { get; set; }
        public string ExecUnitName { get; set; }
        public int? WSDeadCnt { get; set; }
        public int? WSHurtCnt { get; set; }
    }
}