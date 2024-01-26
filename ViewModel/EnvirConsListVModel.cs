using EQC.Common;

namespace EQC.Models
{
    public class EnvirConsListVModel : EnvirConsListModel
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
        public int stdCount { get; set; }
    }
}
