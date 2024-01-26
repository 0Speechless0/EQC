using EQC.Common;
using System;

namespace EQC.Models
{
    public class NcrVModel : NcrModel
    {//NCR程序追蹤改善表
        public void updateDate()
        {
            this.ProcessTrackDate = this._ProcessTrackDate;
        }
        private DateTime? _ProcessTrackDate;
        public string chsProcessTrackDate
        {
            get
            {
                return Utils.ChsDate(this.ProcessTrackDate);
            }
            set
            {
                this._ProcessTrackDate = Utils.StringChsDateToDateTime(value);
            }
        }
    }
}
