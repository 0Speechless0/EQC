using EQC.Common;
using System;

namespace EQC.Models
{
    public class EngMaterialDeviceSummaryVModel: EngMaterialDeviceSummaryModel
    {//材料設備送審管制總表
        public bool edit { get; set; }

        public string QRCodeImageURL { get; set; }

        private DateTime? _SchAuditDate;
        public string chsSchAuditDate
        {//預定送審日期
            get
            {
                return Utils.ChsDate(this.SchAuditDate);
            }
            set
            {
                this._SchAuditDate = Utils.StringChsDateToDateTime(value);
            }
        }

        private DateTime? _RealAutitDate;
        public string chsRealAutitDate
        {//實際送審日期
            get
            {
                return Utils.ChsDate(this.RealAutitDate);
            }
            set
            {
                this._RealAutitDate = Utils.StringChsDateToDateTime(value);
            }
        }

        private DateTime? _FactoryInspDate;
        public string chsFactoryInspDate
        {//廠驗日期
            get
            {
                return Utils.ChsDate(this.FactoryInspDate);
            }
            set
            {
                this._FactoryInspDate = Utils.StringChsDateToDateTime(value);
            }
        }

        private DateTime? _AuditDate;
        public string chsAuditDate
        {//審查日期
            get
            {
                return Utils.ChsDate(this.AuditDate);
            }
            set
            {
                this._AuditDate = Utils.StringChsDateToDateTime(value);
            }
        }
    }
}