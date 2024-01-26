using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Models
{
    public class Role
    {
        public int Seq { get; set; } //(int, not null)

        public string Id { get; set; }  // (varchar(2), null)
        public string Name { get; set; } //(nvarchar(20), null)
        public bool? IsDefault { get; set; } //(bit, null)
        public int? OrderNo { get; set; } //(int, null)
        public bool? IsEnabled { get; set; } //(bit, null)
        public string Memo { get; set; } //(nvarchar(100), null)
        public int? CreateUser { get; set; } //(int, null)

        private DateTime? modifyTime;

        private DateTime? createTime;
        public string CreateTimeStr
        {
            get
            {
                return createTime?.ToString("yyyy-MM-dd HH:mm:ss");
            }

        } //(datetime, null)
        public DateTime? CreateTime
        {
            set
            {
                createTime = value;
            }
        }
        public string ModifyTimeStr
        {
            get
            {
                return modifyTime?.ToString("yyyy-MM-dd HH:mm:ss");
            }

        } //(datetime, null)
        public DateTime? ModifyTime
        {
            set
            {
                modifyTime = value;
            }
        }
        public int? ModifyUser { get; set; } //(int, null)
    }

}