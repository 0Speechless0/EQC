using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Text;
using EQC.Common;
namespace EQC.Services
{
    public class APIUserService : BaseService
    {




        public string getAuthCodeData(string authCode)
        {
            string resultString = new Encryption().decryptCode(authCode);
            if(resultString == null)
            {
                throw new Exception("密碼錯誤");
            }
            return resultString.Split(':')[0];

        }
        public bool checkAuthCodeExist(string authCode)
        {

            string sql = @"
                Select  Count(*)  from APIUser 
                where
                    @AuthCode = AuthCode

            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Origin", HttpContext.Current.Request.UserHostAddress);
            cmd.Parameters.AddWithValue("@AuthCode", authCode);
            return (int)db.ExecuteScalar(cmd) > 0;
        }
        public bool checkActionPermission(string actionName = null)
        {

            string sql = @"
                select Count(*) from APIUser p

                inner join

                (
	                select Max(CreateTime) CreateTime from APIUser
	                group by Origin

                ) pp on p.CreateTime = pp.CreateTime
                where p.Origin = @Origin and p.ActionList Like @ActionName

            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Origin", HttpContext.Current.Request.UserHostAddress);
            cmd.Parameters.AddWithValue("@ActionName", $"%{NulltoDBNull(actionName)}%" );
            return (int)db.ExecuteScalar(cmd) > 0;
        }
        internal void register(string name, string actionList, string authCode)
        {
            if (checkAuthCodeExist(authCode)) throw new Exception("密碼已經註冊");
            string sql = @"
                insert into APIUser(Origin, Name, ActionList, AuthCode)
                    values (@Origin, @Name, @ActionList, @AuthCode)    
        
            
            ";
            var cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Origin", HttpContext.Current.Request.UserHostAddress);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@ActionList", actionList);
            cmd.Parameters.AddWithValue("@AuthCode", authCode);
            db.ExecuteNonQuery(cmd);
        }
    }
}