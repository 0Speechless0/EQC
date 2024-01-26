using EQC.Common;
using System;

namespace EQC.Models
{
    public class SupervisionProjectListVModel : SupervisionProjectListModel
    {//監造計畫書範本
        public bool edit { get; set; }
        public string showModifyTime
        {
            get
            {
                return Utils.EngDateTime(this.ModifyTime);
            }
        }
        public string showRevisionDate
        {
            get
            {
                return Utils.EngDateTime(this.RevisionDate);
            }
        }
    }
}
