namespace EQC.Models
{
    public class SuperviseScheduleFormModel
    {//督導行程安排-日程表
        public int Seq { get; set; }
        public int SuperviseEngSeq { get; set; }
        public int OrderNo { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int ActivityTime { get; set; }
        public string Summary { get; set; }
    }
}
