using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EQC.Common;
using EQC.Interface;

namespace EQC.ViewModel
{
    public class EngMainTreeVModel : EngMainRole
    {

        public int Seq { get; set; }
        public int TreePlantSeq { get; set; }

        public int EngYear { get; set; }
        public string EngNo { get; set; }

        public string EngName { get; set; }
        public string BuildContractorName { get; set; }

        public string BuildContractorTaxId { get; set; }

        public string execUnitName { get; set; }

        public short? execUnitSeq { get; set; }
        

        private DateTime? _ActualBidAwardDate;
        public string ActualBidAwardDate
        {//z
            get
            {
                return _ActualBidAwardDate?.ToString("yyyy/MM/dd");
            }
            set
            {
                _ActualBidAwardDate = Utils.StringChs2DateToDateTime(value);
            }
        }

        public string engTownName { get; set; }
        public string organizerUserName { get; set; }
        public string organizerUnitName { get; set; }
        public string execSubUnitName { get; set; }
    }
}