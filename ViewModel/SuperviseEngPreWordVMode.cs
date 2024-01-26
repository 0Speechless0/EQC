using EQC.Models;
using System;

namespace EQC.ViewModel
{
    public class SuperviseEngPreWordVModel : SuperviseEngModel
    {//督導工程 - 前置作業
        public string EngNo { get; set; }
        public string EngName { get; set; }
        public Int16? PositionSeq { get; set; }
        public string BelongPrj { get; set; }

        public string SuperviseDateStr {
            get
            {
                if(SuperviseDate.HasValue)
                    return SuperviseDate.Value.ToString("yyyy-MM-dd");
                else
                    return "";
            }
        }
        public string SuperviseEndDateStr
        {//s20230316
            get
            {
                if (SuperviseEndDate.HasValue)
                    return SuperviseEndDate.Value.ToString("yyyy-MM-dd");
                else
                    return "";
            }
        }
    }
}
