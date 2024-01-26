
using EQC.Common;
using System;

namespace EQC.Models
{
    public class ECListTpEditModel: EnvirConsListTpModel
    {//環境保育清單範本
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
        public int detailCount { get; set; }//標準項目數
    }
}
