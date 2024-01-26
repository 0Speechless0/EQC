using EQC.Common;
using EQC.Models;
using System;

namespace EQC.ViewModel
{
    public class SupDailyReportWorkVModel : SupDailyReportWorkModel
    {//監造(施工)日誌_設定停復工
        public void updateDate()
        {
            if (this._SStopWorkDate.HasValue) this.SStopWorkDate = this._SStopWorkDate.Value;
            if (this._EStopWorkDate.HasValue) this.EStopWorkDate = this._EStopWorkDate.Value;
            if(this._BackWorkDate.HasValue) this.BackWorkDate = this._BackWorkDate.Value;
        }

        private DateTime? _SStopWorkDate;
        public string chsSStopWorkDate
        {
            get
            {
                return Utils.ChsDate(this.SStopWorkDate);
            }
            set
            {
                this._SStopWorkDate = Utils.StringChsDateToDateTime(value);
            }
        }
        private DateTime? _EStopWorkDate;
        public string chsEStopWorkDate
        {
            get
            {
                return Utils.ChsDate(this.EStopWorkDate);
            }
            set
            {
                this._EStopWorkDate = Utils.StringChsDateToDateTime(value);
            }
        }
        private DateTime? _BackWorkDate;
        public string chsBackWorkDate
        {//工程開始變更日期
            get
            {
                return Utils.ChsDate(this.BackWorkDate);
            }
            set
            {
                this._BackWorkDate = Utils.StringChsDateToDateTime(value);
            }
        }
    }
}
