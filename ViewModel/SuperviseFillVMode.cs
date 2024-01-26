using EQC.Models;
using System.Collections.Generic;

namespace EQC.ViewModel
{
    public class SuperviseFillVModel: SuperviseFillModel
    {//工程督導 - 督導填報
        public SuperviseFillVModel()
        {
            committeeList = new List<string>();
        }
        public List<string> committeeList { get; set; }

        public string missingContent { get; set; }
    }
}
