using EQC.Common;

namespace EQC.Models
{
    public class EquOperTestListVModel : EquOperTestListModel
    {//設備運轉測試清單
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
        public int stdCount { get; set; }
    }
}
