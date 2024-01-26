using EQC.Common;
using System;

namespace EQC.ViewModel
{
    public class EPCPriceAdjustmentVModel
    {//物價調整款 日期
        public int Seq { get; set; }
        public DateTime? AwardDate { get; set; }
        public DateTime? srcAwardDate { get; set; }

        private DateTime? _AwardDate;
        public string chsAwardDate
        {//決標日期
            get
            {
                return Utils.ChsDate(this.AwardDate);
            }
            set
            {
                this._AwardDate = Utils.StringChsDateToDateTime(value);
            }
        }

        public void updateDate()
        {
            this.AwardDate = this._AwardDate;
        }
    }
}
