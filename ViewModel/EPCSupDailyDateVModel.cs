using EQC.Common;
using EQC.Models;
using System;

namespace EQC.ViewModel
{
    public class EPCSupDailyDateVModel : SupDailyDateModel
    {//日誌日期
        public string ItemDateStr
        {
            get
            {
                return this.ItemDate.ToString("yyyy-MM-dd");
            }
        }
        public string FillinDateStr
        {
            get
            {
                if (FillinDate.HasValue)
                    return this.FillinDate.Value.ToString("yyyy-MM-dd");
                else
                    return "";
            }
        }

        public string ModifyTimeStr
        {//s20230908
            get
            {
                if (ModifyTime.HasValue)
                    return this.ModifyTime.Value.ToString("yyyy-MM-dd h:m:s");
                else
                    return "";
            }
        }
    }
}
