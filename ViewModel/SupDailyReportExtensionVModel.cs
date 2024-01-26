using EQC.Common;
using EQC.Models;
using System;

namespace EQC.ViewModel
{
    public class SupDailyReportExtensionVModel : SupDailyReportExtensionModel
    {//監造(施工)日誌_設定展延工期
        public void updateDate()
        {
            this.ApprovalDate = this._ApprovalDate.Value;
        }

        private DateTime? _ApprovalDate;
        public string chsApprovalDate
        {
            get
            {
                return Utils.ChsDate(this.ApprovalDate);
            }
            set
            {
                this._ApprovalDate = Utils.StringChsDateToDateTime(value);
            }
        }
    }
}
