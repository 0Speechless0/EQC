using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    public class EngConstructionList
    {
        public string construction { get; set; }
        public string item { get; set; }
        public DateTime? checkDate { get; set; }
        public string checkFlow { get; set; }

        public string checkDate2
        {
            get
            {
                if (this.checkDate.HasValue)
                {
                    DateTime tar = this.checkDate.Value;
                    int year = tar.Year - 1911;
                    return String.Format("{0}/{1}/{2}", year, tar.Month, tar.Day);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}