using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;

namespace EQC.Common
{
    [Serializable]
    /// <summary>
    /// 使用者資訊
    /// </summary>
    public class UserInfo : UserMain
    {
        /*public int Seq { get; set; } //(int, not null)
        public string UserNo { get; set; } //(nvarchar(20), not null)
        public string PassWord { get; set; } //(varchar(5), not null)
        public byte? AccountStatus { get; set; } //(tinyint, null)
        public byte? AccountType { get; set; } //(tinyint, null)
        public string DisplayName { get; set; } //(nvarchar(20), not null)
        //public Int16? UnitSeq { get; set; } //shioulo 20210521
        public string UnitName { get; set; } //(nvarchar(30), null)
        //public Int16? PositionSeq { get; set; } //shioulo 20210521
        public string PositionName { get; set; } //(nvarchar(30), null)
        public string EmailAddress { get; set; } //(varchar(100), null)
        public string TelArea { get; set; } //(varchar(3), null)
        public string Tel { get; set; } //(varchar(30), null)
        public string TelExt { get; set; } //(varchar(10), null)
        public string Mobile { get; set; } //(varchar(15), null)
        public string FaxArea { get; set; } //(varchar(3), null)
        public string Fax { get; set; } //(varchar(30), null)
        public bool IsEnabled { get; set; } //(bit, not null)
        public bool? IsDeleted { get; set; } //(bit, null)
        public bool? IsLock { get; set; } //(bit, null)

        public DateTime? CreateTime { get; set; } //(datetime, null)
        public int? CreateUser { get; set; } //(int, null)
        public DateTime? ModifyTime { get; set; } //(datetime, null)
        public int? ModifyUser { get; set; } //(int, null)
        public DateTime? LastLoginTime { get; set; } //(datetime, null)
        public string SessionId { get; set; } //(varchar(100), null)*//* shioulo 20210707*/

        public Int16? UnitSeq1 { get; set; } //shioulo 20210707
        public string UnitName1 { get; set; } //shioulo 20210707
        public Int16? UnitSeq2 { get; set; } //shioulo 20210707
        public string UnitName2 { get; set; } //shioulo 20210707
        public string UnitCode2 { get; set; } //shioulo 20210707
        public Int16? UnitSeq3 { get; set; } //shioulo 20210707
        public string UnitName3 { get; set; } //shioulo 20210707

        public List<Role> Role { get; set; }

        /// <summary>
        /// 取得使用者能夠瀏覽的網頁
        /// </summary>
        public List<VMenu> MenuList { get; set; }

        /// <summary>
        /// 取得使用者能夠瀏覽的系統
        /// </summary>
        public List<VSystemMenu> SystemList { get; set; }

        /// <summary>
        /// 是否為 系統管理者
        /// </summary>
        public bool IsAdmin
        {
            get
            {
                try
                {
                    SessionManager sm = new SessionManager();
                    return sm.GetUser().Role.Exists(x => x.Seq == 1);
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 是否為 署管理者
        /// </summary>
        public bool IsEQCAdmin
        {
            get
            {
                try
                {
                    SessionManager sm = new SessionManager();
                    return sm.GetUser().Role.Exists(x => x.Seq == 2);
                }
                catch
                {
                    return false;
                }
            }
        }
        //使用者角色 shioulo 20220516-1851
        public byte UserRole { get; set; }
        public bool IsDepartmentAdmin { get; set; } //署管理者
        public bool IsDepartmentUser { get; set; } //署使用者
        public bool IsBuildContractor { get; set; } //施工廠商
        public bool IsSupervisorUnit { get; set; } //監造單位
        public bool IsOutsourceDesign { get; set; } //委外設計
        public bool IsCommittee { get; set; } //委員
        public bool IsDepartmentExec //各局執行者
        {
            get
            {
                if (this.Role.Count > 0)
                    return this.Role[0].Id == "20";
                return false;
            }

        }

        public int RoleSeq { get; internal set; }

        /// <summary>
        /// 是否為 各局管理者
        /// </summary>
        public bool IsEQCUnitAdmin
        {
            get
            {
                try
                {
                    SessionManager sm = new SessionManager();
                    return sm.GetUser().Role.Exists(x => x.Seq == 3);
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}