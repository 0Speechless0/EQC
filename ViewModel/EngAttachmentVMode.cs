
using EQC.Common;
using System;

namespace EQC.Models
{
    public class EngAttachmentVModel : EngAttachmentModel
    {//上傳監造計畫附件
        public string modifyDate
        {
            get
            {
                return Utils.EngDateTime(this.ModifyTime);
                /*if (this.ModifyTime.HasValue)
                {
                    DateTime tar = this.ModifyTime.Value;
                    return String.Format("{0}{1}{2} {3}:{4}", tar.Year, tar.Month, tar.Day, tar.Hour, tar.Minute);
                }
                else
                {
                    return string.Empty;
                }*/
            }
        }

        public bool edit { get; set; }
    }
}
