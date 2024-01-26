using EQC.Common;
using System;

namespace EQC.Models
{
    //partial 不要蓋掉
    public partial class EngMainEditVModel : EngMainModel
    {//工程主檔
        public bool DredgingEng { get; set; } //s20231006
        public Int16? CitySeq { get; set; }
        public string organizerUnitName { get; set; }
        public string execUnitName { get; set; }
        public string execSubUnitName { get; set; }
        public int? DocState { get; set; }
        //PrjXML標案編號 shioulo 20220504
        public string TenderNo { get; set; }
        //PrjXML標案名稱 shioulo 20220504
        public string TenderName { get; set; }
        public decimal? BidAmount { get; set; } //s20230518

        public void updateDate()
        {
            this.StartDate = this._StartDate;
            this.SchCompDate = this._SchCompDate;
            this.PostCompDate = this._PostCompDate;
            this.ApproveDate = this._ApproveDate;
            this.AwardDate = this._AwardDate;
            SupervisorCommPerson4LicenseExpires = _SupervisorCommPerson4LicenseExpires;
            SupervisorCommPerson3LicenseExpires = _SupervisorCommPerson3LicenseExpires;
            EngChangeStartDate = _EngChangeStartDate;
            WarrantyExpires = _WarrantyExpires;

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
        private DateTime? _AwardDate;
        public string chsAwardDate
        {//決標日期 shioulo 20220618
            get
            {
                return Utils.ChsDate(this.AwardDate);
            }
            set
            {
                this._AwardDate = Utils.StringChsDateToDateTime(value);
            }
        }

        private DateTime? _SupervisorCommPerson4LicenseExpires;
        public string chsSupervisorCommPerson4LicenseExpires
        {//品管證照到期日 s20220825
            get
            {
                return Utils.ChsDate(this.SupervisorCommPerson4LicenseExpires);
            }
            set
            {
                this._SupervisorCommPerson4LicenseExpires = Utils.StringChsDateToDateTime(value);
            }
        }
        private DateTime? _SupervisorCommPerson3LicenseExpires;
        public string chsSupervisorCommPerson3LicenseExpires
        {//職安證照到期日 s20220825
            get
            {
                return Utils.ChsDate(this.SupervisorCommPerson3LicenseExpires);
            }
            set
            {
                this._SupervisorCommPerson3LicenseExpires = Utils.StringChsDateToDateTime(value);
            }
        }
        
        private DateTime? _EngChangeStartDate;
        public string chsEngChangeStartDate
        {//變更期日 s20220825
            get
            {
                return Utils.ChsDate(this.EngChangeStartDate);
            }
            set
            {
                this._EngChangeStartDate = Utils.StringChsDateToDateTime(value);
            }
        }
        private DateTime? _WarrantyExpires;
        public string chsWarrantyExpires
        {//保固到期日 s20220825
            get
            {
                return Utils.ChsDate(this.WarrantyExpires);
            }
            set
            {
                this._WarrantyExpires = Utils.StringChsDateToDateTime(value);
            }
        }

        public string DurationCategory { get; set; }
        public int PCCESSMainSeq { get; set; } //s20230829 0表示該工程為複製, 非由xml建立

    }
}
