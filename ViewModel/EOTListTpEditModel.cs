using EQC.Common;
using System;

namespace EQC.Models
{
    public class EOTListTpEditModel : EquOperTestListTpModel
    {//設備運轉測試清單範本
        public string createDate
        {
            get
            {
                return Utils.ChsDate(this.ModifyTime);
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
        public int detailCount { get; set; }//標準項目數
    }
}