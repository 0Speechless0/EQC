using EQC.Common;
using System;

namespace EQC.ViewModel
{
    public class EPCSchProgressV1Model
    {//預定進度 日期
        public int Seq { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? SchCompDate { get; set; }
        public DateTime? EngChangeStartDate { get; set; }
        public DateTime? ScheChangeCloseDate { get; set; }
        public DateTime? srcStartDate { get; set; }
        public DateTime? srcSchCompDate { get; set; }

        private DateTime? _StartDate;
        public string chsStartDate
        {//開工期限
            get
            {
                return Utils.ChsDate(this.StartDate);
            }
            set
            {
                this._StartDate = Utils.StringChsDateToDateTime(value);
            }
        }

        private DateTime? _SchCompDate;
        public string chsSchCompDate
        {//預定完工日期
            get
            {
                return Utils.ChsDate(this.SchCompDate);
            }
            set
            {
                this._SchCompDate = Utils.StringChsDateToDateTime(value);
            }
        }

        private DateTime? _EngChangeStartDate;
        public string chsEngChangeStartDate
        {//預定完工日期
            get
            {
                return Utils.ChsDate(this.EngChangeStartDate);
            }
            set
            {
                this._EngChangeStartDate = Utils.StringChsDateToDateTime(value);
            }
        }

        public void updateDate()
        {
            this.StartDate = this._StartDate;
            this.SchCompDate = this._SchCompDate;
            this.EngChangeStartDate = this._EngChangeStartDate;
        }

        public string ScheChangeCloseDateStr
        {//預定完工日期
            get
            {
                return Utils.ChsDate(this.ScheChangeCloseDate);
            }
        }
    }
}
