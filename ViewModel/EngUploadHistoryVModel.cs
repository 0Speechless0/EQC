using EQC.Common;

namespace EQC.Models
{
    public class EngUploadHistoryVModel : EngUploadHistoryModel
    {//監造計畫上傳歷程
        public bool edit { get; set; }
        public string showModifyTime
        {
            get
            {
                return Utils.EngDateTime(this.ModifyTime);
            }
        }
    }
}
