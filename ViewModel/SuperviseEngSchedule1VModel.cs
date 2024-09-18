using EQC.Models;

namespace EQC.ViewModel
{
    public class SuperviseEngSchedule1VModel : SuperviseEngScheduleVModel
    {//工程督導 - 督導統計
        public string PhaseCode { get; set; }
        public string EngPlace { get; set; }
        public string BelongPrj { get; set; }

        public string BelongPrjNoNum { 
            get {
                var i = BelongPrj.IndexOf("(");
                return i > 0 ? BelongPrj.Split('(')[0] : BelongPrj;


            } 
        }
    }
}
