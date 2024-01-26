
namespace EQC.Models
{
    public class EngMaterialAdjModel
    {//物料調整
        public int Seq { get; set; }
        public int EngMainSeq { get; set; }
        public bool? SPDate { get; set; }
        public bool? IsChangeContract { get; set; }
        public bool? IsAppendAgreement { get; set; }
        public bool? IsForYear97 { get; set; }
        public string ForMade97Month { get; set; }
        public bool? IsAppleBeforeClose { get; set; }
        public int PriceAdjust { get; set; }
        public int BudgetAvail { get; set; }
    }
}
