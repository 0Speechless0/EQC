using System;

namespace EQC.Models
{
    public class EngYearVModel
    {//工程年分清單
        public int? EngYear { get; set; }

        public string Text {
            get
            {
                if (this.EngYear.HasValue)
                {
                    return this.EngYear.Value.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Value
        {
            get
            {
                if (this.EngYear.HasValue)
                {
                    return this.EngYear.Value.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
