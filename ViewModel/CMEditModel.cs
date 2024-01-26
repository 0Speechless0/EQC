using EQC.Common;
using System;

namespace EQC.Models
{
    public class CMEditModel : ChartMaintainTpModel
    {//圖表維護
        public string createDate
        {
            get
            {
                return Utils.ChsDate(this.CreateTime);
            }
        }
        public string modifyDate
        {
            get
            {
                return Utils.ChsDate(this.ModifyTime);
            }
        }
        public bool edit { get; set; }
        public string ChapterName { get; set; }
    }
}
