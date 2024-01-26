using EQC.Common;
using EQC.Models;
using System;

namespace EQC.ViewModel
{
    public class EPCProgressEngChangeListVModel: EC_SchEngProgressHeaderModel
    {//進度管理 工程變更日期
        public void updateDate()
        {
            this.StartDate = this._StartDate;
            this.SchCompDate = this._SchCompDate;
        }
        private DateTime? _StartDate;
        public string chsStartDate
        {//工程開始變更日期
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
        public string chsEndDate
        {
            get
            {
                return Utils.ChsDate(this.EndDate);
            }
        }
        public string chsModifyTime
        {
            get
            {
                return Utils.ChsDate(this.ModifyTime);
            }
        }

        public string ChangeTypeStr
        {//工程變更類型
            get
            {
                switch (ChangeType)
                {
                    case 1: return "工程變更設計暨修正施工預算"; break;
                    case 2: return "展延工期"; break;
                    case 100: return "停工"; break;
                    case 3: return "復工"; break;
                    case 200: return "契約終止及解除"; break;
                    default:
                        return "未知狀態";
                }
            }
        }
    }
}
