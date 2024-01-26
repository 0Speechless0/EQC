using EQC.Common;

namespace EQC.Models
{
    public class EngMaterialDeviceListVModel : EngMaterialDeviceListModel
    {//材料設備送審清冊
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
        // for EngMaterialDeviceSummary
        public decimal? ContactQty { get; set; }
        public string ContactUnit { get; set; }
        public bool IsSampleTest { get; set; }
        //shioulo 20221216
        public bool IsFactoryInsp { get; set; }        
        public int stdCount { get; set; }
    }
}
