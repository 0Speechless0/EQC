
using EQC.Common;
using System;

namespace EQC.ViewModel
{
    public class EHI_EMDCatalogVModel
    {//材料設備送審管制總表 - 型錄
        public int Seq { get; set; }
        public string OriginFileName { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateTimeStr
        {
            get
            {
                return Utils.ChsDate(this.CreateTime);
            }
        }
    }
}