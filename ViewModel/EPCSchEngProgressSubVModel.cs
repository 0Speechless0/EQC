using EQC.Models;

namespace EQC.ViewModel
{
    public class EPCSchEngProgressSubVModel: SchEngProgressSubModel
    {//工程進度-分項工程
        public decimal Amount { get; set; }
        public decimal? SchProgress { get; set; }
        public decimal? ActualProgress { get; set; }
    }
}
