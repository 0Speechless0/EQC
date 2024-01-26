using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    public class TreeEngMainEditVModel
    {
        public int Seq { get; set; }
        public  string EngNo { get; set; }
        public  string EngName { get; set; }
        public Int16?  EngYear { get; set; }

        public string engTownName { get; set; }
        public string organizerUserName { get; set; }
        public string organizerUnitName { get; set; }
        public string execUnitName { get; set; }
        public string execSubUnitName { get; set; }

        public int ExecUnitSeq { get; set; }
        public int ExecSubUnitSeq { get; set; }
        public decimal? TotalBudget { get; set; }
        public decimal? SubContractingBudget { get; set; }

        public byte? PurchaseAmount { get; set; }
    }
}