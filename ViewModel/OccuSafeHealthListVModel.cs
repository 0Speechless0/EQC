using EQC.Common;

namespace EQC.Models
{
    public class OccuSafeHealthListVModel : OccuSafeHealthListModel
    {//職業安全衛生清單範本
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
