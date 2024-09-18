
namespace EQC.ViewModel
{
    public class EADCarbonEmissionVModel
    {//水利工程淨零碳排分析 清單
        public int Seq { get; set; }
        public int EngYear { get; set; }
        public string EngName { get; set; }
        public string ExecUnit { get; set; }
        public decimal? AwardAmount { get; set; }
        public decimal? Co2Total { get; set; }
        
        public string EngType { get; set; }

        public string BelongPrj { get; set; }
    }
}