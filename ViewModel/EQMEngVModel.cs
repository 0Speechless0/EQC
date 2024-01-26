using EQC.Models;
using System;

namespace EQC.ViewModel
{
    public class EQMEngVModel : EngMainModel
    {//碳排係數比對
        public string ExecUnitName { get; set; }
        public string EngPlace { get; set; }
        public decimal? Co2Total { get; set; }
        public decimal? Co2ItemTotal { get; set; }
    }
}
