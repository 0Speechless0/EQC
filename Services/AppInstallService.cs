using EQC.Common;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class AppInstallService : BaseService
    {
        public bool Update(int PhoneType, int seq,string Phonemodel)
        {
            string sql = "";

            db.BeginTransaction();
            try
            {
                sql = @"update UserMain set
	                        MobileType=@phonetype,
                            MobileModel=@Phonemodel
                        where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", seq);
                cmd.Parameters.AddWithValue("@phonetype", PhoneType);
                cmd.Parameters.AddWithValue("@Phonemodel", Phonemodel);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();

                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("UserMain.Update: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        public List<T> GetLink<T>()
        {
            string sql = @"
                select a.LinkCode as Link
                from APPDownLoadLink a
                where a.IsUse != 1 
				";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();

            return db.GetDataTableWithClass<T>(cmd);
        }

        public bool UpdateLink(int seq,string link)
        {
            string sql = "";

            db.BeginTransaction();
            try
            {
                sql = @"update APPDownLoadLink set
	                        IsUse=1,
                            UseUserSeq=@seq
                        where LinkCode=@link";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", seq);
                cmd.Parameters.AddWithValue("@link", link);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();

                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("UserMain.Update: " + e.Message);
                log.Info(sql);
                return false;
            }
        }



    }
}