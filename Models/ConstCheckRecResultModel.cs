namespace EQC.Models
{
    public class ConstCheckRecResultModel
    {//抽驗紀錄填報結果
        public int Seq { get; set; }
        public int ConstCheckRecSeq { get; set; }
        public int ControllStSeq { get; set; }
        public string CCRRealCheckCond { get; set; }
        public byte? CCRCheckResult { get; set; }
        public bool? CCRIsNCR { get; set; }
        public byte? ResultItem { get; set; }        

        public byte FormConfirm { get; set; }
    }
}
