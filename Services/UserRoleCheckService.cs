using EQC.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EQC.Services
{
    public static class UserRoleCheckService 
    {
        private static DBConn db = new DBConn();
        public static bool checkSupervisor(int userMainSeq)
        {

            //string sql = @"Select Count(*) from EngSupervisor es
            //    inner join UserMain on UserMain.CreateUserSeq = es.UserMainSeq
            //    where es.UserMainSeq = @userMainSeq
            //    or UserMain.Seq = @userMainSeq";
            string sql = @"Select Count(*) from EngSupervisor es
                where es.UserMainSeq = @userMainSeq";
            SqlCommand cmd = new SqlCommand();
            UserInfo user = new SessionManager().GetUser();

            cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@userMainSeq", userMainSeq);

            int isSupervisor = (int)db.ExecuteScalar(cmd);

            if (isSupervisor > 0
                && user.IsDepartmentExec ) return true;

            return false;
        }
    }
}