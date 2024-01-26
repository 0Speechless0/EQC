using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class EngReportLocalCommunicationVModel: EngReportLocalCommunicationModel
    {
        public string FileName { get; set; }
        public string CreateUser { get; set; }
        public string ModifyUser { get; set; }
        public bool edit { get; set; }
        public string DateStr
        {
            get
            {
                return this.Date.ToString("yyyy-MM-dd");
            }
        }
    }
}