namespace EQC.ViewModel
{
    public class CarbonEmissionCTVModel
    {
        //水利署碳排管制表
        public string execUnitName { get; set; }
        public string EngNo { get; set; }
        public string EngName { get; set; }
        public string TenderNo { get; set; }
        public string TenderName { get; set; }
        public int awardStatus { get; set; }
        public int createEng { get; set; }
        public int pccesXML { get; set; }
        public decimal detachableRate { get; set; }
        public int engMaterialDevice { get; set; }
        public int detachableRateCnt { get; set; }
        public int engMaterialDeviceCount { get; set; }
        public int engMaterialDeviceSummaryCount { get; set; }
        public int supDaily { get; set; }
        public int checkRec { get; set; }
    }
}