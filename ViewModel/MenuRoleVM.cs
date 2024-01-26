using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.ViewModel
{
    /// <summary> 選單角色列表 </summary>
    public class MenuRoleVM
    {
        /// <summary> MenuSeq </summary>
        public int MenuSeq { get; set; }

        /// <summary> MenuName </summary>
        public string MenuName { get; set; }

        /// <summary> 系統管理者 </summary>
        public bool Role1 { get; set; }

        /// <summary> 署管理者 </summary>
        public bool Role2 { get; set; }

        /// <summary> 一般使用者 </summary>
        public bool Role3 { get; set; }

        /// <summary> 施工廠商 </summary>
        public bool Role4 { get; set; }

        /// <summary> 監造廠商 </summary>
        public bool Role5 { get; set; }
        //shioulo 20220513
        /// <summary> 委外設計 </summary>
        public bool Role6 { get; set; }

        /// <summary> 委員 </summary>
        public bool Role7 { get; set; }

        /// <summary>
        /// 各局執行者
        /// </summary>
        public bool Role20 { get; set; }

    }
}