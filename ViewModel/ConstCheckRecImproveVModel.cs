using EQC.Common;
using System;

namespace EQC.Models
{
    public class ConstCheckRecImproveVModel : ConstCheckRecImproveModel
    {//抽驗缺失改善
        public void updateDate()
        {
            this.ImproveDeadline = this._ImproveDeadline;
            this.ProcessTrackDate = this._ProcessTrackDate;
        }
        private DateTime? _ImproveDeadline;
        public string chsImproveDeadline
        {
            get
            {
                return Utils.ChsDate(this.ImproveDeadline);
            }
            set
            {
                this._ImproveDeadline = Utils.StringChsDateToDateTime(value);
            }
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
