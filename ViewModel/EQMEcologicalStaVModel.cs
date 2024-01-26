namespace EQC.ViewModel
{
    public class EQMEcologicalStaVModel
    {
        //生態檢核統計
        public string execUnitName { get; set; }
        public int engCount { get; set; }
        public int notChcek { get; set; }
        public int needChcek { get; set; }
        public int execChcek { get; set; }
        public int lostChcek { get; set; }
        public decimal notChcekRate { get; set; }
        public decimal needChcekRate { get; set; }
        public decimal execChcekRate { get; set; }
        public decimal lostChcekRate { get; set; }
    }
}