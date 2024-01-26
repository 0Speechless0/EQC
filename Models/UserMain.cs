using System;

namespace EQC.Models
{
    public class UserMain
    {
        public int Seq { get; set; } //(int, not null)
        public string UserNo { get; set; } //(nvarchar(20), not null)
        public string PassWord { get; set; } //(varchar(5), not null)
        public byte? AccountStatus { get; set; } //(tinyint, null)
        public byte? AccountType { get; set; } //(tinyint, null)
        public string DisplayName { get; set; } //(nvarchar(20), not null)
        //public Int16? UnitSeq { get; set; } //shioulo 20210707
        public string UnitName { get; set; } //(nvarchar(30), null)
        //public Int16? PositionSeq { get; set; } //shioulo 20210707
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
        public string SessionId { get; set; } //(varchar(100), null)
    }
}