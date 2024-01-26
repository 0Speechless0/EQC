namespace EQC.Models
{
    public class SuperviseFillModel
    {//督導填報
        public int Seq { get; set; }
        public int? SuperviseEngSeq { get; set; }
        public string MissingNo { get; set; }
        public string MissingLoc { get; set; }
        public string DeductPointStr { get; set; }
        public decimal DeductPoint { get; set; }
        public string SuperviseMemo { get; set; }
    }
}
