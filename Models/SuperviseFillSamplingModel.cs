namespace EQC.Models
{
    public class SuperviseFillSamplingModel
    {//督導填報-抽驗項目
        public int Seq { get; set; }
        public int? SuperviseEngSeq { get; set; }
        public string SamplingName { get; set; }
        public string Location { get; set; }
        public string Quantity { get; set; }
    }
}
