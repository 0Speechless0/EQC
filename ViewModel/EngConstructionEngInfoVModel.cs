using EQC.Common;
using System;

namespace EQC.Models
{
    public class EngConstructionEngInfoVModel : EngMainModel
    {//工程主檔
        public Int16? CitySeq { get; set; }
        public string organizerUnitName { get; set; }
        public string execUnitName { get; set; }
        public string execSubUnitName { get; set; }
        public int? DocState { get; set; }
        public string subEngName { get; set; }//分項工程名稱
        public string subEngItemNo { get; set; }//分項工程編號
        public int subEngNameSeq { get; set; }//EngConstruction.Seq

        public void updateDate()
        {
            this.StartDate = this._StartDate;
            this.SchCompDate = this._SchCompDate;
            this.PostCompDate = this._PostCompDate;
            this.ApproveDate = this._ApproveDate;
        }
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

        private DateTime? _PostCompDate;
        public string chsPostCompDate
        {//展延完工日期
            get
            {
                return Utils.ChsDate(this.PostCompDate);
            }
            set
            {
                this._PostCompDate = Utils.StringChsDateToDateTime(value);
            }
        }

        private DateTime? _ApproveDate;
        public string chsApproveDate
        {//核定日期
            get
            {
                return Utils.ChsDate(this.ApproveDate);
            }
            set
            {
                this._ApproveDate = Utils.StringChsDateToDateTime(value);
            }
        }
    }
}
