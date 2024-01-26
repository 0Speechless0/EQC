using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class RVFileService : BaseService
    {//拒絕往來廠商
        //清單
        public List<T> GetList<T>()
        {
            string sql = @"SELECT
                    a.Seq,
                    a.Corporation_Number,
                    a.Corporation_Name,
                    a.Case_no,
                    a.Case_Name,
                    a.Effective_Date,
                    a.Expire_Date
                FROM RVFile a
                order by a.Corporation_Number, a.Effective_Date DESC";
            SqlCommand cmd = db.GetCommand(sql);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //新增
        public int Add(RVFileModel m)
        {
            Null2Empty(m);
            string sql = @"
            insert into RVFile (
                Corporation_Number,
                Corporation_Name,
                Case_no,
                Case_Name,
                Effective_Date,
                Expire_Date,
                CreateTime,
                CreateUser,
                ModifyTime,
                ModifyUser
            )values(
                @Corporation_Number,
                @Corporation_Name,
                @Case_no,
                @Case_Name,
                @Effective_Date,
                @Expire_Date,
                GetDate(),
                @ModifyUserSeq,
                GetDate(),
                @ModifyUserSeq
            )";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Corporation_Number", m.Corporation_Number);
                cmd.Parameters.AddWithValue("@Corporation_Name", m.Corporation_Name);
                cmd.Parameters.AddWithValue("@Case_no", m.Case_no);
                cmd.Parameters.AddWithValue("@Case_Name", m.Case_Name);
                cmd.Parameters.AddWithValue("@Effective_Date", this.NulltoDBNull(m.Effective_Date));
                cmd.Parameters.AddWithValue("@Expire_Date", this.NulltoDBNull(m.Expire_Date));

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("RVFileService.Add: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
        //更新
        public int Update(RVFileModel m)
        {
            Null2Empty(m);
            string sql = @"
            update RVFile set 
                Corporation_Number = @Corporation_Number,
                Corporation_Name = @Corporation_Name,
                Case_no = @Case_no,
                Case_Name = @Case_Name,
                Effective_Date = @Effective_Date,
                Expire_Date = @Expire_Date,
                ModifyTime = GetDate(),
                ModifyUser = @ModifyUserSeq
            where Seq=@Seq";

            db.BeginTransaction();
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@Corporation_Number", m.Corporation_Number);
                cmd.Parameters.AddWithValue("@Corporation_Name", m.Corporation_Name);
                cmd.Parameters.AddWithValue("@Case_no", m.Case_no);
                cmd.Parameters.AddWithValue("@Case_Name", m.Case_Name);
                cmd.Parameters.AddWithValue("@Effective_Date", this.NulltoDBNull(m.Effective_Date));
                cmd.Parameters.AddWithValue("@Expire_Date", this.NulltoDBNull(m.Expire_Date));

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("RVFileService.Update: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
        //批次處裡
        public void ImportData(List<RVFileModel> items, ref int iCnt, ref int uCnt, ref string errCnt)
        {
            SqlCommand cmd;
            string sql;
            sql = "delete from  RVFile;";
            db.ExecuteNonQuery(db.GetCommand(sql));
            foreach (RVFileModel m in items)
            {
                if (Add(m) == -1)
                    errCnt += m.Corporation_Number + ",";
                else
                    iCnt++;
            }
        }
    }
}