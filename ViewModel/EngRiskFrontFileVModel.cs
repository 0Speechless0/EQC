using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class EngRiskFrontFileVModel : EngRiskFrontFileModel
    {
        public string FileName { get; set; }
        public string CreateUser { get; set; }
        public string ModifyUser { get; set; }
        public bool edit { get; set; }
    }
}