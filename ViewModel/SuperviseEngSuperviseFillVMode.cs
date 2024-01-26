using System.Collections.Generic;

namespace EQC.ViewModel
{
    public class SuperviseEngSuperviseFillVModel
    {//工程督導 - 督導填報
        public int Seq { get; set; }
        public string EngNo { get; set; }
        public string EngName { get; set; }
        public string PhaseCode { get; set; }
        public int? CommitteeAverageScore { get; set; }
        public string Inspect { get; set; }

        public List<SuperviseFillCommitteeVModel> committees { get; set; }
    }
}
