using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EQC.Services
{
    public class DBConn : SqlDB
    {
        /// <summary>AccountingDB
        /// </summary>
        public DBConn()
            : base(ConfigManager.EQC_ConnString)
        {
        }
    }
}