using EQC.Common;
using System;

namespace EQC.Models
{
    public class DateTimeVModel : EngMainModel
    {//工程主檔
        public DateTime? itemDT { get; set; }

        public string dateTimeStr
        {//開工期限
            get
            {
                if (itemDT == null)
                    return "";
                else
                    return String.Format("{0} {1}", Utils.ChsDate(this.itemDT), itemDT.Value.ToString("HH:mm:ss"));
            }
        }
    }
}
