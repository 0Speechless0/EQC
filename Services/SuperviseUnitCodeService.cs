using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class SuperviseUnitCodeService : BaseService
    {//督導紀錄機關編碼

        //批次處裡
        public void ImportData(List<SuperviseUnitCodeModel> items, ref int iCnt, ref int uCnt, ref string errCnt)
        {
            SqlCommand cmd;
            string sql;
            foreach (SuperviseUnitCodeModel m in items)
            {
                Null2Empty(m);
                sql = @"SELECT Seq FROM SuperviseUnitCode
                        where UnitName=@UnitName";
                cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@UnitName", m.UnitName);
                DataTable dt = db.GetDataTable(cmd);
                if (dt.Rows.Count == 1)
                {
                    m.Seq = Convert.ToInt32(dt.Rows[0]["Seq"].ToString());
                    if (UpdateRecord(m) == -1)
                        errCnt += String.Format("{0},", m.itemNo);
                    else
                        uCnt++;
                }
                else
                {
                    if (AddRecord(m) == -1)
                    {
                        errCnt += String.Format("{0},", m.itemNo);
                    }
                    else
                        iCnt++;
                }
            }
        }
        //新增
        public int AddRecord(SuperviseUnitCodeModel m)
        {
            Null2Empty(m);
            string sql = @"
            insert into SuperviseUnitCode (
                UnitName,
                UnitCode,
                CreateTime,
                CreateUserSeq,
                ModifyTime,
                ModifyUserSeq
            )values(
                @UnitName,
                @UnitCode,
                GetDate(),
                @ModifyUserSeq,
                GetDate(),
                @ModifyUserSeq
            )";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@UnitName", m.UnitName);
                cmd.Parameters.AddWithValue("@UnitCode", m.UnitCode);

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('SuperviseUnitCode') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);

                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                return 0;
            }
            catch (Exception e)
            {
                log.Info("SuperviseUnitCodeService.AddRecord: " + e.Message);
                return -1;
            }
        }
        //更新
        public int UpdateRecord(SuperviseUnitCodeModel m)
        {
            Null2Empty(m);
            string sql = @"
            update SuperviseUnitCode set 
                UnitName = @UnitName,
                UnitCode = @UnitCode,
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq
            where Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@UnitName", m.UnitName);
                cmd.Parameters.AddWithValue("@UnitCode", m.UnitCode);

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("SuperviseUnitCodeService.UpdateRecord: " + e.Message);
                return -1;
            }
        }
        //清單
        public int GetListCount(string keyWord)
        {
            string sql = @"SELECT
                    count(a.Seq) total
                FROM SuperviseUnitCode a 
                where a.UnitName Like @keyWord";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@keyWord", "%" + keyWord + "%");
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetList<T>(int pageRecordCount, int pageIndex, string keyWord)
        {
            string sql = @"SELECT
                    a.Seq,
                    a.UnitName,
                    a.UnitCode
                FROM SuperviseUnitCode a
				where a.UnitName Like @keyWord
                order by a.UnitCode
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            cmd.Parameters.AddWithValue("@keyWord", "%" + keyWord + "%");

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<DateTimeVModel> GetLastDateTime()
        {
            string sql = @"SELECT max(ModifyTime) itemDT FROM SuperviseUnitCode";
            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<DateTimeVModel>(cmd);
        }

        //刪除
        public int DelRecord(int seq)
        {
            string sql = "";
            db.BeginTransaction();
            try
            {
                sql = @"delete from SuperviseUnitCode where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SuperviseUnitCodeService.DelRecord: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
    }
}