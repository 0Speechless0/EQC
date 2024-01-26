using EQC.Common;

namespace EQC.Models
{
    public class ConstCheckListVModel : ConstCheckListModel
    {//施工抽查清單
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
