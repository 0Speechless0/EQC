using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    /// <summary> 人員管理VM </summary>
    public class VUserMain
    {
        /// <summary> 序號 </summary>
        public int Seq { get; set; }

        /// <summary> 使用者代號 </summary>
        public string UserNo { get; set; }

        /// <summary> 密碼 </summary>
        public string PassWord { get; set; }

        /// <summary> 姓名 </summary>
        public string DisplayName { get; set; }

        /// <summary> 聯絡電話-區碼 </summary>
        public string TelRegion { get; set; }

        /// <summary> 聯絡電話 </summary>
        public string Tel { get; set; }

        /// <summary> 分機 </summary>
        public string TelExt { get; set; }

        /// <summary> 行動電話 </summary>
        public string Mobile { get; set; }

        /// <summary> 電子郵件 </summary>
        public string Email { get; set; }

        /// <summary> 是否啟用(0:否, 1:是) </summary>
        public bool IsEnabled { get; set; }

        /// <summary> 是否刪除(0:否,1:是) </summary>
        public bool IsDelete { get; set; }

        /// <summary> 建立時間 </summary>
        public DateTime CreateTime { get; set; }

        /// <summary> 建立人員序號 </summary>
        public int CreateUserSeq { get; set; }

        /// <summary> 異動時間 </summary>
        public DateTime ModifyTime { get; set; }

        /// <summary> 異動人員序號 </summary>
        public int ModifyUserSeq { get; set; }

        /// <summary> 所屬單位_第一層(Name) </summary>
        public string UnitName1 { get; set; }

        /// <summary> 所屬單位_第一層(Seq) </summary>
        public Int16? UnitSeq1 { get; set; }

        /// <summary> 所屬單位_第二層(Name) </summary>
        public string UnitName2 { get; set; }

        /// <summary> 所屬單位_第二層(Seq) </summary>
        public Int16? UnitSeq2 { get; set; }

        /// <summary> 所屬單位_第三層(Name) </summary>
        public string UnitName3 { get; set; }

        /// <summary> 所屬單位_第三層(Seq) </summary>
        public Int16? UnitSeq3 { get; set; }

        /// <summary> 角色(Name) </summary>
        public string RoleName { get; set; }

        /// <summary> 角色(Seq) </summary>
        public int RoleSeq { get; set; }

        /// <summary> 職稱(Seq) </summary>
        public Int16? PositionSeq { get; set; }

        /// <summary> 項次 </summary>
        public long Rows { get; set; }

        /// <summary> 簽名檔筆數 </summary>
        public int SignatureFileCount { get; set; }


        //APP鎖狀態
        public int ConstCheckAppLock { get; set; }

    }
}