namespace EQC.Models
{
    public class CarbonEmissionMachineModel
    {//機具 碳排係數維護
        public int Seq { get; set; }
        public string Kind { get; set; }
        public string NameSpec { get; set; }
        public decimal? KgCo2e { get; set; }
        public string Unit { get; set; }
        public string Memo { get; set; }
        public decimal? ConsumptionRate { get; set; }
        public string ConsumptionRateUnit { get; set; }
        public string FuelKind { get; set; }
        public decimal? FuelKgCo2e { get; set; }
        public string FuelUnit { get; set; }
        public int itemNo { get; set; }
    }
}
