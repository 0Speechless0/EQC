using EQC.Common;
using System;

namespace EQC.Models
{
    public class EngMaterialDeviceTestSummaryVModel: EngMaterialDeviceTestSummaryModel
    {//材料設備檢驗管制總表

        public bool edit { get; set; }

        private DateTime? _SchTestDate;
        public string chsSchTestDate
        {//預定進場日期
            get
            {
                return Utils.ChsDate(this.SchTestDate);
            }
            set
            {
                this._SchTestDate = Utils.StringChsDateToDateTime(value);
            }
        }

        private DateTime? _RealTestDate;
        public string chsRealTestDate
        {//實際進場日期
            get
            {
                return Utils.ChsDate(this.RealTestDate);
            }
            set
            {
                this._RealTestDate = Utils.StringChsDateToDateTime(value);
            }
        }

        private DateTime? _SampleDate;
        public string chsSampleDate
        {//抽樣日期
            get
            {
                return Utils.ChsDate(this.SampleDate);
            }
            set
            {
                this._SampleDate = Utils.StringChsDateToDateTime(value);
            }
        }
    }
}