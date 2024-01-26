using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    public class VRole
    {
        /// <summary> 序號 </summary>
        public int Seq { get; set; }

        /// <summary> 角色代碼 </summary>
        public string Id { get; set; }

        /// <summary> 名稱 </summary>
        public string Name { get; set; }

        /// <summary> 角色說明 </summary>
        public string RoleDesc { get; set; }

        /// <summary> 是否啟用(0:否, 1:是) </summary>
        public bool IsEnabled { get; set; }

        /// <summary> 是否刪除(0:否,1:是) </summary>
        public bool IsDelete { get; set; }

        /// <summary> 是否預設(0:否,1:是) </summary>
        public bool IsDefault { get; set; }

        /// <summary> 建立時間 </summary>
        public DateTime CreateTime { get; set; }

        /// <summary> 建立人員序號 </summary>
        public int CreateUserSeq { get; set; }

        /// <summary> 異動時間 </summary>
        public DateTime ModifyTime { get; set; }

        /// <summary> 異動人員序號 </summary>
        public int ModifyUserSeq { get; set; }

        /// <summary> 項次 </summary>
        public long Rows { get; set; }
    }
}