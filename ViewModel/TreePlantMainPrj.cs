using EQC.EDMXModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    public class TreePlantMainPrj : TreePlantMain
    {
        public static List<Unit> unitList;
        public short? OrderNo { get; set; }
        public int treeMainEngSeq { get; set; }

        public string EngName { get; set; }

        public string EngNo { get; set; }
        private string _execUnitName;
        public string execUnitName {
            get
            {
                if (unitList == null) return _execUnitName;
                return unitList.Find(row => row.Seq == ExecUnitSeq)?.Name;
            }
            set {  _execUnitName = value; }
        }
        private string _execSubUnitName;
        public string execSubUnitName {
            get
            {
                if (unitList == null) return _execSubUnitName;
                return unitList.Find(row => row.Seq == ExecSubUnitSeq)?.Name;
            }
            set {  _execSubUnitName = value; }
        }

        public string TPEngTypeName { get; set; }

        public string ContractorName1 { get; set; }


    }
}