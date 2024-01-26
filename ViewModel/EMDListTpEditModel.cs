using EQC.Common;
using System;

namespace EQC.Models
{
    public class EMDListTpEditModel : EngMaterialDeviceListTpModel
    {//材料設備清冊範本
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
