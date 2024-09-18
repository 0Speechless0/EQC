using EQC.EDMXModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    public class EADPlaneWeaknessEngExcelVModel
    {
        public EADPlaneWeaknessEngVModel EADPlaneWeaknessEng { get; set; }

        public EDMXModel.PrjXML prjXML {get;set;}


        public ProgressData progressData { get; set; }
    }
}