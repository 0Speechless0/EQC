namespace EQC.ViewModel
{
    public class CarbonTradeEngsVModel
    {
        //可碳交易工程清單
        public int? Seq { get; set; }
        public int EngMainSeq { get; set; }
        public string ExecUnitName { get; set; }
        public string EngNo { get; set; }
        public string EngName { get; set; }
        public int ApprovedCarbonQuantity { get; set; }
        public int CarbonDesignQuantity { get; set; }
        public int TradingTotalQuantity { get; set; }
        public int? Quantity { get; set; }
    }
}